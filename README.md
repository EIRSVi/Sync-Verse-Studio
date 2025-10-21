# SyncVerse Studio - Point of Sale System

A POS system built with .NET 8 and Windows Forms featuring role-based authentication, inventory management, and real-time transaction processing.

## Table of Contents

1. Installation and Setup
2. System Architecture
3. Database Schema and Entity Relationships
4. System Workflow and Data Flow
5. User Role Permissions
6. Entity Framework Configuration
7. Development Guide
8. Troubleshooting

---

## Installation and Setup

### Quick Start

| Step | Command |
|------|---------|
| 1. Clone Repository | `git clone https://github.com/EIRSVi/Sync-Verse-Studio.git` |
| 2. Navigate Directory | `cd syncversestudio` |
| 3. Restore Dependencies | `dotnet restore` |
| 4. Build Project | `dotnet build` |
| 5. Run Application | `dotnet run --project syncversestudio` |

### System Requirements

| Component | Requirement |
|-----------|-------------|
| Operating System | Windows 10/11 (64-bit) |
| .NET Framework | .NET 8.0 SDK or later |
| Database Server | SQL Server 2019+, Express Edition, or LocalDB |
| IDE | Visual Studio 2022 (recommended) |
| Memory (RAM) | Minimum 4GB |
| Disk Storage | 500MB available space |

### Database Connection Configuration

Update the connection string in `Data\ApplicationDbContext.cs` (OnConfiguring method):

| SQL Server Type | Connection String Format |
|----------------|---------------------------|
| SQL Server Express | `Data Source=.\SQLEXPRESS;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;` |
| SQL Server LocalDB | `Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;` |
| SQL Server Full Edition | `Data Source=localhost;Initial Catalog=khmerdatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;` |

### Default Test Credentials

| Field | Value |
|-------|-------| 
| Username | `vi` |
| Password | `vi` |
| Role | Administrator |

---

## System Architecture

### Technology Stack

| Layer | Component | Technology |
|-------|-----------|-----------|
| Framework | Runtime | .NET 8.0, C# 12.0 |
| Presentation | UI Framework | Windows Forms |
| Presentation | Icons/Graphics | FontAwesome.Sharp 6.3.0 |
| Business Logic | ORM | Entity Framework Core 8.0 |
| Data Access | Database | SQL Server 2019+ |
| Security | Password Hashing | BCrypt.Net-Next 4.0.3 |
| Features | Barcode Generation | ZXing.Net 0.16.9 |
| Features | Report Generation | QuestPDF 2023.12.6 |

### Project Structure

```
syncversestudio/
├── Data/
│   ├── ApplicationDbContext.cs          # EF Core DbContext with entity configuration
│   └── Migrations/                      # Entity Framework migrations
├── Models/                              # Entity and business model classes
│   ├── User.cs
│   ├── Product.cs
│   ├── Sale.cs
│   ├── Customer.cs
│   ├── Category.cs
│   ├── Supplier.cs
│   ├── SaleItem.cs
│   ├── InventoryMovement.cs
│   └── AuditLog.cs
├── Services/                            # Business logic layer
│   ├── AuthenticationService.cs
│   ├── SalesService.cs
│   ├── InventoryService.cs
│   └── ReportService.cs
├── Views/                               # Windows Forms UI
│   ├── LoginForm.cs
│   ├── MainDashboard.cs
│   ├── POSForm.cs
│   ├── InventoryForm.cs
│   └── AdminPanel.cs
├── Helpers/                             # Utility and helper classes
│   ├── BarcodeHelper.cs
│   ├── ValidationHelper.cs
│   ├── ReportHelper.cs
│   └── EncryptionHelper.cs
├── Form1.cs                             # Main application entry form
├── Program.cs                           # Application entry point
└── syncversestudio.csproj              # Project configuration file
```

---

## Database Schema and Entity Relationships

### Entity Overview Table

| Entity Name | Purpose | Key Field | Primary Relationships |
|-------------|---------|-----------|----------------------|
| Users | Authentication and authorization | UserId (PK) | Sales (1:M), InventoryMovements (1:M), AuditLogs (1:M) |
| Categories | Product classification | CategoryId (PK) | Products (1:M) |
| Suppliers | Vendor management | SupplierId (PK) | Products (1:M) |
| Products | Inventory items | ProductId (PK) | Sales (M:M via SaleItem), Categories (M:1), Suppliers (M:1) |
| Customers | Customer profiles | CustomerId (PK) | Sales (1:M) |
| Sales | Transaction headers | SaleId (PK) | SaleItems (1:M), Customers (M:1), Users/Cashier (M:1) |
| SaleItems | Transaction line items | SaleItemId (PK) | Sales (M:1), Products (M:1) |
| InventoryMovements | Stock tracking history | MovementId (PK) | Products (M:1), Users (M:1) |
| AuditLogs | Security event logging | LogId (PK) | Users (M:1) |

