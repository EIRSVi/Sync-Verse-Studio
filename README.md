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
https://raw.githubusercontent.com/EIRSVi/eirsvi/d8339235bb1765d461e284ab51bd1223d4345dce/assets/brand/mainlogo.svg 
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
| Username | `` |
| Password | `` |
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
dotnet clean
dotnet restore
dotnet build
dotnet run --project syncversestudio

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
# Implementation Plan

- [-] 1. Set up project structure and database foundation


  - Create Windows Forms project in Visual Studio with .NET Framework 4.7.2+
  - Install required NuGet packages: BCrypt.Net-Next, System.Data.SqlClient, Newtonsoft.Json, NLog
  - Create folder structure: Models/, Data/, Business/, Forms/, Controls/, Utilities/
  - Create SQL script to generate all database tables with indexes and sample data (Users, Roles, Categories, Suppliers, Products, Sales, SaleItems, StockMovements, AuditLogs)
  - Configure App.config with connection string and NLog settings
  - _Requirements: 10.1, 10.3, 10.5_

- [ ] 2. Implement data models and utilities
  - [ ] 2.1 Create all model classes
    - Write User, Role, Product, Category, Supplier, Sale, SaleItem, StockMovement, and AuditLog model classes with properties matching database schema
    - _Requirements: 1.1, 3.2, 4.2, 5.2, 6.1, 7.2, 8.1, 9.2_
  
  - [ ] 2.2 Implement utility classes
    - Write SessionManager static class with UserID, Username, RoleID, RoleCode properties and SetSession/ClearSession/GetIPAddress methods
    - Write PasswordHelper static class with HashPassword and VerifyPassword methods using BCrypt
    - Write ValidationHelper static class with IsValidEmail, IsValidPhoneNumber, ValidateRequired, ValidateLength methods
    - _Requirements: 1.4, 15.1, 15.4_

- [ ] 3. Build data access layer (repositories)
  - [ ] 3.1 Create database connection class
    - Write DbConnection class with GetConnection method returning SqlConnection using connection string from App.config
    - Implement connection error handling with NLog logging
    - _Requirements: 10.1, 10.2, 10.5_
  
  - [ ] 3.2 Implement UserRepository
    - Write GetAll, GetById, GetByUsername, Create, Update, Delete (soft delete), Search, UpdateLastLogin methods
    - Use parameterized SQL queries for all operations
    - _Requirements: 1.1, 1.2, 1.5, 3.2, 3.4, 3.5, 10.3_
  
  - [ ] 3.3 Implement ProductRepository
    - Write GetAll, GetById, GetByCode, GetByBarcode, Create, Update, Delete (soft delete), Search, GetLowStockProducts, UpdateStock methods
    - Join with Categories and Suppliers tables to include names
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 8.4, 10.3_
  
  - [ ] 3.4 Implement CategoryRepository
    - Write GetAll, GetById, GetByCode, Create, Update, Delete (soft delete with product check), Search, GetSubCategories methods
    - Join with parent categories to include parent names
    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 10.3_
  
  - [ ] 3.5 Implement SupplierRepository
    - Write GetAll, GetById, GetByCode, Create, Update, Delete (soft delete), Search methods
    - _Requirements: 6.1, 6.3, 6.4, 6.5, 10.3_
  
  - [ ] 3.6 Implement SalesRepository
    - Write GetAll, GetById, GetByUser, GetByDateRange, Create (with transaction for Sales and SaleItems), GetSaleItems, Search methods
    - Implement transaction handling for multi-table inserts
    - _Requirements: 7.2, 10.3, 10.4, 14.1_
  
  - [ ] 3.7 Implement StockMovementRepository
    - Write GetAll, GetByProduct, GetByDateRange, Create, Search methods
    - _Requirements: 8.1, 8.2, 8.3, 8.5, 10.3_
  
  - [ ] 3.8 Implement AuditLogRepository
    - Write GetAll, GetByUser, GetByDateRange, GetByAction, GetByTable, Create, Search methods
    - _Requirements: 9.1, 9.2, 9.3, 9.4, 10.3_

