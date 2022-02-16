@echo off
call clean-directories.bat
dotnet build "CAAPS API Full Solution\CAAPS_API_FULL_SOLUTION.sln" -c Release --nologo -v quiet
echo ERRORLEVEL: %ERRORLEVEL%
echo DONE
REM dotnet clean CAAPS_API.sln
REM cd Clients\ELANOR
REM dotnet clean
REM cd ..\..
REM cd Clients\URBANISE
REM dotnet clean
REM cd ..\..
REM cd Clients\JLLGST
REM dotnet clean
REM cd ..\..
REM dotnet build CAAPS_API.sln
REM cd Clients\ELANOR
REM dotnet build
REM cd ..\..
REM cd Clients\URBANISE
REM dotnet build
REM cd ..\..
REM cd Clients\JLLGST
REM dotnet build
REM cd ..\..
