CREATE PROCEDURE [dbo].[sp_GetTagsByMail]
	@paramID int
AS
	SELECT Tags.* 
    FROM MailTags INNER JOIN Tags ON MailTags.Tag_Name = Tags.Name
    WHERE MailTags.Mail_ID = @paramID
    ORDER BY Tags.Name
