#!/bin/sh

if [ -z "$1" ]
then
  echo No version specified! Please specify a valid version like 1.2.3!
  exit 1
else
  echo version $1
fi

echo Restore solution
dotnet restore microwf.sln

echo Packaging solution
dotnet pack src/microwf.Core -c Release /p:PackageVersion=$1 /p:Version=$1 -o ./dist/nupkgs/
dotnet pack src/microwf.AspNetCoreEngine -c Release /p:PackageVersion=$1 /p:Version=$1 -o ./dist/nupkgs/

echo Done
