CREATE TABLE [dbo].[StickMan_UserSesion](
	[SessionID] [varchar](max) NULL,
	[UserID] [int] NULL,
	[LoginTime] [datetime] NULL,
	[LogOutTime] [datetime] NULL,
	[Active] [bit] NULL
)