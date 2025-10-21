# SyncVerse Studio - Point of Sale System

A comprehensive Point of Sale (POS) system built with .NET 8 and Windows Forms, featuring role-based authentication, inventory management, and real-time analytics.

## Prerequisites

Before running the application, ensure you have the following installed:

1. Windows 10 or Windows 11 (64-bit)
2. .NET 8 SDK - Download from https://dotnet.microsoft.com/download/dotnet/8.0
3. SQL Server (one of the following):
   - SQL Server 2019 or later (Full edition)
   - SQL Server Express (Free edition)
   - SQL Server LocalDB (Included with Visual Studio)
4. Visual Studio 2022 (optional but recommended) or any code editor
5. Minimum 4GB RAM
6. 500MB available disk space

## Database Setup

### Step 1: Install SQL Server

If you do not have SQL Server installed, choose one of these options:

**Option A: SQL Server Express (Recommended for local development)**
1. Download SQL Server Express from https://www.microsoft.com/sql-server/sql-server-downloads
2. Run the installer and select "Basic" installation
3. Note down the server instance name (usually "LOCALHOST\SQLEXPRESS" or ".\SQLEXPRESS")

**Option B: SQL Server LocalDB (Lightweight option)**
1. Install Visual Studio 2022 (includes LocalDB by default)
2. Or download SQL Server Express LocalDB separately
3. Default instance is "(localdb)\MSSQLLocalDB"

### Step 2: Verify SQL Server Installation

Open Command Prompt or PowerShell and run:

```bash
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

If successful, you will see your SQL Server version information.

### Step 3: Configure Database Connection String

1. Navigate to the project directory
2. Open `Data\ApplicationDbContext.cs`
3. Locate the `OnConfiguring` method
4. Update the connection string to match your SQL Server instance:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer("Data Source=YOUR_SERVER_NAME;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;");
}
```

Replace `YOUR_SERVER_NAME` with one of the following:

- For SQL Server Express: `localhost\SQLEXPRESS` or `.\SQLEXPRESS`
- For SQL Server LocalDB: `(localdb)\MSSQLLocalDB`
- For SQL Server Full: `localhost` or `YOUR_COMPUTER_NAME`
- For Named Instance: `YOUR_COMPUTER_NAME\INSTANCE_NAME`

Example connection strings:

```
SQL Server Express:
Data Source=.\SQLEXPRESS;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;

SQL Server LocalDB:
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;

SQL Server Full (localhost):
Data Source=localhost;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;
```

## Installation and Setup

### Method 1: Using Visual Studio 2022

1. Clone or download the repository:
```bash
git clone https://github.com/EIRSVi/Sync-Verse-Studio.git
cd syncversestudio
```

2. Open the solution file:
   - Double-click `syncversestudio.sln` to open in Visual Studio

3. Restore NuGet packages:
   - Visual Studio will automatically restore packages when you open the solution
   - Or manually: Right-click solution > Restore NuGet Packages

4. Build the solution:
   - Press Ctrl+Shift+B or go to Build > Build Solution

5. Run the application:
   - Press F5 to run with debugging
   - Or Press Ctrl+F5 to run without debugging

### Method 2: Using .NET CLI

1. Clone the repository:
```bash
git clone https://github.com/EIRSVi/Sync-Verse-Studio.git
cd syncversestudio
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run --project syncversestudio
```

### Method 3: Using the Batch File (Windows Only)

1. Download and extract the project to your local machine

2. Navigate to the project root directory

3. Double-click `RunApplication.bat`

4. The batch file will:
   - Check for .NET 8 installation
   - Build the project
   - Launch the application

## Entity Framework Core Database Initialization

The application uses Entity Framework Core Code-First approach. The database will be automatically created on first run.

### Automatic Database Creation

When you run the application for the first time:

1. The application will automatically create the database named `khmerdatabase`
2. All tables will be created based on the entity models
3. Sample data will be seeded including:
   - Default administrator account
   - Sample categories (Electronics, Beverages, Snacks, Stationery)
   - Sample suppliers
   - Sample products
   - Guest customer account

This is handled by the `DatabaseInitializer` class which runs on application startup.

### Manual Database Migration (Optional)

If you prefer to manage database schema using migrations:

1. Install Entity Framework Core tools (if not already installed):
```bash
dotnet tool install --global dotnet-ef
```

2. Create an initial migration:
```bash
dotnet ef migrations add InitialCreate --project syncversestudio
```

3. Apply the migration to create the database:
```bash
dotnet ef database update --project syncversestudio
```

4. View migration SQL (optional):
```bash
dotnet ef migrations script --project syncversestudio
```

### Database Schema

The application includes the following entities:

- **Users**: Authentication and role management (Administrator, Cashier, InventoryClerk)
- **Categories**: Product organization and grouping
- **Suppliers**: Vendor and supplier information
- **Products**: Inventory items with pricing, stock levels, and barcodes
- **Customers**: Customer database with loyalty points
- **Sales**: Sales transactions and invoices
- **SaleItems**: Individual items in each sale
- **InventoryMovements**: Stock tracking and movement history
- **AuditLogs**: Security and compliance logging

## Default Login Credentials

After the database is initialized, use these credentials to log in:

**Administrator Account:**
- Username: `vi`
- Password: `vi`
- Role: Administrator
- Full system access

## Application Features by Role

