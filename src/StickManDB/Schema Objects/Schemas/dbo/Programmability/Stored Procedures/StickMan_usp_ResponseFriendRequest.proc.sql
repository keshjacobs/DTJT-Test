CREATE PROCEDURE [dbo].[StickMan_usp_ResponseFriendRequest]    
(    
 @UserID    INT,  
 @RespondingToUserID INT,  
 @FriendRequestID INT,   --1026  
 @FriendRequestReply INT,  --1  
 @SessionToken  VARCHAR(512)    
)    
AS    
BEGIN    
 DECLARE @validRequest AS BIT = (SELECT Active FROM [StickMan_UserSesion] WHERE SessionID = @SessionToken AND UserID = @UserID)  
     
 IF @validRequest = 1  
 BEGIN    
  DECLARE @FriendID AS INT = (SELECT RecieverID FROM StickMan_FriendRequest WHERE FriendRequestID = @FriendRequestID AND RecieverID = @UserID  
  AND UserID = @RespondingToUserID)  
  
  IF @FriendID IS NULL     
  BEGIN    
   SELECT [ResponseMesssage] = 'Data Not Found', [ResponseCode] = 302    
  END    
  ELSE    
  BEGIN      
   IF @FriendRequestReply = 1    
   BEGIN    
    UPDATE StickMan_FriendRequest    
    SET FriendRequestStatus = 1    
    WHERE FriendRequestID = @FriendRequestID AND RecieverID = @UserID AND UserID = @RespondingToUserID  
    
    INSERT INTO StickMan_UsersFriendList    
    VALUES(@UserID, @FriendID)    
    
    SELECT [ResponseMesssage] = 'Friend Request Accepted Successfully', [ResponseCode] = 200    
   END    
   ELSE    
   BEGIN    
     
   UPDATE StickMan_FriendRequest    
    SET FriendRequestStatus = 2  
    WHERE FriendRequestID = @FriendRequestID AND RecieverID = @UserID AND UserID = @RespondingToUserID  
  
     
    DELETE FROM StickMan_FriendRequest    
    WHERE FriendRequestID = @FriendRequestID AND RecieverID = @UserID AND UserID = @RespondingToUserID  
    
    SELECT [ResponseMesssage] = 'Friend Request Rejected Successfully', [ResponseCode] = 200    
   END    
  END    
 END    
 ELSE    
 BEGIN    
  SELECT [ResponseMesssage] = 'User Request is not Vaild', [ResponseCode] = 306    
 END    
END  
  
  
  
select * from StickMan_UsersFriendList 