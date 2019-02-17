CREATE PROCEDURE [dbo].[sp_SetMailsData]
	@cID int,
	@cName nvarchar(50),
	@cDate Date,
	@cSent int,
	@cRecieved int,
	@cContents nvarchar(max),
	@cTagNameString nvarchar(max)
AS
BEGIN
	BEGIN TRAN
	BEGIN TRY
		IF (@cID = 0)
		BEGIN
			INSERT INTO Mail values (@cName, @cDate, @cContents);

			SELECT @cID = SCOPE_IDENTITY();

			INSERT INTO MailEmployees values (@cID, @cSent, 1);

			INSERT INTO MailEmployees values (@cID, @cRecieved, 0);
		
			IF (@cTagNameString is not null)
			BEGIN
				INSERT INTO MailTags (Mail_ID, Tag_Name)
				SELECT @cID, E1.*
				FROM string_split(@cTagNameString, ',') AS E1;
			END
		END
	ELSE
		BEGIN
			UPDATE Mail SET Name = @cName, RegistrationDate = @cDate, Text = @cContents
			WHERE ID = @cID;

			UPDATE MailEmployees SET Employee_ID = @cSent
			WHERE Mail_ID = @cID AND IsSender = 1;

			UPDATE MailEmployees SET Employee_ID = @cRecieved
			WHERE Mail_ID = @cID AND IsSender = 0;
		
			DELETE FROM MailTags WHERE Mail_ID = @cID;
			
			IF (@cTagNameString is not null)
			BEGIN
				INSERT INTO MailTags (Mail_ID, Tag_Name)
				SELECT @cID, E1.*
				FROM string_split(@cTagNameString, ',') AS E1;
			END
		END

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRAN
		END
	END CATCH
END