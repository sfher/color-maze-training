$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$gameRoot = Join-Path $repoRoot "Color Maze"
$mainPath = Join-Path $gameRoot "Assets\Scenes\Main.unity"
$menuPath = Join-Path $gameRoot "Assets\Scenes\MainMenu.unity"

function Get-ComponentBlock {
    param(
        [string]$Text,
        [string]$TypeName
    )

    $pattern = "(?ms)m_EditorClassIdentifier: " +
        [regex]::Escape("Assembly-CSharp::$TypeName") +
        ".*?(?=\n--- !u!|\z)"
    $match = [regex]::Match($Text, $pattern)

    if (-not $match.Success) {
        throw "Missing component: $TypeName"
    }

    return $match.Value
}

function Assert-Reference {
    param(
        [string]$Block,
        [string]$Field
    )

    if ($Block -notmatch "$Field`: \{fileID: (?!0\})") {
        throw "Missing object reference: $Field"
    }
}

$main = Get-Content -Raw $mainPath
$menu = Get-Content -Raw $menuPath

$flow = Get-ComponentBlock $main "LevelFlowController"
$hud = Get-ComponentBlock $main "GameHud"
$menuController = Get-ComponentBlock $menu "MainMenuController"

Assert-Reference $flow "progress"
Assert-Reference $hud "returnHomeButton"
Assert-Reference $menuController "quitButton"

if ($main -match "Assembly-CSharp::SaveCoordinator") {
    throw "Main scene still contains the deprecated SaveCoordinator writer."
}

if ($main -notmatch "m_Name: ReturnHomeButton") {
    throw "Pause panel is missing ReturnHomeButton."
}

$prices = [ordered]@{
    "content_skin_blue.asset" = 10
    "content_skin_purple.asset" = 20
    "content_theme_deepsea.asset" = 15
    "content_theme_lava_heart.asset" = 25
    "content_theme_cyberpunk.asset" = 35
    "content_level_water_world.asset" = 20
}