### Detailed Entity Specifications

#### Users Entity

```
Users Table Structure:
┌─────────────────────────────────────────────────────┐
│ Column Name      │ Data Type        │ Constraints   │
├─────────────────────────────────────────────────────┤
│ UserId           │ INT              │ PK, Identity  │
│ Username         │ NVARCHAR(100)    │ UNIQUE        │
│ PasswordHash     │ NVARCHAR(255)    │ NOT NULL      │
│ FullName         │ NVARCHAR(150)    │ NOT NULL      │
│ Email            │ NVARCHAR(100)    │ -             │
│ Role             │ NVARCHAR(20)     │ NOT NULL      │
│ IsActive         │ BIT              │ DEFAULT 1     │
│ CreatedDate      │ DATETIME         │ DEFAULT NOW   │
│ LastLogin        │ DATETIME         │ NULLABLE      │
└─────────────────────────────────────────────────────┘

Roles: Administrator, Cashier, InventoryClerk
```

#### Products Entity

```
Products Table Structure:
┌──────────────────────────────────────────────────────┐
│ Column Name      │ Data Type         │ Constraints   │
├──────────────────────────────────────────────────────┤
│ ProductId        │ INT               │ PK, Identity  │
│ SKU              │ NVARCHAR(50)      │ UNIQUE        │
│ Name             │ NVARCHAR(200)     │ NOT NULL      │
│ Description      │ NVARCHAR(500)     │ -             │
│ Barcode          │ NVARCHAR(100)     │ UNIQUE        │
│ CategoryId       │ INT               │ FK, NULLABLE  │
│ SupplierId       │ INT               │ FK, NULLABLE  │
│ UnitPrice        │ DECIMAL(10,2)     │ NOT NULL      │
│ CostPrice        │ DECIMAL(10,2)     │ NOT NULL      │
│ QuantityInStock  │ INT               │ DEFAULT 0     │
│ ReorderLevel     │ INT               │ DEFAULT 10    │
│ IsActive         │ BIT               │ DEFAULT 1     │
│ CreatedDate      │ DATETIME          │ DEFAULT NOW   │
└──────────────────────────────────────────────────────┘
```

#### Sales Entity

```
Sales Table Structure:
┌──────────────────────────────────────────────────────┐
│ Column Name      │ Data Type         │ Constraints   │
├──────────────────────────────────────────────────────┤
│ SaleId           │ INT               │ PK, Identity  │
│ InvoiceNumber    │ NVARCHAR(50)      │ UNIQUE        │
│ CashierId        │ INT               │ FK (Users)    │
│ CustomerId       │ INT               │ FK, NULLABLE  │
│ SaleDate         │ DATETIME          │ DEFAULT NOW   │
│ Subtotal         │ DECIMAL(10,2)     │ NOT NULL      │
│ TaxAmount        │ DECIMAL(10,2)     │ NOT NULL      │
│ DiscountAmount   │ DECIMAL(10,2)     │ DEFAULT 0     │
│ TotalAmount      │ DECIMAL(10,2)     │ NOT NULL      │
│ PaymentMethod    │ NVARCHAR(20)      │ NOT NULL      │
│ Status           │ NVARCHAR(20)      │ NOT NULL      │
│ Notes            │ NVARCHAR(500)     │ NULLABLE      │
└──────────────────────────────────────────────────────┘

Payment Methods: Cash, Card, Check
Status Values: Completed, Pending, Cancelled
```

#### SaleItems Entity

```
SaleItems Table Structure:
┌──────────────────────────────────────────────────────┐
│ Column Name      │ Data Type         │ Constraints   │
├──────────────────────────────────────────────────────┤
│ SaleItemId       │ INT               │ PK, Identity  │
│ SaleId           │ INT               │ FK (Sales)    │
│ ProductId        │ INT               │ FK (Products) │
│ Quantity         │ INT               │ NOT NULL      │
│ UnitPrice        │ DECIMAL(10,2)     │ NOT NULL      │
│ LineTotal        │ DECIMAL(10,2)     │ NOT NULL      │
│ Discount         │ DECIMAL(10,2)     │ DEFAULT 0     │
└──────────────────────────────────────────────────────┘

Calculation: LineTotal = (Quantity * UnitPrice) - Discount
```

#### Customers Entity

