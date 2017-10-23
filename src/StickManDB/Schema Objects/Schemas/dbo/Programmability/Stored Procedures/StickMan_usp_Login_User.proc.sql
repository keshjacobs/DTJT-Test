CREATE PROCEDURE StickMan_usp_Login_User  
(  
 @UserName VARCHAR(32),  
 @Password VARCHAR(1024),  
 @DeviceId VARCHAR(1024)  
)  
AS BEGIN  
 DECLARE @UserID AS INT = (SELECT UserID FROM [StickMan_Users] WHERE [UserName] = @UserName)  
 IF @UserID IS NULL  
  SELECT [Message] = 'Incorrect User Name', [UserId] = 0, [ResponseCode] = 301, [Token] = NULL  
 ELSE  
 BEGIN  
  SET @UserID = (SELECT UserID FROM [StickMan_Users] WHERE [UserName] = @UserName AND [Password] = HASHBYTES('SHA1', @Password))  
  IF @UserID IS NULL  
  SELECT [Message] = 'Password Incorrect', [UserId] = 0, [ResponseCode] = 302, [Token] = NULL  
 ELSE  
  BEGIN  
   DECLARE @RandomID AS VARCHAR(MAX) = REPLACE(NEWID(), '-', '')  
   INSERT INTO StickMan_UserSesion    
   VALUES(@RandomID, @UserID, GETDATE(), NULL, 1)   
    
   UPDATE StickMan_Users  
   SET DeviceId = @DeviceId  
   WHERE UserID = @UserID  
  
   SELECT [Message] = 'Login Successful', [UserId] = @UserID, [ResponseCode] = 200, [Token] = @RandomID,  
   [username] = UserName,[FullName] = FullName,[MobileNo] = MobileNo,[EmailID] = EmailID,[DOB] = DOB,[Sex] = Sex,  
   [imagePath] = imagePath, DeviceId  
   FROM [StickMan_Users]  
   WHERE UserID = @UserID  
  END  
 END  
END