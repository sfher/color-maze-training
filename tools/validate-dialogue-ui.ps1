param(
    [string]$ProjectRoot = (
        Join-Path $PSScriptRoot '..\Color Maze'
    )
)

$ErrorActionPreference = 'Stop'

$scenePath = Join-Path $ProjectRoot 'Assets\Scenes\Main.unity'
$hudPath = Join-Path $ProjectRoot 'Assets\Scripts\GameHud.cs'
$dialoguePath = Join-Path $ProjectRoot 'Assets\Scripts\MayaDialogueView.cs'
$scene = Get-Content -Raw $scenePath
$hud = Get-Content -Raw $hudPath
$dialogue = Get-Content -Raw $dialoguePath

$bodyMatch = [regex]::Match(
    $scene,
    '--- !u!114 &124644423(?<body>.*?)(?=--- !u!)',
    [System.Text.RegularExpressions.RegexOptions]::Singleline)
$rectMatch = [regex]::Match(
    $scene,
    '--- !u!224 &124644422(?<body>.*?)(?=--- !u!)',
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

if (-not $bodyMatch.Success) {
    throw 'FAIL: EndHintText component 124644423 was not found.'
}

if (-not $rectMatch.Success) {
    throw 'FAIL: EndHintText RectTransform 124644422 was not found.'
}

$body = $bodyMatch.Groups['body'].Value
$rect = $rectMatch.Groups['body'].Value
$alignment = [regex]::Match(
    $body,
    'm_VerticalAlignment:\s*(?<value>\d+)').Groups['value'].Value
$height = [double][regex]::Match(
    $rect,
    'm_SizeDelta:\s*\{x:\s*[^,]+, y:\s*(?<value>[^}]+)\}'
).Groups['value'].Value
$bodyY = [double][regex]::Match(
    $rect,
    'm_AnchoredPosition:\s*\{x:\s*[^,]+, y:\s*(?<value>[^}]+)\}'
).Groups['value'].Value

function Get-AnchoredY([string]$fileId) {
    $match = [regex]::Match(
        $scene,
        "--- !u!224 &$fileId(?<body>.*?)(?=--- !u!)",
        [System.Text.RegularExpressions.RegexOptions]::Singleline)

    if (-not $match.Success) {
        throw "FAIL: RectTransform $fileId was not found."
    }

    return [double][regex]::Match(
        $match.Groups['body'].Value,
        'm_AnchoredPosition:\s*\{x:\s*[^,]+, y:\s*(?<value>[^}]+)\}'
    ).Groups['value'].Value
}

$titleY = Get-AnchoredY '848171399'
$restartY = Get-AnchoredY '496534921'
$nextY = Get-AnchoredY '1810034211'

$bodyTop = $bodyY + $height / 2
$bodyBottom = $bodyY - $height / 2
$titleBottom = $titleY - 25
$buttonTop = [Math]::Max($restartY, $nextY) + 28

$finishedMatch = [regex]::Match(
    $hud,
    'private void HandleDialogueFinished\(\)\s*\{(?<body>.*?)\n    \}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

if (-not $finishedMatch.Success) {
    throw 'FAIL: HandleDialogueFinished was not found.'
}

$finishedBody = $finishedMatch.Groups['body'].Value
$checks = [ordered]@{
    'Dialogue text is top-aligned' = $alignment -eq '256'
    'Dialogue view enforces top alignment' =
        $dialogue.Contains('VerticalAlignmentOptions.Top')
    'Dialogue text has room for the continue cue' = $height -ge 100
    'Dialogue text does not overlap the title' =
        $bodyTop -lt $titleBottom
    'Dialogue text does not overlap action buttons' =
        $bodyBottom -gt $buttonTop
    'Dialogue ends by changing the title to 关卡完成' =
        $finishedBody.Contains('endTitleText.text = "关卡完成";')
}

$failed = $false
foreach ($check in $checks.GetEnumerator()) {
    if ($check.Value) {
        "PASS: $($check.Key)"
    }
    else {
        "FAIL: $($check.Key)"
        $failed = $true
    }
}

if ($failed) {
    exit 1
}
