$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
$NightlyRevFile = Join-Path -Path $ScriptDir -ChildPath nightly.rev


$PreviousHash = git rev-parse HEAD~1
$HashOfLatestBuild = Get-Content -Path $NightlyRevFile -ErrorAction SilentlyContinue

Write-Output "Previous hash:     $PreviousHash"
Write-Output "Latest Build Hash: $HashOfLatestBuild"

$HasChanges = -Not ($PreviousHash -eq $HashOfLatestBuild)

if ($HasChanges -eq $true)
{
    "Some changes detected"
    exit 0
}
else
{
    Write-Output "No changes detected"
    exit 1
}
