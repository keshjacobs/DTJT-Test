-- CHECK
select SUM(DuplicatesCount)
FROM (select count(*)	as DuplicatesCount
  from [StickManDB].[dbo].[StickMan_FriendRequest]
  group by UserID,RecieverID,[FriendRequestStatus]
  having count(*) > 1		   
	  ) src


-- REMOVE By grouping
delete from   [StickManDB].[dbo].[StickMan_FriendRequest]
where FriendRequestID in (
Select FriendRequestID FROM [StickManDB].[dbo].[StickMan_FriendRequest]
LEFT OUTER JOIN (
   SELECT MIN(FriendRequestID) as RowId, UserID,RecieverID,[FriendRequestStatus]
   FROM [StickManDB].[dbo].[StickMan_FriendRequest] 
	group by UserID,RecieverID,[FriendRequestStatus]
) as KeepRows ON
   [StickManDB].[dbo].[StickMan_FriendRequest].FriendRequestID = KeepRows.RowId
WHERE
   KeepRows.RowId IS NULL	 )






-- REMOVE visa versa friends

DECLARE @maxUser INT
SET @maxUser = 651
DECLARE @userId INT
DECLARE @anotherUserId INT

SET @userId = 0
WHILE @userId < @maxUser
BEGIN
	SET @anotherUserId = 0

	WHILE @anotherUserId < @maxUser
	BEGIN
		DECLARE @count int
		SELECT @count = COUNT(*)
			FROM [StickManDB].[dbo].[StickMan_FriendRequest]
			WHERE 	([RecieverID] = @userId	AND UserID = @anotherUserId) OR ( UserID = @userId	AND RecieverID = @anotherUserId)
		IF (@count > 1)
		BEGIN
			PRINT 'DUPLICATE!!!'
			DECLARE @duplicatesCount INT

			SELECT @duplicatesCount = COUNT(*)
			FROM [StickManDB].[dbo].[StickMan_FriendRequest]
			WHERE 	([RecieverID] = @userId	AND UserID = @anotherUserId) OR ( UserID = @userId	AND RecieverID = @anotherUserId)
			PRINT 'DUPLICATES COUNT '+ CAST(@duplicatesCount AS VARCHAR(16))

			DELETE FROM 	   [StickManDB].[dbo].[StickMan_FriendRequest]
			WHERE FriendRequestID IN (
				SELECT TOP (@duplicatesCount - 1) FriendRequestID
				FROM [StickManDB].[dbo].[StickMan_FriendRequest]
				WHERE 	([RecieverID] = @userId	AND UserID = @anotherUserId) OR ( UserID = @userId	AND RecieverID = @anotherUserId))
			PRINT 'DUPLICATIONS REMOVED'
		END
		
		SET @anotherUserId = @anotherUserId + 1
	END

	SET @userId = @userId + 1
END