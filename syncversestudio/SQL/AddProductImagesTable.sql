-- Create ProductImages table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductImages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductImages](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ProductId] [int] NOT NULL,
        [ImagePath] [nvarchar](500) NOT NULL,
        [ImageType] [nvarchar](50) NOT NULL,
        [ImageName] [nvarchar](200) NULL,
        [IsPrimary] [bit] NOT NULL DEFAULT 0,
        [DisplayOrder] [int] NOT NULL DEFAULT 0,
        [Description] [nvarchar](500) NULL,
        [FileSize] [bigint] NOT NULL DEFAULT 0,
        [FileExtension] [nvarchar](50) NULL,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [IsActive] [bit] NOT NULL DEFAULT 1,
        CONSTRAINT [PK_ProductImages] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY([ProductId])
            REFERENCES [dbo].[Products] ([Id])
            ON DELETE CASCADE
    )

    CREATE NONCLUSTERED INDEX [IX_ProductImages_ProductId_IsPrimary] 
    ON [dbo].[ProductImages] ([ProductId], [IsPrimary])
END
GO

PRINT 'ProductImages table created successfully'
