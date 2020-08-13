#!/bin/sh

if [ -z "$1" ]
then
  echo No version specified
  exit 1
else
  echo version $1
fi

echo Prepare build environment
if [ -d "publish" ]
then
  rm -Rf publish;
fi

echo Restore solution
dotnet restore samples/WebApi/WebApi.csproj

echo Building solution
dotnet publish samples/WebApi/WebApi.csproj -c Release -o ./publish/

# echo Deploy web app
# WebClient comes built in with WebApi
# cd samples/WebClient && npm i && npm run build-webclient && cd ../..;

echo Building docker image tomware/microwf-playground:$1
#echo $PWD
docker build --build-arg source=publish -t tomware/microwf-playground:$1 .

#echo Cleaning up
#if [ -d "publish" ]
#then
#  rm -Rf publish;
#fi

echo Done
docker images