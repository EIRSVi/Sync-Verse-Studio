---
inclusion: always
---

# Technology Stack

## Core Framework

- **.NET 8.0** (Windows Desktop)
- **Windows Forms** for UI
- **C# 12** with implicit usings enabled
- **Entity Framework Core 8.0** for data access

## Database

- **SQL Server LocalDB** (development)
- **Entity Framework Core** with Code-First approach
- Connection string configured in `ApplicationDbContext.cs`

## Key Libraries

- **FontAwesome.Sharp 6.3.0** - Icon library
- **MaterialSkin.2 2.1.0** - Modern UI components
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **QRCoder 1.7.0** - QR code generation
- **ZXing.Net 0.16.9** - Barcode scanning
- **QuestPDF 2023.12.6** - PDF invoice generation
- **Newtonsoft.Json 13.0.3** - JSON serialization
- **System.Drawing.Common 8.0.0** - Image handling

## Architecture Patterns

- **MVC Pattern**: Model-View-Controller separation
- **Repository Pattern**: Data access abstraction via DbContext
- **Service Layer**: Business logic in Services folder
- **Dependency Injection**: Loose coupling for services

## Common Commands

### Build & Run
```bash
# Build solution
dotnet build syncversestudio/syncversestudio.csproj

# Run application
dotnet run --project syncversestudio/syncversestudio.csproj

# Build for release
dotnet build syncversestudio/syncversestudio.csproj --configuration Release
```

### Database Operations
```bash
# Create new migration
dotnet ef migrations add MigrationName --project syncversestudio

# Apply migrations to database
dotnet ef database update --project syncversestudio

# Remove last migration
dotnet ef migrations remove --project syncversestudio
```

### Project Structure
```bash
# Restore NuGet packages
dotnet restore

# Clean build artifacts
dotnet clean
```

## Compiler Warnings Suppressed

The project suppresses these warnings (see .csproj):
- CS8618, CS8622, CS8602, CS8604, CS8600, CS8603 (nullable reference types)
- CS1998 (async without await)
- CS0168, CS0169 (unused variables)
- CS4014 (unawaited async calls)

## Entry Point

- Main entry: `syncversestudio/Program.cs`
- Initial form: `LoginForm`
- QuestPDF license set to Community in Program.cs
