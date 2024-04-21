USE [PvbErm]
GO

/****** Object:  Table [dbo].[Pasts_WageItem]    Script Date: 07/18/2019 10:31:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Pasts_WageItem](
	[ID] [VARCHAR](50) NOT NULL,
	[DateIndex] [VARCHAR](50) NOT NULL,
	[WorkCityID] [VARCHAR](50) NULL,
	[EnterpriseID] [VARCHAR](50) NOT NULL,
	[StaffID] [VARCHAR](50) NOT NULL,
	[Name] [VARCHAR](50) NOT NULL,
	[WageItemID] [VARCHAR](50) NOT NULL,
	[Value] [DECIMAL](18, 5) NOT NULL,
	[Currency] [INT] NULL,
 CONSTRAINT [PK_Pasts_WageItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������(����201901 (���·�))' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem', @level2type=N'COLUMN',@level2name=N'DateIndex'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem', @level2type=N'COLUMN',@level2name=N'WorkCityID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������˾ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem', @level2type=N'COLUMN',@level2name=N'EnterpriseID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ա��ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem', @level2type=N'COLUMN',@level2name=N'StaffID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���ƣ�����չʾ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem', @level2type=N'COLUMN',@level2name=N'WageItemID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ۼ�ֵ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem', @level2type=N'COLUMN',@level2name=N'Value'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem', @level2type=N'COLUMN',@level2name=N'Currency'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�������ۼ�ֵ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_WageItem'
GO


