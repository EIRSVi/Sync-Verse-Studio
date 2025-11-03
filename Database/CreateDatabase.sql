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


-- Insert sample categories (Cambodia-based)
INSERT INTO [dbo].[Categories] ([Name], [Description], [IsActive], [CreatedAt])
VALUES 
    ('Soft Drinks', 'Carbonated and non-carbonated beverages', 1, GETDATE()),
    ('Beer & Alcohol', 'Alcoholic beverages and beer', 1, GETDATE()),
    ('Water', 'Bottled water and mineral water', 1, GETDATE()),
    ('Energy Drinks', 'Energy and sports drinks', 1, GETDATE()),
    ('Cosmetics', 'Beauty and personal care products', 1, GETDATE());

-- Insert sample suppliers (Cambodia-based)
INSERT INTO [dbo].[Suppliers] ([Name], [ContactPerson], [Phone], [Email], [Address], [IsActive], [CreatedAt])
VALUES 
    ('KRUD Khmer Beverages', 'Sok Pisey', '+855 23 720 123', 'sales@krudbeverage.com.kh', 'Phnom Penh, Cambodia', 1, GETDATE()),
    ('Hanuman Trading Co', 'Chea Sophea', '+855 12 345 678', 'orders@hanuman.com.kh', 'Siem Reap, Cambodia', 1, GETDATE()),
    ('Cambodia Cosmetics Supply', 'Lim Dara', '+855 92 888 999', 'contact@cambodiacosmetics.com', 'Phnom Penh, Cambodia', 1, GETDATE()),
    ('Vital Water Cambodia', 'Pov Samnang', '+855 77 123 456', 'info@vitalwater.com.kh', 'Battambang, Cambodia', 1, GETDATE()),
    ('Hanuman Beer Distributor', 'Meas Chanthy', '+855 89 456 789', 'sales@hanumanbeer.com.kh', 'Kampong Cham, Cambodia', 1, GETDATE());

