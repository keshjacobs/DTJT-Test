USE StickManDB
GO

ALTER TABLE StickMan_Users_Cast_AudioData_UploadInformation 
ADD Title NVARCHAR(500)
GO

ALTER TABLE StickMan_UsersFriendList ALTER COLUMN UserID int NOT NULL
GO

ALTER TABLE StickMan_UsersFriendList ALTER COLUMN FriendID int NOT NULL
GO

ALTER TABLE StickMan_FriendRequest ALTER COLUMN UserID int NOT NULL
GO

ALTER TABLE StickMan_FriendRequest ALTER COLUMN RecieverID int NOT NULL
GO