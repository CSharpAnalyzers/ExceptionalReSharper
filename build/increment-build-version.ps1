$workspace = ([System.IO.FileInfo] $PSCommandPath).Directory.Parent.FullName
$solutionDirectory = Join-Path $workspace "src"
$solutionPath = Join-Path $solutionDirectory "Exceptional.sln"
$projectDirectory = Join-Path $solutionDirectory "Exceptional"
$directoryBuildPropertiesPath = Join-Path $projectDirectory "Directory.Build.props"

# use object initialization to be able to read special characters like ©
$directoryBuildProperties = New-Object -TypeName System.Xml.XmlDocument
$directoryBuildProperties.Load($directoryBuildPropertiesPath)

$oldVersion = [version]::Parse($directoryBuildProperties.Project.PropertyGroup.Version)

$newBuildVersion = $oldVersion.Build + 1

$newVersion = [version]::new($oldVersion.Major, $oldVersion.Minor, $newBuildVersion, 0)

$directoryBuildProperties.Project.PropertyGroup.Version = $newVersion.ToString()
$directoryBuildProperties.Save($directoryBuildPropertiesPath)

MSBuild $solutionPath /t:Exceptional:UpdateAssemblyInfo
