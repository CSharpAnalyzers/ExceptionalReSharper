$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
$NuspecFile = Join-Path -Path $ScriptDir -ChildPath ExceptionalDevs.Exceptional.nuspec

$Xml = [xml](Get-Content $NuspecFile)
$Version = New-Object -TypeName Version -ArgumentList $Xml.package.metadata.version

return $Version