```
Customers Table Structure:
┌──────────────────────────────────────────────────────┐
│ Column Name      │ Data Type         │ Constraints   │
├──────────────────────────────────────────────────────┤
│ CustomerId       │ INT               │ PK, Identity  │
│ Name             │ NVARCHAR(150)     │ NOT NULL      │
│ PhoneNumber      │ NVARCHAR(20)      │ -             │
│ Email            │ NVARCHAR(100)     │ -             │
│ Address          │ NVARCHAR(300)     │ -             │
│ LoyaltyPoints    │ INT               │ DEFAULT 0     │
│ TotalSpent       │ DECIMAL(10,2)     │ DEFAULT 0     │
│ CreatedDate      │ DATETIME          │ DEFAULT NOW   │
│ LastPurchase     │ DATETIME          │ NULLABLE      │
└──────────────────────────────────────────────────────┘
```

#### InventoryMovements Entity

```
InventoryMovements Table Structure:
┌──────────────────────────────────────────────────────┐
│ Column Name      │ Data Type         │ Constraints   │
├──────────────────────────────────────────────────────┤
│ MovementId       │ INT               │ PK, Identity  │
│ ProductId        │ INT               │ FK (Products) │
│ UserId           │ INT               │ FK (Users)    │
│ MovementType     │ NVARCHAR(20)      │ NOT NULL      │
│ Quantity         │ INT               │ NOT NULL      │
│ MovementDate     │ DATETIME          │ DEFAULT NOW   │
│ Reference        │ NVARCHAR(100)     │ -             │
│ Notes            │ NVARCHAR(500)     │ -             │
└──────────────────────────────────────────────────────┘

MovementType: Stock In, Stock Out, Adjustment, Return
```

#### Categories Entity

```
Categories Table Structure:
┌──────────────────────────────────────────────────────┐
│ Column Name      │ Data Type         │ Constraints   │
├──────────────────────────────────────────────────────┤
│ CategoryId       │ INT               │ PK, Identity  │
│ Name             │ NVARCHAR(100)     │ NOT NULL      │
│ Description      │ NVARCHAR(300)     │ -             │
│ IsActive         │ BIT               │ DEFAULT 1     │
│ CreatedDate      │ DATETIME          │ DEFAULT NOW   │
└──────────────────────────────────────────────────────┘
```

#### Suppliers Entity

```
Suppliers Table Structure:
┌──────────────────────────────────────────────────────┐
│ Column Name      │ Data Type         │ Constraints   │
├──────────────────────────────────────────────────────┤
│ SupplierId       │ INT               │ PK, Identity  │
│ Name             │ NVARCHAR(150)     │ NOT NULL      │
│ ContactPerson    │ NVARCHAR(100)     │ -             │
│ PhoneNumber      │ NVARCHAR(20)      │ -             │
│ Email            │ NVARCHAR(100)     │ -             │
│ Address          │ NVARCHAR(300)     │ -             │
│ IsActive         │ BIT               │ DEFAULT 1     │
│ CreatedDate      │ DATETIME          │ DEFAULT NOW   │
└──────────────────────────────────────────────────────┘
```

#### AuditLogs Entity

```
AuditLogs Table Structure:
┌──────────────────────────────────────────────────────┐
│ Column Name      │ Data Type         │ Constraints   │
├──────────────────────────────────────────────────────┤
│ LogId            │ INT               │ PK, Identity  │
│ UserId           │ INT               │ FK (Users)    │
│ Action           │ NVARCHAR(100)     │ NOT NULL      │
│ TableName        │ NVARCHAR(50)      │ NOT NULL      │
│ RecordId         │ INT               │ -             │
│ OldValues        │ NVARCHAR(MAX)     │ -             │
│ NewValues        │ NVARCHAR(MAX)     │ -             │
│ Timestamp        │ DATETIME          │ DEFAULT NOW   │
│ IPAddress        │ NVARCHAR(50)      │ -             │
└──────────────────────────────────────────────────────┘

Actions: Create, Update, Delete, View, Login
```

### Entity Relationships Diagram

```
┌────────────────────────────────────────────────────────────────────┐
│                      DATABASE RELATIONSHIP MAP                      │
└────────────────────────────────────────────────────────────────────┘

                           ┌──────────┐
                           │  Users   │
                           └────┬─────┘
                                │
                ┌───────────────┼───────────────┐
                │               │               │
                ▼               ▼               ▼
           ┌────────┐    ┌──────────────┐  ┌────────────┐
           │ Sales  │    │InventoryMove│  │ AuditLogs  │
           └────┬───┘    └───────┬──────┘  └────────────┘
                │                │
                ▼                ▼
           ┌────────────┐   ┌─────────────┐
           │ SaleItems  │   │  Products   │
           └────┬───────┘   └──────┬──────┘
                │                 │
                └────────┬────────┘
                         │
            ┌────────────┴────────────┐
            │                         │
            ▼                         ▼
        ┌────────────┐         ┌──────────┐
        │ Categories │         │ Suppliers│
        └────────────┘         └──────────┘

                    ┌──────────────┐
                    │  Customers   │
                    └────────┬─────┘
                             │
                             ▼
                         ┌────────┐
                         │ Sales  │(M:1)
                         └────────┘
```

