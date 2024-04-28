
USE [BookManagementApplication]
GO

/****** Object:  Table [dbo].[UserDetail]    Script Date: 6/20/2022 10:56:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserDetail](
	[UserId] [int] IDENTITY(1,1) PRIMARY key NOT NULL,
	[UserName] [varchar](255) NULL,
	[PassWord] [varchar](255) NULL,
	[InsertionDate] [varchar](255) NULL,
	[IsActive] [bit] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserDetail] ADD  DEFAULT (getdate()) FOR [InsertionDate]
GO

ALTER TABLE [dbo].[UserDetail] ADD  DEFAULT ((1)) FOR [IsActive]
GO
