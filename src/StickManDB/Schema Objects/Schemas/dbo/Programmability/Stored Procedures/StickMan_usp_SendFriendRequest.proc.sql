CREATE PROCEDURE [StickMan_usp_SendFriendRequest]  
(  
 @SenderID  INT,  
 @RecieverID  INT,  
 @SessionToken VARCHAR(512)  
)  
AS  
BEGIN  
 DECLARE @validRequest AS BIT = (SELECT Active FROM [StickMan_UserSesion] WHERE SessionID = @SessionToken AND UserID = @SenderID)  
  
 IF @validRequest = 1  
 BEGIN  
  INSERT INTO StickMan_FriendRequest(UserID,RecieverID,DateTimeStamp,FriendRequestStatus)  
  VALUES(@SenderID,@RecieverID,GETDATE(),0)  
  
  DECLARE @deviceID VARCHAR(1024)  
  
  SELECT @deviceID = DeviceId FROM StickMan_Users WHERE UserID = @RecieverID  
  
  SELECT DeviceID = @deviceID, [FriendRequestId] = SCOPE_IDENTITY(),   
  [FriendRequestState] = 'Pending', [ResponseMesssage] = 'Friend Request Sent Successfully', [ResponseCode] = 200  
  
 SELECT U.UserID, U.UserName, U.FullName, U.MobileNo, U.EmailID, U.DOB, U.Sex, U.ImagePath  

 FROM [StickMan_Users] U  
 WHERE U.UserID = @SenderID  
  
  
 END  
 ELSE  
 BEGIN  
  SELECT [DeviceID] = 0, [FriendRequestId] = 0, [FriendRequestState] = 'Not Sent',[ResponseMesssage] = 'User Request is not vaild', [ResponseCode] = 306  
 END  
END  