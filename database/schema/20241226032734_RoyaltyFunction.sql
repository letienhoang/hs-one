BEGIN TRANSACTION;
GO

ALTER TABLE [Posts] ADD [PaidDate] datetime2 NULL;
GO

CREATE TABLE [Transactions] (
    [Id] uniqueidentifier NOT NULL,
    [FromUserId] uniqueidentifier NOT NULL,
    [FromUserName] nvarchar(256) NOT NULL,
    [ToUserId] uniqueidentifier NOT NULL,
    [ToUserName] nvarchar(256) NOT NULL,
    [Amount] float NOT NULL,
    [TransactionType] int NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    [Note] nvarchar(256) NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241226032734_RoyaltyFunction', N'8.0.11');
GO

COMMIT;
GO

