# SyncVerse Studio - Point of Sale System

A comprehensive POS system built with .NET 8 and Windows Forms with role-based authentication and inventory management.

## Quick Start

| Step | Command |
|------|---------|
| 1. Clone | `git clone https://github.com/EIRSVi/Sync-Verse-Studio.git` |
| 2. Navigate | `cd syncversestudio` |
| 3. Restore | `dotnet restore` |
| 4. Build | `dotnet build` |
| 5. Run | `dotnet run --project syncversestudio` |

## System Requirements

| Component | Requirement |
|-----------|-------------|
| Operating System | Windows 10/11 (64-bit) |
| Framework | .NET 8 SDK |
| Database | SQL Server 2019+, Express, or LocalDB |
| IDE | Visual Studio 2022 (optional) |
| RAM | Minimum 4GB |
| Storage | 500MB |

## Database Connection Strings

| SQL Server Type | Connection String Example |
|----------------|---------------------------|
| SQL Server Express | `Data Source=.\SQLEXPRESS;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;` |
| SQL Server LocalDB | `Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;` |
| SQL Server Full | `Data Source=localhost;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;` |

**Update connection string in:** `Data\ApplicationDbContext.cs` (OnConfiguring method)

## Default Login
TEST CREDENTIALS:
| Field | Value |
|-------|-------| 
| Username | `vi` |
| Password | `vi` |
| Role | Administrator |

## Database Schema

| Entity | Description |
|--------|-------------|
| Users | Authentication and role management |
| Categories | Product organization |
| Suppliers | Vendor information |
| Products | Inventory items with pricing and stock |
| Customers | Customer database with loyalty points |
| Sales | Sales transactions |
| SaleItems | Sale line items |
| InventoryMovements | Stock tracking history |
| AuditLogs | Security and compliance logging |

## Features by Role

| Role | Access |
|------|--------|
| **Administrator** | Full system access, user management, all reports, audit logs |
| **Cashier** | POS system, sales processing, customer management, personal sales history |
| **Inventory Clerk** | Product management, categories, suppliers, stock reports |

## Technology Stack

| Category | Technology |
|----------|-----------|
| Framework | .NET 8.0 Windows Forms, C# 12.0 |
| Database | Entity Framework Core 8.0, SQL Server |
| Security | BCrypt.Net-Next 4.0.3 |
| UI | FontAwesome.Sharp 6.3.0 |
| Barcode | ZXing.Net 0.16.9 |
| PDF | QuestPDF 2023.12.6 |

## Entity Framework Commands

| Task | Command |
|------|---------|
| Install EF Tools | `dotnet tool install --global dotnet-ef` |
| Create Migration | `dotnet ef migrations add MigrationName --project syncversestudio` |
| Update Database | `dotnet ef database update --project syncversestudio` |
| Drop Database | `dotnet ef database drop --project syncversestudio` |
| View SQL Script | `dotnet ef migrations script --project syncversestudio` |

## Common Issues

| Problem | Solution |
|---------|----------|
| Cannot connect to SQL Server | 1. Check SQL Server service is running (services.msc)<br>2. Verify connection string<br>3. Test with: `sqlcmd -S YOUR_SERVER_NAME -E` |
| Missing NuGet packages | `dotnet restore && dotnet clean && dotnet build` |
| SDK version mismatch | Verify .NET 8: `dotnet --version` |
| Login fails | Ensure database is initialized, check Users table exists |
| Database already exists | `dotnet ef database drop --project syncversestudio` |

## Project Structure

```
syncversestudio/
├── Data/
│   └── ApplicationDbContext.cs     # EF Core DbContext
├── Models/           # Entity classes
├── Services/       # Business logic
├── Views/     # UI forms
├── Helpers/    # Utility classes
├── Form1.cs   # Login form
├── Program.cs         # Entry point
└── syncversestudio.csproj          # Project file
```

## Installation Methods

### Method 1: Visual Studio
1. Open `syncversestudio.sln`
2. Press F5 to run

### Method 2: .NET CLI
```bash
dotnet restore
dotnet build
dotnet run --project syncversestudio
```

### Method 3: Batch File
Double-click `RunApplication.bat`

## Verify SQL Server

```bash
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

## Development Workflow

| Task | Steps |
|------|-------|
| Add New Entity | 1. Create class in Models/<br>2. Add DbSet to ApplicationDbContext<br>3. Configure in OnModelCreating<br>4. Create migration<br>5. Update database |
| Modify Entity | 1. Update entity class<br>2. Create migration<br>3. Review migration<br>4. Apply migration |

## Support

- GitHub: https://github.com/EIRSVi/Sync-Verse-Studio
- Issues: Report via GitHub Issues
- Contributions: Submit pull requests

---

Built with .NET 8 and Entity Framework Core