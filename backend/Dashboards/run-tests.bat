@echo off
echo ========================================
echo Running tests and generating HTML report
echo ========================================
echo.

REM Show current directory
echo Current folder: %cd%
echo.

REM Delete old results
if exist TestResults rmdir /s /q TestResults
if exist coveragereport rmdir /s /q coveragereport

REM Run tests
echo [1/3] Running tests...
dotnet test --collect:"XPlat Code Coverage" --results-directory "TestResults"

REM Find the coverage file
echo.
echo [2/3] Looking for coverage file...
dir TestResults\*.xml /s /b

REM Generate HTML report with full path
echo.
echo [2/3] Generating HTML report...
for /r "TestResults" %%i in (*.cobertura.xml) do (
    echo Found: %%i
    reportgenerator -reports:"%%i" -targetdir:"coveragereport" -reporttypes:Html
)

REM Open report in browser
echo.
echo [3/3] Opening report in browser...
if exist coveragereport\index.html (
    start coveragereport\index.html
) else (
    echo WARNING: Report file not found!
    echo Check coveragereport folder manually.
)

echo.
echo ========================================
echo Done!
echo ========================================
pause