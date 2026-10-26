$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$gameRoot = Join-Path $repoRoot "Color Maze"
$mainScenePath = Join-Path $gameRoot "Assets\Scenes\Main.unity"
$catalogPath = Join-Path $gameRoot (
    "Assets\Data\Content\ContentCatalog.asset")
$contentRoot = Join-Path $gameRoot "Assets\Data\Content"

$main = Get-Content -Raw $mainScenePath
$applier = [regex]::Match(
    $main,
    '(?ms)m_EditorClassIdentifier: ' +
    'Assembly-CSharp::PlayerSkinApplier.*?(?=\n--- !u!|\z)')

if (-not $applier.Success) {
    throw "Main scene is missing PlayerSkinApplier."
}

if ($applier.Value -notmatch
        'catalog: \{fileID: (?!0\})') {
    throw "PlayerSkinApplier.catalog is not assigned."
}

if ($applier.Value -notmatch
        'spriteRenderer: \{fileID: (?!0\})') {
    throw "PlayerSkinApplier.spriteRenderer is not assigned."
}

$catalog = Get-Content -Raw $catalogPath
$entryGuids = [regex]::Matches(
    $catalog,
    '(?m)^  - \{fileID: 11400000, guid: ([0-9a-f]+), type: 2\}\s*$'
)

$defaultTheme = $null
foreach ($match in $entryGuids) {
    $guid = $match.Groups[1].Value
    $meta = Get-ChildItem $contentRoot -Filter "*.asset.meta" |
        Where-Object {
            (Get-Content -Raw $_.FullName) -match "guid: $guid"
        } |
        Select-Object -First 1

    if ($meta -eq $null) {
        continue
    }

    $assetPath = $meta.FullName.Substring(
        0,
        $meta.FullName.Length - 5)
    $asset = Get-Content -Raw $assetPath
    if ($asset -match '(?m)^  id: theme-default$') {
        $defaultTheme = $asset
        break
    }
}

if ($defaultTheme -eq $null) {
    throw "Content catalog has no theme-default entry."
}

if ($defaultTheme -notmatch '(?m)^  contentType: 1$' -or
    $defaultTheme -notmatch '(?m)^  price: 0$' -or
    $defaultTheme -notmatch
        '(?m)^  themeVisuals: \{fileID: 0\}$') {
    throw "theme-default must be a free theme with no override."
}

Write-Output "Cosmetics validation passed."
