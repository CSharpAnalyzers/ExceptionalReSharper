set /p apiKey=NuGet API Key: 
set /p version=Package Version: 

nuget.exe push Packages/ExceptionalDevs.Exceptional.%version%.nupkg %apiKey% -source https://resharper-plugins.jetbrains.com