### Administrator
- Dashboard with system-wide analytics
- User management (create, edit, delete users)
- Product management (full CRUD operations)
- Category and supplier management
- Point of sale access
- Sales history and reports
- Customer management
- Inventory reports
- Analytics dashboard
- Audit log viewing

### Cashier
- Dashboard with personal performance metrics
- Point of sale (POS) system
- Sales processing and transactions
- Receipt generation
- Customer management
- Sales history (own transactions)

### Inventory Clerk
- Dashboard with inventory metrics
- Product management
- Category management
- Supplier management
- Stock level monitoring
- Inventory reports
- Stock adjustment capabilities

## Technology Stack

### Core Framework
- .NET 8.0 Windows Forms Application
- C# 12.0

### Database
- Entity Framework Core 8.0
- SQL Server (Express/Full/LocalDB)

### NuGet Packages
- Microsoft.EntityFrameworkCore 8.0.0
- Microsoft.EntityFrameworkCore.SqlServer 8.0.0
- Microsoft.EntityFrameworkCore.Tools 8.0.0
- BCrypt.Net-Next 4.0.3 (Password hashing)
- FontAwesome.Sharp 6.3.0 (UI icons)
- ZXing.Net 0.16.9 (Barcode generation)
- QuestPDF 2023.12.6 (PDF generation)
- System.Drawing.Common 8.0.0

## Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to SQL Server

**Solutions**:
1. Verify SQL Server is running:
   - Open Services (services.msc)
   - Look for "SQL Server (MSSQLSERVER)" or "SQL Server (SQLEXPRESS)"
   - Ensure the service is running

2. Check SQL Server Configuration Manager:
   - Enable TCP/IP protocol
   - Restart SQL Server service

3. Verify connection string:
   - Check server name matches your SQL Server instance
   - Ensure "Integrated Security=True" for Windows Authentication

4. Test connection using SQLCMD:
```bash
sqlcmd -S YOUR_SERVER_NAME -E
```

### Entity Framework Errors

**Problem**: "A network-related or instance-specific error occurred"

**Solution**: 
- Verify SQL Server service is running
- Check firewall settings
- Ensure correct server name in connection string

**Problem**: "Login failed for user"

**Solution**:
- Use Windows Authentication (Integrated Security=True)
- Or configure SQL Server Authentication and update connection string

**Problem**: "Database already exists" error during migration

**Solution**:
```bash
dotnet ef database drop --project syncversestudio
dotnet ef database update --project syncversestudio
```

### Build Errors

**Problem**: Missing NuGet packages

**Solution**:
```bash
dotnet restore
dotnet clean
dotnet build
```

**Problem**: SDK version mismatch

**Solution**:
- Verify .NET 8 SDK is installed: `dotnet --version`
- Download from https://dotnet.microsoft.com/download/dotnet/8.0

### Runtime Errors

**Problem**: Application crashes on startup

**Solution**:
1. Check the Output window in Visual Studio for error details
2. Verify database connection string
3. Ensure all NuGet packages are restored
4. Check SQL Server is accessible

**Problem**: Login fails with correct credentials

**Solution**:
- Database may not be initialized
- Check if Users table exists and contains the admin user
- Re-run database initialization

## Project Structure

```
syncversestudio/
??? Data/
?   ??? ApplicationDbContext.cs       # EF Core DbContext
??? Models/
?   ??? User.cs             # User entity
?   ??? Product.cs        # Product entity
?   ??? Category.cs     # Category entity
?   ??? Supplier.cs     # Supplier entity
?   ??? Customer.cs            # Customer entity
?   ??? Sale.cs            # Sale entity
?   ??? SaleItem.cs   # SaleItem entity
?   ??? InventoryMovement.cs           # Inventory tracking
?   ??? AuditLog.cs      # Audit logging
??? Services/
?   ??? AuthenticationService.cs       # User authentication
?   ??? DatabaseInitializer.cs # Database setup
??? Views/
?   ??? MainDashboard.cs        # Main application UI
?   ??? DashboardView.cs    # Dashboard analytics
?   ??? ProductManagementView.cs       # Product CRUD
?   ??? PointOfSaleView.cs  # POS system
?   ??? [Other views...]
??? Helpers/
?   ??? IconHelper.cs    # Icon management
??? Form1.cs     # Login form
??? Program.cs             # Application entry point
??? syncversestudio.csproj     # Project file
```

## Development Guidelines

### Adding New Entities

1. Create the entity class in Models folder
2. Add DbSet property to ApplicationDbContext
3. Configure relationships in OnModelCreating
4. Create migration: `dotnet ef migrations add AddNewEntity`
5. Update database: `dotnet ef database update`

### Modifying Existing Entities

1. Update the entity class
2. Create migration: `dotnet ef migrations add UpdateEntity`
3. Review generated migration code
4. Apply migration: `dotnet ef database update`

### Connection String Configuration

For production deployment, consider using:
- appsettings.json for configuration
- Environment variables for sensitive data
- Azure Key Vault for secrets management

## Support and Contribution

For issues, questions, or contributions:

1. GitHub Repository: https://github.com/EIRSVi/Sync-Verse-Studio
2. Report bugs via GitHub Issues
3. Submit pull requests for improvements

## License

This project is open source. Check the LICENSE file for details.

---

Built with .NET 8 and Entity Framework Core