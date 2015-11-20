nuget.exe restore ../src/Exceptional.sln
msbuild ../src/Exceptional.sln /p:Configuration=Release /t:rebuild

nuget.exe pack Exceptional.R8.nuspec -OutputDirectory "Packages"
nuget.exe pack Exceptional.R9.nuspec -OutputDirectory "Packages"
nuget.exe pack Exceptional.R10.nuspec -OutputDirectory "Packages"