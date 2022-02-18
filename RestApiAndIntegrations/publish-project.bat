echo publishing %project-name% @ %date% %time%
echo publish type: %1
if [%1] == ["dll"] (
  echo "Publishing as DLL"
  set PublishSingleFile=false
) else (
  echo "Publishing as EXE"
  set PublishSingleFile=true
)
dotnet publish "%project-folder%" --configuration "Release" ^
    --framework "netcoreapp3.1" ^
    --runtime "win-x64" ^
    --force ^
    --nologo ^
    -p:PublishReadyToRun=true ^
    -p:PublishSingleFile=%PublishSingleFile% ^
    -p:PublishTrimmed=false ^
    --no-self-contained ^
    --verbosity %verbosity% ^
    --output "%publish-dest%%project-name%\%version-suffix%" ^
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
