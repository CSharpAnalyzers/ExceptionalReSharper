How to use the source code: 

- **Before opening the solution, run the `build/00_RestorePackages.bat` file.** Explanation: The Exceptional.csproj needs a ReSharper msbuild target file which is available in the NuGet package. The NuGet package is not stored in the Git repository, this is why you need to restore the packages before you can build the project. 

How to debug the extension: 
IMPORTANT: Uninstall the "Exceptional" extension first

1. Open the "Properties" of the "Exceptional" project in the solution
2. Go to the "Debug" section, select "Debug" as configuration and change the following settings: 
    - Start external program: Select your Visual Studio application "devenv.exe"
    - Command line arguments: 

		/ReSharper.Plugin "PATH/TO/SOURCES/Exceptional/bin/Debug/ReSharper.Exceptional.dll"

How to release a new version: 

1. Update extension version in Exceptional.nuspec and AssemblyInfo.cs if necessary
2. Rebuild the whole solution in "Release" configuration
3. Run NuGet/Build.bat
4. Upload new package version from NuGet/Packages
5. Update extension version in Exceptional.nuspec and AssemblyInfo.cs to the next planned version 
6. Push changes (also new NuGet package) to Git

What to look for in pull requests: 

- Check with community if feature makes sense
- Check that the changes do not decrease performance (this is very important!)

Log files to find problem why exception crashed:

- The ReSharper log files can be found here: 

	C:\Users\USERNAME\AppData\Local\JetBrains\ReSharper\v8.2\ExceptionStorage