---

## System Workflow and Data Flow

### Complete Application Flow

```
APPLICATION LIFECYCLE FLOW:

1. APPLICATION STARTUP
   ├─ Program.cs Entry Point
   ├─ Initialize Windows Forms Application
   ├─ Load Connection String from ApplicationDbContext
   ├─ Establish Database Connection
   └─ Display Login Form (Form1.cs)

2. USER AUTHENTICATION
   ├─ User Enters Username and Password
   ├─ AuthenticationService Processes Credentials
   ├─ Query Users Table
   ├─ Validate with BCrypt Password Hash
   ├─ Check User Role and IsActive Status
   ├─ Update LastLogin Timestamp
   ├─ Log Authentication Event to AuditLogs
   └─ Redirect to Appropriate Dashboard

3. ROLE-BASED ACCESS CONTROL
   ├─ Administrator Role
   │  ├─ User Management Module
   │  ├─ System Configuration
   │  ├─ All Reports Access
   │  └─ Full Audit Log Viewing
   │
   ├─ Cashier Role
   │  ├─ Point of Sale Module
   │  ├─ Customer Management
   │  ├─ Personal Sales History
   │  └─ Limited Reports
   │
   └─ InventoryClerk Role
       ├─ Product Management Module
       ├─ Stock Level Monitoring
       ├─ Supplier Management
       └─ Inventory Reports

4. CORE OPERATIONS FLOW
   ├─ Business Logic Service Layer
   │  ├─ AuthenticationService
   │  ├─ SalesService
   │  ├─ InventoryService
   │  └─ ReportService
   │
   ├─ Data Access via Entity Framework
   │  └─ ApplicationDbContext
   │
   └─ Database Persistence
       └─ SQL Server Database
```

### Point of Sale Transaction Flow

```
POINT OF SALE TRANSACTION WORKFLOW:

START: Customer Initiates Purchase
   │
   ▼
[1] TRANSACTION INITIALIZATION
   ├─ Cashier Opens POS Interface
   ├─ Create New Sale Record (Header)
   ├─ Generate Unique InvoiceNumber
   ├─ Record CashierId (logged-in user)
   ├─ Set Initial Sale Status: Pending
   └─ Initialize Subtotal: 0, TaxAmount: 0

   │
   ▼
[2] PRODUCT SELECTION
   ├─ Cashier Scans Product Barcode OR Searches by Name/SKU
   ├─ System Queries Products Table
   ├─ Validate Product Exists and IsActive = True
   ├─ Check Quantity Available (QuantityInStock > 0)
   ├─ Retrieve ProductId, UnitPrice, CostPrice
   └─ Display Product Information to Cashier

   │
   ▼
[3] ADD TO CART
   ├─ Cashier Enters Quantity to Purchase
   ├─ Validate Quantity Against Stock Level
   ├─ Create SaleItem Record
   │  ├─ Set SaleId (Current Transaction)
   │  ├─ Set ProductId (Selected Product)
   │  ├─ Set Quantity
   │  ├─ Set UnitPrice (from Products table)
   │  ├─ Calculate LineTotal = (Quantity * UnitPrice) - Discount
   │  └─ Add to Cart/SaleItems Collection
   ├─ Update Running Subtotal
   └─ Option to Continue or Complete

   │
   ▼
[4] CUSTOMER SELECTION
   ├─ Cashier Searches for Existing Customer OR Creates New
   ├─ If Existing: Query Customers Table, Retrieve CustomerId
   ├─ If New: Create Customer Record, Store in Customers Table
   ├─ Link CustomerId to Sale Record
   ├─ Check Customer LoyaltyPoints Balance
   └─ Option to Apply Points to Discount

   │
   ▼
[5] CALCULATIONS & DISCOUNTS
   ├─ Calculate Subtotal (Sum of All LineItems)
   ├─ Calculate Tax Amount (Subtotal * TaxRate)
   ├─ Apply Promotional Discounts (if any)
   ├─ Apply Loyalty Points Discount (if customer selected)
   ├─ Calculate TotalAmount = Subtotal + Tax - AllDiscounts
   └─ Display Final Amount to Customer

   │
   ▼
[6] PAYMENT PROCESSING
   ├─ Select Payment Method (Cash / Card / Check)
   ├─ Record PaymentMethod in Sale Record
   ├─ Process Payment (External System)
   ├─ Verify Payment Authorization
   ├─ Record Payment Success/Failure
   └─ If Failed: Return to Step [5]

   │
   ▼
[7] TRANSACTION FINALIZATION
   ├─ Set Sale Status: Completed
   ├─ Save Sale Record to Database
   ├─ Save All SaleItems to Database
   ├─ Generate Receipt (PDF) using QuestPDF
   ├─ Print or Display Receipt to Customer
   └─ Record Transaction Timestamp

   │
   ▼
[8] INVENTORY UPDATES
   ├─ For Each Product in SaleItems:
   │  ├─ Query Products Table
   │  ├─ Reduce QuantityInStock by Quantity Sold
   │  ├─ Check if New Stock <= ReorderLevel
   │  ├─ Flag for Reordering if Below Threshold
   │  └─ Update Products Table
   │
   ├─ Create InventoryMovement Record for Audit
   │  ├─ Set MovementType: Stock Out
   │  ├─ Set ProductId and Quantity
   │  ├─ Record UserId (Cashier)
   │  ├─ Set Reference: SaleId
   │  └─ Save to InventoryMovements Table
   └─ Update Customer TotalSpent & LastPurchase

   │
   ▼
[9] LOYALTY POINTS UPDATE
   ├─ Calculate Points Earned (TotalAmount / PointsConversionRate)
   ├─ Deduct Used Points (if applied)
   ├─ Update Customer LoyaltyPoints Balance
   ├─ Save Updated Customer Record
   └─ Display Points Information

   │
   ▼
[10] AUDIT LOGGING
   ├─ Create AuditLog Record for Transaction
   ├─ Record Action: CreateSale
   ├─ Set TableName: Sales
   ├─ Store RecordId (SaleId)
   ├─ Record UserId (Cashier)
   ├─ Capture NewValues (Complete Transaction Data)
   ├─ Set Timestamp
   └─ Save to AuditLogs Table

   │
   ▼
END: Transaction Complete
   ├─ Display Transaction Success Message
   ├─ Reset POS Interface for Next Transaction
   └─ Return to Step [1]
```

