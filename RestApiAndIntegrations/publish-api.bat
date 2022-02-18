call set-build-env.bat
set project-name=API
set project-folder=API
echo publishing %project-name% @ %date% %time%
dotnet publish "%project-folder%" --configuration "Release" ^
    --framework "netcoreapp3.1" ^
    --runtime "win-x64" ^
    --force ^
    --nologo ^
    -p:PublishReadyToRun=false ^
    -p:PublishSingleFile=true ^
    -p:PublishTrimmed=false ^
    --self-contained true ^
    --verbosity %verbosity% ^
    --output "%publish-dest%%project-name%" ^
    --version-suffix "%version-suffix%"

@echo off
REM dotnet publish [<PROJECT>|<SOLUTION>] [-c|--configuration <CONFIGURATION>]
REM     [-f|--framework <FRAMEWORK>] [--force] [--interactive]
REM     [--manifest <PATH_TO_MANIFEST_FILE>] [--no-build] [--no-dependencies]
REM     [--no-restore] [--nologo] [-o|--output <OUTPUT_DIRECTORY>]
REM     [-p:PublishReadyToRun=true] [-p:PublishSingleFile=true] [-p:PublishTrimmed=true]
REM     [-r|--runtime <RUNTIME_IDENTIFIER>] [--self-contained [true|false]]
REM     [--no-self-contained] [-v|--verbosity <LEVEL>]
REM     [--version-suffix <VERSION_SUFFIX>]
