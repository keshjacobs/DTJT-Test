create PROCEDURE [dbo].[StickMan_usp_GetAudioFileMessages]  
 @SessionToken VARCHAR(512) ,  
 @UserID int  
AS BEGIN  
 DECLARE @vaillRequest AS BIT = (SELECT Active FROM [StickMan_UserSesion] WHERE SessionID = @SessionToken AND UserID = @UserId)  
  
 IF @vaillRequest = 1  
 BEGIN  
	WITH UserAudioFileInformation ( UserName,FullName,MobileNo,EmailID,DOB,Sex,ImagePath,DeviceId, UserID,RecieverID,AudioFilePath,Filter,UploadTime, MessageType )
	AS 
	(
		SELECT U.UserName,U.FullName,U.MobileNo,U.EmailID,U.DOB,U.Sex,U.ImagePath,U.DeviceId,  
		AUI.UserID,RecieverID,AudioFilePath,Filter,UploadTime, MessageType = 'Receieved'  
		FROM StickMan_Users_AudioData_UploadInformation AUI  
		LEFT JOIN royal_mutiyar.StickMan_Users U ON AUI.UserID = U.UserID  
		WHERE AUI.RecieverID = @UserID  
		UNION
		SELECT U.UserName,U.FullName,U.MobileNo,U.EmailID,U.DOB,U.Sex,U.ImagePath,U.DeviceId,  
		AUI.UserID,RecieverID,AudioFilePath,Filter,UploadTime, MessageType = 'Sent'  
		FROM StickMan_Users_AudioData_UploadInformation AUI  
		LEFT JOIN royal_mutiyar.StickMan_Users U ON AUI.RecieverID = U.UserID  
		WHERE AUI.UserID = @UserID  
	)
	Select * from UserAudioFileInformation
	Order By UploadTime DESC

	SELECT [ResponseMesssage] = 'Vaild Request', [ResponseCode] = 200  
 END  
 ELSE  
 BEGIN  
	SELECT [ResponseMesssage] = 'User Request is not vaild', [ResponseCode] = 306  
	SELECT [ResponseMesssage] = 'User Request is not vaild', [ResponseCode] = 306  
 END  
END