CREATE PROCEDURE [dbo].[StickMan_usp_GetPendingFriendRequests]    
 @SessionToken VARCHAR(512) ,    
 @UserId int    
AS BEGIN    
 DECLARE @vaillRequest AS BIT = (SELECT Active FROM [StickMan_UserSesion] WHERE SessionID = @SessionToken AND UserID = @UserId)    
    
 IF @vaillRequest = 1    
 BEGIN    
  SELECT [ResponseMesssage] = 'Vaild Request', [ResponseCode] = 200    
    
  SELECT U.UserID, U.UserName, U.FullName, U.ImagePath,U.Sex,U.MobileNo,U.EmailID,U.DOB,FriendRequestStatus = 'Pending',    
  FR.FriendRequestID    
  FROM [StickMan_Users] U    
  INNER JOIN [StickMan_FriendRequest] FR ON U.UserID = FR.UserID    
  WHERE FR.RecieverID = @UserId AND FriendRequestStatus = 0 -- 0 for pending status    
 END    
 ELSE    
 BEGIN    
  SELECT [ResponseMesssage] = 'User Request is not vaild', [ResponseCode] = 306    
 END    
END