### Inventory Management Flow

```
INVENTORY MANAGEMENT WORKFLOW:

[STOCK RECEIPT]
   │
   ▼
Inventory Clerk Receives Stock from Supplier
   │
   ├─ Verify Delivery Documentation
   ├─ Scan Product Barcodes
   ├─ Count Received Quantities
   │
   ▼
Create Stock-In Request
   │
   ├─ Query Products Table
   ├─ Validate Product Details Match
   ├─ Update ProductId QuantityInStock += ReceivedQuantity
   │
   ▼
Record InventoryMovement
   │
   ├─ MovementType: Stock In
   ├─ Set Quantity (Amount Received)
   ├─ Set UserId (Inventory Clerk)
   ├─ Set Reference (PO Number/Supplier)
   ├─ Record Timestamp
   │
   ▼
Generate Audit Trail
   │
   └─ Log to AuditLogs Table


[STOCK ADJUSTMENT]
   │
   ▼
Count Physical Inventory
   │
   ├─ Compare Physical Count vs System QuantityInStock
   │
   ▼
Identify Discrepancy
   │
   ├─ Query Products Table Current Stock
   │
   ▼
Create Adjustment
   │
   ├─ Calculate Adjustment Amount (Physical - System)
   ├─ Update Products QuantityInStock
   │
   ▼
Record InventoryMovement
   │
   ├─ MovementType: Adjustment
   ├─ Set Quantity (Positive or Negative)
   ├─ Set Reason/Notes
   │
   ▼
Log to AuditLogs
   │
   └─ Create Record for Adjustment


[STOCK MONITORING]
   │
   ▼
System Continuously Monitors Stock Levels
   │
   ├─ Query Products Table
   ├─ Compare QuantityInStock vs ReorderLevel
   │
   ▼
Low Stock Alert
   │
   ├─ If QuantityInStock <= ReorderLevel
   │
   ▼
Generate Reorder Report
   │
   ├─ List Products Requiring Reordering
   ├─ Notify Inventory Clerk
   ├─ Display in Admin Dashboard
   │
   ▼
Create Purchase Order
   │
   ├─ Contact Supplier from Suppliers Table
   └─ Initiate Stock Receipt Process
```

### User Authentication Flow

