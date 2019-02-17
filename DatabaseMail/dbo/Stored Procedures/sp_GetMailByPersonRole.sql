CREATE PROCEDURE [dbo].[sp_GetMailByPersonRole]
	@personID int,
	@paramIsSender bit
AS
	SELECT Mail.ID, Mail.Name, Mail.RegistrationDate, Mail.Text
    FROM Mail INNER JOIN MailEmployees ON Mail.ID = MailEmployees.Mail_ID
    WHERE (MailEmployees.IsSender = @paramIsSender) AND (MailEmployees.Employee_ID = @personID)