- [ ] 4. Build business logic layer (services)
  - [ ] 4.1 Implement UserService
    - Write AuthenticateUser method with BCrypt password verification
    - Write CreateUser method with password hashing and audit logging
    - Write UpdateUser and DeleteUser methods with audit logging
    - Write GetAllUsers, SearchUsers, ValidateUser methods
    - _Requirements: 1.1, 1.2, 3.1, 3.2, 3.4, 3.5, 15.4_
  
  - [ ] 4.2 Implement ProductService
    - Write CreateProduct, UpdateProduct, DeleteProduct methods with audit logging
    - Write GetAllProducts, GetLowStockProducts, SearchProducts methods
    - Write ValidateProduct method checking unique code/barcode/SKU and positive prices
    - Write GenerateProductCode method to auto-generate next code
    - _Requirements: 4.1, 4.2, 4.4, 4.5, 8.4_
  
  - [ ] 4.3 Implement CategoryService
    - Write CreateCategory, UpdateCategory, DeleteCategory methods with audit logging
    - Write GetAllCategories, SearchCategories methods
    - Write ValidateCategory method checking unique code/name
    - Write GenerateCategoryCode method to auto-generate next code
    - _Requirements: 5.1, 5.4, 5.5_
  
  - [ ] 4.4 Implement SupplierService
    - Write CreateSupplier, UpdateSupplier, DeleteSupplier methods with audit logging
    - Write GetAllSuppliers, SearchSuppliers methods
    - Write ValidateSupplier method checking unique code/name and email format
    - Write GenerateSupplierCode method to auto-generate next code
    - _Requirements: 6.1, 6.2, 6.4, 6.5_
  
  - [ ] 4.5 Implement SalesService
    - Write ProcessSale method with transaction handling for sale creation, stock updates, stock movements, and audit logging
    - Write GetAllSales, GetSalesByUser, GetSalesByDateRange, SearchSales methods
    - Write CalculateSaleTotals method for SubTotal, TaxAmount, TotalAmount calculations
    - Write GenerateSaleCode and ValidateSale methods
    - _Requirements: 7.1, 7.2, 7.3, 7.4, 10.4, 14.5_
  
  - [ ] 4.6 Implement StockMovementService
    - Write RecordStockIn, RecordStockOut, RecordStockAdjustment methods
    - Write GetAllMovements, GetMovementsByProduct, GetMovementsByDateRange, SearchMovements methods
    - _Requirements: 8.1, 8.2, 8.3, 8.5_
  
  - [ ] 4.7 Implement AuditService
    - Write LogAction method to create audit log entries with JSON serialization for old/new values
    - Write GetAllLogs, GetLogsByUser, GetLogsByDateRange, GetLogsByAction, GetLogsByTable, SearchLogs methods
    - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_

