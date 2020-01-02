$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
$NightlyRevFile = Join-Path -Path $ScriptDir -ChildPath nightly.rev

$GitRev = [string](git rev-parse HEAD)

Set-Content -Path $NightlyRevFile -Value ($GitRev)
