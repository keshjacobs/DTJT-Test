CREATE PROCEDURE [StickMan_usp_SaveAudioPath]    
(    
 @UserID    INT,    
 @RecieverID INT,  
 @FilePath   VARCHAR(2048),    
 @Filter VARCHAR(100),
 @SessionToken VARCHAR(512)    
)    
AS BEGIN    
 DECLARE @vaillRequest AS BIT = (SELECT Active FROM [StickMan_UserSesion] WHERE SessionID = @SessionToken AND UserID = @UserID)    
     
 IF @vaillRequest = 1    
 BEGIN    
  INSERT INTO StickMan_Users_AudioData_UploadInformation    
  VALUES(@UserID,@RecieverID,@FilePath,GETDATE(),@Filter)    
    
  DECLARE @DeviceID VARCHAR(1024)  
    
  SELECT @DeviceID = DeviceId FROM StickMan_Users WHERE UserID = @RecieverID  
    
  SELECT [ResponseMesssage] = 'Audio File Path Saved Successfully', [ResponseCode] = 200  , [DeviceID] = @DeviceID  
 END    
 ELSE    
 BEGIN    
  SELECT [ResponseMesssage] = 'User Request is not vaild', [ResponseCode] = 306    
 END    
    
END 