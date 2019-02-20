@echo off
set /p version=Enter Releaseversion:  

if [%version%] == [] (
  echo No version specified! Please specify a valid version like 1.2.3!
  goto Done
)
if [%version%] == [""] (
  echo No version specified! Please specify a valid version like 1.2.3!
  goto Done
)

echo version: %version%

echo Restore solution
dotnet restore microwf.sln

echo Packaging solution
dotnet pack src/microwf.Core -c Release /p:PackageVersion=%version% /p:Version=%version% -o ./../../dist/nupkgs
dotnet pack src/microwf.AspNetCoreEngine -c Release /p:PackageVersion=%version% /p:Version=%version% -o ./../../dist/nupkgs

:Done
echo Done
