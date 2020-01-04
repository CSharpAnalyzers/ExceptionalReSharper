# Local Build, Installation And Development Guide

## Build And Pack Using Scripts

1. Change directory to the build directory `cd path\to\ExceptionalReSharper\build`
2. Download dependencies `00_RestorePackages.bat`
3. Trigger the build `01_Build.bat`
4. Create a package `02_CreatePackages.bat`
5. Now you should find a new package in `path\to\ExceptionalReSharper\build\Packages`
6. Copy the *.nupkg file to your local repository

## Build Using IDE

1. Make sure allrequired packages are downloaded, e. g. by running `build\00_RestrePackages.bat`
2. Build the project as usual

## Download Latest Package from GitHub

1. Open `https://github.com/ManticSic/ExceptionalReSharper/releases`
2. Search for the latest release
3. Download the attached *.nupkg file
4. Copy the *.nupkg file to your local repository

## Setup your Environment

Your can find a full guide on [JetBrains.com](https://www.jetbrains.com/help/resharper/sdk/HowTo/Start/SetUpEnvironment.html).

### Setup the IDE

1. Install ReSharper to a VS hive
2. Run the experimental hive
3. Add a local ReSharper Package Source

### Add a local ReSharper Package Source

1. Open Visual Studio
2. Open ReSharper Extension Manager
3. Open Options
4. Add a new package source
   * Name: local
   * Source: `path\to\a\local\directory`, e.g. `path\to\ExceptionalReSharper\build\Packages`
5. Save 

### Notes

* You have to reinstall the plugin on changes
* Instead of reinstalling the plugin, you can replace `ReSharper.Exceptional.dll` directly in `C:\Users\%username%\AppData\Local\JetBrains\Installations\{ReSharperInstallation}` with the new assembly in `path\to\ExceptionalReSharper\src\Exceptional\bin\{configuration}`

## Running and Debugging

Please read this [guide](https://www.jetbrains.com/help/resharper/sdk/Extensions/Plugins/Debugging.html).

## Troubleshooting

### I cannot install my local version

Uninstall previous installations of Exceptional for ReSharper and clear `C:\Users\%username%\AppData\Local\JetBrains\plugins` and `C:\Users\%username%\AppData\Local\NuGet\Cache`, after that restart VS.