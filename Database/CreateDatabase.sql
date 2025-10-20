-- SQL Database Schema for SyncVerse Studio POS System
-- Target Database: khmerdatabase

USE khmerdatabase;

-- Users table for authentication and role management
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL CHECK ([Role] IN ('Administrator', 'Cashier', 'InventoryClerk')),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Categories table for product organization
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
CREATE TABLE [dbo].[Categories] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(255),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Suppliers table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Suppliers]') AND type in (N'U'))
CREATE TABLE [dbo].[Suppliers] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [ContactPerson] NVARCHAR(100),
    [Phone] NVARCHAR(20),
    [Email] NVARCHAR(100),
    [Address] NVARCHAR(255),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Products table for inventory management
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
CREATE TABLE [dbo].[Products] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(255),
    [Barcode] NVARCHAR(50) UNIQUE,
    [SKU] NVARCHAR(50) UNIQUE,
    [CategoryId] INT FOREIGN KEY REFERENCES [Categories]([Id]),
    [SupplierId] INT FOREIGN KEY REFERENCES [Suppliers]([Id]),
    [CostPrice] DECIMAL(18,2) NOT NULL,
    [SellingPrice] DECIMAL(18,2) NOT NULL,
    [Quantity] INT NOT NULL DEFAULT 0,
    [MinQuantity] INT NOT NULL DEFAULT 10,
    [ImagePath] NVARCHAR(255),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Customers table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
CREATE TABLE [dbo].[Customers] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FirstName] NVARCHAR(50),
    [LastName] NVARCHAR(50),
    [Phone] NVARCHAR(20),
    [Email] NVARCHAR(100),
    [Address] NVARCHAR(255),
    [LoyaltyPoints] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Sales transactions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
CREATE TABLE [dbo].[Sales] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [InvoiceNumber] NVARCHAR(20) NOT NULL UNIQUE,
    [CustomerId] INT FOREIGN KEY REFERENCES [Customers]([Id]),
    [CashierId] INT FOREIGN KEY REFERENCES [Users]([Id]),
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [DiscountAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [PaymentMethod] NVARCHAR(20) NOT NULL CHECK ([PaymentMethod] IN ('Cash', 'Card', 'Mobile', 'Mixed')),
    [SaleDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Completed' CHECK ([Status] IN ('Pending', 'Completed', 'Cancelled', 'Returned'))
);

-- Sale items table for transaction details
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleItems]') AND type in (N'U'))
CREATE TABLE [dbo].[SaleItems] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SaleId] INT FOREIGN KEY REFERENCES [Sales]([Id]),
    [ProductId] INT FOREIGN KEY REFERENCES [Products]([Id]),
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [TotalPrice] DECIMAL(18,2) NOT NULL,
    [DiscountAmount] DECIMAL(18,2) NOT NULL DEFAULT 0
);

-- Inventory movements table for stock tracking
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryMovements]') AND type in (N'U'))
CREATE TABLE [dbo].[InventoryMovements] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ProductId] INT FOREIGN KEY REFERENCES [Products]([Id]),
    [MovementType] NVARCHAR(20) NOT NULL CHECK ([MovementType] IN ('Sale', 'Purchase', 'Adjustment', 'Transfer', 'Return')),
    [Quantity] INT NOT NULL,
    [Reference] NVARCHAR(100),
    [UserId] INT FOREIGN KEY REFERENCES [Users]([Id]),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Audit logs table for security tracking
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
CREATE TABLE [dbo].[AuditLogs] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT FOREIGN KEY REFERENCES [Users]([Id]),
    [Action] NVARCHAR(100) NOT NULL,
    [TableName] NVARCHAR(50),
    [RecordId] INT,
    [OldValues] NVARCHAR(MAX),
    [NewValues] NVARCHAR(MAX),
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [IpAddress] NVARCHAR(45)
);

-- Insert default admin user
INSERT INTO [dbo].[Users] ([Username], [Password], [Email], [FirstName], [LastName], [Role])
VALUES ('vi', '$2a$11$8K1p/a0dL8B9WvjqRJlqaOK9vF8JXw8tJ1K1K1K1K1K1K1K1K1K1K1', 'vi@syncverse.com', 'Vi', 'Admin', 'Administrator');

-- Insert sample categories
INSERT INTO [dbo].[Categories] ([Name], [Description])
VALUES 
    ('Electronics', 'Electronic devices and accessories'),
    ('Beverages', 'Drinks and beverages'),
    ('Snacks', 'Snacks and confectionery'),
    ('Stationery', 'Office and school supplies');

-- Insert sample suppliers
INSERT INTO [dbo].[Suppliers] ([Name], [ContactPerson], [Phone], [Email])
VALUES 
    ('Tech Solutions Ltd', 'John Smith', '+855123456789', 'contact@techsolutions.com'),
    ('Fresh Beverages Co', 'Mary Johnson', '+855987654321', 'orders@freshbev.com');

-- Insert sample products
INSERT INTO [dbo].[Products] ([Name], [Description], [Barcode], [SKU], [CategoryId], [SupplierId], [CostPrice], [SellingPrice], [Quantity], [MinQuantity])
VALUES 
    ('USB Cable Type-C', '1-meter USB-C charging cable', '1234567890123', 'USB-C-001', 1, 1, 2.50, 5.00, 50, 10),
    ('Coca Cola 330ml', 'Classic Coca Cola can', '2345678901234', 'COKE-330', 2, 2, 0.50, 1.00, 100, 20),
    ('Notebook A4', 'Ruled notebook 200 pages', '3456789012345', 'NOTE-A4-200', 4, 1, 1.00, 2.50, 75, 15),
    ('Potato Chips', 'Original flavor potato chips', '4567890123456', 'CHIPS-001', 3, 2, 0.75, 1.50, 80, 20);

-- Insert sample customer
INSERT INTO [dbo].[Customers] ([FirstName], [LastName], [Phone], [Email])
VALUES ('Guest', 'Customer', '', '');

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products] ([CategoryId]);
CREATE NONCLUSTERED INDEX [IX_Products_Barcode] ON [dbo].[Products] ([Barcode]);
CREATE NONCLUSTERED INDEX [IX_Sales_CashierId] ON [dbo].[Sales] ([CashierId]);
CREATE NONCLUSTERED INDEX [IX_Sales_SaleDate] ON [dbo].[Sales] ([SaleDate]);
CREATE NONCLUSTERED INDEX [IX_SaleItems_SaleId] ON [dbo].[SaleItems] ([SaleId]);
CREATE NONCLUSTERED INDEX [IX_InventoryMovements_ProductId] ON [dbo].[InventoryMovements] ([ProductId]);