cd NuGet

nuget.exe restore ../Exceptional.sln
msbuild ../Exceptional.sln /p:Configuration=Release /t:rebuild

nuget.exe pack Exceptional.R8.nuspec -OutputDirectory "Packages"
nuget.exe pack Exceptional.R9.nuspec -OutputDirectory "Packages"

cd ..