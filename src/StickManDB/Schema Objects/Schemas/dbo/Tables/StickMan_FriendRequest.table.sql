CREATE TABLE [dbo].[StickMan_FriendRequest]
(
	FriendRequestID		[INT]		NOT NULL IDENTITY(1000,1), 
	UserID				[INT]		NULL,
	RecieverID			[INT]		NULL,
	DateTimeStamp		[DATETIME]	NULL,
	FriendRequestStatus [INT]
)