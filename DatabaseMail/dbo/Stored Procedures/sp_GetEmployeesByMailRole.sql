CREATE PROCEDURE [dbo].[sp_GetEmployeesByMailRole]
	@paramID int,
	@paramIsSender bit
AS
	select Employees.*
    from Employees INNER JOIN MailEmployees ON Employees.ID = MailEmployees.Employee_ID INNER JOIN Mail ON MailEmployees.Mail_ID = Mail.ID
    where  (MailEmployees.Mail_ID = @paramID) AND (MailEmployees.IsSender = @paramIsSender)
