CREATE PROCEDURE [dbo].[StickMan_usp_GetFriends]  
 @SessionToken VARCHAR(512) ,  
 @UserId int  
AS BEGIN  
 DECLARE @vaillRequest AS BIT = (SELECT Active FROM [StickMan_UserSesion] WHERE SessionID = @SessionToken AND UserID = @UserId)  
  
 IF @vaillRequest = 1  
 BEGIN  
  SELECT [ResponseMesssage] = 'Vaild Request', [ResponseCode] = 200  
  
  DECLARE @T1 TABLE (  
   Id INT  
  )  
    
  DECLARE @T2 TABLE (  
   Id INT  
  )  
    
    
  Insert into @T1  
  SELECT UserID from StickMan_FriendRequest WHERE RecieverID = @UserId AND FriendRequestStatus = 1  
    
  Insert into @T1  
  SELECT RecieverID from StickMan_FriendRequest WHERE UserID = @UserId AND FriendRequestStatus = 1  
    
  INSERT INTO @T2  
  SELECT DISTINCT(Id) FROM @T1      
    
    
  --select * from @T2  
    
  SELECT DISTINCT(U.UserID), U.UserName, U.FullName, U.ImagePath,U.Sex,U.MobileNo,U.EmailID,U.DOB,FR.FriendRequestStatus  
   FROM [StickMan_Users] U  
  INNER JOIN [StickMan_FriendRequest] FR ON U.UserID IN (SELECT DISTINCT(Id) FROM @T2)  
  WHERE FR.FriendRequestStatus = 1  
    
  --= FR.UserID  
  --WHERE (FR.RecieverID = @UserId OR ) AND FriendRequestStatus = 1 -- 1 for accepted status  
 END  
 ELSE  
 BEGIN  
  SELECT [ResponseMesssage] = 'User Request is not vaild', [ResponseCode] = 306  
 END  
END  
  