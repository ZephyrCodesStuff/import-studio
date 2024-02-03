#!/bin/sh

cd ../Import
rm -rf bin
rm -rf obj
dotnet clean
dotnet publish -r osx-x64 -c Release
fileicon set bin/Release/net5.0/osx-x64/publish/Import icon.ico

echo

cd ../ImportUpdate
rm -rf bin
rm -rf obj
dotnet clean
dotnet publish -r osx-x64 -c Release
fileicon set bin/Release/net5.0/osx-x64/publish/ImportUpdate icon.ico

echo
echo Merging...

cd ..
rm -rf Build
mkdir Build
cd Build

mkdir Import
mkdir M4L
mkdir Update

cp -r ../Import/bin/Release/net5.0/osx-x64/publish/* Import
cp -r ../ImportUpdate/bin/Release/net5.0/osx-x64/publish/* Update

cp ../M4L/*.amxd M4L

echo Creating macOS Package...

cd ..
rm -rf Dist
mkdir Dist

packagesbuild Publish/Import.pkgproj

echo Done.