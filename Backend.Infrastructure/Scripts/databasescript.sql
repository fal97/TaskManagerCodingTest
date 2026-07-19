/*
    database script for SQL Server.

    - SQL Server chooses its configured default data and log directories.
    - Existing databases and tables are not dropped.
    - Seed data is inserted only when UserTasks is empty.
    - Status:   0 Pending, 1 InProgress, 2 Completed, 3 Cancelled, 4 OnHold
    - Priority: 0 Low, 1 Normal, 2 High, 3 Critical
*/

USE [master];
GO

IF DB_ID(N'TaskManagerDb') IS NULL
BEGIN
    CREATE DATABASE [TaskManagerDb];
END;
GO

USE [TaskManagerDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
GO

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory]
    (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory]
            PRIMARY KEY CLUSTERED ([MigrationId] ASC)
    );
END;
GO

IF OBJECT_ID(N'[dbo].[UserTasks]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserTasks]
    (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Status] int NOT NULL,
        [Priority] int NOT NULL,
        [DueDate] datetime2(7) NULL,
        [CompletedDate] datetime2(7) NULL,
        [Notes] nvarchar(2000) NULL,
        [CreatedDate] datetime2(7) NOT NULL,
        [LastModifiedDate] datetime2(7) NOT NULL,
        [IsDeleted] bit NOT NULL
            CONSTRAINT [DF_UserTasks_IsDeleted] DEFAULT (CONVERT(bit, 0)),
        CONSTRAINT [PK_UserTasks]
            PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718051657_InitialCreate'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260718051657_InitialCreate', N'8.0.2');
