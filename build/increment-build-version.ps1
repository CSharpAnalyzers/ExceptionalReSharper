$workspace = ([System.IO.FileInfo] $PSCommandPath).Directory.Parent.FullName
$buildDirectory = Join-Path $workspace "build"
$nuspecFile = Join-Path $buildDirectory "ExceptionalDevs.Exceptional.nuspec"

$xml = New-Object -TypeName System.Xml.XmlDocument
$xml.Load($nuspecFile)

$oldVersion = [version]::Parse($xml.package.metadata.version)

$newMajor = $oldVersion.Major
$newMinor = $oldVersion.Minor
$newBuild = $oldVersion.Build + 1

$newVersion = [version]::new($newMajor, $newMinor, $newBuild)

$xml.package.metadata.version = $newVersion.ToString()

$xml.Save($nuspecFile)
