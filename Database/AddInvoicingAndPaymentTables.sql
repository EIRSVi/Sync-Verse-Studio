-- Migration: Add Invoicing and Payment System Tables
-- Date: 2025-10-26
-- Description: Creates tables for Invoice, InvoiceItem, Payment, PaymentLink, HeldTransaction, and OnlineStoreIntegration

USE POSDB;
GO

-- Add new columns to Products table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'IsSyncedToOnlineStore')
BEGIN
    ALTER TABLE Products ADD IsSyncedToOnlineStore BIT NOT NULL DEFAULT 0;
    ALTER TABLE Products ADD LastSyncedAt DATETIME2 NULL;
    ALTER TABLE Products ADD OnlineStoreProductId NVARCHAR(100) NULL;
END
GO

-- Create Invoices table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE Invoices (
        Id INT PRIMARY KEY IDENTITY(1,1),
        InvoiceNumber NVARCHAR(20) NOT NULL UNIQUE,
        CustomerId INT NULL,
        CustomerName NVARCHAR(100) NULL,
        CreatedByUserId INT NOT NULL,
        SubTotal DECIMAL(18,2) NOT NULL,
        TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalAmount DECIMAL(18,2) NOT NULL,
        PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        BalanceAmount DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
        InvoiceDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        DueDate DATETIME2 NULL,
        Notes NVARCHAR(500) NULL,
        VoidReason NVARCHAR(500) NULL,
        VoidedAt DATETIME2 NULL,
        VoidedByUserId INT NULL,
        SaleId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Invoices_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE SET NULL,
        CONSTRAINT FK_Invoices_CreatedByUser FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
        CONSTRAINT FK_Invoices_VoidedByUser FOREIGN KEY (VoidedByUserId) REFERENCES Users(Id),
        CONSTRAINT FK_Invoices_Sales FOREIGN KEY (SaleId) REFERENCES Sales(Id) ON DELETE SET NULL
    );
    CREATE INDEX IX_Invoices_InvoiceNumber ON Invoices(InvoiceNumber);
    CREATE INDEX IX_Invoices_CustomerId ON Invoices(CustomerId);
    CREATE INDEX IX_Invoices_Status ON Invoices(Status);
    CREATE INDEX IX_Invoices_InvoiceDate ON Invoices(InvoiceDate);
END
GO

-- Create InvoiceItems table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE InvoiceItems (
        Id INT PRIMARY KEY IDENTITY(1,1),
        InvoiceId INT NOT NULL,
        ProductId INT NOT NULL,
        ProductName NVARCHAR(100) NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalPrice DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_InvoiceItems_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE CASCADE,
        CONSTRAINT FK_InvoiceItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
    );
    CREATE INDEX IX_InvoiceItems_InvoiceId ON InvoiceItems(InvoiceId);
    CREATE INDEX IX_InvoiceItems_ProductId ON InvoiceItems(ProductId);
END
GO

-- Create Payments table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
BEGIN
    CREATE TABLE Payments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        InvoiceId INT NULL,
        SaleId INT NULL,
        PaymentReference NVARCHAR(50) NOT NULL UNIQUE,
        Amount DECIMAL(18,2) NOT NULL,
        PaymentMethod NVARCHAR(20) NOT NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        TransactionId NVARCHAR(100) NULL,
        PaymentGateway NVARCHAR(50) NULL,
        Notes NVARCHAR(500) NULL,
        FailureReason NVARCHAR(500) NULL,
        ProcessedByUserId INT NOT NULL,
        PaymentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Payments_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE SET NULL,
        CONSTRAINT FK_Payments_Sales FOREIGN KEY (SaleId) REFERENCES Sales(Id) ON DELETE SET NULL,
        CONSTRAINT FK_Payments_ProcessedByUser FOREIGN KEY (ProcessedByUserId) REFERENCES Users(Id)
    );
    CREATE INDEX IX_Payments_PaymentReference ON Payments(PaymentReference);
    CREATE INDEX IX_Payments_InvoiceId ON Payments(InvoiceId);
    CREATE INDEX IX_Payments_SaleId ON Payments(SaleId);
    CREATE INDEX IX_Payments_Status ON Payments(Status);
END
GO

-- Create PaymentLinks table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentLinks]') AND type in (N'U'))
BEGIN
    CREATE TABLE PaymentLinks (
        Id INT PRIMARY KEY IDENTITY(1,1),
        LinkCode NVARCHAR(50) NOT NULL UNIQUE,
        InvoiceId INT NULL,
        CustomerId INT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Description NVARCHAR(200) NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
        ExpiryDate DATETIME2 NOT NULL,
        PaidAt DATETIME2 NULL,
        PaymentId INT NULL,
        CreatedByUserId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_PaymentLinks_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE SET NULL,
        CONSTRAINT FK_PaymentLinks_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE SET NULL,
        CONSTRAINT FK_PaymentLinks_Payments FOREIGN KEY (PaymentId) REFERENCES Payments(Id) ON DELETE SET NULL,
        CONSTRAINT FK_PaymentLinks_CreatedByUser FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
    );
    CREATE INDEX IX_PaymentLinks_LinkCode ON PaymentLinks(LinkCode);
    CREATE INDEX IX_PaymentLinks_Status ON PaymentLinks(Status);
END
GO

-- Create HeldTransactions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HeldTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE HeldTransactions (
        Id INT PRIMARY KEY IDENTITY(1,1),
        TransactionCode NVARCHAR(50) NOT NULL UNIQUE,
        CustomerId INT NULL,
        CustomerName NVARCHAR(100) NULL,
        HeldByUserId INT NOT NULL,
        SubTotal DECIMAL(18,2) NOT NULL,
        TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalAmount DECIMAL(18,2) NOT NULL,
        CartItemsJson NVARCHAR(MAX) NOT NULL,
        Notes NVARCHAR(500) NULL,
        HeldAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        ResumedAt DATETIME2 NULL,
        IsCompleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_HeldTransactions_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE SET NULL,
        CONSTRAINT FK_HeldTransactions_HeldByUser FOREIGN KEY (HeldByUserId) REFERENCES Users(Id)
    );
    CREATE INDEX IX_HeldTransactions_TransactionCode ON HeldTransactions(TransactionCode);
    CREATE INDEX IX_HeldTransactions_IsCompleted ON HeldTransactions(IsCompleted);
END
GO

-- Create OnlineStoreIntegrations table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OnlineStoreIntegrations]') AND type in (N'U'))
BEGIN
    CREATE TABLE OnlineStoreIntegrations (
        Id INT PRIMARY KEY IDENTITY(1,1),
        StoreName NVARCHAR(100) NOT NULL,
        Platform NVARCHAR(50) NOT NULL,
        ApiKey NVARCHAR(500) NULL,
        ApiSecret NVARCHAR(500) NULL,
        StoreUrl NVARCHAR(500) NULL,
        WebhookUrl NVARCHAR(500) NULL,
        IsEnabled BIT NOT NULL DEFAULT 1,
        LastSyncDate DATETIME2 NULL,
        LastSyncStatus NVARCHAR(20) NOT NULL DEFAULT 'Never',
        LastSyncMessage NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

PRINT 'Migration completed successfully!';
GO
