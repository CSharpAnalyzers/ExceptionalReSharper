set /p apiKey=NuGet API Key: 
set /p version=Package Version: 

nuget.exe push Packages/Exceptional.%version%.nupkg %apiKey% -source https://resharper-plugins.jetbrains.com
nuget.exe push Packages/Exceptional.R9.%version%.nupkg %apiKey% -source https://resharper-plugins.jetbrains.com
nuget.exe push Packages/Exceptional.R10.%version%.nupkg %apiKey% -source https://resharper-plugins.jetbrains.com
nuget.exe push Packages/Exceptional.2016.1.%version%.nupkg %apiKey% -source https://resharper-plugins.jetbrains.com
nuget.exe push Packages/Exceptional.2016.2.%version%.nupkg %apiKey% -source https://resharper-plugins.jetbrains.com