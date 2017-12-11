USE StickManDB
GO

CREATE TABLE [dbo].[CastMessageListeners] (
    CastId int NOT NULL,
    UserId int NOT NULL,
	CONSTRAINT CastMessageListeners_PK PRIMARY KEY (CastId, UserId),
	CONSTRAINT FK_Cast FOREIGN KEY (CastId) REFERENCES [dbo].[StickMan_Users_Cast_AudioData_UploadInformation] (Id),
	CONSTRAINT FK_User FOREIGN KEY (UserId) REFERENCES [dbo].[StickMan_Users] (UserId)
);
GO