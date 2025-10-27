# SyncVerse Studio - Point of Sale System

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Technology Stack](#technology-stack)
- [System Architecture](#system-architecture)
- [Data Model](#data-model)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [User Roles and Permissions](#user-roles-and-permissions)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## Overview

SyncVerse Studio is a comprehensive, enterprise-grade Point of Sale (POS) system built with .NET 8.0 and Windows Forms. The application provides a complete retail management solution featuring inventory control, sales processing, customer relationship management, invoicing, and real-time analytics.

### Purpose

This system is designed to streamline retail operations by providing:

- Efficient point-of-sale transaction processing
- Real-time inventory management and tracking
- Customer relationship management with purchase history
- Comprehensive invoicing and payment processing
- Role-based access control for security
- Advanced analytics and reporting capabilities
- Multi-payment method support including QR code generation

### Scope

The application serves three primary user roles:

- **Administrators**: Full system access including user management, system configuration, and comprehensive reporting
- **Cashiers**: Point-of-sale operations, customer management, and transaction processing
- **Inventory Clerks**: Product management, stock control, supplier management, and inventory reporting


## Key Features

### Point of Sale

- Modern, intuitive cashier interface with real-time dashboard
- Product search and filtering by category
- Shopping cart with real-time calculations
- Multiple payment methods: Cash, Card, Mobile/QR
- Automatic change calculation for cash transactions
- QR code generation for mobile payments
- Transaction hold and resume functionality
- Barcode scanning support

### Inventory Management

- Complete product lifecycle management (CRUD operations)
- Category and supplier management
- Stock level tracking with low-stock alerts
- Inventory movement logging
- Product image management
- SKU and barcode support
- Cost and selling price tracking
- Profit margin calculations

### Customer Relationship Management

- Customer profile management
- Purchase history tracking
- Loyalty points system
- Walk-in customer support
- Customer data encryption for privacy
- Customer analytics and insights

### Invoicing and Payments

- Automated invoice generation with unique numbering
- Professional invoice printing with company branding
- Multiple payment method support
- Partial payment tracking
- Payment link generation for remote payments
- Invoice status management (Active, Paid, Void, Overdue)
- Tax calculation with configurable rates

### Analytics and Reporting

- Real-time sales dashboard with key metrics
- Revenue and profit tracking
- Sales trend visualization with charts
- Invoice status distribution
- Product popularity analysis
- Inventory reports
- Audit trail for all transactions

### Security

- Role-based access control (RBAC)
- BCrypt password hashing
- User authentication and session management
- Comprehensive audit logging
- Customer data encryption
- Permission-based feature access


## Technology Stack

### Core Framework

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Application framework |
| C# | 12 | Programming language |
| Windows Forms | 8.0 | User interface framework |
| Entity Framework Core | 8.0.0 | Object-relational mapping (ORM) |

### Database

| Technology | Version | Purpose |
|------------|---------|---------|
| SQL Server | 2019+ | Primary database |
| SQL Server LocalDB | Latest | Development database |

### NuGet Packages

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 8.0.0 | Data access layer |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.0 | SQL Server provider |
| Microsoft.EntityFrameworkCore.Tools | 8.0.0 | EF Core CLI tools |
| BCrypt.Net-Next | 4.0.3 | Password hashing |
| FontAwesome.Sharp | 6.3.0 | Icon library |
| MaterialSkin.2 | 2.1.0 | Material design UI components |
| QRCoder | 1.7.0 | QR code generation |
| ZXing.Net | 0.16.9 | Barcode scanning |
| QuestPDF | 2023.12.6 | PDF invoice generation |
| Newtonsoft.Json | 13.0.3 | JSON serialization |
| System.Drawing.Common | 8.0.0 | Image processing |
| System.Windows.Forms.DataVisualization | 1.0.0-prerelease | Charts and graphs |

### Development Tools

- Visual Studio 2022 or later
- SQL Server Management Studio (SSMS)
- .NET 8.0 SDK
- Git for version control


## System Architecture

### Architectural Pattern

The application follows a layered architecture with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  (Windows Forms Views - LoginForm, MainDashboard, POS, etc.) │
└────────────────────────┬────────────────────────────────────┘
						 │
┌────────────────────────┴────────────────────────────────────┐
│                     Business Logic Layer                     │
│        (Services - AuthenticationService, etc.)              │
└────────────────────────┬────────────────────────────────────┘
						 │
┌────────────────────────┴────────────────────────────────────┐
│                      Data Access Layer                       │
│         (Entity Framework Core - ApplicationDbContext)       │
└────────────────────────┬────────────────────────────────────┘
						 │
┌────────────────────────┴────────────────────────────────────┐
│                       Database Layer                         │
│                    (SQL Server - POSDB)                      │
└─────────────────────────────────────────────────────────────┘
```

### Application Flow

```
User Login
	│
	├─→ Authentication Service
	│       │
	│       └─→ Verify Credentials (BCrypt)
	│               │
	│               ├─→ Success: Load Role-Based Dashboard
	│               │       │
	│               │       ├─→ Administrator Dashboard
	│               │       │       ├─→ User Management
	│               │       │       ├─→ System Configuration
	│               │       │       ├─→ Reports & Analytics
	│               │       │       └─→ Audit Logs
	│               │       │
	│               │       ├─→ Cashier Dashboard
	│               │       │       ├─→ Point of Sale
	│               │       │       ├─→ Customer Management
	│               │       │       ├─→ Sales History
	│               │       │       └─→ Real-time Metrics
	│               │       │
	│               │       └─→ Inventory Clerk Dashboard
	│               │               ├─→ Product Management
	│               │               ├─→ Category Management
	│               │               ├─→ Supplier Management
	│               │               └─→ Inventory Reports
	│               │
	│               └─→ Failure: Display Error Message
	│
	└─→ Audit Log Entry Created
```

### Point of Sale Transaction Flow

```
Product Selection
	│
	├─→ Search/Filter Products
	│       │
	│       └─→ Add to Shopping Cart
	│               │
	│               ├─→ Validate Stock Availability
	│               └─→ Calculate Line Total
	│
	├─→ Review Cart
	│       │
	│       ├─→ Adjust Quantities
	│       ├─→ Remove Items
	│       └─→ Apply Tax Rate
	│
	├─→ Select Payment Method
	│       │
	│       ├─→ Cash Payment
	│       │       ├─→ Enter Amount
	│       │       └─→ Calculate Change
	│       │
	│       ├─→ Card Payment
	│       │       └─→ Process Card Transaction
	│       │
	│       └─→ Mobile/QR Payment
	│               └─→ Generate QR Code
	│
	├─→ Customer Selection
	│       │
	│       ├─→ Walk-in Customer
	│       ├─→ Existing Customer
	│       └─→ New Customer Registration
	│
	├─→ Complete Transaction
	│       │
	│       ├─→ Create Sale Record
	│       ├─→ Generate Invoice
	│       ├─→ Record Payment
	│       ├─→ Update Inventory
	│       └─→ Create Audit Log
	│
	└─→ Print/Email Invoice
			│
			└─→ Clear Cart & Reset
```


## Data Model

### Entity Relationship Diagram

```
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│   Category   │         │   Supplier   │         │   Customer   │
├──────────────┤         ├──────────────┤         ├──────────────┤
│ Id (PK)      │         │ Id (PK)      │         │ Id (PK)      │
│ Name         │         │ Name         │         │ FirstName    │
│ Description  │         │ ContactPerson│         │ LastName     │
└──────┬───────┘         │ Phone        │         │ Phone        │
	   │                 │ Email        │         │ Email        │
	   │                 │ Address      │         │ Address      │
	   │                 └──────┬───────┘         │ LoyaltyPoints│
	   │                        │                 └──────┬───────┘
	   │                        │                        │
	   │                        │                        │
	   └────────┬───────────────┘                        │
				│                                        │
		 ┌──────▼──────────┐                            │
		 │    Product      │                            │
		 ├─────────────────┤                            │
		 │ Id (PK)         │                            │
		 │ Name            │                            │
		 │ Description     │                            │
		 │ Barcode         │                            │
		 │ SKU             │                            │
		 │ CategoryId (FK) │                            │
		 │ SupplierId (FK) │                            │
		 │ CostPrice       │                            │
		 │ SellingPrice    │                            │
		 │ Quantity        │                            │
		 │ MinQuantity     │                            │
		 │ ImagePath       │                            │
		 │ IsActive        │                            │
		 └────────┬────────┘                            │
				  │                                     │
				  │                                     │
	┌─────────────┼─────────────────────────────────────┤
	│             │                                     │
	│      ┌──────▼──────────┐                  ┌──────▼──────────┐
	│      │   SaleItem      │                  │      Sale       │
	│      ├─────────────────┤                  ├─────────────────┤
	│      │ Id (PK)         │                  │ Id (PK)         │
	│      │ SaleId (FK)     │◄─────────────────┤ InvoiceNumber   │
	│      │ ProductId (FK)  │                  │ CustomerId (FK) │
	│      │ Quantity        │                  │ CashierId (FK)  │
	│      │ UnitPrice       │                  │ TotalAmount     │
	│      │ TotalPrice      │                  │ TaxAmount       │
	│      └─────────────────┘                  │ DiscountAmount  │
	│                                           │ PaymentMethod   │
	│                                           │ SaleDate        │
	│                                           │ Status          │
	│                                           └────────┬────────┘
	│                                                    │
	│                                                    │
	│      ┌─────────────────┐                  ┌───────▼─────────┐
	│      │  InvoiceItem    │                  │    Invoice      │
	│      ├─────────────────┤                  ├─────────────────┤
	│      │ Id (PK)         │                  │ Id (PK)         │
	│      │ InvoiceId (FK)  │◄─────────────────┤ InvoiceNumber   │
	│      │ ProductId (FK)  │                  │ CustomerId (FK) │
	│      │ Quantity        │                  │ CreatedByUserId │
	│      │ UnitPrice       │                  │ SubTotal        │
	│      │ TotalPrice      │                  │ TaxAmount       │
	│      └─────────────────┘                  │ DiscountAmount  │
	│                                           │ TotalAmount     │
	│                                           │ PaidAmount      │
	│                                           │ BalanceAmount   │
	│                                           │ Status          │
	│                                           │ InvoiceDate     │
	│                                           │ DueDate         │
	│                                           │ SaleId (FK)     │
	│                                           └────────┬────────┘
	│                                                    │
	│                                                    │
	│                                           ┌────────▼────────┐
	│                                           │    Payment      │
	│                                           ├─────────────────┤
	│                                           │ Id (PK)         │
	│                                           │ PaymentReference│
	│                                           │ InvoiceId (FK)  │
	│                                           │ SaleId (FK)     │
	│                                           │ Amount          │
	│                                           │ PaymentMethod   │
	│                                           │ PaymentDate     │
	│                                           │ Status          │
	│                                           │ ProcessedByUserId│
	│                                           └─────────────────┘
	│
	│      ┌─────────────────────┐
	└─────►│ InventoryMovement   │
		   ├─────────────────────┤
		   │ Id (PK)             │
		   │ ProductId (FK)      │
		   │ UserId (FK)         │
		   │ MovementType        │
		   │ Quantity            │
		   │ Reason              │
		   │ MovementDate        │
		   └─────────────────────┘

┌──────────────┐         ┌──────────────────┐         ┌──────────────────┐
│     User     │         │   PaymentLink    │         │ HeldTransaction  │
├──────────────┤         ├──────────────────┤         ├──────────────────┤
│ Id (PK)      │         │ Id (PK)          │         │ Id (PK)          │
│ Username     │         │ LinkCode         │         │ TransactionCode  │
│ Password     │         │ InvoiceId (FK)   │         │ CustomerId (FK)  │
│ Email        │         │ CustomerId (FK)  │         │ HeldByUserId (FK)│
│ FirstName    │         │ Amount           │         │ CartData (JSON)  │
│ LastName     │         │ Status           │         │ TotalAmount      │
│ Role         │         │ CreatedByUserId  │         │ HeldAt           │
│ IsActive     │         │ ExpiresAt        │         │ ResumedAt        │
│ CreatedAt    │         │ PaymentId (FK)   │         └──────────────────┘
│ UpdatedAt    │         └──────────────────┘
└──────────────┘

┌──────────────────────┐         ┌──────────────────────────┐
│      AuditLog        │         │ OnlineStoreIntegration   │
├──────────────────────┤         ├──────────────────────────┤
│ Id (PK)              │         │ Id (PK)                  │
│ UserId (FK)          │         │ Platform                 │
│ Action               │         │ ApiKey                   │
│ TableName            │         │ ApiSecret                │
│ RecordId             │         │ StoreUrl                 │
│ OldValues            │         │ IsActive                 │
│ NewValues            │         │ LastSyncedAt             │
│ Timestamp            │         │ LastSyncStatus           │
│ IpAddress            │         │ SyncFrequencyMinutes     │
└──────────────────────┘         └──────────────────────────┘
```


### Core Data Entities

#### User

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique user identifier |
| Username | nvarchar(50) | Unique, Required | Login username |
| Password | nvarchar(255) | Required | BCrypt hashed password |
| Email | nvarchar(100) | Required | User email address |
| FirstName | nvarchar(50) | Required | User first name |
| LastName | nvarchar(50) | Required | User last name |
| Role | nvarchar(20) | Required | Administrator, Cashier, InventoryClerk |
| IsActive | bit | Default: 1 | Account status |
| CreatedAt | datetime2 | Default: GETDATE() | Account creation timestamp |
| UpdatedAt | datetime2 | Default: GETDATE() | Last update timestamp |

#### Product

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique product identifier |
| Name | nvarchar(100) | Required | Product name |
| Description | nvarchar(255) | Nullable | Product description |
| Barcode | nvarchar(50) | Unique, Nullable | Product barcode |
| SKU | nvarchar(50) | Unique, Nullable | Stock keeping unit |
| CategoryId | int | FK, Nullable | Reference to Category |
| SupplierId | int | FK, Nullable | Reference to Supplier |
| CostPrice | decimal(18,2) | Required | Purchase cost |
| SellingPrice | decimal(18,2) | Required | Retail price |
| Quantity | int | Default: 0 | Current stock level |
| MinQuantity | int | Default: 10 | Reorder threshold |
| ImagePath | nvarchar(255) | Nullable | Product image file path |
| IsActive | bit | Default: 1 | Product availability status |
| IsSyncedToOnlineStore | bit | Default: 0 | E-commerce sync status |
| LastSyncedAt | datetime2 | Nullable | Last sync timestamp |
| OnlineStoreProductId | nvarchar(100) | Nullable | External product ID |
| CreatedAt | datetime2 | Default: GETDATE() | Creation timestamp |
| UpdatedAt | datetime2 | Default: GETDATE() | Last update timestamp |

#### Invoice

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Unique invoice identifier |
| InvoiceNumber | nvarchar(20) | Unique, Required | Format: INV-YYYYMMDD-HHMMSS |
| CustomerId | int | FK, Nullable | Reference to Customer |
| CustomerName | nvarchar(100) | Nullable | Walk-in customer name |
| CreatedByUserId | int | FK, Required | User who created invoice |
| SubTotal | decimal(18,2) | Required | Sum of line items |
| TaxAmount | decimal(18,2) | Default: 0 | Calculated tax |
| DiscountAmount | decimal(18,2) | Default: 0 | Applied discount |
| TotalAmount | decimal(18,2) | Required | Final amount |
| PaidAmount | decimal(18,2) | Default: 0 | Amount received |
| BalanceAmount | decimal(18,2) | Required | Remaining balance |
| Status | nvarchar(20) | Required | Active, Paid, Void, Overdue |
| InvoiceDate | datetime2 | Default: GETDATE() | Invoice creation date |
| DueDate | datetime2 | Nullable | Payment due date |
| Notes | nvarchar(500) | Nullable | Additional notes |
| VoidReason | nvarchar(500) | Nullable | Reason for voiding |
| VoidedAt | datetime2 | Nullable | Void timestamp |
| VoidedByUserId | int | FK, Nullable | User who voided invoice |
| SaleId | int | FK, Nullable | Linked sale transaction |
| CreatedAt | datetime2 | Default: GETDATE() | Creation timestamp |
| UpdatedAt | datetime2 | Default: GETDATE() | Last update timestamp |

#### Sale

.......