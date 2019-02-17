CREATE TABLE [dbo].[Employees] (
    [ID]         INT           IDENTITY (1, 1) NOT NULL,
    [Surname]    NVARCHAR (50) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [Patronymic] NVARCHAR (50) NULL,
    [Email]      VARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([ID] ASC)
);

