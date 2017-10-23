CREATE PROCEDURE [StickMan_usp_CreateUpdate_User]
(
	@UserID		INT = 0,
	@UserName	VARCHAR(32) = '',
	@FullName	VARCHAR(64) = '',
	@Password	VARCHAR(32) = '',
	@MobileNo	VARCHAR(32) = '',
	@EmailID	VARCHAR(32) = '',
	@DOB		DATE = NULL,
	@Sex		VARCHAR(8) = '',
	@ImagePath	VARCHAR(MAX) = '',
	@DeviceId	VARCHAR(1024)=''
)
AS BEGIN
/*
	This procedure will Create new user and update users information as well.
*/
	
	IF @UserID = 0
	BEGIN
		IF EXISTS (SELECT 1 FROM [StickMan_Users] WHERE [UserName] = @UserName)
			SELECT [UserID] = 0, [Message] = 'User Name already registered.', [ResponseCode] = 301, [Token] = NULL
		ELSE IF EXISTS (SELECT 1 FROM [StickMan_Users] WHERE [EmailID] = @EmailID)
			SELECT [UserID] = 0, [Message] = 'User with this Email ID already registered.', [ResponseCode] = 302 , [Token] = NULL
		ELSE
		BEGIN
			INSERT INTO StickMan_Users
			VALUES(@UserName,@FullName,HASHBYTES('SHA1', @Password),@MobileNo,@EmailID,@DOB,@Sex,@ImagePath,@DeviceId);

			DECLARE @ScopeUserId INT = 0

			SET @ScopeUserId = SCOPE_IDENTITY()

			--Login
			DECLARE @RandomID AS VARCHAR(MAX) = REPLACE(NEWID(), '-', '')
			INSERT INTO StickMan_UserSesion
			VALUES(@RandomID, @ScopeUserId, GETDATE(), NULL, 1)

			SELECT [UserID] = @ScopeUserId, [Message] = 'User has been created successfully.', [ResponseCode] = 200 , [Token] = @RandomID,
			[username] = UserName,[FullName] = FullName,[MobileNo] = MobileNo,[EmailID] = EmailID,[DOB] = DOB,[Sex] = Sex,
			[imagePath] = imagePath,DeviceId
			FROM [StickMan_Users]
			WHERE UserID = @ScopeUserId
		END
	END
	ELSE
	BEGIN
		UPDATE StickMan_Users
		SET FullName = @FullName,
			Password = HASHBYTES('SHA1', @Password),
			MobileNo = @MobileNo,
			DOB		 = @DOB,
			Sex		 = @Sex,
			ImagePath = @ImagePath,
			DeviceId = @DeviceId
		WHERE UserID = @UserID
		
		SELECT [UserID] = @UserID, [Message] = 'User information updated successfully.', [ResponseCode] = 200, [Token] = null
	END
END