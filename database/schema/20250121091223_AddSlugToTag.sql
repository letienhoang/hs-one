BEGIN TRANSACTION;
GO

ALTER TABLE [Tags] ADD [Slug] nvarchar(128) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250121091223_AddSlugToTag', N'8.0.11');
GO

COMMIT;
GO

