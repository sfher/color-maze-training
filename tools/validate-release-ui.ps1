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

Write-Output "Release UI validation passed."
