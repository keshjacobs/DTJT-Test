CREATE TABLE [dbo].[StickMan_Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](32) NULL,
	[FullName] [varchar](64) NULL,
	[Password] [varchar](32) NULL,
	[MobileNo] [varchar](32) NULL,
	[EmailID] [varchar](32) NULL,
	[DOB] [date] NULL,
	[Sex] [varchar](8) NULL,
	[ImagePath] [varchar](max) NULL,
	[DeviceId] [varchar](1024) NULL
)