@echo off
REM Motel Management System - Professional Startup Script for Windows
REM ===================================================================

echo =========================================
echo   Motel Management System
echo   موتيل دهب - نظام الإدارة
echo =========================================
echo.

cd src\Motel.Web

echo [1/4] Checking .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET 8 SDK from: https://dotnet.microsoft.com/download
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo [OK] .NET SDK %DOTNET_VERSION% found
echo.

echo [2/4] Restoring NuGet packages...
dotnet restore
if errorlevel 1 (
    echo ERROR: Package restore failed!
    exit /b 1
)
echo [OK] Packages restored successfully
echo.

echo [3/4] Building application...
dotnet build --no-restore
if errorlevel 1 (
    echo ERROR: Build failed!
    exit /b 1
)
echo [OK] Build completed successfully
echo.

echo [4/4] Starting application...
echo.
echo ==========================================
echo   Application starting...
echo   Open: http://localhost:5000
echo   or:   https://localhost:5001
echo ==========================================
echo.

dotnet run --no-build
