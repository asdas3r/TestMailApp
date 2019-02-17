CREATE TABLE [dbo].[MailEmployees] (
    [ID]          INT IDENTITY (1, 1) NOT NULL,
    [Mail_ID]     INT NOT NULL,
    [Employee_ID] INT NOT NULL,
    [IsSender]    BIT NOT NULL,
    CONSTRAINT [PK_MailEmployees] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MailEmployees_Employees] FOREIGN KEY ([Employee_ID]) REFERENCES [dbo].[Employees] ([ID]),
    CONSTRAINT [FK_MailEmployees_Mail] FOREIGN KEY ([Mail_ID]) REFERENCES [dbo].[Mail] ([ID]),
    CONSTRAINT [UQ_MailEmployees] UNIQUE NONCLUSTERED ([Employee_ID] ASC, [Mail_ID] ASC, [IsSender] ASC)
);