foreach ($entry in $prices.GetEnumerator()) {
    $path = Join-Path $gameRoot (
        "Assets\Data\Content\" + $entry.Key)
    $match = [regex]::Match(
        (Get-Content -Raw $path),
        '(?m)^  price: (\d+)\s*$')

    if (-not $match.Success) {
        throw "Missing price: $($entry.Key)"
    }

    $actual = [int]$match.Groups[1].Value

    if ($actual -ne $entry.Value) {
        throw "Wrong price: $($entry.Key) expected " +
            "$($entry.Value), got $actual"
    }
}

$expectedButtons = [ordered]@{
    'Button_\u5F00\u59CB\u6E38\u620F' = '0,120|340,66'
    'Button_\u8BFB\u53D6\u5B58\u6863' = '0,45|340,66'
    'Button_\u6E05\u7A7A\u5B58\u6863' = '0,-30|340,66'
    'Button_\u9009\u62E9\u5173\u5361' = '0,-105|340,66'
    'Button_\u5546\u5E97' = '0,-180|340,66'
    'Button_\u8BBE\u7F6E' = '0,-255|340,66'
    'Button_\u9000\u51FA\u6E38\u620F' = '-110,-55|160,48'
}

$objectNames = @{}

foreach ($match in [regex]::Matches(
        $menu,
        '(?ms)^--- !u!1 &(\d+)\r?\nGameObject:\r?\n.*?^  m_Name: (.*?)\r?$')) {
    $objectNames[$match.Groups[1].Value] =
        $match.Groups[2].Value.Trim('"')
}

foreach ($match in [regex]::Matches(
        $menu,
        '(?ms)^--- !u!224 &(\d+)\r?\nRectTransform:\r?\n.*?(?=^--- !u!|\z)')) {
    $block = $match.Value
    $gameObjectId = [regex]::Match(
        $block,
        'm_GameObject: \{fileID: (\d+)\}').Groups[1].Value

    if (-not $objectNames.ContainsKey($gameObjectId)) {
        continue
    }

    $name = $objectNames[$gameObjectId]
    if (-not $expectedButtons.Contains($name)) {
        continue
    }

    $position = [regex]::Match(
        $block,
        'm_AnchoredPosition: \{x: ([^,]+), y: ([^}]+)\}')
    $size = [regex]::Match(
        $block,
        'm_SizeDelta: \{x: ([^,]+), y: ([^}]+)\}')
    $actual = "$($position.Groups[1].Value),$($position.Groups[2].Value)|" +
        "$($size.Groups[1].Value),$($size.Groups[2].Value)"

    if ($actual -ne $expectedButtons[$name]) {
        throw "Wrong menu button layout: $name expected " +
            "$($expectedButtons[$name]), got $actual"
    }

    $expectedButtons.Remove($name)
}

if ($expectedButtons.Count -ne 0) {
    throw "Missing menu buttons: $($expectedButtons.Keys -join ', ')"
}

if ($menuController -notmatch
        'catalog: \{fileID: (?!0\})\d+, guid: [0-9a-f]+') {
    throw "Main menu catalog reference is empty."
}

$menuBuilder = Get-Content -Raw (
    Join-Path $gameRoot "Assets\Editor\MainMenuSceneBuilder.cs")

foreach ($position in 120, 45, -30, -105, -180, -255) {
    if ($menuBuilder -notmatch ", ${position}f\)") {
        throw "Main menu builder is missing button position: $position"
    }
}

if ($menuBuilder -notmatch 'LoadCatalog\(\)' -or
    $menuBuilder -notmatch
        'Assets/Data/Content/ContentCatalog\.asset' -or
    $menuBuilder -notmatch
        'Assets/Data/ContentCatalog\.asset') {
    throw "Main menu builder does not support both catalog paths."
}

Add-Type -AssemblyName System.Drawing

$themeSprites = @{
    'deepsea/tile_stone.png' = 'stone'
    'deepsea/tile_material.png' = 'material'
    'deepsea/tile_exit.png' = 'exit'
    'deepsea/tile_chest.png' = 'chest'
    'lava_heart/tile_material.png' = 'material'
    'lava_heart/tile_exit.png' = 'exit'
    'lava_heart/tile_chest.png' = 'chest'
    'cyberpunk/tile_stone.png' = 'stone'
    'cyberpunk/tile_water.png' = 'water'
    'cyberpunk/tile_lava.png' = 'lava'
    'cyberpunk/tile_material.png' = 'material'
    'cyberpunk/tile_exit.png' = 'exit'
    'cyberpunk/tile_chest.png' = 'chest'
}

foreach ($entry in $themeSprites.GetEnumerator()) {
    $relative = $entry.Key -replace '/', '\'
    $path = Join-Path $gameRoot "Assets\Art\Themes\$relative"
    $metaPath = "$path.meta"

    if (-not (Test-Path $path) -or -not (Test-Path $metaPath)) {
        throw "Missing themed sprite: $relative"
    }

    $image = [System.Drawing.Image]::FromFile($path)
    try {
        if ($image.Width -ne 32 -or $image.Height -ne 32) {
            throw "Wrong theme sprite size: $relative " +
                "$($image.Width)x$($image.Height)"
        }
    }
    finally {
        $image.Dispose()
    }

    $meta = Get-Content -Raw $metaPath
    if ($meta -notmatch 'spritePixelsToUnits: 32') {
        throw "Wrong pixels-per-unit: $relative"
    }

    $guid = [regex]::Match(
        $meta,
        '(?m)^guid: ([0-9a-f]+)$').Groups[1].Value
    $theme = Split-Path (Split-Path $path -Parent) -Leaf
    $field = $entry.Value
    $visuals = Get-Content -Raw (
        Join-Path $gameRoot "Assets\Data\TileVisuals_$theme.asset")

    if ($visuals -notmatch "$field`: \{fileID: 21300000, guid: $guid") {
        throw "Unbound themed sprite: $theme.$field"
    }
}

$classicChestPath = Join-Path $gameRoot (
    "Assets\Art\Tiles\tile_chest.png")
$classicChestMeta = "$classicChestPath.meta"
if (-not (Test-Path $classicChestPath) -or
    -not (Test-Path $classicChestMeta)) {
    throw "Missing classic rectangular chest."
}

$classicImage = [System.Drawing.Image]::FromFile(
    $classicChestPath)
try {
    if ($classicImage.Width -ne 32 -or
        $classicImage.Height -ne 32) {
        throw "Classic chest must be 32x32."
    }
}
finally {
    $classicImage.Dispose()
}

$classicGuid = [regex]::Match(
    (Get-Content -Raw $classicChestMeta),
    '(?m)^guid: ([0-9a-f]+)$').Groups[1].Value
$classicVisuals = Get-Content -Raw (
    Join-Path $gameRoot "Assets\Data\Tile Visuals.asset")
if ($classicVisuals -notmatch
        "chest: \{fileID: 21300000, guid: $classicGuid") {
    throw "Classic chest is not bound."
}

Write-Output "Release UI validation passed."
