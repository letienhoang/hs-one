IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AppRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] uniqueidentifier NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AppRoleClaims] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AppRoles] (
    [Id] uniqueidentifier NOT NULL,
    [DisplayName] nvarchar(256) NOT NULL,
    [Name] nvarchar(max) NULL,
    [NormalizedName] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AppRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AppUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] uniqueidentifier NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AppUserClaims] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AppUserLogins] (
    [UserId] uniqueidentifier NOT NULL,
    [LoginProvider] nvarchar(max) NOT NULL,
    [ProviderKey] nvarchar(max) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    CONSTRAINT [PK_AppUserLogins] PRIMARY KEY ([UserId])
);
GO

CREATE TABLE [AppUserRoles] (
    [UserId] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AppUserRoles] PRIMARY KEY ([UserId], [RoleId])
);
GO

CREATE TABLE [AppUsers] (
    [Id] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(128) NOT NULL,
    [LastName] nvarchar(128) NOT NULL,
    [IsActive] bit NOT NULL,
    [RefreshToken] nvarchar(max) NULL,
    [RefreshTokenExpiryTime] datetime2 NULL,
    [DateCreated] datetime2 NOT NULL,
    [Dob] datetime2 NULL,
    [Avatar] nvarchar(512) NULL,
    [VipStartDate] datetime2 NULL,
    [VipExpireDate] datetime2 NULL,
    [LastLoginDate] datetime2 NULL,
    [Balance] float NOT NULL,
    [RoyaltyAmountPerPost] float NOT NULL,
    [UserName] nvarchar(max) NULL,
    [NormalizedUserName] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [NormalizedEmail] nvarchar(max) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AppUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AppUserTokens] (
    [UserId] uniqueidentifier NOT NULL,
    [LoginProvider] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AppUserTokens] PRIMARY KEY ([UserId])
);
GO

CREATE TABLE [PostActivityLogs] (
    [Id] uniqueidentifier NOT NULL,
    [PostId] uniqueidentifier NOT NULL,
    [FromStatus] int NOT NULL,
    [ToStatus] int NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    [Note] nvarchar(512) NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_PostActivityLogs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PostCategories] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    [Slug] varchar(250) NOT NULL,
    [ParentId] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    [DateModified] datetime2 NULL,
    [SeoDescription] nvarchar(160) NULL,
    [SortOrder] int NOT NULL,
    CONSTRAINT [PK_PostCategories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PostInSeries] (
    [PostId] uniqueidentifier NOT NULL,
    [SeriesId] uniqueidentifier NOT NULL,
    [DisplayOrder] int NOT NULL,
    CONSTRAINT [PK_PostInSeries] PRIMARY KEY ([PostId], [SeriesId])
);
GO

CREATE TABLE [Posts] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(256) NOT NULL,
    [Slug] varchar(256) NOT NULL,
    [Description] nvarchar(512) NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    [Thumbnail] nvarchar(512) NULL,
    [Content] nvarchar(max) NULL,
    [AuthorUserId] uniqueidentifier NOT NULL,
    [Source] nvarchar(512) NULL,
    [Tags] nvarchar(256) NULL,
    [SeoDescription] nvarchar(160) NULL,
    [ViewCount] int NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    [DateModified] datetime2 NULL,
    [IsPaid] bit NOT NULL,
    [RoyaltyAmount] float NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PostTags] (
    [PostId] uniqueidentifier NOT NULL,
    [TagId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_PostTags] PRIMARY KEY ([PostId], [TagId])
);
GO

CREATE TABLE [Series] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    [Slug] varchar(256) NOT NULL,
    [Description] nvarchar(256) NULL,
    [IsActive] bit NOT NULL,
    [SortOrder] int NOT NULL,
    [SeoDescription] nvarchar(160) NULL,
    [Thumbnail] nvarchar(256) NULL,
    [Content] nvarchar(max) NULL,
    [AuthorUserId] uniqueidentifier NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    CONSTRAINT [PK_Series] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Tags] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_PostCategories_Slug] ON [PostCategories] ([Slug]);
GO

CREATE UNIQUE INDEX [IX_Posts_Slug] ON [Posts] ([Slug]);
GO

CREATE UNIQUE INDEX [IX_Series_Slug] ON [Series] ([Slug]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241121094955_Initial', N'8.0.11');
GO

COMMIT;
GO

