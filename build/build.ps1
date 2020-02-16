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
$projectDirectory = Join-Path $solutionDirectory "Exceptional"
$nuspecFile = Join-Path $buildDirectory "ExceptionalDevs.Exceptional.nuspec"

# restore packages
nuget restore $solutionDirectory

# build the project in debug mode
MSBuild $projectDirectory /p:Configuration=$configuration /t:rebuild

if($packageDirectoryRelativeToBuildDirectory -and -not [System.IO.Path]::IsPathRooted($packageDirectory))
{
    $packageDirectory = Join-Path $buildDirectory $packageDirectoryName
}
else
{
    $packageDirectory = $packageDirectoryName
}

# create package
nuget pack $nuspecFile -OutputDirectory $packageDirectory -Suffix $prereleaseLabel -properties Configuration=$configuration