```
USER AUTHENTICATION PROCESS:

START: User Launches Application
   │
   ▼
Display LoginForm (Form1.cs)
   │
   ├─ Render Username Input Field
   ├─ Render Password Input Field
   ├─ Render Login Button
   └─ Render Cancel Button

   │
   ▼
User Enters Credentials
   │
   ├─ Input Username
   ├─ Input Password
   └─ Click Login Button

   │
   ▼
AuthenticationService.ValidateUser()
   │
   ├─ Query Users Table
   ├─ WHERE Username = Entered Username
   │
   ▼
Check User Existence
   │
   ├─ User Found? ─NO─> Display "Invalid Username"
   │       │
   │      YES
   │       │
   │       ▼
   │   Check IsActive Status
   │       │
   │   IsActive = False? ─YES─> Display "User Disabled"
   │       │
   │       NO
   │       │
   │       ▼
   │   Password Validation using BCrypt
   │       │
   │   BCrypt.Verify(EnteredPassword, StoredHash)
   │       │
   │   Valid? ─NO─> Display "Invalid Password"
   │       │
   │      YES
   │       │
   │       ▼
   │   Update LastLogin = Current DateTime
   │       │
   │       ▼
   │   Retrieve User Role
   │       │
   │       ├─ Role = Administrator
   │       ├─ Role = Cashier
   │       └─ Role = InventoryClerk
   │       │
   │       ▼
   │   Create AuditLog Record
   │       │
   │       ├─ Action: Login
   │       ├─ UserId: Authenticated User
   │       ├─ Timestamp: Current DateTime
   │       ├─ IPAddress: Source IP
   │       └─ Status: Success
   │       │
   │       ▼
   │   Load Appropriate Dashboard Based on Role
   │       │
   │       ├─ Administrator ─> AdminPanel.cs
   │       ├─ Cashier ─> POSForm.cs
   │       └─ InventoryClerk ─> InventoryForm.cs
   │       │
   │       ▼
   └─> Display Main Application Interface

LOGIN FAILURE
   │
   ▼
Display Error Message
   │
   ├─ Log Failed Attempt to AuditLogs
   │  ├─ Action: FailedLogin
   │  ├─ Username: Attempted Username
   │  ├─ Reason: Invalid Credentials / Account Disabled
   │  └─ Status: Failed
   │
   └─ Return to Login Screen
```

### Report Generation Flow

```
REPORT GENERATION WORKFLOW:

User Requests Report from Dashboard
   │
   ├─ Sales Report
   ├─ Inventory Report
   ├─ Customer Report
   └─ Audit Report

   │
   ▼
ReportService Processes Request
   │
   ├─ Validate User Permissions
   │  └─ Check User Role Against Report Access
   │
   ├─ Query Required Database Tables
   │  ├─ Apply Filters (Date Range, Product Category, etc.)
   │  ├─ Sort Data (By Date, Amount, Quantity, etc.)
   │  └─ Calculate Aggregations (Totals, Averages, Counts)
   │
   ▼
SALES REPORT QUERY
   │
   ├─ SELECT Sales, SaleItems, Products, Customers
   ├─ JOIN Sales -> SaleItems -> Products
   ├─ JOIN Sales -> Customers
   ├─ Filter WHERE SaleDate BETWEEN StartDate AND EndDate
   ├─ GROUP BY Product / Category / Customer / Date
   ├─ SUM(TotalAmount, Quantity, DiscountAmount, TaxAmount)
   └─ ORDER BY Date DESC

   │
   ▼
INVENTORY REPORT QUERY
   │
   ├─ SELECT Products, Categories, Suppliers
   ├─ JOIN Products -> Categories
   ├─ JOIN Products -> Suppliers
   ├─ SELECT QuantityInStock, ReorderLevel, UnitPrice
   ├─ Calculate StockValue = QuantityInStock * CostPrice
   ├─ Identify Low Stock Items (QuantityInStock <= ReorderLevel)
   └─ ORDER BY ReorderLevel DESC

   │
   ▼
Format Report Data
   │
   ├─ Arrange Data into Tables
   ├─ Calculate Totals and Summaries
   ├─ Format Currency and Date Values
   └─ Prepare for PDF Generation

   │
   ▼
Generate PDF using QuestPDF
   │
   ├─ Create Document Structure
   ├─ Add Report Title and Metadata
   ├─ Insert Company Information
   ├─ Add Report Data Tables
   ├─ Include Charts/Graphs (if applicable)
   ├─ Add Date Range and Generated Timestamp
   ├─ Insert Footer with User Information
   └─ Generate PDF File

   │
   ▼
Export/Display Report
   │
   ├─ Save to File System
   ├─ Display Preview in Viewer
   ├─ Option to Print
   ├─ Option to Save to Disk
   └─ Log Report Generation to AuditLogs
```

---

## User Role Permissions Matrix

### Permission Breakdown

| Feature | Administrator | Cashier | Inventory Clerk |
|---------|---|---|---|
| User Management | Full CRUD | None | None |
| Dashboard Access | Admin Dashboard | POS Dashboard | Inventory Dashboard |
| Product Management | View/Edit/Delete | View Only | Full CRUD |
| Inventory Tracking | View | View (Personal) | Full CRUD |
| Sales Processing | None | Full | None |
| Customer Management | View | Full | View |
| Stock Management | View | Limited | Full |
| Reports - Sales | Yes | Limited | No |
| Reports - Inventory | Yes | No | Yes |
| Reports - Audit Log | Yes | No | No |
| System Settings | Full | None | None |
| Audit Log Access | Full | None | None |

### Role Responsibilities

