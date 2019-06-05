if "%~1" == "" goto invalid_argument

if exist "%~1" goto build_exists

cd src/Frontend
dotnet publish --configuration Release
if errorlevel 1 goto build_error

cd ../BackendApi
dotnet publish --configuration Release
if errorlevel 1 goto build_error

cd ../TextListener
dotnet publish --configuration Release
if errorlevel 1 goto build_error

cd ../TextRankCalc
dotnet publish --configuration Release
if errorlevel 1 goto build_error

cd ../TextStatistics
dotnet publish --configuration Release
if errorlevel 1 goto build_error

cd ../VowelConsCounter
dotnet publish --configuration Release
if errorlevel 1 goto build_error

cd ../VowelConsRater
dotnet publish --configuration Release
if errorlevel 1 goto build_error

cd ../..                         

mkdir "%~1"\Frontend
mkdir "%~1"\BackendApi
mkdir "%~1"\TextListener
mkdir "%~1"\TextRankCalc
mkdir "%~1"\TextStatistics
mkdir "%~1"\VowelConsCounter
mkdir "%~1"\VowelConsRater

xcopy src\Frontend\bin\Release\netcoreapp2.2 "%~1"\Frontend\
xcopy src\BackendApi\bin\Release\netcoreapp2.2 "%~1"\BackendApi\
xcopy src\TextListener\bin\Release\netcoreapp2.2 "%~1"\TextListener\
xcopy src\TextRankCalc\bin\Release\netcoreapp2.2 "%~1"\TextRankCalc\
xcopy src\TextStatistics\bin\Release\netcoreapp2.2 "%~1"\TextStatistics\
xcopy src\VowelConsCounter\bin\Release\netcoreapp2.2 "%~1"\VowelConsCounter\
xcopy src\VowelConsRater\bin\Release\netcoreapp2.2 "%~1"\VowelConsRater\
xcopy run_vowel_counter_rater.bat "%~1"

echo VowelConsCounter:3 > "%~1"\config.txt
echo VowelConsRater:2 >> "%~1"\config.txt

echo start "Frontend" dotnet Frontend\Frontend.dll > "%~1"\run.bat
echo start "BackendApi" dotnet BackendApi\BackendApi.dll >> "%~1"\run.bat
echo start "TextListener" dotnet TextListener\TextListener.dll >> "%~1"\run.bat
echo start "TextRankCalc" dotnet TextRankCalc\TextRankCalc.dll >> "%~1"\run.bat
echo start "TextStatistics" dotnet TextStatistics\TextStatistics.dll >> "%~1"\run.bat
echo call run_vowel_counter_rater.bat >> "%~1"\run.bat

echo taskkill /IM dotnet.exe /F > "%~1"\stop.bat

echo BUILD SUCCESS
exit 0

:invalid_argument
    echo Incorrect number of arguments.
    echo Example: build.cmd <MAJOR.MINOR.PATCH>
    exit 1

:build_error
    echo Failed to build project.
    exit 1	

:build_exists
   echo Build "%~1" already exists.
   exit 1