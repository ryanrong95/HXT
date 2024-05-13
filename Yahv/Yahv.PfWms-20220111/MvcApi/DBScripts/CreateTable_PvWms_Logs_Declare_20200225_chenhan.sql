USE [PvWms]
GO

/****** Object:  Table [dbo].[Logs_Declare]    Script Date: 02/25/2020 11:08:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Logs_Declare](
	[ID] [varchar](50) NOT NULL,
	[TinyOrderID] [varchar](50) NOT NULL,
	[WaybillID] [varchar](50) NULL,
	[EnterCode] [varchar](50) NOT NULL,
	[GrossWeight] [varchar](50) NULL,
	[BoxCode] [varchar](50) NULL,
	[Specs] [varchar](50) NULL,
	[LotNumber] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[AdminID] [varchar](50) NULL,
	[Summary] [varchar](50) NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Logs_Declare] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码：Pick' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'小订单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'TinyOrderID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属的分拣或是装箱的Waybill的ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'WaybillID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户入仓号（从Waybill中获取）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'EnterCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总毛重' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'GrossWeight'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'装箱后把箱号输入' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'BoxCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'规格（暂时不用）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'Specs'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运输批次，芯达通回填。你这里不用处理' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'LotNumber'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后更新时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'UpdateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申报人,(备用)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'AdminID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明（备用）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'Summary'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申报日志' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare'
GO