-- Insert sample products (Cambodia-based - 21 products)
INSERT INTO [dbo].[Products] ([Name], [Description], [Barcode], [SKU], [CategoryId], [SupplierId], [CostPrice], [SellingPrice], [Quantity], [MinQuantity], [IsActive], [CreatedAt], [UpdatedAt])
VALUES 
    -- Cambodie/Cambodia Beverages (Soft Drinks)
    ('ABC Stout 330ml', 'Cambodian stout beer', '8850001001001', 'ABC-STOUT-330', 1, 1, 0.50, 0.90, 150, 40, 1, GETDATE(), GETDATE()),
    ('Bayon Beer 330ml', 'Local Cambodian beer', '8850001001002', 'BAYON-330', 1, 1, 0.45, 0.85, 180, 50, 1, GETDATE(), GETDATE()),
    ('Cambodia Beer 330ml', 'Premium local beer', '8850001001003', 'CAMBODIA-330', 1, 1, 0.55, 0.95, 160, 45, 1, GETDATE(), GETDATE()),
    
    -- Cole/Cola Drinks (Soft Drinks)
    ('Coca Cola 330ml', 'Classic Coca Cola', '8850002002001', 'COKE-330', 1, 1, 0.40, 0.75, 200, 60, 1, GETDATE(), GETDATE()),
    ('Pepsi 330ml', 'Pepsi cola drink', '8850002002002', 'PEPSI-330', 1, 1, 0.38, 0.70, 180, 55, 1, GETDATE(), GETDATE()),
    ('Fanta Orange 330ml', 'Orange flavored soda', '8850002002003', 'FANTA-330', 1, 1, 0.35, 0.65, 190, 50, 1, GETDATE(), GETDATE()),
    
    -- Cusmic/Cosmetics
    ('Khmer Beauty Cream 50g', 'Natural beauty cream', '8850005005001', 'COSMETIC-001', 5, 3, 2.50, 5.00, 80, 20, 1, GETDATE(), GETDATE()),
    ('Cambodian Face Wash 100ml', 'Herbal face cleanser', '8850005005002', 'COSMETIC-002', 5, 3, 3.00, 6.00, 70, 15, 1, GETDATE(), GETDATE()),
    ('Natural Soap Bar 100g', 'Traditional Khmer soap', '8850005005003', 'COSMETIC-003', 5, 3, 1.50, 3.00, 100, 25, 1, GETDATE(), GETDATE()),
    
    -- Food
    ('Instant Noodles', 'Cambodian style noodles', '8850001003001', 'FOOD-001', 1, 2, 0.30, 0.60, 250, 80, 1, GETDATE(), GETDATE()),
    ('Rice Crackers 100g', 'Traditional rice snack', '8850001003002', 'FOOD-002', 1, 2, 0.50, 1.00, 150, 40, 1, GETDATE(), GETDATE()),
    ('Dried Fish Snack 50g', 'Cambodian dried fish', '8850001003003', 'FOOD-003', 1, 2, 1.20, 2.50, 100, 30, 1, GETDATE(), GETDATE()),
    
    -- Hanuman Energy Drinks
    ('Hanuman Energy Red 250ml', 'Red energy drink', '8850004004001', 'HANUMAN-RED-250', 4, 2, 0.70, 1.20, 120, 30, 1, GETDATE(), GETDATE()),
    ('Hanuman Energy Blue 250ml', 'Blue energy drink', '8850004004002', 'HANUMAN-BLUE-250', 4, 2, 0.70, 1.20, 110, 30, 1, GETDATE(), GETDATE()),
    ('Hanuman Energy Green 250ml', 'Green energy drink', '8850004004003', 'HANUMAN-GREEN-250', 4, 2, 0.70, 1.20, 115, 30, 1, GETDATE(), GETDATE()),
    
    -- Health Products
    ('Vitamin C Tablets', 'Health supplement', '8850005006001', 'HEALTH-001', 5, 3, 3.50, 7.00, 60, 15, 1, GETDATE(), GETDATE()),
    ('Herbal Tea 20 bags', 'Traditional Khmer tea', '8850005006002', 'HEALTH-002', 5, 3, 2.00, 4.00, 80, 20, 1, GETDATE(), GETDATE()),
    
    -- Spirits/Alcohol
    ('Sombai Rice Wine 750ml', 'Cambodian rice wine', '8850002007001', 'SPIRITS-001', 2, 5, 8.00, 15.00, 40, 10, 1, GETDATE(), GETDATE()),
    
    -- Vitaranc/Vitamin Drinks
    ('Vitaranc Orange 500ml', 'Vitamin C orange drink', '8850004008001', 'VITARANC-500', 4, 4, 0.60, 1.10, 140, 35, 1, GETDATE(), GETDATE()),
    
    -- Water
    ('Vital Water 500ml', 'Pure drinking water', '8850003003001', 'VITAL-500', 3, 4, 0.15, 0.30, 300, 100, 1, GETDATE(), GETDATE()),
    ('Aqua Mineral Water 1.5L', 'Mineral water large', '8850003003002', 'AQUA-1500', 3, 4, 0.35, 0.70, 200, 60, 1, GETDATE(), GETDATE());

-- Insert guest customer
INSERT INTO [dbo].[Customers] ([FirstName], [LastName], [Phone], [Email], [Address], [LoyaltyPoints], [CreatedAt])
VALUES 
    ('Guest', 'Customer', '09876543224', 'ac@df.csd', 'pp#dkejr', 9, GETDATE());


-- Create indexes for better performance
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products] ([CategoryId]);
CREATE NONCLUSTERED INDEX [IX_Products_Barcode] ON [dbo].[Products] ([Barcode]);
CREATE NONCLUSTERED INDEX [IX_Sales_CashierId] ON [dbo].[Sales] ([CashierId]);
CREATE NONCLUSTERED INDEX [IX_Sales_SaleDate] ON [dbo].[Sales] ([SaleDate]);
CREATE NONCLUSTERED INDEX [IX_SaleItems_SaleId] ON [dbo].[SaleItems] ([SaleId]);
CREATE NONCLUSTERED INDEX [IX_InventoryMovements_ProductId] ON [dbo].[InventoryMovements] ([ProductId]);