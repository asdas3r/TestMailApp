CREATE TABLE [dbo].[Tags] (
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED ([Name] ASC)
);

