CREATE TABLE [dbo].[Mail] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (50)  NOT NULL,
    [RegistrationDate] DATE           NOT NULL,
    [Text]             NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Mail] PRIMARY KEY CLUSTERED ([ID] ASC)
);

