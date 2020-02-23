[CmdletBinding()]
param (
    [string] $prereleaseLabel="dev",
    [string] $configuration="Debug",
    [string] $packageDirectoryName="packages",
    [bool]   $packageDirectoryRelativeToBuildDirectory=$true
)

$workspace = ([System.IO.FileInfo] $PSCommandPath).Directory.Parent.FullName
$buildDirectory = Join-Path $workspace "build"
$solutionDirectory = Join-Path $workspace "src"
$solution = Join-Path $solutionDirectory "Exceptional.sln"
$nuspecFile = Join-Path $buildDirectory "ExceptionalDevs.Exceptional.nuspec"
$projectDirectory = Join-Path $solutionDirectory "Exceptional"
$directoryBuildPropertiesPath = Join-Path $projectDirectory "Directory.Build.props"

# restore packages
nuget restore $solutionDirectory

# build the project in debug mode
MSBuild $solution /t:Exceptional:Rebuild /p:Configuration=$configuration

if($packageDirectoryRelativeToBuildDirectory -and -not [System.IO.Path]::IsPathRooted($packageDirectoryName))
{
    $packageDirectory = Join-Path $buildDirectory $packageDirectoryName
}
else
{
    $packageDirectory = $packageDirectoryName
}

# create package
$directoryBuildProperties = New-Object -TypeName System.Xml.XmlDocument
$directoryBuildProperties.Load($directoryBuildPropertiesPath)

$version = [version]::Parse($directoryBuildProperties.Project.PropertyGroup.Version)
$title = $directoryBuildProperties.Project.PropertyGroup.Title
$description = $directoryBuildProperties.Project.PropertyGroup.Description
$authors = $directoryBuildProperties.Project.PropertyGroup.Authors
$copyright = $directoryBuildProperties.Project.PropertyGroup.Copyright


nuget pack `
      $nuspecFile `
      -OutputDirectory $packageDirectory `
      -Suffix $prereleaseLabel `
      -properties "Configuration=$configuration;Version=$version;Title=$title;Description=$description;Authors=$authors;Copyright=$copyright"


