create PROCEDURE [dbo].[StickMan_usp_GetUsersList]    
 @SessionToken VARCHAR(512) ,    
 @UserId int,    
 @SearchKeyword VARCHAR(512)    
AS BEGIN    
 DECLARE @vaillRequest AS BIT = (SELECT Active FROM [StickMan_UserSesion] 
 WHERE SessionID = @SessionToken AND UserID = @UserId)    
 
 IF @vaillRequest = 1    
 BEGIN    
  SELECT [ResponseMesssage] = 'Vaild Request', [ResponseCode] = 200    
    
    DECLARE @T1 TABLE (  
   Id INT  
  )
   
   insert @T1
   SELECT RecieverID from StickMan_FriendRequest WHERE UserID = @UserId
    
    insert into @T1
    SELECT UserID from StickMan_FriendRequest WHERE RecieverID = @UserId
    
    --select * from @T1
    
  SELECT U.UserID, U.UserName, U.FullName, U.MobileNo, U.EmailID, U.DOB, U.Sex, U.ImagePath  
  
  --FriendRequestID = ISNULL(FR.FriendRequestID,0)  
  -- FriendRequestStatus = CASE WHEN FR.FriendRequestStatus = 0 THEN 'Pending'     
    --     ELSE CASE WHEN FR.FriendRequestStatus = 1 THEN 'Accepted'    
      --   ELSE 'Not Sent' END END    
  
         FROM royal_mutiyar.StickMan_Users U  
   WHERE 
    --OR 
   --(U.UserID NOT IN ( SELECT UserID FROM StickMan_FriendRequest WHERE UserID = 59))
  --LEFT JOIN [StickMan_FriendRequest] FR ON U.UserID <> FR.UserID    
  --AND 
  (LOWER(U.UserName) LIKE '%'+LOWER(@SearchKeyword)+'%' OR LOWER(U.FullName) LIKE '%'+LOWER(@SearchKeyword)+'%')    
  AND
  (U.UserID NOT IN ( select * from @T1 ) )
    
    
    --SELECT RecieverID FROM StickMan_FriendRequest WHERE UserID = @UserId
    --SELECT UserID FROM StickMan_FriendRequest WHERE UserID = @UserId
 END    
 ELSE    
 BEGIN    
  SELECT [ResponseMesssage] = 'User Request is not vaild', [ResponseCode] = 306    
 END    
END  

go

exec StickMan_usp_GetUsersList @SessionToken = '90186563228C47E9853A5201C9EB9D6C',@UserId =  58,
@SearchKeyword = 'fe'