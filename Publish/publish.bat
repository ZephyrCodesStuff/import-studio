@echo off

cd ..\Import
rd /S /Q bin
rd /S /Q obj
dotnet clean
dotnet publish -r win-x64 -c Release
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\VC\Tools\MSVC\14.29.30037\bin\Hostx64\x64\editbin.exe" /subsystem:windows bin\Release\net5.0\win-x64\publish\Import.exe >nul 2>&1

echo.

cd ..\ImportUpdate
rd /S /Q bin
rd /S /Q obj
dotnet clean
dotnet publish -r win-x64 -c Release
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\VC\Tools\MSVC\14.29.30037\bin\Hostx64\x64\editbin.exe" /subsystem:windows bin\Release\net5.0\win-x64\publish\ImportUpdate.exe >nul 2>&1

echo.
echo Merging...

cd ..
rd /S /Q Build >nul 2>&1
mkdir Build
cd Build

mkdir Import
mkdir M4L
mkdir Update

robocopy ..\Import\bin\Release\net5.0\win-x64\publish Import /E >nul 2>&1
robocopy ..\ImportUpdate\bin\Release\net5.0\win-x64\publish Update /E >nul 2>&1

robocopy ..\M4L M4L *.amxd >nul 2>&1

echo Creating Windows Installer...

cd ..
rd /S /Q Dist >nul 2>&1
mkdir Dist

"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /q Publish\Import.iss

echo Done.