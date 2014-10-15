@echo Off
set target=%1
if "%target%" == "" (
   set target=build-ci
)

set config=%2
if "%config%" == "" (
   set config=Debug
)

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Build.proj /t:"%target%" /p:Configuration="%config%" /m /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false