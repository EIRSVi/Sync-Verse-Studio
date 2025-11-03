-- SQL Database Schema for Accounting System
-- Add to existing khmerdatabase

USE khmerdatabase;
GO

-- General Ledger Entries table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GeneralLedgerEntries]') AND type in (N'U'))
CREATE TABLE [dbo].[GeneralLedgerEntries] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [EntryNumber] NVARCHAR(50) NOT NULL UNIQUE,
    [EntryDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [AccountName] NVARCHAR(100) NOT NULL,
    [AccountType] NVARCHAR(20) NOT NULL CHECK ([AccountType] IN ('Asset', 'Liability', 'Equity', 'Revenue', 'Expense')),
    [DebitAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [CreditAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [Description] NVARCHAR(500),
    [ReferenceNumber] NVARCHAR(50),
    [BookOfEntry] NVARCHAR(30) NOT NULL CHECK ([BookOfEntry] IN ('SalesDayBook', 'PurchasesDayBook', 'CashBook', 'GeneralJournal')),
    [RelatedSaleId] INT FOREIGN KEY REFERENCES [Sales]([Id]),
    [RelatedPurchaseId] INT,
    [RelatedPaymentId] INT,
    [CreatedByUserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([Id]),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Purchases table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Purchases]') AND type in (N'U'))
CREATE TABLE [dbo].[Purchases] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [PurchaseNumber] NVARCHAR(50) NOT NULL UNIQUE,
    [SupplierId] INT NOT NULL FOREIGN KEY REFERENCES [Suppliers]([Id]),
    [PurchaseDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [PaidAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK ([Status] IN ('Pending', 'Completed', 'Cancelled')),
    [Notes] NVARCHAR(500),
    [CreatedByUserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([Id]),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Purchase Items table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseItems]') AND type in (N'U'))
CREATE TABLE [dbo].[PurchaseItems] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [PurchaseId] INT NOT NULL FOREIGN KEY REFERENCES [Purchases]([Id]) ON DELETE CASCADE,
    [ProductId] INT NOT NULL FOREIGN KEY REFERENCES [Products]([Id]),
    [Quantity] INT NOT NULL,
    [UnitCost] DECIMAL(18,2) NOT NULL,
    [TotalCost] DECIMAL(18,2) NOT NULL
);
GO

-- Financial Accounts table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FinancialAccounts]') AND type in (N'U'))
CREATE TABLE [dbo].[FinancialAccounts] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [AccountCode] NVARCHAR(50) NOT NULL UNIQUE,
    [AccountName] NVARCHAR(100) NOT NULL,
    [AccountType] NVARCHAR(20) NOT NULL CHECK ([AccountType] IN ('Asset', 'Liability', 'Equity', 'Revenue', 'Expense')),
    [Category] NVARCHAR(50) NOT NULL,
    [CurrentBalance] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [Description] NVARCHAR(500),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Add foreign key for RelatedPurchaseId after Purchases table is created
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_GeneralLedgerEntries_Purchases')
BEGIN
    ALTER TABLE [dbo].[GeneralLedgerEntries]
    ADD CONSTRAINT FK_GeneralLedgerEntries_Purchases
    FOREIGN KEY ([RelatedPurchaseId]) REFERENCES [Purchases]([Id]);
END
GO

-- Add foreign key for RelatedPaymentId after Payments table exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_GeneralLedgerEntries_Payments')
    BEGIN
        ALTER TABLE [dbo].[GeneralLedgerEntries]
        ADD CONSTRAINT FK_GeneralLedgerEntries_Payments
        FOREIGN KEY ([RelatedPaymentId]) REFERENCES [Payments]([Id]);
    END
END
GO

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX [IX_GeneralLedgerEntries_EntryDate] ON [dbo].[GeneralLedgerEntries] ([EntryDate]);
CREATE NONCLUSTERED INDEX [IX_GeneralLedgerEntries_AccountType] ON [dbo].[GeneralLedgerEntries] ([AccountType]);
CREATE NONCLUSTERED INDEX [IX_GeneralLedgerEntries_BookOfEntry] ON [dbo].[GeneralLedgerEntries] ([BookOfEntry]);
CREATE NONCLUSTERED INDEX [IX_Purchases_SupplierId] ON [dbo].[Purchases] ([SupplierId]);
CREATE NONCLUSTERED INDEX [IX_Purchases_PurchaseDate] ON [dbo].[Purchases] ([PurchaseDate]);
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_PurchaseId] ON [dbo].[PurchaseItems] ([PurchaseId]);
CREATE NONCLUSTERED INDEX [IX_FinancialAccounts_AccountCode] ON [dbo].[FinancialAccounts] ([AccountCode]);
GO

-- Insert default financial accounts
IF NOT EXISTS (SELECT * FROM [dbo].[FinancialAccounts])
BEGIN
    INSERT INTO [dbo].[FinancialAccounts] ([AccountCode], [AccountName], [AccountType], [Category], [Description])
    VALUES 
        -- Assets
        ('1000', 'Cash', 'Asset', 'CurrentAssets', 'Cash on hand and in bank'),
        ('1100', 'Accounts Receivable', 'Asset', 'CurrentAssets', 'Money owed by customers'),
        ('1200', 'Inventory', 'Asset', 'CurrentAssets', 'Products in stock'),
        ('1500', 'Equipment', 'Asset', 'FixedAssets', 'Business equipment and fixtures'),
        
        -- Liabilities
        ('2000', 'Accounts Payable', 'Liability', 'CurrentLiabilities', 'Money owed to suppliers'),
        ('2100', 'Loans Payable', 'Liability', 'LongTermLiabilities', 'Long-term loans'),
        
        -- Equity
        ('3000', 'Owner''s Capital', 'Equity', 'OwnersEquity', 'Owner investment'),
        ('3100', 'Retained Earnings', 'Equity', 'RetainedEarnings', 'Accumulated profits'),
        
        -- Revenue
        ('4000', 'Sales Revenue', 'Revenue', 'SalesRevenue', 'Revenue from product sales'),
        ('4100', 'Other Income', 'Revenue', 'OtherIncome', 'Other sources of income'),
        
        -- Expenses
        ('5000', 'Cost of Goods Sold', 'Expense', 'CostOfGoodsSold', 'Direct cost of products sold'),
        ('6000', 'Operating Expenses', 'Expense', 'OperatingExpenses', 'General business expenses'),
        ('6100', 'Utilities', 'Expense', 'OperatingExpenses', 'Electricity, water, internet'),
        ('6200', 'Rent', 'Expense', 'OperatingExpenses', 'Store rent');
END
GO

PRINT 'Accounting tables created successfully!';
GO
