$scriptDir = Split-Path $script:MyInvocation.MyCommand.Path
$nightlyRevFile = Join-Path -Path $scriptDir -ChildPath nightly.rev

$previousHash = git rev-parse HEAD~1
$hashOfLatestBuild = Get-Content -Path $nightlyRevFile -ErrorAction SilentlyContinue

$hasChanges = -Not ($previousHash -eq $hashOfLatestBuild)

return @{
    "LatestBuildHash" = $hashOfLatestBuild;
    "PreviousHash" = $previousHash;
    "HasChanges" = $hasChanges;
}
