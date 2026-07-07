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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [Agents] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [IsPresent] bit NOT NULL,
        [Position] int NOT NULL,
        CONSTRAINT [PK_Agents] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
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
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NULL,
        [UserName] nvarchar(150) NOT NULL,
        [Action] nvarchar(80) NOT NULL,
        [Detail] nvarchar(2000) NOT NULL,
        [Timestamp] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [DashboardSettings] (
        [Id] int NOT NULL IDENTITY,
        [TerminalTitle] nvarchar(200) NOT NULL,
        [Period] nvarchar(200) NOT NULL,
        [IspsLevel] int NOT NULL,
        [ResponsibleName] nvarchar(150) NOT NULL,
        [AgentsRequired] int NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [UpdatedById] nvarchar(max) NULL,
        CONSTRAINT [PK_DashboardSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [Exercises] (
        [Id] int NOT NULL IDENTITY,
        [Type] int NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Icon] nvarchar(10) NOT NULL,
        [PlannedDate] datetime2 NOT NULL,
        [Responsible] nvarchar(120) NOT NULL,
        [Status] int NOT NULL,
        [Observations] nvarchar(2000) NOT NULL,
        [DisplayOrder] int NOT NULL,
        CONSTRAINT [PK_Exercises] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [Gauges] (
        [Id] int NOT NULL IDENTITY,
        [Label] nvarchar(100) NOT NULL,
        [Value] int NOT NULL,
        [DisplayOrder] int NOT NULL,
        CONSTRAINT [PK_Gauges] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [KpiCards] (
        [Id] int NOT NULL IDENTITY,
        [Label] nvarchar(100) NOT NULL,
        [Value] nvarchar(50) NOT NULL,
        [Subtitle] nvarchar(100) NOT NULL,
        [TrendBadge] nvarchar(50) NOT NULL,
        [Color] int NOT NULL,
        [DisplayOrder] int NOT NULL,
        CONSTRAINT [PK_KpiCards] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [KpiTableRows] (
        [Id] int NOT NULL IDENTITY,
        [Category] nvarchar(80) NOT NULL,
        [Indicator] nvarchar(150) NOT NULL,
        [CurrentValue] nvarchar(50) NOT NULL,
        [Target] nvarchar(50) NOT NULL,
        [Threshold] nvarchar(50) NOT NULL,
        [Status] int NOT NULL,
        [DisplayOrder] int NOT NULL,
        CONSTRAINT [PK_KpiTableRows] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [Kind] int NOT NULL,
        [Message] nvarchar(500) NOT NULL,
        [Recipients] nvarchar(1000) NOT NULL,
        [SentAt] datetime2 NOT NULL,
        [DeliverySuccess] bit NOT NULL,
        [RelatedExerciseId] int NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [ProgressBars] (
        [Id] int NOT NULL IDENTITY,
        [Label] nvarchar(120) NOT NULL,
        [Value] int NOT NULL,
        [DisplayOrder] int NOT NULL,
        CONSTRAINT [PK_ProgressBars] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE TABLE [KpiHistories] (
        [Id] int NOT NULL IDENTITY,
        [KpiTableRowId] int NOT NULL,
        [Value] nvarchar(50) NOT NULL,
        [Period] nvarchar(20) NOT NULL,
        [RecordedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_KpiHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_KpiHistories_KpiTableRows_KpiTableRowId] FOREIGN KEY ([KpiTableRowId]) REFERENCES [KpiTableRows] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Agents_Position] ON [Agents] ([Position]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Exercises_Type_DisplayOrder] ON [Exercises] ([Type], [DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_KpiCards_DisplayOrder] ON [KpiCards] ([DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_KpiHistories_KpiTableRowId] ON [KpiHistories] ([KpiTableRowId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_KpiTableRows_Category_DisplayOrder] ON [KpiTableRows] ([Category], [DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623041451_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623041451_InitialCreate', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623043647_AddNotificationSettings'
)
BEGIN
    CREATE TABLE [NotificationSettings] (
        [Id] int NOT NULL IDENTITY,
        [SmtpHost] nvarchar(200) NOT NULL,
        [SmtpPort] int NOT NULL,
        [SmtpUsername] nvarchar(200) NOT NULL,
        [SmtpPassword] nvarchar(200) NOT NULL,
        [UseStartTls] bit NOT NULL,
        [FromAddress] nvarchar(200) NOT NULL,
        [FromName] nvarchar(200) NOT NULL,
        [Recipients] nvarchar(2000) NOT NULL,
        [EnableD30Alerts] bit NOT NULL,
        [EnableD7Alerts] bit NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_NotificationSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623043647_AddNotificationSettings'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623043647_AddNotificationSettings', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092448_AddRowVersionConcurrency'
)
BEGIN
    ALTER TABLE [NotificationSettings] ADD [RowVersion] rowversion NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092448_AddRowVersionConcurrency'
)
BEGIN
    ALTER TABLE [KpiTableRows] ADD [RowVersion] rowversion NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092448_AddRowVersionConcurrency'
)
BEGIN
    ALTER TABLE [KpiCards] ADD [RowVersion] rowversion NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092448_AddRowVersionConcurrency'
)
BEGIN
    ALTER TABLE [Exercises] ADD [RowVersion] rowversion NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092448_AddRowVersionConcurrency'
)
BEGIN
    ALTER TABLE [DashboardSettings] ADD [RowVersion] rowversion NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092448_AddRowVersionConcurrency'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623092448_AddRowVersionConcurrency', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092830_AddIncidentsModule'
)
BEGIN
    CREATE TABLE [Incidents] (
        [Id] int NOT NULL IDENTITY,
        [Reference] nvarchar(20) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Category] int NOT NULL,
        [Severity] int NOT NULL,
        [Status] int NOT NULL,
        [OccurredAt] datetime2 NOT NULL,
        [Zone] nvarchar(150) NOT NULL,
        [ReportedBy] nvarchar(150) NOT NULL,
        [Investigator] nvarchar(150) NOT NULL,
        [Description] nvarchar(4000) NOT NULL,
        [ActionsTaken] nvarchar(4000) NOT NULL,
        [LessonsLearned] nvarchar(4000) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [ClosedAt] datetime2 NULL,
        [CreatedById] nvarchar(max) NULL,
        [ClosedById] nvarchar(max) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Incidents] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092830_AddIncidentsModule'
)
BEGIN
    CREATE TABLE [IncidentAttachments] (
        [Id] int NOT NULL IDENTITY,
        [IncidentId] int NOT NULL,
        [FileName] nvarchar(200) NOT NULL,
        [StoredPath] nvarchar(500) NOT NULL,
        [ContentType] nvarchar(100) NOT NULL,
        [SizeBytes] bigint NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_IncidentAttachments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_IncidentAttachments_Incidents_IncidentId] FOREIGN KEY ([IncidentId]) REFERENCES [Incidents] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092830_AddIncidentsModule'
)
BEGIN
    CREATE INDEX [IX_IncidentAttachments_IncidentId] ON [IncidentAttachments] ([IncidentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092830_AddIncidentsModule'
)
BEGIN
    CREATE INDEX [IX_Incidents_OccurredAt] ON [Incidents] ([OccurredAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092830_AddIncidentsModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Incidents_Reference] ON [Incidents] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092830_AddIncidentsModule'
)
BEGIN
    CREATE INDEX [IX_Incidents_Status] ON [Incidents] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623092830_AddIncidentsModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623092830_AddIncidentsModule', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    ALTER TABLE [Agents] ADD [BadgeNumber] nvarchar(30) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    ALTER TABLE [Agents] ADD [Email] nvarchar(150) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    ALTER TABLE [Agents] ADD [HiredAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    ALTER TABLE [Agents] ADD [Notes] nvarchar(1000) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    ALTER TABLE [Agents] ADD [Phone] nvarchar(30) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    ALTER TABLE [Agents] ADD [PhotoPath] nvarchar(500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    ALTER TABLE [Agents] ADD [Role] nvarchar(80) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    ALTER TABLE [Agents] ADD [RowVersion] rowversion NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE TABLE [Checkpoints] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(20) NOT NULL,
        [Label] nvarchar(120) NOT NULL,
        [Zone] nvarchar(120) NOT NULL,
        [TargetIntervalMinutes] int NOT NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Checkpoints] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE TABLE [Habilitations] (
        [Id] int NOT NULL IDENTITY,
        [AgentId] int NOT NULL,
        [Category] int NOT NULL,
        [Title] nvarchar(150) NOT NULL,
        [Issuer] nvarchar(150) NOT NULL,
        [ObtainedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [Reference] nvarchar(80) NOT NULL,
        [DocumentPath] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Habilitations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Habilitations_Agents_AgentId] FOREIGN KEY ([AgentId]) REFERENCES [Agents] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE TABLE [MarsecLevelChanges] (
        [Id] int NOT NULL IDENTITY,
        [FromLevel] int NOT NULL,
        [ToLevel] int NOT NULL,
        [Reason] nvarchar(1000) NOT NULL,
        [DecisionSource] nvarchar(200) NOT NULL,
        [DecidedBy] nvarchar(150) NOT NULL,
        [ChangedAt] datetime2 NOT NULL,
        [ChangedById] nvarchar(max) NULL,
        CONSTRAINT [PK_MarsecLevelChanges] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE TABLE [NonConformities] (
        [Id] int NOT NULL IDENTITY,
        [Reference] nvarchar(20) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Source] int NOT NULL,
        [Status] int NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [CorrectiveAction] nvarchar(2000) NOT NULL,
        [Owner] nvarchar(150) NOT NULL,
        [IdentifiedAt] datetime2 NOT NULL,
        [DueDate] datetime2 NULL,
        [ClosedAt] datetime2 NULL,
        [ClosureEvidence] nvarchar(1000) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_NonConformities] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE TABLE [Visitors] (
        [Id] int NOT NULL IDENTITY,
        [Reference] nvarchar(20) NOT NULL,
        [FullName] nvarchar(150) NOT NULL,
        [Company] nvarchar(150) NOT NULL,
        [IdDocumentNumber] nvarchar(50) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [VehiclePlate] nvarchar(20) NOT NULL,
        [ScheduledArrival] datetime2 NOT NULL,
        [ScheduledDeparture] datetime2 NULL,
        [Purpose] nvarchar(150) NOT NULL,
        [Host] nvarchar(150) NOT NULL,
        [EscortedBy] nvarchar(150) NOT NULL,
        [BadgeIssued] nvarchar(20) NOT NULL,
        [Status] int NOT NULL,
        [CheckInAt] datetime2 NULL,
        [CheckOutAt] datetime2 NULL,
        [CheckedInBy] nvarchar(150) NOT NULL,
        [CheckedOutBy] nvarchar(150) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [Notes] nvarchar(1000) NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Visitors] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE TABLE [PatrolScans] (
        [Id] int NOT NULL IDENTITY,
        [CheckpointId] int NOT NULL,
        [AgentId] int NULL,
        [AgentLabel] nvarchar(150) NOT NULL,
        [ScannedAt] datetime2 NOT NULL,
        [Observations] nvarchar(500) NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [AnomalyType] nvarchar(200) NULL,
        CONSTRAINT [PK_PatrolScans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PatrolScans_Agents_AgentId] FOREIGN KEY ([AgentId]) REFERENCES [Agents] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_PatrolScans_Checkpoints_CheckpointId] FOREIGN KEY ([CheckpointId]) REFERENCES [Checkpoints] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE TABLE [MarsecChecklistItems] (
        [Id] int NOT NULL IDENTITY,
        [MarsecLevelChangeId] int NOT NULL,
        [Action] nvarchar(300) NOT NULL,
        [Completed] bit NOT NULL,
        [CompletedAt] datetime2 NULL,
        [CompletedBy] nvarchar(150) NOT NULL,
        [Notes] nvarchar(500) NULL,
        CONSTRAINT [PK_MarsecChecklistItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarsecChecklistItems_MarsecLevelChanges_MarsecLevelChangeId] FOREIGN KEY ([MarsecLevelChangeId]) REFERENCES [MarsecLevelChanges] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Checkpoints_Code] ON [Checkpoints] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_Habilitations_AgentId] ON [Habilitations] ([AgentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_Habilitations_ExpiresAt] ON [Habilitations] ([ExpiresAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_MarsecChecklistItems_MarsecLevelChangeId] ON [MarsecChecklistItems] ([MarsecLevelChangeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_NonConformities_DueDate] ON [NonConformities] ([DueDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NonConformities_Reference] ON [NonConformities] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_NonConformities_Status] ON [NonConformities] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_PatrolScans_AgentId] ON [PatrolScans] ([AgentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_PatrolScans_CheckpointId_ScannedAt] ON [PatrolScans] ([CheckpointId], [ScannedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Visitors_Reference] ON [Visitors] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_Visitors_ScheduledArrival] ON [Visitors] ([ScheduledArrival]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    CREATE INDEX [IX_Visitors_Status] ON [Visitors] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623095310_AddOperationalModules'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623095310_AddOperationalModules', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE TABLE [Cameras] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(30) NOT NULL,
        [Label] nvarchar(150) NOT NULL,
        [Zone] nvarchar(150) NOT NULL,
        [Type] int NOT NULL,
        [Status] int NOT NULL,
        [Model] nvarchar(50) NOT NULL,
        [IpAddress] nvarchar(80) NOT NULL,
        [LastCheckedAt] datetime2 NULL,
        [LastCheckedBy] nvarchar(150) NOT NULL,
        [Notes] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Cameras] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE TABLE [ExerciseRexes] (
        [Id] int NOT NULL IDENTITY,
        [ExerciseId] int NOT NULL,
        [Sequence] nvarchar(4000) NOT NULL,
        [PositivePoints] nvarchar(4000) NOT NULL,
        [ImprovementPoints] nvarchar(4000) NOT NULL,
        [CorrectiveActions] nvarchar(4000) NOT NULL,
        [FollowUp] nvarchar(4000) NOT NULL,
        [WrittenBy] nvarchar(150) NOT NULL,
        [WrittenAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_ExerciseRexes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExerciseRexes_Exercises_ExerciseId] FOREIGN KEY ([ExerciseId]) REFERENCES [Exercises] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE TABLE [ExternalContacts] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [Type] int NOT NULL,
        [Role] nvarchar(120) NOT NULL,
        [PrimaryPhone] nvarchar(30) NOT NULL,
        [EmergencyPhone] nvarchar(30) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Address] nvarchar(300) NOT NULL,
        [RadioChannel] nvarchar(80) NOT NULL,
        [IsEmergency24x7] bit NOT NULL,
        [Notes] nvarchar(1000) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_ExternalContacts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE TABLE [ShiftBriefings] (
        [Id] int NOT NULL IDENTITY,
        [ShiftDate] datetime2 NOT NULL,
        [Slot] int NOT NULL,
        [OutgoingAgent] nvarchar(150) NOT NULL,
        [IncomingAgent] nvarchar(150) NOT NULL,
        [CurrentMarsecLevel] int NOT NULL,
        [EventsSummary] nvarchar(4000) NOT NULL,
        [AttentionPoints] nvarchar(4000) NOT NULL,
        [StandingOrders] nvarchar(4000) NOT NULL,
        [AcknowledgedByIncoming] bit NOT NULL,
        [AcknowledgedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_ShiftBriefings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE TABLE [VesselCalls] (
        [Id] int NOT NULL IDENTITY,
        [Reference] nvarchar(20) NOT NULL,
        [VesselName] nvarchar(150) NOT NULL,
        [ImoNumber] nvarchar(15) NOT NULL,
        [CallSign] nvarchar(20) NOT NULL,
        [Flag] nvarchar(80) NOT NULL,
        [Operator] nvarchar(150) NOT NULL,
        [Cso] nvarchar(150) NOT NULL,
        [Sso] nvarchar(150) NOT NULL,
        [ShipIspsLevel] int NOT NULL,
        [Eta] datetime2 NOT NULL,
        [Etd] datetime2 NULL,
        [ActualArrival] datetime2 NULL,
        [ActualDeparture] datetime2 NULL,
        [Berth] nvarchar(50) NOT NULL,
        [Status] int NOT NULL,
        [SecurityNotes] nvarchar(2000) NOT NULL,
        [LastTenPorts] nvarchar(2000) NOT NULL,
        [CrewCount] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_VesselCalls] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE TABLE [CameraMaintenances] (
        [Id] int NOT NULL IDENTITY,
        [CameraId] int NOT NULL,
        [Action] nvarchar(200) NOT NULL,
        [PerformedAt] datetime2 NOT NULL,
        [PerformedBy] nvarchar(150) NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [ResultingStatus] int NOT NULL,
        CONSTRAINT [PK_CameraMaintenances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CameraMaintenances_Cameras_CameraId] FOREIGN KEY ([CameraId]) REFERENCES [Cameras] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE TABLE [ContactInteractions] (
        [Id] int NOT NULL IDENTITY,
        [ExternalContactId] int NOT NULL,
        [OccurredAt] datetime2 NOT NULL,
        [Direction] int NOT NULL,
        [Channel] int NOT NULL,
        [Subject] nvarchar(200) NOT NULL,
        [HandledBy] nvarchar(150) NOT NULL,
        [Summary] nvarchar(2000) NOT NULL,
        CONSTRAINT [PK_ContactInteractions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ContactInteractions_ExternalContacts_ExternalContactId] FOREIGN KEY ([ExternalContactId]) REFERENCES [ExternalContacts] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE TABLE [DeclarationsOfSecurity] (
        [Id] int NOT NULL IDENTITY,
        [VesselCallId] int NOT NULL,
        [Reference] nvarchar(20) NOT NULL,
        [Status] int NOT NULL,
        [PortLevel] int NOT NULL,
        [ShipLevel] int NOT NULL,
        [EffectiveFrom] datetime2 NOT NULL,
        [EffectiveTo] datetime2 NULL,
        [SignedByPfso] nvarchar(150) NOT NULL,
        [SignedByShip] nvarchar(150) NOT NULL,
        [SignedAt] datetime2 NULL,
        [AgreedMeasures] nvarchar(4000) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_DeclarationsOfSecurity] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeclarationsOfSecurity_VesselCalls_VesselCallId] FOREIGN KEY ([VesselCallId]) REFERENCES [VesselCalls] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE INDEX [IX_CameraMaintenances_CameraId] ON [CameraMaintenances] ([CameraId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cameras_Code] ON [Cameras] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE INDEX [IX_Cameras_Status] ON [Cameras] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE INDEX [IX_ContactInteractions_ExternalContactId] ON [ContactInteractions] ([ExternalContactId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DeclarationsOfSecurity_Reference] ON [DeclarationsOfSecurity] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE INDEX [IX_DeclarationsOfSecurity_VesselCallId] ON [DeclarationsOfSecurity] ([VesselCallId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExerciseRexes_ExerciseId] ON [ExerciseRexes] ([ExerciseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE INDEX [IX_ExternalContacts_Type] ON [ExternalContacts] ([Type]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE INDEX [IX_ShiftBriefings_ShiftDate_Slot] ON [ShiftBriefings] ([ShiftDate], [Slot]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE INDEX [IX_VesselCalls_Eta] ON [VesselCalls] ([Eta]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE UNIQUE INDEX [IX_VesselCalls_Reference] ON [VesselCalls] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    CREATE INDEX [IX_VesselCalls_Status] ON [VesselCalls] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623141216_AddVesselsCamerasContactsBriefingsRex'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623141216_AddVesselsCamerasContactsBriefingsRex', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE TABLE [SecurityAudits] (
        [Id] int NOT NULL IDENTITY,
        [Reference] nvarchar(20) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Type] int NOT NULL,
        [Status] int NOT NULL,
        [ScheduledDate] datetime2 NOT NULL,
        [CompletedDate] datetime2 NULL,
        [Auditor] nvarchar(150) NOT NULL,
        [Scope] nvarchar(150) NOT NULL,
        [Conclusion] nvarchar(2000) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_SecurityAudits] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE TABLE [SecurityDocuments] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(200) NOT NULL,
        [Category] int NOT NULL,
        [Status] int NOT NULL,
        [Version] nvarchar(20) NOT NULL,
        [Owner] nvarchar(150) NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [EffectiveDate] datetime2 NULL,
        [NextReviewDate] datetime2 NULL,
        [FilePath] nvarchar(500) NOT NULL,
        [FileName] nvarchar(200) NOT NULL,
        [FileSizeBytes] bigint NOT NULL,
        [IsConfidential] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_SecurityDocuments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE TABLE [VehicleAccesses] (
        [Id] int NOT NULL IDENTITY,
        [Reference] nvarchar(20) NOT NULL,
        [Plate] nvarchar(20) NOT NULL,
        [Type] int NOT NULL,
        [Direction] int NOT NULL,
        [Result] int NOT NULL,
        [DriverName] nvarchar(150) NOT NULL,
        [DriverIdNumber] nvarchar(50) NOT NULL,
        [Carrier] nvarchar(150) NOT NULL,
        [ContainerNumber] nvarchar(30) NOT NULL,
        [SealNumber] nvarchar(30) NOT NULL,
        [SealVerified] bit NOT NULL,
        [BookingReference] nvarchar(80) NOT NULL,
        [Searched] bit NOT NULL,
        [Controller] nvarchar(150) NOT NULL,
        [Gate] nvarchar(50) NOT NULL,
        [OccurredAt] datetime2 NOT NULL,
        [Notes] nvarchar(1000) NOT NULL,
        CONSTRAINT [PK_VehicleAccesses] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE TABLE [AuditFindings] (
        [Id] int NOT NULL IDENTITY,
        [SecurityAuditId] int NOT NULL,
        [ItemNumber] int NOT NULL,
        [CheckItem] nvarchar(500) NOT NULL,
        [Result] int NOT NULL,
        [Observation] nvarchar(2000) NOT NULL,
        CONSTRAINT [PK_AuditFindings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditFindings_SecurityAudits_SecurityAuditId] FOREIGN KEY ([SecurityAuditId]) REFERENCES [SecurityAudits] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE INDEX [IX_AuditFindings_SecurityAuditId] ON [AuditFindings] ([SecurityAuditId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SecurityAudits_Reference] ON [SecurityAudits] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE INDEX [IX_SecurityAudits_Status] ON [SecurityAudits] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE INDEX [IX_SecurityDocuments_Category] ON [SecurityDocuments] ([Category]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE INDEX [IX_SecurityDocuments_Status] ON [SecurityDocuments] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE INDEX [IX_VehicleAccesses_OccurredAt] ON [VehicleAccesses] ([OccurredAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE INDEX [IX_VehicleAccesses_Plate] ON [VehicleAccesses] ([Plate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    CREATE UNIQUE INDEX [IX_VehicleAccesses_Reference] ON [VehicleAccesses] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623142846_AddVehicleDocumentsAudits'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623142846_AddVehicleDocumentsAudits', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623220303_AddAccessPass'
)
BEGIN
    CREATE TABLE [AccessPasses] (
        [Id] int NOT NULL IDENTITY,
        [Reference] nvarchar(20) NOT NULL,
        [Type] int NOT NULL,
        [Category] int NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [Contact] nvarchar(40) NOT NULL,
        [Matricule] nvarchar(50) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Plate] nvarchar(20) NOT NULL,
        [Company] nvarchar(150) NOT NULL,
        [IssueDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [IssuedBy] nvarchar(150) NOT NULL,
        [Notes] nvarchar(1000) NOT NULL,
        [Revoked] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_AccessPasses] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623220303_AddAccessPass'
)
BEGIN
    CREATE INDEX [IX_AccessPasses_EndDate] ON [AccessPasses] ([EndDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623220303_AddAccessPass'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AccessPasses_Reference] ON [AccessPasses] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623220303_AddAccessPass'
)
BEGIN
    CREATE INDEX [IX_AccessPasses_Type] ON [AccessPasses] ([Type]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623220303_AddAccessPass'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623220303_AddAccessPass', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624043912_AddRestrictedZones'
)
BEGIN
    CREATE TABLE [RestrictedZones] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(20) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [AccessLevel] int NOT NULL,
        [Status] int NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [ProtectionMeasures] nvarchar(2000) NOT NULL,
        [AuthorizedPersonnel] nvarchar(2000) NOT NULL,
        [ZoneManager] nvarchar(150) NOT NULL,
        [RequiresEscort] bit NOT NULL,
        [RequiresClearance] bit NOT NULL,
        [CctvMonitored] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_RestrictedZones] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624043912_AddRestrictedZones'
)
BEGIN
    CREATE INDEX [IX_RestrictedZones_AccessLevel] ON [RestrictedZones] ([AccessLevel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624043912_AddRestrictedZones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RestrictedZones_Code] ON [RestrictedZones] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624043912_AddRestrictedZones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624043912_AddRestrictedZones', N'9.0.15');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Visitors] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Visitors] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Visitors] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [VesselCalls] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [VesselCalls] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [VesselCalls] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [VehicleAccesses] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [VehicleAccesses] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [VehicleAccesses] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ShiftBriefings] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ShiftBriefings] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ShiftBriefings] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [SecurityDocuments] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [SecurityDocuments] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [SecurityDocuments] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [SecurityAudits] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [SecurityAudits] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [SecurityAudits] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [RestrictedZones] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [RestrictedZones] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [RestrictedZones] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [NonConformities] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [NonConformities] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [NonConformities] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [KpiTableRows] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [KpiTableRows] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [KpiTableRows] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [KpiCards] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [KpiCards] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [KpiCards] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Incidents] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Incidents] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Incidents] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Habilitations] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Habilitations] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Habilitations] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ExternalContacts] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ExternalContacts] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ExternalContacts] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Exercises] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Exercises] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Exercises] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ExerciseRexes] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ExerciseRexes] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [ExerciseRexes] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Checkpoints] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Checkpoints] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Checkpoints] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Cameras] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Cameras] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Cameras] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Agents] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Agents] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Agents] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [AccessPasses] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [AccessPasses] ADD [DeletedById] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    ALTER TABLE [AccessPasses] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Visitors_IsDeleted] ON [Visitors] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_VesselCalls_IsDeleted] ON [VesselCalls] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_VehicleAccesses_IsDeleted] ON [VehicleAccesses] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_ShiftBriefings_IsDeleted] ON [ShiftBriefings] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_SecurityDocuments_IsDeleted] ON [SecurityDocuments] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_SecurityAudits_IsDeleted] ON [SecurityAudits] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_RestrictedZones_IsDeleted] ON [RestrictedZones] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_NonConformities_IsDeleted] ON [NonConformities] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_KpiTableRows_IsDeleted] ON [KpiTableRows] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_KpiCards_IsDeleted] ON [KpiCards] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Incidents_IsDeleted] ON [Incidents] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Habilitations_IsDeleted] ON [Habilitations] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_ExternalContacts_IsDeleted] ON [ExternalContacts] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Exercises_IsDeleted] ON [Exercises] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_ExerciseRexes_IsDeleted] ON [ExerciseRexes] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Checkpoints_IsDeleted] ON [Checkpoints] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Cameras_IsDeleted] ON [Cameras] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_Agents_IsDeleted] ON [Agents] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    CREATE INDEX [IX_AccessPasses_IsDeleted] ON [AccessPasses] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624092948_AddSoftDelete'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624092948_AddSoftDelete', N'9.0.15');
END;

COMMIT;
GO