```
ADMINISTRATOR RESPONSIBILITIES:

System Management:
├─ Create and manage user accounts
├─ Assign roles to users
├─ Configure system settings
├─ Monitor system performance
└─ Manage security and access control

Reporting & Analysis:
├─ View all reports
├─ Generate custom reports
├─ Access audit logs
├─ Monitor compliance
└─ Financial reconciliation

Maintenance:
├─ Database administration
├─ Backup and recovery
├─ User activity monitoring
└─ System updates and patches


CASHIER RESPONSIBILITIES:

Point of Sale:
├─ Process customer sales transactions
├─ Scan products and add to cart
├─ Calculate totals and process payments
├─ Generate receipts
└─ Handle refunds/returns

Customer Service:
├─ Manage customer profiles
├─ Apply loyalty programs
├─ Process customer inquiries
├─ Handle customer complaints
└─ Track customer purchase history

Reporting:
├─ View personal sales history
├─ Generate sales reports (own)
├─ Track daily reconciliation
└─ View transaction details


INVENTORY CLERK RESPONSIBILITIES:

Product Management:
├─ Create new products
├─ Update product information
├─ Manage product categories
├─ Handle product pricing
└─ Manage barcodes

Stock Management:
├─ Receive stock from suppliers
├─ Monitor stock levels
├─ Create stock adjustments
├─ Process stock transfers
├─ Generate reorder reports

Supplier Management:
├─ Maintain supplier information
├─ Track supplier performance
├─ Manage purchase orders
├─ Process deliveries
└─ Handle vendor inquiries

Reporting:
├─ Generate inventory reports
├─ Track stock movements
├─ Monitor reorder points
└─ Create stock value reports
```

---

## Entity Framework Configuration

### ApplicationDbContext Setup

```csharp
DbSet Declarations (Entity Collections):

public DbSet<User> Users { get; set; }
public DbSet<Category> Categories { get; set; }
public DbSet<Supplier> Suppliers { get; set; }
public DbSet<Product> Products { get; set; }
public DbSet<Customer> Customers { get; set; }
public DbSet<Sale> Sales { get; set; }
public DbSet<SaleItem> SaleItems { get; set; }
public DbSet<InventoryMovement> InventoryMovements { get; set; }
public DbSet<AuditLog> AuditLogs { get; set; }

Connection Configuration:
OnConfiguring Method -> Configure SQL Server Connection String

Entity Relationships Configuration:
OnModelCreating Method -> Define relationships and constraints
```

### Key Configurations

```
USER ENTITY CONFIGURATION:
├─ Username: UNIQUE constraint
├─ Role: Stored as string (Enum conversion)
└─ Relationships: (1:M) to Sales, InventoryMovements, AuditLogs

PRODUCT ENTITY CONFIGURATION:
├─ Barcode: UNIQUE constraint
├─ SKU: UNIQUE constraint
├─ Category: (M:1) relationship, ON DELETE SET NULL
├─ Supplier: (M:1) relationship, ON DELETE SET NULL
└─ Related to: SaleItems, InventoryMovements

SALE ENTITY CONFIGURATION:
├─ InvoiceNumber: UNIQUE constraint
├─ PaymentMethod: Stored as string (Enum conversion)
├─ Status: Stored as string (Enum conversion)
├─ Customer: (M:1) relationship, ON DELETE SET NULL
├─ Cashier: (M:1) relationship, ON DELETE RESTRICT
└─ SaleItems: (1:M) relationship, ON DELETE CASCADE

SALEITEM ENTITY CONFIGURATION:
├─ Sale: (M:1) relationship, ON DELETE CASCADE
└─ Product: (M:1) relationship, ON DELETE RESTRICT

INVENTORYMOVEMENT ENTITY CONFIGURATION:
├─ MovementType: Stored as string (Enum conversion)
├─ Product: (M:1) relationship, ON DELETE RESTRICT
└─ User: (M:1) relationship, ON DELETE RESTRICT
```

---

## Development Guide

### Entity Framework Migrations

| Command | Purpose |
|---------|---------|
| `dotnet tool install --global dotnet-ef` | Install Entity Framework CLI tools globally |
| `dotnet ef migrations add InitialCreate --project syncversestudio` | Create initial migration for database schema |
| `dotnet ef migrations add MigrationName --project syncversestudio` | Create new migration after entity changes |
| `dotnet ef database update --project syncversestudio` | Apply pending migrations to database |
| `dotnet ef database drop --project syncversestudio` | Drop and recreate entire database |
| `dotnet ef migrations script --project syncversestudio` | Generate SQL script from migrations |
| `dotnet ef migrations remove --project syncversestudio` | Remove last migration (if not applied) |

### Adding New Entity to System

