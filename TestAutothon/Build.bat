@echo off
msbuild.exe "TestAutothon.sln" /p:configuration=debug /p:TargetFrameworkVersion=v4.7.2
pause
@echo on