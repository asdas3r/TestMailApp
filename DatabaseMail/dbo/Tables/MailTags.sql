CREATE TABLE [dbo].[MailTags] (
    [Mail_ID]  INT           NOT NULL,
    [Tag_Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MailTags] PRIMARY KEY CLUSTERED ([Mail_ID] ASC, [Tag_Name] ASC),
    CONSTRAINT [FK_MailTags_Mail] FOREIGN KEY ([Mail_ID]) REFERENCES [dbo].[Mail] ([ID]),
    CONSTRAINT [FK_MailTags_Tags] FOREIGN KEY ([Tag_Name]) REFERENCES [dbo].[Tags] ([Name])
);

