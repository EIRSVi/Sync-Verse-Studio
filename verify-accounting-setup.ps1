# Accounting System Verification Script
# Run this to check if everything is set up correctly

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Accounting System Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if files exist
Write-Host "Checking files..." -ForegroundColor Yellow

$files = @(
    "Models\AccountingModels.cs",
    "Views\AccountingReportsView.cs",
    "Services\AccountingService.cs",
    "Database\AddAccountingTables.sql",
    "Data\ApplicationDbContext.cs",
    "Views\MainDashboard.cs"
)

$allFilesExist = $true
foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  ✓ $file" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $file (MISSING!)" -ForegroundColor Red
        $allFilesExist = $false
    }
}

Write-Host ""

if (-not $allFilesExist) {
    Write-Host "ERROR: Some files are missing!" -ForegroundColor Red
    Write-Host "Please make sure all files were created correctly." -ForegroundColor Red
    exit 1
}

# Check if AccountingReportsView is referenced in MainDashboard
Write-Host "Checking MainDashboard.cs for Accounting Reports menu..." -ForegroundColor Yellow

$mainDashboardContent = Get-Content "Views\MainDashboard.cs" -Raw

if ($mainDashboardContent -match "Accounting Reports") {
    Write-Host "  ✓ Menu item found in MainDashboard.cs" -ForegroundColor Green
} else {
    Write-Host "  ✗ Menu item NOT found in MainDashboard.cs" -ForegroundColor Red
    Write-Host "    The menu item should be added to all user roles." -ForegroundColor Yellow
}

if ($mainDashboardContent -match "AccountingReportsView") {
    Write-Host "  ✓ AccountingReportsView class referenced" -ForegroundColor Green
} else {
    Write-Host "  ✗ AccountingReportsView class NOT referenced" -ForegroundColor Red
}

Write-Host ""

# Check documentation
Write-Host "Checking documentation..." -ForegroundColor Yellow

$docs = @(
    "GUIDE\ACCOUNTING_SYSTEM_COMPLETE.md",
    "GUIDE\ACCOUNTING_QUICK_START.md",
    "GUIDE\ACCOUNTING_FEATURES_SUMMARY.md",
    "ACCOUNTING_INSTALLATION_CHECKLIST.md",
    "START_HERE.md"
)

foreach ($doc in $docs) {
    if (Test-Path $doc) {
        Write-Host "  ✓ $doc" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $doc (missing)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Next Steps" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Run Database Migration:" -ForegroundColor Yellow
Write-Host "   - Open SQL Server Management Studio" -ForegroundColor White
Write-Host "   - Connect to: DESKTOP-6RCREN5\MSSQLSERVER01" -ForegroundColor White
Write-Host "   - Open: Database\AddAccountingTables.sql" -ForegroundColor White
Write-Host "   - Execute the script (F5)" -ForegroundColor White
Write-Host ""
Write-Host "2. Rebuild Application:" -ForegroundColor Yellow
Write-Host "   - In Visual Studio: Build > Rebuild Solution" -ForegroundColor White
Write-Host "   - Or run: dotnet clean && dotnet build" -ForegroundColor White
Write-Host ""
Write-Host "3. Run Application:" -ForegroundColor Yellow
Write-Host "   - Press F5 in Visual Studio" -ForegroundColor White
Write-Host "   - Login and look for 'Accounting Reports' in sidebar" -ForegroundColor White
Write-Host ""
Write-Host "For detailed instructions, see: START_HERE.md" -ForegroundColor Cyan
Write-Host ""