END;
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[UserTasks])
BEGIN
    SET IDENTITY_INSERT [dbo].[UserTasks] ON;

    INSERT INTO [dbo].[UserTasks]
    (
        [Id], [Title], [Description], [Status], [Priority], [DueDate],
        [CompletedDate], [Notes], [CreatedDate], [LastModifiedDate], [IsDeleted]
    )
    VALUES
    (1, N'task1', N'1', 2, 1, NULL,
        CONVERT(datetime2(7), N'2026-07-18T18:35:31.2344568'), N'ddde',
        CONVERT(datetime2(7), N'2026-07-18T18:35:26.7483604'),
        CONVERT(datetime2(7), N'2026-07-18T18:35:52.4760173'), 1),
    (2, N'eat', N'rice', 2, 1, NULL,
        CONVERT(datetime2(7), N'2026-07-18T18:36:19.6709042'), N'dasda',
        CONVERT(datetime2(7), N'2026-07-18T18:36:09.7313032'),
        CONVERT(datetime2(7), N'2026-07-18T19:09:18.3604938'), 1),
    (3, N'drink', N'd', 2, 2,
        CONVERT(datetime2(7), N'2026-07-22T13:00:00.0000000'),
        CONVERT(datetime2(7), N'2026-07-18T18:59:19.8635067'), NULL,
        CONVERT(datetime2(7), N'2026-07-18T18:58:40.8763348'),
        CONVERT(datetime2(7), N'2026-07-18T19:09:16.5387282'), 1),
    (4, N'dasd', N'dasdad', 2, 1,
        CONVERT(datetime2(7), N'2026-07-25T07:30:00.0000000'),
        CONVERT(datetime2(7), N'2026-07-18T19:08:47.2109837'), N'dadssa',
        CONVERT(datetime2(7), N'2026-07-18T19:08:25.3654214'),
        CONVERT(datetime2(7), N'2026-07-18T19:09:20.5313524'), 1),
    (5, N'Prepare quarterly project report',
        N'Compile delivery metrics, risks, and project highlights for the quarterly review.',
        1, 2, CONVERT(datetime2(7), N'2026-07-23T19:12:26.9146858'), NULL,
        N'Confirm final figures with the engineering and finance teams.',
        CONVERT(datetime2(7), N'2026-07-14T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-18T19:12:26.9146858'), 0),
    (6, N'Review API security settings',
        N'Review CORS, authentication readiness, exception responses, and exposed configuration.',
        0, 3, CONVERT(datetime2(7), N'2026-07-20T13:42:26.9140000'),
        CONVERT(datetime2(7), N'2026-07-18T19:15:50.0443540'),
        N'Document any changes required before deployment.',
        CONVERT(datetime2(7), N'2026-07-17T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-18T19:15:53.7645301'), 0),
    (7, N'Write task handler unit tests',
        N'Add unit tests for create, update, delete, and query handlers.',
        1, 2, CONVERT(datetime2(7), N'2026-07-25T19:12:26.9146858'), NULL,
        N'Include success, validation, and not-found scenarios.',
        CONVERT(datetime2(7), N'2026-07-15T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-18T19:12:26.9146858'), 0),
    (8, N'Update onboarding documentation',
        N'Improve the local setup guide for the API, SQL Server, and Angular application.',
        0, 1, CONVERT(datetime2(7), N'2026-07-30T19:12:26.9146858'), NULL,
        N'Add common troubleshooting steps.',
        CONVERT(datetime2(7), N'2026-07-16T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-18T19:12:26.9146858'), 0),
    (9, N'Create database backup plan',
        N'Define backup frequency, retention, restoration testing, and ownership.',
        4, 2, CONVERT(datetime2(7), N'2026-08-07T19:12:26.9146858'), NULL,
        N'Waiting for infrastructure requirements.',
        CONVERT(datetime2(7), N'2026-07-12T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-18T19:12:26.9146858'), 0),
    (10, N'Clean up obsolete feature flags',
        N'Remove feature flags that are permanently enabled and update related tests.',
        0, 0, CONVERT(datetime2(7), N'2026-08-17T19:12:26.9146858'), NULL, NULL,
        CONVERT(datetime2(7), N'2026-07-17T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-18T19:12:26.9146858'), 0),
    (11, N'Validate production deployment checklist',
        N'Confirm migrations, configuration, monitoring, rollback, and smoke-test steps.',
        2, 3, CONVERT(datetime2(7), N'2026-07-17T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-16T19:12:26.9146858'),
        N'Checklist reviewed and approved by the delivery team.',
        CONVERT(datetime2(7), N'2026-07-08T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-18T19:16:42.3738215'), 1),
    (12, N'Optimize task list query',
        N'Project task results directly to response DTOs and review the generated SQL.',
        2, 1, CONVERT(datetime2(7), N'2026-07-15T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-14T19:12:26.9146858'),
        N'Query performance verified against representative data.',
        CONVERT(datetime2(7), N'2026-07-06T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-14T19:12:26.9146858'), 0),
    (13, N'Evaluate notification provider',
        N'Compare email and messaging options for overdue-task notifications.',
        3, 1, NULL, NULL,
        N'Cancelled because notifications are outside the current release scope.',
        CONVERT(datetime2(7), N'2026-07-03T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-13T19:12:26.9146858'), 0),
    (14, N'Plan accessibility review',
        N'Review keyboard navigation, focus states, color contrast, and screen-reader labels.',
        0, 2, CONVERT(datetime2(7), N'2026-07-27T19:12:26.9146858'), NULL,
        N'Include the task form, filters, dialogs, and task table.',
        CONVERT(datetime2(7), N'2026-07-18T19:12:26.9146858'),
        CONVERT(datetime2(7), N'2026-07-18T19:12:26.9146858'), 0);

    SET IDENTITY_INSERT [dbo].[UserTasks] OFF;
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_UserTasks_DueDate'
      AND [object_id] = OBJECT_ID(N'[dbo].[UserTasks]')
)
    CREATE INDEX [IX_UserTasks_DueDate] ON [dbo].[UserTasks] ([DueDate]);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_UserTasks_IsDeleted'
      AND [object_id] = OBJECT_ID(N'[dbo].[UserTasks]')
)
    CREATE INDEX [IX_UserTasks_IsDeleted] ON [dbo].[UserTasks] ([IsDeleted]);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_UserTasks_Priority'
      AND [object_id] = OBJECT_ID(N'[dbo].[UserTasks]')
)
    CREATE INDEX [IX_UserTasks_Priority] ON [dbo].[UserTasks] ([Priority]);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_UserTasks_Status'
      AND [object_id] = OBJECT_ID(N'[dbo].[UserTasks]')
)
    CREATE INDEX [IX_UserTasks_Status] ON [dbo].[UserTasks] ([Status]);
GO

SELECT
    COUNT(*) AS [TotalTasks],
    SUM(CASE WHEN [IsDeleted] = 0 THEN 1 ELSE 0 END) AS [ActiveTasks]
FROM [dbo].[UserTasks];
GO
