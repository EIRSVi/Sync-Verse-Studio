# SyncVerse Studio - Project Reorganization Script
# This script fixes the double nesting issue: syncversestudio\syncversestudio -> syncversestudio

Write-Host "=======================================" -ForegroundColor Cyan
Write-Host " SyncVerse Studio - Project Reorganization" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""

# Set paths
$rootPath = "V:\Github\syncversestudio"
$nestedPath = "$rootPath\syncversestudio"
$tempPath = "$rootPath\temp_reorganize"

# Check if Visual Studio is running
$vsRunning = Get-Process | Where-Object { $_.ProcessName -like "*devenv*" }
if ($vsRunning) {
 Write-Host "WARNING: Visual Studio is running!" -ForegroundColor Red
    Write-Host "Please close Visual Studio before running this script." -ForegroundColor Yellow
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit
}

Write-Host "Step 1: Analyzing structure..." -ForegroundColor Yellow
Write-Host "Root path: $rootPath"
Write-Host "Nested path: $nestedPath"
Write-Host ""

# Confirm with user
Write-Host "This script will:" -ForegroundColor Cyan
Write-Host "1. Move files from $nestedPath to root"
Write-Host "2. Remove duplicate folders"
Write-Host "3. Update project structure"
Write-Host ""
Write-Host "A backup has already been created." -ForegroundColor Green
Write-Host ""
$confirm = Read-Host "Do you want to continue? (yes/no)"

if ($confirm -ne "yes") {
    Write-Host "Operation cancelled." -ForegroundColor Red
    exit
}

try {
    Write-Host "`nStep 2: Creating temporary folder..." -ForegroundColor Yellow
    if (Test-Path $tempPath) {
  Remove-Item -Path $tempPath -Recurse -Force
    }
    New-Item -Path $tempPath -ItemType Directory | Out-Null

    Write-Host "Step 3: Moving unique files from nested folder..." -ForegroundColor Yellow
    
    # Files to move from nested folder
    $filesToMove = @(
        "syncversestudio.csproj",
        "syncversestudio.csproj.user",
 "Program.cs",
        "Form1.cs",
    "Form1.Designer.cs",
 "Form1.resx",
        "GlobalUsings.cs"
    )
    
    foreach ($file in $filesToMove) {
        $sourcePath = Join-Path $nestedPath $file
        if (Test-Path $sourcePath) {
    Write-Host "  Moving $file..." -ForegroundColor Gray
            Move-Item -Path $sourcePath -Destination $rootPath -Force
        }
    }

    Write-Host "Step 4: Moving bin and obj folders..." -ForegroundColor Yellow
    $binPath = Join-Path $nestedPath "bin"
    $objPath = Join-Path $nestedPath "obj"
    
    if (Test-Path $binPath) {
        Move-Item -Path $binPath -Destination $rootPath -Force
    Write-Host "  Moved bin folder" -ForegroundColor Gray
  }
    
    if (Test-Path $objPath) {
     Move-Item -Path $objPath -Destination $rootPath -Force
        Write-Host "  Moved obj folder" -ForegroundColor Gray
    }

    Write-Host "Step 5: Checking for duplicate folders..." -ForegroundColor Yellow
    
    # Compare and merge folders
  $foldersToCheck = @("Data", "Models", "Services", "Views")
    
    foreach ($folder in $foldersToCheck) {
        $rootFolderPath = Join-Path $rootPath $folder
        $nestedFolderPath = Join-Path $nestedPath $folder

        if (Test-Path $nestedFolderPath) {
     Write-Host "  Comparing $folder folders..." -ForegroundColor Gray
        
     # Get all files in nested folder
            $nestedFiles = Get-ChildItem -Path $nestedFolderPath -Recurse -File
            
      foreach ($file in $nestedFiles) {
    $relativePath = $file.FullName.Substring($nestedFolderPath.Length + 1)
      $rootFilePath = Join-Path $rootFolderPath $relativePath
       
      # Check if file exists in root
     if (Test-Path $rootFilePath) {
         # Compare file dates - keep newer
    $rootFile = Get-Item $rootFilePath
       if ($file.LastWriteTime -gt $rootFile.LastWriteTime) {
       Write-Host "    Updating $relativePath (nested is newer)" -ForegroundColor Cyan
     Copy-Item -Path $file.FullName -Destination $rootFilePath -Force
         }
    } else {
      Write-Host "    Moving new file: $relativePath" -ForegroundColor Green
         $targetDir = Split-Path $rootFilePath -Parent
         if (!(Test-Path $targetDir)) {
            New-Item -Path $targetDir -ItemType Directory -Force | Out-Null
              }
        Copy-Item -Path $file.FullName -Destination $rootFilePath -Force
   }
   }
      }
    }

    Write-Host "Step 6: Removing nested syncversestudio folder..." -ForegroundColor Yellow
    if (Test-Path $nestedPath) {
        Remove-Item -Path $nestedPath -Recurse -Force
        Write-Host "  Nested folder removed" -ForegroundColor Green
    }

    Write-Host "Step 7: Cleaning up temporary files..." -ForegroundColor Yellow
    if (Test-Path $tempPath) {
        Remove-Item -Path $tempPath -Recurse -Force
    }

    Write-Host ""
    Write-Host "=======================================" -ForegroundColor Green
    Write-Host " REORGANIZATION COMPLETE!" -ForegroundColor Green
    Write-Host "=======================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Your project structure is now:" -ForegroundColor Cyan
    Write-Host "V:\Github\syncversestudio\" -ForegroundColor White
    Write-Host "  ??? syncversestudio.sln" -ForegroundColor White
    Write-Host "  ??? syncversestudio.csproj" -ForegroundColor White
    Write-Host "  ??? Views\" -ForegroundColor White
    Write-Host "  ??? Services\" -ForegroundColor White
    Write-Host "  ??? Models\" -ForegroundColor White
    Write-Host "  ??? Data\" -ForegroundColor White
 Write-Host "  ??? ..." -ForegroundColor White
    Write-Host ""
    Write-Host "You can now:" -ForegroundColor Yellow
    Write-Host "1. Open Visual Studio" -ForegroundColor White
    Write-Host "2. Open the solution: V:\Github\syncversestudio\syncversestudio.sln" -ForegroundColor White
    Write-Host "3. Clean and rebuild the solution" -ForegroundColor White
    Write-Host ""
    Write-Host "Backup location: V:\Github\syncversestudio_backup_*" -ForegroundColor Gray
    Write-Host ""

} catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "If you encounter issues, restore from backup:" -ForegroundColor Yellow
    Write-Host "  V:\Github\syncversestudio_backup_*" -ForegroundColor White
    exit 1
}

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
