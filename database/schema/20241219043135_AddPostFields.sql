BEGIN TRANSACTION;
GO

ALTER TABLE [Posts] ADD [AuthorName] nvarchar(256) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Posts] ADD [AuthorUserName] nvarchar(256) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Posts] ADD [CategoryName] nvarchar(256) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Posts] ADD [CategorySlug] varchar(256) NOT NULL DEFAULT '';
GO

ALTER TABLE [PostActivityLogs] ADD [UserName] nvarchar(256) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241219043135_AddPostFields', N'8.0.11');
GO

COMMIT;
GO

