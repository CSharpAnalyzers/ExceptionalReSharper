Param([string]$VersionIncrementation)

$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
$GetVersionScript = Join-Path -Path $ScriptDir -ChildPath get-version.ps1
$NuspecFile = Join-Path -Path $ScriptDir -ChildPath ExceptionalDevs.Exceptional.nuspec

$Incrementation = New-Object -TypeName Version -ArgumentList $VersionIncrementation
$OldVersion = [Version](& $GetVersionScript)

$NewMajor = $OldVersion.Major + $Incrementation.Major
$NewMinor = $OldVersion.Minor + $Incrementation.Minor
$NewBuild = $OldVersion.Build + $Incrementation.Build
$NewRevision = $OldVersion.Revision + $Incrementation.Revision

$NewVersion = New-Object -TypeName Version -ArgumentList ($NewMajor, $NewMinor, $NewBuild, $NewRevision)

$Xml = [xml](Get-Content $NuspecFile)
$Xml.package.metadata.version = $NewVersion.ToString()
$Xml.Save($NuspecFile)