- [ ] 5. Create custom UI controls with VS Code theme
  - [ ] 5.1 Implement CustomButton control
    - Create CustomButton class inheriting from Button
    - Apply VS Code theme colors (#0E639C primary, #3C3C3C secondary)
    - Implement hover effect (brightness +10%) and active state (#007ACC)
    - Set border radius 2px, height 28px, padding 12px horizontal
    - _Requirements: 11.1, 11.2_
  
  - [ ] 5.2 Implement CustomTextBox control
    - Create CustomTextBox class inheriting from TextBox
    - Apply VS Code theme colors (background #3C3C3C, text #CCCCCC, border #3E3E42)
    - Implement focus border color #007ACC
    - Add PlaceholderText, IsPassword, ValidationMessage properties
    - Add inline error message label below textbox with red color #F48771
    - _Requirements: 11.1, 11.2, 12.2, 12.3_
  
  - [ ] 5.3 Implement CustomDataGridView control
    - Create CustomDataGridView class inheriting from DataGridView
    - Apply VS Code theme colors (header #252526, alternating rows #2D2D30, grid lines #3E3E42)
    - Implement row hover effect (background #2A2D2E with #007ACC left border 3px)
    - Set selection color #264F78
    - _Requirements: 11.1, 11.2, 13.3, 13.4_
  
  - [ ] 5.4 Implement ToastNotification control
    - Create ToastNotification user control with width 320px
    - Implement slide-in animation from right side
    - Add auto-dismiss timer (4 seconds) and close button
    - Create Success, Error, Warning, Info types with appropriate border colors (#4EC9B0, #F48771, #CE9178, #007ACC)
    - Implement queue management for multiple toasts
    - Position at bottom-right corner with 20px margin
    - _Requirements: 11.5, 12.5_
  
  - [ ] 5.5 Implement SidebarNavigation control
    - Create SidebarNavigation user control with width 250px (expanded) / 48px (collapsed)
    - Apply background color #252526 with border right #3E3E42
    - Implement active item styling (#37373D background, #007ACC left border 2px)
    - Implement hover effect (#2A2D2E background)
    - Add collapse/expand button functionality
    - _Requirements: 11.4_
  
  - [ ] 5.6 Implement ConfirmationPanel control
    - Create ConfirmationPanel user control that slides in from right
    - Add semi-transparent overlay background
    - Include message label, optional details, Confirm and Cancel buttons
    - Implement slide-in/out animation
    - _Requirements: 12.4_

- [ ] 6. Implement login and authentication
  - [ ] 6.1 Create LoginForm
    - Design form with centered layout (600x400), VS Code dark theme
    - Add PictureBox for github.com.png logo at top
    - Add CustomTextBox controls for Username and Password
    - Add CustomButton for Login
    - Add Label for inline error messages below form
    - _Requirements: 1.2, 1.3, 11.1, 11.2_
  
  - [ ] 6.2 Implement login logic
    - Wire Login button click event to call UserService.AuthenticateUser
    - On success, call SessionManager.SetSession and UserRepository.UpdateLastLogin
    - Redirect to appropriate dashboard based on RoleCode
    - On failure, display inline error message without popup
    - Handle database connection errors with inline message
    - _Requirements: 1.1, 1.2, 1.4, 1.5, 10.2, 15.1_

- [ ] 7. Create base dashboard form and role-specific dashboards
  - [ ] 7.1 Create BaseDashboardForm
    - Design full-screen form (1920x1080) with VS Code dark theme
    - Add SidebarNavigation control on left
    - Add top bar panel with user info label (SessionManager.FullName) and logout button
    - Add content panel on right for module cards
    - Implement logout button to call SessionManager.ClearSession and redirect to LoginForm
    - _Requirements: 2.5, 11.1, 11.4, 15.2, 15.3_
  
  - [ ] 7.2 Create AdminDashboardForm
    - Inherit from BaseDashboardForm
    - Add 10 module cards/buttons: User Management, Role Management, Product Management, Category Management, Supplier Management, Sales Overview, Inventory Overview, Reports Center, System Settings, Audit Trail
    - Wire each button to open corresponding form
    - _Requirements: 2.1_
  
  - [ ] 7.3 Create ManagerDashboardForm
    - Inherit from BaseDashboardForm
    - Add 7 module cards/buttons: Product Management, Category Management, Supplier Management, Sales Reports, Inventory Management, Staff Performance, Financial Reports
    - Wire each button to open corresponding form
    - _Requirements: 2.2_
  
  - [ ] 7.4 Create CashierDashboardForm
    - Inherit from BaseDashboardForm
    - Add 5 module cards/buttons: Point of Sale, Product Lookup, Customer Management, My Sales Report, Transaction History
    - Wire each button to open corresponding form
    - _Requirements: 2.3_
  
  - [ ] 7.5 Create ClerkDashboardForm
    - Inherit from BaseDashboardForm (same layout as Admin)
    - Add 8 module cards/buttons: Product Management, Category Management, Supplier Management, Stock Receiving, Stock Adjustments, Stock Transfer, Inventory Reports, Low Stock Alerts
    - Wire each button to open corresponding form
    - _Requirements: 2.4_

- [ ] 8. Implement User Management module (Admin only)
  - [ ] 8.1 Create UserManagementForm
    - Design form with CustomDataGridView for users list
    - Add search CustomTextBox with real-time filtering
    - Add Add, Edit, Delete CustomButtons
    - Add inline form panel for create/edit with fields: Username, Email, Password, FullName, RoleID dropdown, ProfileImagePath, Status dropdown
    - Add validation labels for each field
    - _Requirements: 3.1, 3.3, 11.1, 12.1, 12.2, 13.1_
  
  - [ ] 8.2 Implement user CRUD operations
    - Wire Add button to show inline form with empty fields
    - Wire Edit button to populate inline form with selected user data
    - Wire Save button to call UserService.CreateUser or UpdateUser with validation
    - Display inline validation errors without popups
    - Wire Delete button to show ConfirmationPanel, then call UserService.DeleteUser
    - Display toast notification on success
    - Refresh DataGridView after each operation
    - _Requirements: 3.1, 3.2, 3.4, 3.5, 12.2, 12.4, 12.5_
  
  - [ ] 8.3 Implement search and filter
    - Wire search textbox TextChanged event to filter DataGridView rows
    - Filter by Username, Email, or FullName columns
    - _Requirements: 3.3, 13.1, 13.2_

- [ ] 9. Implement Product Management module
  - [ ] 9.1 Create ProductManagementForm
    - Design form with CustomDataGridView for products list
    - Add search CustomTextBox and category filter dropdown
    - Add Add, Edit, Delete CustomButtons
    - Add inline form panel with fields: ProductCode (auto-generated), ProductName, Description, CategoryID dropdown, SupplierID dropdown, CostPrice, SellingPrice, DiscountPercent, TaxRate, StockQuantity, ReorderLevel, MainImagePath, Barcode, SKU, Status dropdown
    - Add validation labels and low stock indicator
    - _Requirements: 4.1, 4.3, 11.1, 12.1, 12.2, 13.1_
  
  - [ ] 9.2 Implement product CRUD operations
    - Wire Add button to show inline form with auto-generated ProductCode
    - Wire Edit button to populate inline form with selected product data
    - Wire Save button to call ProductService.CreateProduct or UpdateProduct with validation
    - Display inline validation errors for required fields, unique constraints, positive prices
    - Wire Delete button to show ConfirmationPanel, then call ProductService.DeleteProduct
    - Display toast notification on success
    - Refresh DataGridView after each operation
    - _Requirements: 4.1, 4.2, 4.4, 4.5, 12.2, 12.4, 12.5_
  
  - [ ] 9.3 Implement search, filter, and low stock alerts
    - Wire search textbox to filter by ProductCode, ProductName, Barcode, or SKU
    - Wire category dropdown to filter by CategoryID
    - Highlight rows in red where StockQuantity <= ReorderLevel
    - _Requirements: 4.3, 8.4, 13.1, 13.2_

- [ ] 10. Implement Category Management module
  - [ ] 10.1 Create CategoryManagementForm
    - Design form with CustomDataGridView for categories list
    - Add search CustomTextBox
    - Add Add, Edit, Delete CustomButtons
    - Add inline form panel with fields: CategoryCode (auto-generated), CategoryName, ParentCategoryID dropdown, Description, DisplayOrder, Status dropdown
    - Add validation labels
    - _Requirements: 5.1, 5.3, 11.1, 12.1, 12.2, 13.1_
  
  - [ ] 10.2 Implement category CRUD operations
    - Wire Add button to show inline form with auto-generated CategoryCode
    - Wire Edit button to populate inline form with selected category data
    - Wire Save button to call CategoryService.CreateCategory or UpdateCategory with validation
    - Display inline validation errors for unique constraints
    - Wire Delete button to check for associated products, show ConfirmationPanel or error message, then call CategoryService.DeleteCategory
    - Display toast notification on success
    - Refresh DataGridView after each operation
    - _Requirements: 5.1, 5.4, 5.5, 12.2, 12.4, 12.5_
  
  - [ ] 10.3 Implement search and hierarchical display
    - Wire search textbox to filter by CategoryCode or CategoryName
    - Display ParentCategoryName in DataGridView for hierarchical view
    - _Requirements: 5.2, 5.3, 13.1_

- [ ] 11. Implement Supplier Management module
  - [ ] 11.1 Create SupplierManagementForm
    - Design form with CustomDataGridView for suppliers list
    - Add search CustomTextBox
    - Add Add, Edit, Delete CustomButtons
    - Add inline form panel with fields: SupplierCode (auto-generated), SupplierName, ContactPerson, PhoneNumber, Email, Address, City, PostalCode, Country, SupplierType, PaymentTerms, Status dropdown
    - Add validation labels
    - _Requirements: 6.1, 6.3, 11.1, 12.1, 12.2, 13.1_
  
  - [ ] 11.2 Implement supplier CRUD operations
    - Wire Add button to show inline form with auto-generated SupplierCode
    - Wire Edit button to populate inline form with selected supplier data
    - Wire Save button to call SupplierService.CreateSupplier or UpdateSupplier with validation
    - Display inline validation errors for unique constraints and email/phone format
    - Wire Delete button to show ConfirmationPanel, then call SupplierService.DeleteSupplier
    - Display toast notification on success
    - Refresh DataGridView after each operation
    - _Requirements: 6.1, 6.2, 6.4, 6.5, 12.2, 12.4, 12.5_
  
  - [ ] 11.3 Implement search functionality
    - Wire search textbox to filter by SupplierCode, SupplierName, or ContactPerson
    - _Requirements: 6.3, 13.1_

- [ ] 12. Implement Sales/POS module (Cashier)
  - [ ] 12.1 Create SalesForm (POS interface)
    - Design form with product search/scan CustomTextBox at top
    - Add shopping cart CustomDataGridView with columns: ProductName, Quantity, UnitPrice, DiscountAmount, TaxAmount, LineTotal
    - Add product list panel on left with available products
    - Add quantity NumericUpDown and Add to Cart button
    - Add Remove Item button for selected cart item
    - Add labels for SubTotal, TaxAmount, DiscountAmount, TotalAmount with real-time calculation
    - Add payment method dropdown (Cash, Credit Card, Debit Card)
    - Add Complete Sale and Clear Cart CustomButtons
    - _Requirements: 7.1, 7.3, 11.1_
  
  - [ ] 12.2 Implement POS operations
    - Wire product search textbox to filter product list by ProductCode, ProductName, or Barcode
    - Wire Add to Cart button to add selected product with quantity to cart DataGridView
    - Implement real-time calculation of SubTotal, TaxAmount, TotalAmount as items are added/removed
    - Wire Remove Item button to remove selected item from cart
    - Wire Clear Cart button to empty cart DataGridView
    - Wire Complete Sale button to call SalesService.ProcessSale with validation
    - Display inline error if insufficient stock
    - Display toast notification on successful sale completion
    - Clear cart after successful sale
    - _Requirements: 7.1, 7.2, 7.4, 7.5, 12.5_
  
  - [ ] 12.3 Implement stock validation
    - Check product StockQuantity before adding to cart
    - Display inline error message if quantity exceeds available stock
    - Validate entire cart before completing sale
    - _Requirements: 7.2_

- [ ] 13. Implement Inventory Management module (Clerk)
  - [ ] 13.1 Create InventoryForm
    - Design form with CustomDataGridView for stock movements list
    - Add filter dropdowns for Product, MovementType (IN/OUT/ADJUSTMENT)
    - Add date range pickers for start and end dates
    - Add Stock In, Stock Out, Stock Adjustment CustomButtons
    - Add inline form panel with fields: ProductID dropdown, Quantity, Reason, Notes
    - Add Low Stock Alerts section with CustomDataGridView showing products where StockQuantity <= ReorderLevel
    - _Requirements: 8.3, 8.4, 11.1, 13.1_
  
  - [ ] 13.2 Implement stock movement operations
    - Wire Stock In button to show inline form, then call StockMovementService.RecordStockIn
    - Wire Stock Out button to show inline form, then call StockMovementService.RecordStockOut
    - Wire Stock Adjustment button to show inline form, then call StockMovementService.RecordStockAdjustment
    - Display toast notification on success
    - Refresh stock movements DataGridView after each operation
    - Refresh low stock alerts section
    - _Requirements: 8.1, 8.2, 8.5, 12.5_
  
  - [ ] 13.3 Implement filters and low stock alerts
    - Wire product dropdown to filter movements by ProductID
    - Wire movement type dropdown to filter by MovementType
    - Wire date range pickers to filter by MovementDate
    - Load low stock products using ProductService.GetLowStockProducts
    - _Requirements: 8.3, 8.4, 13.1, 13.2_

- [ ] 14. Implement Reports module
  - [ ] 14.1 Create ReportsForm
    - Design form with report type dropdown (Sales Report, Inventory Report, User Activity Report)
    - Add date range pickers for start and end dates
    - Add filter options panel (varies by report type)
    - Add CustomDataGridView for report results
    - Add Generate Report and Export to PDF CustomButtons
    - _Requirements: 14.1, 14.2, 11.1_
  
  - [ ] 14.2 Implement report generation
    - Wire Generate Report button to call appropriate service method based on report type
    - For Sales Report, call SalesService.GetSalesByDateRange and display in DataGridView
    - For Inventory Report, call StockMovementService.GetMovementsByDateRange
    - For User Activity Report, call AuditService.GetLogsByDateRange
    - Filter results by logged-in user if role is Cashier
    - _Requirements: 14.1, 14.2, 14.5_
  
  - [ ] 14.3 Implement PDF export
    - Install QuestPDF NuGet package
    - Wire Export to PDF button to generate PDF from DataGridView data
    - Save PDF directly to Documents folder without showing file dialog
    - Display toast notification with file path on success
    - _Requirements: 14.3, 14.4_

- [ ] 15. Implement Audit Trail module (Admin only)
  - [ ] 15.1 Create AuditLogForm
    - Design form with CustomDataGridView for audit logs list
    - Add filter dropdowns for User, Action, Table
    - Add date range pickers for start and end dates
    - Add search CustomTextBox
    - Add View Details button to show OldValue and NewValue in a panel
    - _Requirements: 9.3, 11.1, 13.1_
  
  - [ ] 15.2 Implement audit log viewing and filtering
    - Load all logs using AuditService.GetAllLogs on form load
    - Wire user dropdown to filter by UserID
    - Wire action dropdown to filter by Action
    - Wire table dropdown to filter by TableName
    - Wire date range pickers to filter by LogDate
    - Wire search textbox to filter by Action, TableName, or Description
    - Wire View Details button to display OldValue and NewValue JSON in formatted panel
    - _Requirements: 9.3, 9.4, 13.1, 13.2_

- [ ] 16. Implement session validation and role-based access control
  - [ ] 16.1 Add session validation to all forms
    - Override OnLoad method in BaseDashboardForm to check SessionManager.IsLoggedIn
    - Redirect to LoginForm if session is invalid
    - _Requirements: 15.2_
  
  - [ ] 16.2 Implement role-based form access
    - Check SessionManager.RoleCode before opening each form
    - Hide/disable module buttons in dashboards based on role permissions
    - For UserManagementForm, only allow access if SessionManager.IsAdmin
    - For AuditLogForm, only allow access if SessionManager.IsAdmin
    - For ProductManagementForm, allow access if IsAdmin, IsManager, or IsClerk
    - For SalesForm, allow access if IsCashier
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 3.6_

- [ ] 17. Final integration and testing
  - [ ] 17.1 Test complete user workflows
    - Test login flow for each role (Admin, Manager, Cashier, Clerk)
    - Test product creation by Clerk, then sale by Cashier
    - Test stock movement recording and low stock alerts
    - Test user creation by Admin and login with new user
    - Test report generation and PDF export
    - _Requirements: All_
  
  - [ ] 17.2 Verify VS Code theme consistency
    - Review all forms and controls for consistent color scheme
    - Verify all colors match specification (#1E1E1E, #252526, #007ACC, etc.)
    - Test hover and active states on all interactive elements
    - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5_
  
  - [ ] 17.3 Verify no popup dialogs
    - Test all error scenarios to ensure inline error messages display
    - Test all delete operations to ensure ConfirmationPanel displays instead of MessageBox
    - Test all success operations to ensure toast notifications display
    - Search codebase for MessageBox.Show() calls and remove any found
    - _Requirements: 1.2, 12.2, 12.4, 12.5_
  
  - [ ] 17.4 Test audit logging
    - Perform CRUD operations on all entities
    - Verify AuditLogs table contains entries for each operation
    - Verify OldValue and NewValue are stored as JSON
    - Verify UserID and IPAddress are captured
    - _Requirements: 9.1, 9.2_
  
  - [ ] 17.5 Test database error handling
    - Disconnect database and test login form error display
    - Test transaction rollback by simulating errors during sale processing
    - Verify all errors are logged to file using NLog
    - Verify no technical error details are shown to users
    - _Requirements: 10.2, 10.4, 10.5_
