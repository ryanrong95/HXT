USE [PvWms]
GO

/****** Object:  Table [dbo].[Logs_DeclareItem]    Script Date: 02/25/2020 11:11:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Logs_DeclareItem](
	[ID] [varchar](50) NOT NULL,
	[TinyOrderID] [varchar](50) NOT NULL,
	[OrderItemID] [varchar](50) NOT NULL,
	[StorageID] [varchar](50) NOT NULL,
	[Quantity] [int] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Logs_DeclareItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码：Pick' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要申报的库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'StorageID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要申报的数量(一般来自分拣的数量)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'Quantity'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'装箱人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'AdminID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申报项目日志' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem'
GO