| Step | Action |
|------|--------|
| 1 | Create entity class in Models/ folder with properties and relationships |
| 2 | Add DbSet<EntityName> property to ApplicationDbContext class |
| 3 | Configure entity relationships in OnModelCreating method |
| 4 | Create migration: `dotnet ef migrations add AddEntityName --project syncversestudio` |
| 5 | Review generated migration file for correctness |
| 6 | Update database: `dotnet ef database update --project syncversestudio` |
| 7 | Create service class in Services/ folder for business logic |
| 8 | Create UI form in Views/ folder for user interface |
| 9 | Add form to main navigation/menu |
| 10 | Test CRUD operations and relationships |

### Modifying Existing Entity

| Step | Action |
|------|--------|
| 1 | Update entity class properties and relationships in Models/ |
| 2 | Review relationships with other entities |
| 3 | Create migration: `dotnet ef migrations add UpdateEntityName --project syncversestudio` |
| 4 | Verify migration handles existing data appropriately |
| 5 | Update database: `dotnet ef database update --project syncversestudio` |
| 6 | Update related service classes if business logic changed |
| 7 | Update UI forms if display requirements changed |
| 8 | Test migration rollback scenario |
| 9 | Run integration tests |
| 10 | Deploy to test environment |

### Building and Running

| Method | Steps |
|--------|-------|
| Visual Studio | 1. Open syncversestudio.sln 2. Press F5 3. Application launches with debugger |
| .NET CLI | 1. `dotnet restore` 2. `dotnet build` 3. `dotnet run --project syncversestudio` |
| Batch File | Double-click RunApplication.bat in project root directory |

---

## Troubleshooting Guide

### Connection Issues

| Problem | Solution |
|---------|----------|
| Cannot connect to SQL Server | 1. Open services.msc and verify SQL Server service is running 2. Check connection string syntax 3. Test: `sqlcmd -S YOUR_SERVER -E -Q "SELECT @@VERSION"` 4. Ensure database exists or EF migrations are applied |
| Authentication Failed | 1. Verify Windows Authentication is enabled 2. Check user has SQL Server login 3. Verify database permissions 4. Test connection with SQL Server Management Studio |
| Connection Timeout | 1. Check network connectivity 2. Verify firewall allows SQL Server port 1433 3. Increase command timeout in connection string 4. Check SQL Server performance |

### Migration Issues

| Problem | Solution |
|---------|----------|
| Migration not found | 1. Verify migration file exists in Migrations folder 2. Check migration name spelling 3. Run: `dotnet ef migrations list --project syncversestudio` 4. Ensure EF tools are installed |
| Migration fails to apply | 1. Backup database first 2. Review migration SQL for syntax errors 3. Check for conflicting changes 4. Run: `dotnet ef database drop` and restart if necessary 5. Review entity configuration |
| Pending migrations | 1. Run: `dotnet ef database update --project syncversestudio` 2. Verify migration files in Migrations folder 3. Check for naming conflicts 4. Review OnModelCreating configuration |

### Application Errors

| Problem | Solution |
|---------|----------|
| Login fails with valid credentials | 1. Verify Users table exists and has data 2. Check user IsActive = 1 3. Verify database initialized with EF migrations 4. Check BCrypt hashing matches stored value 5. Review AuditLogs for failed login details |
| Products not displaying | 1. Verify Products table populated 2. Check product IsActive status 3. Verify category and supplier relationships 4. Check barcode format validity 5. Run test query against Products table |
| Reports not generating | 1. Verify report data exists in database 2. Check user role has report access permissions 3. Verify QuestPDF library installed correctly 4. Check output file path permissions 5. Review error logs for specific failure reason |
| Inventory sync issues | 1. Verify InventoryMovements table has records 2. Check product stock levels accurate 3. Run inventory reconciliation report 4. Check for orphaned SaleItems 5. Review recent transactions |

### Database Issues

| Problem | Solution |
|---------|----------|
| Database already exists error | `dotnet ef database drop --project syncversestudio` then recreate |
| Missing tables or schema | `dotnet ef database update --project syncversestudio` to apply migrations |
| Corrupted database | Restore from backup or `dotnet ef database drop && dotnet ef database update` |
| Duplicate key violations | Review data for duplicate SKUs, Barcodes, or UserIds; manually delete or update conflicts |

---

## SQL Server Verification

To verify SQL Server installation and connectivity:

```bash
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT @@VERSION"
sqlcmd -S (localdb)\MSSQLLocalDB -E -Q "SELECT @@VERSION"
```

---

## Support and Resources

- Repository: https://github.com/EIRSVi/Sync-Verse-Studio
- Issue Tracking: GitHub Issues
- Pull Requests: GitHub Pull Requests
- Documentation: This README and inline code comments

---

**Document Version:** 2.0
**Last Updated:** 2025-10-21 10:49:53 UTC
**Author:** SyncVerse Studio Development Team
**License:** Refer to LICENSE file in repository