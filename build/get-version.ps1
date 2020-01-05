$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
$NuspecFile = Join-Path -Path $ScriptDir -ChildPath ExceptionalDevs.Exceptional.nuspec

$Xml = [xml](Get-Content $NuspecFile)
$Version = [Semver.SemVersion]::Parse($Xml.package.metadata.version)

$DateString = Get-Date -Format "yyyyMMddhhmm"
$PrereleaseVersion = "nightly"

$Version = $Version.Change($null, $null, $null, $PrereleaseVersion, $DateString)

# nuget does only support semver 1.0.0, not semver 2.0.0
$Version = ([string]$Version.ToString()).Replace("+", ".")

return $Version
