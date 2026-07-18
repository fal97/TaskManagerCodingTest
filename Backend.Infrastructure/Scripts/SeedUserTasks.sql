USE [TaskManagerDb];
GO

SET NOCOUNT ON;

DECLARE @Now datetime2 = SYSUTCDATETIME();

INSERT INTO [dbo].[UserTasks]
(
    [Title],
    [Description],
    [Status],
    [Priority],
    [DueDate],
    [CompletedDate],
    [Notes],
    [CreatedDate],
    [LastModifiedDate],
    [IsDeleted]
)
VALUES
(
    N'Prepare quarterly project report',
    N'Compile delivery metrics, risks, and project highlights for the quarterly review.',
    1, -- InProgress
    2, -- High
    DATEADD(DAY, 5, @Now),
    NULL,
    N'Confirm final figures with the engineering and finance teams.',
    DATEADD(DAY, -4, @Now),
    @Now,
    0
),
(
    N'Review API security settings',
    N'Review CORS, authentication readiness, exception responses, and exposed configuration.',
    0, -- Pending
    3, -- Critical
    DATEADD(DAY, 2, @Now),
    NULL,
    N'Document any changes required before deployment.',
    DATEADD(DAY, -1, @Now),
    @Now,
    0
),
(
    N'Write task handler unit tests',
    N'Add unit tests for create, update, delete, and query handlers.',
    1, -- InProgress
    2, -- High
    DATEADD(DAY, 7, @Now),
    NULL,
    N'Include success, validation, and not-found scenarios.',
    DATEADD(DAY, -3, @Now),
    @Now,
    0
),
(
    N'Update onboarding documentation',
    N'Improve the local setup guide for the API, SQL Server, and Angular application.',
    0, -- Pending
    1, -- Normal
    DATEADD(DAY, 12, @Now),
    NULL,
    N'Add common troubleshooting steps.',
    DATEADD(DAY, -2, @Now),
    @Now,
    0
),
(
    N'Create database backup plan',
    N'Define backup frequency, retention, restoration testing, and ownership.',
    4, -- OnHold
    2, -- High
    DATEADD(DAY, 20, @Now),
    NULL,
    N'Waiting for infrastructure requirements.',
    DATEADD(DAY, -6, @Now),
    @Now,
    0
),
(
    N'Clean up obsolete feature flags',
    N'Remove feature flags that are permanently enabled and update related tests.',
    0, -- Pending
    0, -- Low
    DATEADD(DAY, 30, @Now),
    NULL,
    NULL,
    DATEADD(DAY, -1, @Now),
    @Now,
    0
),
(
    N'Validate production deployment checklist',
    N'Confirm migrations, configuration, monitoring, rollback, and smoke-test steps.',
    2, -- Completed
    3, -- Critical
    DATEADD(DAY, -1, @Now),
    DATEADD(DAY, -2, @Now),
    N'Checklist reviewed and approved by the delivery team.',
    DATEADD(DAY, -10, @Now),
    DATEADD(DAY, -2, @Now),
    0
),
(
    N'Optimize task list query',
    N'Project task results directly to response DTOs and review the generated SQL.',
    2, -- Completed
    1, -- Normal
    DATEADD(DAY, -3, @Now),
    DATEADD(DAY, -4, @Now),
    N'Query performance verified against representative data.',
    DATEADD(DAY, -12, @Now),
    DATEADD(DAY, -4, @Now),
    0
),
(
    N'Evaluate notification provider',
    N'Compare email and messaging options for overdue-task notifications.',
    3, -- Cancelled
    1, -- Normal
    NULL,
    NULL,
    N'Cancelled because notifications are outside the current release scope.',
    DATEADD(DAY, -15, @Now),
    DATEADD(DAY, -5, @Now),
    0
),
(
    N'Plan accessibility review',
    N'Review keyboard navigation, focus states, color contrast, and screen-reader labels.',
    0, -- Pending
    2, -- High
    DATEADD(DAY, 9, @Now),
    NULL,
    N'Include the task form, filters, dialogs, and task table.',
    @Now,
    @Now,
    0
);

SELECT
    [Id],
    [Title],
    [Status],
    [Priority],
    [DueDate],
    [CompletedDate]
FROM [dbo].[UserTasks]
WHERE [IsDeleted] = 0
ORDER BY [Priority] DESC, [DueDate];
GO
