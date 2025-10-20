@echo off
title SyncVerse Studio - POS System
color 0A

echo.
echo  ========================================================
echo  ^|                  SyncVerse Studio                   ^|
echo  ^|              Point of Sale System                   ^|
echo  ^|                   Version 2.0                       ^|
echo  ========================================================
echo.
echo  Welcome to SyncVerse Studio POS System!
echo  This application provides a complete point-of-sale
echo  solution with role-based access and real-time analytics.
echo.
echo  SYSTEM REQUIREMENTS:
echo  - Windows 10/11 (64-bit)
echo  - .NET 8 Runtime
echo  - SQL Server (LocalDB/Express/Full)
echo  - 4GB RAM minimum
echo  - 1920x1080 resolution minimum
echo.
echo  DEFAULT LOGIN CREDENTIALS:
echo  Username: vi
echo  Password: vi
echo  Role: Administrator
echo.
echo  FEATURES:
echo  - Real-time dashboard analytics
echo  - Complete inventory management
echo  - Role-based user access (Admin/Cashier/Inventory Clerk)
echo  - Secure authentication with BCrypt
echo  - Modern Material Design UI
echo  - Comprehensive audit logging
echo.
echo  The application will automatically:
echo  1. Initialize the database and create sample data
echo  2. Set up user accounts and permissions
echo  3. Load the professional login interface
echo.
echo  Press any key to start SyncVerse Studio...
pause >nul

cls
echo.
echo  Starting SyncVerse Studio POS System...
echo  Please wait while the application initializes...
echo.

REM Check if .NET 8 is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo  ERROR: .NET 8 Runtime is not installed!
    echo  Please download and install .NET 8 from:
    echo  https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

REM Build and run the application
echo  Building the application...
dotnet build syncversestudio --configuration Release --verbosity quiet
if errorlevel 1 (
    echo  ERROR: Build failed! Please check the source code.
    pause
    exit /b 1
)

echo  Launching SyncVerse Studio...
echo.
dotnet run --project syncversestudio --configuration Release

REM Check if application exited with error
if errorlevel 1 (
    echo.
    echo  Application encountered an error during execution.
    echo  Please check the error messages above and try again.
    echo.
    pause
)

echo.
echo  Thank you for using SyncVerse Studio!
echo  Application has been closed successfully.
pause