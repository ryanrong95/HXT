USE [master]
GO
/****** Object:  Database [PvWms]    Script Date: 05/22/2020 18:36:04 ******/
CREATE DATABASE [PvWms] ON  PRIMARY 
( NAME = N'PvWms', FILENAME = N'E:\sqldata\PvWms.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'PvWms_log', FILENAME = N'E:\sqldata\PvWms_log.ldf' , SIZE = 8384KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [PvWms] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PvWms].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PvWms] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [PvWms] SET ANSI_NULLS OFF
GO
ALTER DATABASE [PvWms] SET ANSI_PADDING OFF
GO
ALTER DATABASE [PvWms] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [PvWms] SET ARITHABORT OFF
GO
ALTER DATABASE [PvWms] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [PvWms] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [PvWms] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [PvWms] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [PvWms] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [PvWms] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [PvWms] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [PvWms] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [PvWms] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [PvWms] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [PvWms] SET  DISABLE_BROKER
GO
ALTER DATABASE [PvWms] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [PvWms] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [PvWms] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [PvWms] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [PvWms] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [PvWms] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [PvWms] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [PvWms] SET  READ_WRITE
GO
ALTER DATABASE [PvWms] SET RECOVERY SIMPLE
GO
ALTER DATABASE [PvWms] SET  MULTI_USER
GO
ALTER DATABASE [PvWms] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [PvWms] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'PvWms', N'ON'
GO
USE [PvWms]
GO
/****** Object:  User [udata]    Script Date: 05/22/2020 18:36:04 ******/
CREATE USER [udata] FOR LOGIN [udata] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [u_b1b]    Script Date: 05/22/2020 18:36:04 ******/
CREATE USER [u_b1b] FOR LOGIN [u_b1b] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Specs]    Script Date: 05/22/2020 18:36:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Specs](
	[ID] [varchar](50) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Width] [decimal](18, 7) NULL,
	[Length] [decimal](18, 7) NULL,
	[Height] [decimal](18, 7) NULL,
	[Load] [decimal](18, 7) NULL,
 CONSTRAINT [PK_Specs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码，建议与Name相同' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Specs', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'A B C D' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Specs', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'宽' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Specs', @level2type=N'COLUMN',@level2name=N'Width'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'长' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Specs', @level2type=N'COLUMN',@level2name=N'Length'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'高' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Specs', @level2type=N'COLUMN',@level2name=N'Height'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'承重' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Specs', @level2type=N'COLUMN',@level2name=N'Load'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'规格：目前只针对库位有规格' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Specs'
GO
/****** Object:  Table [dbo].[Warehouses]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Warehouses](
	[ID] [varchar](50) NOT NULL,
	[IsOnOrder] [bit] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](150) NULL,
	[CrmCode] [varchar](50) NULL,
 CONSTRAINT [PK_Warehouses1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'在途' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Warehouses', @level2type=N'COLUMN',@level2name=N'IsOnOrder'
GO
/****** Object:  Table [dbo].[_bak_Logs_Declare]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_bak_Logs_Declare](
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
 CONSTRAINT [PK_临时] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码：Pick' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'小订单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'TinyOrderID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属的分拣或是装箱的Waybill的ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'WaybillID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户入仓号（从Waybill中获取）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'EnterCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总毛重' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'GrossWeight'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'装箱后把箱号输入' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'BoxCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'规格（暂时不用）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'Specs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运输批次，华芯通回填。你这里不用处理' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'LotNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后更新时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'UpdateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申报人,(备用)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'AdminID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明（备用）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'_bak_Logs_Declare', @level2type=N'COLUMN',@level2name=N'Status'
GO
/****** Object:  Table [dbo].[Boxes]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Boxes](
	[ID] [varchar](50) NOT NULL,
	[Series] [varchar](50) NULL,
	[Day] [int] NOT NULL,
	[Index] [int] NOT NULL,
	[EnterCode] [varchar](50) NOT NULL,
	[WhCode] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[PackageType] [varchar](50) NULL,
 CONSTRAINT [PK_Boxes_] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Index_Main] ON [dbo].[Boxes] 
(
	[Day] ASC,
	[Index] ASC,
	[WhCode] ASC
)
INCLUDE ( [ID],
[EnterCode],
[CreateDate],
[AdminID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_Series] ON [dbo].[Boxes] 
(
	[Series] ASC
)
INCLUDE ( [ID],
[Day],
[Index],
[EnterCode],
[WhCode],
[CreateDate],
[AdminID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'连续编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'Series'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前的日序号（与日期类型的PrimaryKeys保持一致）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'Day'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序号（与日期类型的PrimaryKeys保持一致）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'Index'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入仓号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'EnterCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房编号 HK \  SZ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'WhCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'AdminID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'包装类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes', @level2type=N'COLUMN',@level2name=N'PackageType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'装箱管理' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Boxes'
GO
/****** Object:  Table [dbo].[Inputs]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Inputs](
	[ID] [varchar](50) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[OriginID] [varchar](50) NULL,
	[OrderID] [varchar](50) NULL,
	[TinyOrderID] [varchar](50) NULL,
	[ItemID] [varchar](50) NULL,
	[ProductID] [varchar](50) NOT NULL,
	[ClientID] [varchar](50) NULL,
	[PayeeID] [varchar](50) NULL,
	[ThirdID] [varchar](50) NULL,
	[TrackerID] [varchar](50) NULL,
	[SalerID] [varchar](50) NULL,
	[PurchaserID] [varchar](50) NULL,
	[Currency] [int] NULL,
	[UnitPrice] [decimal](18, 7) NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Inputs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Index_TinyOrderID_OrderID] ON [dbo].[Inputs] 
(
	[TinyOrderID] ASC,
	[OrderID] ASC
)
INCLUDE ( [ID],
[Code],
[OriginID],
[ItemID],
[ProductID],
[ClientID],
[PayeeID],
[ThirdID],
[TrackerID],
[SalerID],
[PurchaserID],
[Currency],
[UnitPrice],
[CreateDate]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Inputs_Order] ON [dbo].[Inputs] 
(
	[OrderID] ASC,
	[ItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'四位年+2位月+2日+6位流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'全局唯一码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原inputid，拆项是用，不拆项时为空' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'OriginID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'订单编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'OrderID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'小订单编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'TinyOrderID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'ItemID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'ProductID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'ClientID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'内部公司ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'PayeeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'第三方（现在默认万路通）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'ThirdID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'跟单员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'TrackerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'AdminID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'SalerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采购员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'PurchaserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'Currency'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'UnitPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Inputs', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
/****** Object:  Table [dbo].[Logs_DeclareItem]    Script Date: 05/22/2020 18:36:07 ******/
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
	[Quantity] [decimal](18, 7) NOT NULL,
	[BoxCode] [varchar](50) NULL,
	[AdminID] [varchar](50) NOT NULL,
	[OutputID] [varchar](50) NULL,
	[Weight] [decimal](18, 7) NULL,
	[NetWeight] [decimal](18, 7) NULL,
	[Volume] [decimal](18, 7) NULL,
 CONSTRAINT [PK_Logs_DeclareItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Index_TinyOrderID] ON [dbo].[Logs_DeclareItem] 
(
	[TinyOrderID] ASC
)
INCLUDE ( [ID],
[StorageID],
[Quantity],
[AdminID],
[OrderItemID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_TinyOrderID_OrderItemID] ON [dbo].[Logs_DeclareItem] 
(
	[TinyOrderID] ASC,
	[OrderItemID] ASC
)
INCLUDE ( [ID],
[StorageID],
[Quantity],
[AdminID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码：Pick' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要申报的库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'StorageID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要申报的数量(一般来自分拣的数量)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'装箱人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'AdminID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem', @level2type=N'COLUMN',@level2name=N'Volume'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申报项目日志' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_DeclareItem'
GO
/****** Object:  Table [dbo].[Logs_Declare]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Logs_Declare](
	[ID] [varchar](50) NOT NULL,
	[TinyOrderID] [varchar](50) NOT NULL,
	[EnterCode] [varchar](50) NOT NULL,
	[GrossWeight] [varchar](50) NULL,
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
CREATE NONCLUSTERED INDEX [Index_Status] ON [dbo].[Logs_Declare] 
(
	[Status] ASC,
	[TinyOrderID] ASC,
	[CreateDate] ASC
)
INCLUDE ( [ID],
[EnterCode],
[GrossWeight],
[LotNumber],
[UpdateDate],
[AdminID],
[Summary]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [Index_TinyOrderID] ON [dbo].[Logs_Declare] 
(
	[TinyOrderID] ASC
)
INCLUDE ( [ID],
[Status],
[CreateDate],
[EnterCode],
[GrossWeight],
[LotNumber],
[UpdateDate],
[AdminID],
[Summary]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码：Pick' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'小订单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'TinyOrderID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户入仓号（从Waybill中获取）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'EnterCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总毛重' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'GrossWeight'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运输批次，华芯通回填。你这里不用处理' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Declare', @level2type=N'COLUMN',@level2name=N'LotNumber'
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
/****** Object:  Table [dbo].[Modifed_Storage]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Modifed_Storage](
	[ID] [varchar](50) NOT NULL,
	[StorageID] [varchar](50) NOT NULL,
	[OriginQuantity] [decimal](18, 7) NOT NULL,
	[ModifiedQuantity] [decimal](18, 7) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](200) NULL,
	[AdminID] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pick 日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Modifed_Storage', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'源所属库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Modifed_Storage', @level2type=N'COLUMN',@level2name=N'StorageID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原有数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Modifed_Storage', @level2type=N'COLUMN',@level2name=N'OriginQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改后数量（模拟在库的移动、装箱、捡货的）数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Modifed_Storage', @level2type=N'COLUMN',@level2name=N'ModifiedQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'摘要（修改原因，做保留设计）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Modifed_Storage', @level2type=N'COLUMN',@level2name=N'Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人（跟单）AdminID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Modifed_Storage', @level2type=N'COLUMN',@level2name=N'AdminID'
GO
/****** Object:  Table [dbo].[Shelves]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Shelves](
	[ID] [varchar](50) NOT NULL,
	[Name] [nvarchar](150) NULL,
	[WhCode] [varchar](50) NULL,
	[DoorCode] [varchar](50) NOT NULL,
	[RegionCode] [varchar](50) NULL,
	[PlaceCode] [varchar](50) NULL,
	[BoxCode] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[Summary] [nvarchar](300) NOT NULL,
	[LeaseID] [varchar](50) NULL,
 CONSTRAINT [PK_Shelves] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID 库位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'目前只用于库区' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'WhCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门牌' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'DoorCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库区编码:' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'RegionCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'[货架][位置]  or  [卡板] or [箱号]' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'PlaceCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'箱号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'BoxCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'UpdateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：正常、停用、删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同 （租赁）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Shelves', @level2type=N'COLUMN',@level2name=N'LeaseID'
GO
/****** Object:  Table [dbo].[Printings]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Printings](
	[ID] [varchar](50) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[Url] [varchar](200) NOT NULL,
	[Width] [int] NULL,
	[Height] [int] NULL,
	[Summary] [nvarchar](300) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Printings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码，“P”+3位流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Printings', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Printings', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印类型(标签,文件),枚举' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Printings', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地址，标签模板地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Printings', @level2type=N'COLUMN',@level2name=N'Url'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'规格(长宽),枚举' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Printings', @level2type=N'COLUMN',@level2name=N'Height'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'摘要' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Printings', @level2type=N'COLUMN',@level2name=N'Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：200 正常，400 禁用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Printings', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印对象' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Printings'
GO
/****** Object:  Table [dbo].[PrimaryKeys]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PrimaryKeys](
	[Name] [varchar](10) NOT NULL,
	[Type] [int] NOT NULL,
	[Length] [int] NOT NULL,
	[Value] [int] NOT NULL,
	[Day] [int] NOT NULL,
 CONSTRAINT [PK_PrimaryKeys] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'键值名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PrimaryKeys', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 正常返回 2 时间有关返回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PrimaryKeys', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'补长' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PrimaryKeys', @level2type=N'COLUMN',@level2name=N'Length'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PrimaryKeys', @level2type=N'COLUMN',@level2name=N'Value'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PrimaryKeys', @level2type=N'COLUMN',@level2name=N'Day'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PrimaryKeys'
GO
/****** Object:  Table [dbo].[LsNotice]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LsNotice](
	[ID] [varchar](50) NOT NULL,
	[SpecID] [varchar](50) NOT NULL,
	[Quantity] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](100) NULL,
	[OrderID] [varchar](50) NULL,
	[ClientID] [varchar](50) NOT NULL,
	[PayeeID] [varchar](50) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_LsNotice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_LsNotice_client] ON [dbo].[LsNotice] 
(
	[ClientID] ASC,
	[Status] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开始时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LsNotice', @level2type=N'COLUMN',@level2name=N'StartDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'结束时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LsNotice', @level2type=N'COLUMN',@level2name=N'EndDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LsNotice', @level2type=N'COLUMN',@level2name=N'Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LsNotice', @level2type=N'COLUMN',@level2name=N'ClientID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收款人ID，这里指平台所属公司：华芯通、' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LsNotice', @level2type=N'COLUMN',@level2name=N'PayeeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'未分配，已分配 ，已删除 , 已过期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LsNotice', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'租赁通知' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LsNotice'
GO
/****** Object:  Table [dbo].[Logs_Stotage]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Logs_Stotage](
	[ID] [varchar](50) NOT NULL,
	[OriginStorageID] [varchar](50) NOT NULL,
	[InfactStorageID] [varchar](50) NOT NULL,
	[OriginQuantity] [decimal](18, 7) NOT NULL,
	[InfactQuantity] [decimal](18, 7) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](200) NULL,
	[AdminID] [varchar](50) NOT NULL,
	[NoticeID] [varchar](50) NULL,
	[BoxCode] [varchar](50) NULL,
 CONSTRAINT [PK_Logs_Stotage] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pick 日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'源所属库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'OriginStorageID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新所属库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'InfactStorageID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原有数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'OriginQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改后数量（模拟在库的移动、装箱、捡货的）数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'InfactQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'摘要（修改原因，做保留设计）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人（跟单）AdminID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'AdminID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'先做个保留设计' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'NoticeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'先做个保留设计' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage', @level2type=N'COLUMN',@level2name=N'BoxCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存日志，尽量模拟在库的设计' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs_Stotage'
GO
/****** Object:  Table [dbo].[Outputs]    Script Date: 05/22/2020 18:36:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Outputs](
	[ID] [varchar](50) NOT NULL,
	[InputID] [varchar](50) NOT NULL,
	[OrderID] [varchar](50) NULL,
	[TinyOrderID] [varchar](50) NULL,
	[ItemID] [varchar](50) NULL,
	[OwnerID] [varchar](50) NULL,
	[SalerID] [varchar](50) NULL,
	[PurchaserID] [varchar](50) NULL,
	[Currency] [int] NULL,
	[Price] [decimal](18, 7) NULL,
	[ReviewerID] [varchar](50) NULL,
	[TrackerID] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CustomerServiceID] [varchar](50) NULL,
 CONSTRAINT [PK_Outputs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Index_TinyOrderID_OrderID] ON [dbo].[Outputs] 
(
	[TinyOrderID] ASC,
	[OrderID] ASC
)
INCLUDE ( [ID],
[InputID],
[ItemID],
[OwnerID],
[SalerID],
[PurchaserID],
[Currency],
[Price],
[ReviewerID],
[TrackerID],
[CreateDate],
[CustomerServiceID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'四位年+2位月+2日+6位流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'MainID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'OrderID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'ItemID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'法人(所有人)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'OwnerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售人 AdminID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'SalerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采购人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'PurchaserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保值(价格管理开发)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'Currency'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'Price'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出库复核人 adminID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'ReviewerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'跟单员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'TrackerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发生时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'跟单员(无用)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Outputs', @level2type=N'COLUMN',@level2name=N'CustomerServiceID'
GO
/****** Object:  View [dbo].[OrdersTopView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OrdersTopView]
AS
SELECT   ID, Type, ClientID, InvoiceID, PayeeID, BeneficiaryID, CreateDate, ModifyDate, MainStatus, PaymentStatus, 
                InvoiceStatus, Summary, CreatorID, SupplierID, SettlementCurrency, inBeneficiaryID, IsPayCharge, InWayBillID, 
                inCurrency, InConditions, outBeneficiaryID, IsReciveCharge, OutWayBillID, OutConditions, outCurrency, 
                RemittanceStatus
FROM      PvWsOrder.dbo.OrdersTopView AS OrdersTopView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "OrdersTopView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 22
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OrdersTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OrdersTopView'
GO
/****** Object:  View [dbo].[Logs_PvLsOrderTopView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Logs_PvLsOrderTopView]
AS
SELECT   ID, MainID, Type, Status, CreateDate, CreatorID, IsCurrent
FROM      PvCenter.dbo.Logs_PvLsOrderTopView AS Logs_PvLsOrderTopView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Logs_PvLsOrderTopView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 192
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Logs_PvLsOrderTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Logs_PvLsOrderTopView'
GO
/****** Object:  View [dbo].[Logs_PvLsOrderCurrentTopView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Logs_PvLsOrderCurrentTopView]
AS
SELECT   MainID, MainStatus, InvoiceStatus
FROM      PvCenter.dbo.Logs_PvLsOrderCurrentTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Logs_PvLsOrderCurrentTopView (PvCenter.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 203
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Logs_PvLsOrderCurrentTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Logs_PvLsOrderCurrentTopView'
GO
/****** Object:  View [dbo].[InvoiceNoticeFilesView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[InvoiceNoticeFilesView]
AS
SELECT   InvoiceNoticeFileID, InvoiceNoticeID, ErmAdminID, RealName, Name, FileType, FileFormat, Url, CreateDate
FROM      foricScCustoms.dbo.InvoiceNoticeFilesView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "InvoiceNoticeFilesView (foricScCustoms.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 238
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'InvoiceNoticeFilesView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'InvoiceNoticeFilesView'
GO
/****** Object:  View [dbo].[ReceivedsTopView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ReceivedsTopView]
AS
SELECT     ID, ReceivableID, AccountType, Price, Currency1, Price1, Rate1, AdminID, OrderID, WaybillID, CreateDate, Summay, AccountCode, FlowID, CouponID
FROM         PvbCrm.dbo.ReceivedsTopView AS ReceivedsTopView_1
GO
/****** Object:  View [dbo].[ProductsTopView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ProductsTopView]
AS
SELECT   ID, PartNumber, Manufacturer, PackageCase, Packaging, CreateDate
FROM      PvData.dbo.ProductsTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "ProductsTopView (PvData.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 205
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ProductsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ProductsTopView'
GO
/****** Object:  View [dbo].[MapsTrackerTopView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MapsTrackerTopView]
AS
SELECT   *
FROM      PvbCrm.dbo.MapsTrackerTopView AS MapsTrackerTopView_1
GO
/****** Object:  View [dbo].[LsOrderTopView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[LsOrderTopView]
AS
SELECT   ID, FatherID, Type, Source, ClientID, PayeeID, BeneficiaryID, Currency, IsInvoiced, InvoiceID, Status, InheritStatus, 
                Creator, CreateDate, ModifyDate, Summary, StartDate, EndDate
FROM      PvLsOrder.dbo.LsOrderTopView AS LsOrderTopView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "LsOrderTopView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 333
               Right = 247
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 18
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LsOrderTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LsOrderTopView'
GO
/****** Object:  View [dbo].[OrderItemsTopView]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OrderItemsTopView]
AS
SELECT   ID, OrderID, TinyOrderID, InputID, OutputID, ProductID, CustomName, Origin, DateCode, Quantity, Currency, UnitPrice, 
                Unit, TotalPrice, CreateDate, ModifyDate, GrossWeight, Volume, Conditions, Status, IsAuto, WayBillID, Type
FROM      PvWsOrder.dbo.OrderItemsTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "OrderItemsTopView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 206
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OrderItemsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OrderItemsTopView'
GO
/****** Object:  Table [dbo].[Notices]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Notices](
	[ID] [varchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[WareHouseID] [varchar](50) NOT NULL,
	[WaybillID] [varchar](50) NOT NULL,
	[InputID] [varchar](50) NULL,
	[OutputID] [varchar](50) NULL,
	[ProductID] [varchar](50) NOT NULL,
	[Quantity] [decimal](18, 7) NOT NULL,
	[DateCode] [varchar](50) NULL,
	[Origin] [nvarchar](50) NULL,
	[Weight] [decimal](18, 7) NULL,
	[NetWeight] [decimal](18, 7) NULL,
	[Volume] [decimal](18, 7) NULL,
	[Source] [int] NOT NULL,
	[Target] [int] NOT NULL,
	[BoxCode] [varchar](50) NULL,
	[BoxingSpecs] [int] NULL,
	[ShelveID] [varchar](50) NULL,
	[Conditions] [nvarchar](max) NOT NULL,
	[Supplier] [nvarchar](150) NULL,
	[Summary] [nvarchar](300) NULL,
	[StorageID] [varchar](50) NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CustomsName] [nvarchar](150) NULL,
 CONSTRAINT [PK_Notices] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Index_InputID] ON [dbo].[Notices] 
(
	[InputID] ASC
)
INCLUDE ( [ID],
[Type],
[WareHouseID],
[WaybillID],
[OutputID],
[ProductID],
[Quantity],
[DateCode],
[Origin],
[Weight],
[NetWeight],
[Volume],
[Source],
[Target],
[BoxCode],
[BoxingSpecs],
[ShelveID],
[Conditions],
[Supplier],
[Summary],
[StorageID],
[Status],
[CreateDate],
[CustomsName]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_OutputID] ON [dbo].[Notices] 
(
	[OutputID] ASC
)
INCLUDE ( [ID],
[Type],
[WareHouseID],
[WaybillID],
[ProductID],
[Quantity],
[DateCode],
[Origin],
[Weight],
[NetWeight],
[Volume],
[Source],
[Target],
[BoxCode],
[BoxingSpecs],
[ShelveID],
[Conditions],
[Supplier],
[Summary],
[StorageID],
[Status],
[CreateDate],
[CustomsName],
[InputID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_ProductID] ON [dbo].[Notices] 
(
	[ProductID] ASC
)
INCLUDE ( [ID],
[Type],
[WareHouseID],
[WaybillID],
[InputID],
[OutputID],
[Supplier],
[DateCode],
[Quantity],
[Conditions],
[CreateDate],
[Status],
[Source],
[Target],
[BoxCode],
[Weight],
[Volume],
[ShelveID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_wbid_whid] ON [dbo].[Notices] 
(
	[WaybillID] ASC,
	[WareHouseID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_whid_StorageID_Quantity] ON [dbo].[Notices] 
(
	[WareHouseID] ASC,
	[StorageID] ASC,
	[Quantity] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_whid_Type_StorageID_Quantity] ON [dbo].[Notices] 
(
	[WareHouseID] ASC,
	[Type] ASC,
	[StorageID] ASC,
	[Quantity] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [index_whid_wbid] ON [dbo].[Notices] 
(
	[WareHouseID] ASC,
	[WaybillID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'通知类型：入库通知、出库通知、分拣通知、检测通知、捡货通知、客户自提通知、装箱通知' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'仓库编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'WareHouseID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运单编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'WaybillID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'进项ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'InputID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销项ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'OutputID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'ProductID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'批次号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'DateCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原产地' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Origin'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'重量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Weight'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'体积' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Volume'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'业务来源' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Source'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'目标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Target'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'箱号(不用)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'BoxCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'(不用)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'BoxingSpecs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'货架编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'ShelveID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'条件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Conditions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'StorageID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：等待Waiting、关闭Closed、（货物）丢失Lost、完成Completed' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报关品名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices', @level2type=N'COLUMN',@level2name=N'CustomsName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'通知表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Notices'
GO
/****** Object:  View [dbo].[InvoiceNoticeForWin]    Script Date: 05/22/2020 18:36:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[InvoiceNoticeForWin]
AS
SELECT   InvoiceNoticeID, ClientCode, CompanyName, InvoiceType, CreateDate, Amount, DeliveryType, InvoiceNoticeStatus, 
                ApplyName
FROM      foricScCustoms.dbo.InvoiceNoticeForWin
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "InvoiceNoticeForWin_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 196
               Right = 180
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'InvoiceNoticeForWin'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'InvoiceNoticeForWin'
GO
/****** Object:  View [dbo].[EnterprisesTopView]    Script Date: 05/22/2020 18:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EnterprisesTopView]
AS
SELECT   ID, Name, AdminCode, Status, Corporation, RegAddress, Uscc, District
FROM      PvbCrm.dbo.EnterprisesTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EnterprisesTopView (PvbCrm.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EnterprisesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EnterprisesTopView'
GO
/****** Object:  View [dbo].[DriversTopView]    Script Date: 05/22/2020 18:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[DriversTopView]
AS
SELECT   ID, EnterpriseID, Name, IDCard, Mobile, Status
FROM      PvbCrm.dbo.DriversTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "DriversTopView (PvbCrm.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 199
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 3825
         Width = 3780
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DriversTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DriversTopView'
GO
/****** Object:  View [dbo].[AdminsTopView]    Script Date: 05/22/2020 18:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[AdminsTopView]
AS
SELECT   ID, StaffID, UserName, RealName, SelCode, Status, LastLoginDate, RoleID, RoleName, RoleStatus
FROM      PvbCrm.dbo.AdminsTopView AS AdminsTopView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "AdminsTopView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 209
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 11
         Width = 284
         Width = 1920
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdminsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdminsTopView'
GO
/****** Object:  View [dbo].[CgPvWmsDeclareSzPricesTopView]    Script Date: 05/22/2020 18:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgPvWmsDeclareSzPricesTopView]
AS
SELECT   MainOrderId, TinyOrderID, CustomsExchangeRate, OrderItemID, DeclTotal, Qty, ReceiptRate
FROM      foricScCustoms.dbo.PvWmsDeclareSzPricesTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "PvWmsDeclareSzPricesTopView (foricScCustoms.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 258
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsDeclareSzPricesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsDeclareSzPricesTopView'
GO
/****** Object:  View [dbo].[CreditsStatisticsView]    Script Date: 05/22/2020 18:36:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CreditsStatisticsView]
AS
SELECT  Payer, Payee, Business, Catalog, Currency, Total, Cost
FROM     PvbCrm.dbo.CreditsStatisticsView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "CreditsStatisticsView (PvbCrm.dbo)"
            Begin Extent = 
               Top = 6
               Left = 42
               Bottom = 133
               Right = 190
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CreditsStatisticsView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CreditsStatisticsView'
GO
/****** Object:  View [dbo].[CouponsTopView]    Script Date: 05/22/2020 18:36:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CouponsTopView]
AS
SELECT   ID, Name, Code, Type, Conduct, Catalog, Subject, Currency, Price, InOrderCount, CreateDate, CreatorID, Status
FROM      PvbCrm.dbo.CouponsTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "CouponsTopView (PvbCrm.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 207
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CouponsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CouponsTopView'
GO
/****** Object:  View [dbo].[ClientsTopView]    Script Date: 05/22/2020 18:36:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ClientsTopView]
AS
SELECT   ID, Name, AdminCode, Corporation, RegAddress, Uscc, Grade, Vip, EnterCode, CustomsCode, Status, CreateDate, 
                UpdateDate, AdminID
FROM      PvbCrm.dbo.WsClientsTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "WsClientsTopView (PvbCrm.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ClientsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ClientsTopView'
GO
/****** Object:  StoredProcedure [dbo].[clear_leaseIDs]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[clear_leaseIDs]
as
begin

  ---开启事务
  begin tran

  begin try 
select ID,OrderID  into #temptable from LsNotice  where Status='3' and EndDate<convert(datetime,convert(varchar(10),getdate(),120))

select distinct OrderID into #orders  from #temptable

update Shelves set LeaseID=NULL where ID in( select ID from Shelves where LeaseID in ( select id from #temptable))

update LsNotice set Status='4' where ID in ( select id from #temptable)

update Logs_PvLsOrderTopView set IsCurrent='0' where MainID in (select OrderID  from #orders) and [Type]=1

insert into Logs_PvLsOrderTopView  select NEWID() ID, orderID MainID,1 [Type],4 [Status],GETDATE() CreateDate,'' CreatorID,1 IsCurrent from #orders
end try
begin catch
if(@@trancount>0)
begin
   rollback tran;        -- 回滚事务
end
end catch
 if(@@trancount>0)
 commit tran

end
GO
/****** Object:  View [dbo].[CgXdtCreditsStatisticsView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgXdtCreditsStatisticsView]
AS
SELECT  Payer, PayerName = enterprise1.Name, Payee, PayeeName = enterprise2.Name, Business, Catalog, Currency, Total, Cost
FROM     PvbCrm.dbo.CreditsStatisticsView as credit inner join 
  dbo.EnterprisesTopView as enterprise1 on credit.Payer = enterprise1.ID inner join
  dbo.EnterprisesTopView as enterprise2 on credit.Payee = enterprise2.ID
GO
/****** Object:  View [dbo].[CarriersTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CarriersTopView]
AS
SELECT   *
FROM      PvbCrm.dbo.CarriersTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[12] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "CarriersTopView (PvbCrm.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CarriersTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CarriersTopView'
GO
/****** Object:  View [dbo].[CgPvWmsSzOutItemsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgPvWmsSzOutItemsTopView]
AS
SELECT   ID, ExitNoticeID, SortingID, Quantity, ExitNoticeStatus, CreateDate, UpdateDate, OrderID, OrderItemID
FROM      foricScCustoms.dbo.PvWmsSzOutItemsTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "PvWmsSzOutItemsTopView (foricScCustoms.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 220
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsSzOutItemsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsSzOutItemsTopView'
GO
/****** Object:  View [dbo].[CgPvWmsOrdersTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgPvWmsOrdersTopView]
AS
SELECT   o.ID, o.Type, o.ClientID, o.InvoiceID, o.PayeeID, o.CreateDate, o.ModifyDate, o.MainStatus, o.PaymentStatus, 
                o.InvoiceStatus, o.CreatorID, o.SupplierID, o.SettlementCurrency, o.InWayBillID, o.inCurrency, o.OutWayBillID, 
                o.outCurrency, dechead.VoyNo, o.RemittanceStatus
FROM      dbo.OrdersTopView AS o LEFT OUTER JOIN
                foricScCustoms.dbo.PvWmsDecheadsTopView AS dechead ON o.ID = dechead.OrderID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导数据专用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsOrdersTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "dechead"
            Begin Extent = 
               Top = 6
               Left = 277
               Bottom = 108
               Right = 419
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "o"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 22
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 30
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsOrdersTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsOrdersTopView'
GO
/****** Object:  View [dbo].[wsnSuppliersTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[wsnSuppliersTopView]
AS
SELECT  ID, OwnID, OwnName, RealEnterpriseID, RealEnterpriseName, nGrade, Place, EnterCode, ClientGrade, Status, Corporation, Uscc, RegAddress, ChineseName, EnglishName, CHNabbreviation
FROM     PvbCrm.dbo.wsnSuppliersTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "wsnSuppliersTopView (PvbCrm.dbo)"
            Begin Extent = 
               Top = 6
               Left = 42
               Bottom = 133
               Right = 241
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'wsnSuppliersTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'wsnSuppliersTopView'
GO
/****** Object:  View [dbo].[WsClientsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[WsClientsTopView]
AS
SELECT   ID, Name, AdminCode, Corporation, RegAddress, Uscc, Grade, Vip, EnterCode, CustomsCode, Status, CreateDate, 
                UpdateDate, AdminID
FROM      PvbCrm.dbo.WsClientsTopView AS WsClientsTopView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "WsClientsTopView_1"
            Begin Extent = 
               Top = 5
               Left = 38
               Bottom = 145
               Right = 296
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 15
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'WsClientsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'WsClientsTopView'
GO
/****** Object:  View [dbo].[WayLoadingsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[WayLoadingsTopView]
AS
SELECT   ID, TakingDate, TakingAddress, TakingContact, TakingPhone, CarNumber1, Driver, Carload, CreateDate, ModifyDate, 
                CreatorID, ModifierID, ExcuteStatus, DriverName, transCarNumber1, transCarNumber2, Carrier
FROM      PvCenter.dbo.WayLoadingsTopView AS WayLoadingsTopView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "WayLoadingsTopView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 13
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 18
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 3000
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'WayLoadingsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'WayLoadingsTopView'
GO
/****** Object:  View [dbo].[VouchersStatisticsView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VouchersStatisticsView]
AS
SELECT     OrderID, WaybillID, ReceivableID, Payer, Payee, Business, Catalog, Subject, OriginCurrency, OriginPrice, Currency, LeftPrice, LeftDate, RightPrice, RightDate, ReducePrice, ChangeDate, Summay, 
                      AdminID, Status, TinyID, OriginalDate, OriginalIndex, ChangeIndex, ItemID, ApplicationID, PayerID, PayeeID, Quantity
FROM         PvbCrm.dbo.VouchersStatisticsView AS VouchersStatisticsView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "VouchersStatisticsView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 199
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VouchersStatisticsView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VouchersStatisticsView'
GO
/****** Object:  View [dbo].[WaybillsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[WaybillsTopView]
AS
SELECT   wbID, wbCode, wbType, wbSubcodes, wbCarrierID, wbConsignorID, wbConsigneeID, wbFreightPayer, wbCarrierAccount, 
                wbVoyageNumber, wbCondition, wbCreateDate, wbModifyDate, wbEnterCode, wbStatus, wbCreatorID, wbModifierID, 
                wbTransferID, wbIsClearance, corID, corCompany, corPlace, corAddress, corContact, corPhone, corZipcode, corEmail, 
                corCreateDate, coeID, coeCompany, coePlace, coeAddress, coeContact, coePhone, coeZipcode, coeEmail, 
                coeCreateDate, chcdID, chcdLotNumber, chcdCarNumber1, chcdCarNumber2, chcdCarload, chcdIsOnevehicle, 
                chcdDriver, chcdPlanDate, chcdDepartDate, chcdTotalQuantity, wldID, wldTakingDate, wldTakingAddress, 
                wldTakingContact, wldTakingPhone, wldCarNumber1, wldDriver, wldCarload, wldCreateDate, wldModifyDate, 
                wldCreatorID, wldModifierID, wldCarrier, wbTotalParts, wbTotalWeight, wbTotalVolume, wbPackaging, chgID, chgPayer, 
                chgPayMethod, chgCurrency, chgTotalPrice, wbFatherID, wbSupplier, wbExcuteStatus, corIDType, corIDNumber, 
                coeIDType, coeIDNumber, wbSummary, CuttingOrderStatus, chcdPhone, AppointTime, ConfirmReceiptStatus, OrderID, 
                Source, NoticeType, TempEnterCode, loadExcuteStatus, ExType, ExPayType
FROM      PvCenter.dbo.WaybillsTopView AS WaybillsTopView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[59] 4[3] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "WaybillsTopView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 333
               Right = 236
            End
            DisplayFlags = 280
            TopColumn = 52
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 21
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'WaybillsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'WaybillsTopView'
GO
/****** Object:  View [dbo].[TransportsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TransportsTopView]
AS
SELECT   ID, EnterpriseID, Type, CarNumber1, CarNumber2, Weight, Status
FROM      PvbCrm.dbo.TransportsTopView
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TransportsTopView (PvbCrm.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 202
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TransportsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TransportsTopView'
GO
/****** Object:  View [dbo].[PaymentsStatisticsView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[PaymentsStatisticsView]
AS
SELECT     OrderID, WaybillID, PayableID, Payer, Payee, Business, Catalog, Subject, Currency, LeftPrice, LeftDate, RightPrice, RightDate, ReducePrice, Summay, AdminID, Status, TinyID, ItemID, 
                      ApplicationID
FROM         PvbCrm.dbo.PaymentsStatisticsView AS PaymentsStatisticsView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "PaymentsStatisticsView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 205
            End
            DisplayFlags = 280
            TopColumn = 15
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PaymentsStatisticsView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PaymentsStatisticsView'
GO
/****** Object:  View [dbo].[FilesDescriptionTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[FilesDescriptionTopView]
AS
SELECT   ID, WaybillID, NoticeID, StorageID, InputID, AdminID, ClientID, CustomName, PayID, Type, Url, CreateDate, Status, 
                WsOrderID, LsOrderID
FROM      PvCenter.dbo.FilesDescriptionTopView AS FilesDescriptionTopView_1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "FilesDescriptionTopView_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 145
               Right = 206
            End
            DisplayFlags = 280
            TopColumn = 1
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'FilesDescriptionTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'FilesDescriptionTopView'
GO
/****** Object:  Table [dbo].[Sortings]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Sortings](
	[ID] [varchar](50) NOT NULL,
	[NoticeID] [varchar](50) NULL,
	[InputID] [varchar](50) NULL,
	[WaybillID] [varchar](50) NULL,
	[BoxCode] [varchar](50) NULL,
	[Quantity] [decimal](18, 7) NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Weight] [decimal](18, 7) NULL,
	[NetWeight] [decimal](18, 7) NULL,
	[Volume] [decimal](18, 7) NULL,
	[Summary] [nvarchar](300) NULL,
 CONSTRAINT [PK_Sortings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [index_BoxCoue] ON [dbo].[Sortings] 
(
	[BoxCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_InputID_CreateDate] ON [dbo].[Sortings] 
(
	[InputID] ASC,
	[CreateDate] ASC
)
INCLUDE ( [ID],
[NoticeID],
[WaybillID],
[BoxCode],
[Quantity],
[AdminID],
[Weight],
[NetWeight],
[Volume],
[Summary]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'四位年+2位月+2日+6位流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sortings', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sortings', @level2type=N'COLUMN',@level2name=N'WaybillID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'装箱信息（箱号）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sortings', @level2type=N'COLUMN',@level2name=N'BoxCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sortings', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分拣人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sortings', @level2type=N'COLUMN',@level2name=N'AdminID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间(发生时间)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sortings', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
/****** Object:  View [dbo].[CgPvWmsOrderItemsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgPvWmsOrderItemsTopView]
AS
SELECT   item.ID, item.OrderID, item.TinyOrderID, item.InputID, item.OutputID, item.ProductID, item.CustomName, item.Origin, 
                item.DateCode, item.Quantity, item.Currency, item.UnitPrice, item.Unit, item.TotalPrice, item.CreateDate, item.Status, 
                item.Type, xdtitem.BoxCode, xdtitem.hkSortingDate, xdtitem.hkExitDate, xdtitem.szSortingDate, xdtitem.szExitDate, 
                xdtitem.GrossWeight, xdtitem.NetWeight, xdtitem.declareDate
FROM      dbo.OrderItemsTopView AS item LEFT OUTER JOIN
                foricScCustoms.dbo.PvWmsOrderItemsTopView AS xdtitem ON item.ID = xdtitem.ID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导数据专用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsOrderItemsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "item"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 206
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xdtitem"
            Begin Extent = 
               Top = 6
               Left = 244
               Bottom = 146
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsOrderItemsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgPvWmsOrderItemsTopView'
GO
/****** Object:  View [dbo].[CgWaybillsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgWaybillsTopView]
AS
SELECT   waybill.wbID, waybill.wbCode, waybill.wbType, waybill.wbSubcodes, waybill.wbCarrierID, waybill.wbConsignorID, 
                waybill.wbConsigneeID, waybill.wbFreightPayer, waybill.wbCarrierAccount, waybill.wbVoyageNumber, 
                waybill.wbCondition, waybill.wbCreateDate, waybill.wbModifyDate, waybill.wbEnterCode, waybill.wbStatus, 
                waybill.wbCreatorID, waybill.wbModifierID, waybill.wbTransferID, waybill.wbIsClearance, waybill.corID, 
                waybill.corCompany, waybill.corPlace, waybill.corAddress, waybill.corContact, waybill.corPhone, waybill.corZipcode, 
                waybill.corEmail, waybill.corCreateDate, waybill.coeID, waybill.coeCompany, waybill.coePlace, waybill.coeAddress, 
                waybill.coeContact, waybill.coePhone, waybill.coeZipcode, waybill.coeEmail, waybill.coeCreateDate, waybill.chcdID, 
                waybill.chcdLotNumber, waybill.chcdCarNumber1, waybill.chcdCarNumber2, waybill.chcdCarload, 
                waybill.chcdIsOnevehicle, waybill.chcdDriver, waybill.chcdPlanDate, waybill.chcdDepartDate, waybill.chcdTotalQuantity, 
                waybill.wldID, waybill.wldTakingDate, waybill.wldTakingAddress, waybill.wldTakingContact, waybill.wldTakingPhone, 
                waybill.wldCarNumber1, waybill.wldDriver, waybill.wldCarload, waybill.wldCreateDate, waybill.wldModifyDate, 
                waybill.wldCreatorID, waybill.wldModifierID, waybill.wldCarrier, waybill.wbTotalParts, waybill.wbTotalWeight, 
                waybill.wbTotalVolume, waybill.wbPackaging, waybill.chgID, waybill.chgPayer, waybill.chgPayMethod, 
                waybill.chgCurrency, waybill.chgTotalPrice, waybill.wbFatherID, waybill.wbSupplier, waybill.wbExcuteStatus, 
                waybill.corIDType, waybill.corIDNumber, waybill.coeIDType, waybill.coeIDNumber, waybill.wbSummary, 
                waybill.CuttingOrderStatus, waybill.chcdPhone, waybill.AppointTime, waybill.ConfirmReceiptStatus, waybill.OrderID, 
                waybill.Source, waybill.NoticeType, waybill.TempEnterCode, waybill.loadExcuteStatus, waybill.ExType, 
                waybill.ExPayType, WareHouse.WareHouseID
FROM      dbo.WaybillsTopView AS waybill INNER JOIN
                    (SELECT   WareHouseID, WaybillID
                     FROM      dbo.Notices WITH (nolock)
                     GROUP BY WareHouseID, WaybillID) AS WareHouse ON waybill.wbID = WareHouse.WaybillID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "waybill"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 292
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 52
         End
         Begin Table = "WareHouse"
            Begin Extent = 
               Top = 6
               Left = 269
               Bottom = 303
               Right = 425
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 90
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgWaybillsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgWaybillsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgWaybillsTopView'
GO
/****** Object:  View [dbo].[CgSzOutputWaybillsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgSzOutputWaybillsTopView]
AS
SELECT     waybill.wbID AS WaybillID, waybill.wbType AS WaybillType, waybill.wbTotalParts, waybill.OrderID, waybill.wbExcuteStatus AS IsModify, waybill.AppointTime, waybill.wbCreateDate, 
                      waybill.wbCode AS Code, waybill.ConfirmReceiptStatus
FROM         dbo.WaybillsTopView AS waybill WITH (nolock) INNER JOIN
                          (SELECT     WaybillID
                            FROM          dbo.Notices AS notice WITH (nolock)
                            WHERE      (WareHouseID LIKE 'SZ%') AND (Type = 200)
                            GROUP BY WareHouseID, WaybillID) AS nsv ON waybill.wbID = nsv.WaybillID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "waybill"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 173
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "nsv"
            Begin Extent = 
               Top = 6
               Left = 269
               Bottom = 81
               Right = 411
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgSzOutputWaybillsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgSzOutputWaybillsTopView'
GO
/****** Object:  View [dbo].[CgSzOutputDetailsTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgSzOutputDetailsTopView]
AS
SELECT     waybill.wbID, waybill.wbType, waybill.wbCarrierID, waybill.wbCreateDate, waybill.corCompany, waybill.corPlace, waybill.corAddress, waybill.corContact, waybill.corPhone, waybill.coeCompany, 
                      waybill.coePlace, waybill.coeAddress, waybill.coeContact, waybill.coePhone, waybill.coeZipcode, waybill.coeEmail, waybill.wbTotalParts, waybill.wbExcuteStatus AS IsModify, waybill.coeIDNumber, 
                      waybill.wbSummary, waybill.AppointTime, waybill.ExType, waybill.ExPayType, notice.Quantity, notice.DateCode, notice.Origin, notice.Weight, notice.NetWeight, notice.Volume, notice.BoxCode, 
                      notice.ShelveID, product.PartNumber, product.Manufacturer, product.PackageCase, product.Packaging, outputs.OrderID, outputs.TinyOrderID, outputs.ItemID, outputs.TrackerID, waybill.wbEnterCode, 
                      waybill.corEmail, waybill.corZipcode, waybill.corIDNumber, waybill.corIDType, waybill.coeIDType, waybill.chcdCarNumber1
FROM         dbo.WaybillsTopView AS waybill INNER JOIN
                      dbo.Notices AS notice ON waybill.wbID = notice.WaybillID INNER JOIN
                      dbo.ProductsTopView AS product ON notice.ProductID = product.ID INNER JOIN
                      dbo.Outputs AS outputs ON notice.OutputID = outputs.ID
WHERE     (notice.WareHouseID LIKE 'SZ%') AND (notice.Type = 200)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "waybill"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 271
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 7
         End
         Begin Table = "notice"
            Begin Extent = 
               Top = 6
               Left = 269
               Bottom = 126
               Right = 426
            End
            DisplayFlags = 280
            TopColumn = 21
         End
         Begin Table = "product"
            Begin Extent = 
               Top = 6
               Left = 683
               Bottom = 126
               Right = 837
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "outputs"
            Begin Extent = 
               Top = 6
               Left = 464
               Bottom = 223
               Right = 645
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgSzOutputDetailsTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgSzOutputDetailsTopView'
GO
/****** Object:  Table [dbo].[Storages]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Storages](
	[ID] [varchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[WareHouseID] [varchar](50) NULL,
	[SortingID] [varchar](50) NOT NULL,
	[InputID] [varchar](50) NULL,
	[ProductID] [varchar](50) NULL,
	[Total] [decimal](18, 7) NULL,
	[Quantity] [decimal](18, 7) NOT NULL,
	[Origin] [varchar](50) NULL,
	[IsLock] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[ShelveID] [varchar](50) NULL,
	[Supplier] [nvarchar](150) NULL,
	[DateCode] [varchar](50) NULL,
	[Summary] [nvarchar](200) NULL,
	[CustomsName] [nvarchar](150) NULL,
 CONSTRAINT [PK_Storages] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Inderx_WareHouseID_Input_Total_Quantity] ON [dbo].[Storages] 
(
	[WareHouseID] ASC,
	[InputID] ASC,
	[Total] ASC,
	[Quantity] ASC
)
INCLUDE ( [ID],
[Type],
[SortingID],
[ProductID],
[Origin],
[IsLock],
[CreateDate],
[Status],
[ShelveID],
[Supplier],
[DateCode],
[Summary],
[CustomsName]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_ID_InputID_CreateDate0] ON [dbo].[Storages] 
(
	[ID] ASC,
	[InputID] ASC,
	[CreateDate] ASC
)
INCLUDE ( [Type],
[WareHouseID],
[SortingID],
[ProductID],
[Total],
[Quantity],
[Origin],
[IsLock],
[Status],
[ShelveID],
[Supplier],
[DateCode],
[Summary],
[CustomsName]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_InputID] ON [dbo].[Storages] 
(
	[InputID] ASC
)
INCLUDE ( [ID],
[Type],
[WareHouseID],
[SortingID],
[ProductID],
[Total],
[Quantity],
[Origin],
[IsLock],
[CreateDate],
[Status],
[ShelveID],
[Supplier],
[DateCode],
[Summary],
[CustomsName]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [index_ShelveID] ON [dbo].[Storages] 
(
	[WareHouseID] ASC,
	[Quantity] ASC,
	[ShelveID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_SortingID_WareHouseID_CreateDate0] ON [dbo].[Storages] 
(
	[SortingID] ASC,
	[WareHouseID] ASC,
	[CreateDate] ASC
)
INCLUDE ( [ID],
[Type],
[InputID],
[ProductID],
[Total],
[Quantity],
[Origin],
[IsLock],
[Status],
[ShelveID],
[Supplier],
[DateCode],
[Summary],
[CustomsName]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_Type_SortingID_ProcutID] ON [dbo].[Storages] 
(
	[Type] ASC,
	[SortingID] ASC,
	[ProductID] ASC
)
INCLUDE ( [ID],
[WareHouseID],
[InputID],
[Total],
[Quantity],
[Origin],
[IsLock],
[CreateDate],
[Status],
[ShelveID],
[Supplier],
[DateCode],
[Summary],
[CustomsName]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Storages_ProductID] ON [dbo].[Storages] 
(
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码,四位年+2位月+2日+6位流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流水库、库存库、运营库、在途库、报废库、检测库、暂存库	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'WareHouseID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' 分拣ID:入库时是分拣编号，出库时是拣货编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'SortingID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'进项' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'InputID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'ProductID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'Total'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'现存数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否锁定' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'IsLock'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：正常、废弃' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库位编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'ShelveID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'批次号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'DateCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报关品名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'COLUMN',@level2name=N'CustomsName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存对象' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages'
GO
EXEC sys.sp_addextendedproperty @name=N'用途', @value=N'视图：ClientIOStorageTopView' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Storages', @level2type=N'INDEX',@level2name=N'Index_ID_InputID_CreateDate0'
GO
/****** Object:  Table [dbo].[Pickings]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Pickings](
	[ID] [varchar](50) NOT NULL,
	[StorageID] [varchar](50) NOT NULL,
	[NoticeID] [varchar](50) NOT NULL,
	[OutputID] [varchar](50) NOT NULL,
	[BoxCode] [varchar](50) NULL,
	[Quantity] [decimal](18, 7) NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Weight] [decimal](18, 7) NULL,
	[NetWeight] [decimal](18, 7) NULL,
	[Volume] [decimal](18, 7) NULL,
	[Summary] [nvarchar](300) NULL,
 CONSTRAINT [PK_Pickings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Index_OutputID_CreateDate] ON [dbo].[Pickings] 
(
	[OutputID] ASC,
	[CreateDate] ASC
)
INCLUDE ( [ID],
[StorageID],
[NoticeID],
[BoxCode],
[Quantity],
[AdminID],
[Weight],
[NetWeight],
[Volume],
[Summary]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Index_StorageID_CreateDate] ON [dbo].[Pickings] 
(
	[StorageID] ASC,
	[CreateDate] ASC
)
INCLUDE ( [Quantity]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'四位年+2位月+2日+6位流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' 库存ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'StorageID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' 通知ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'NoticeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销项ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'OutputID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'装箱信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'BoxCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'AdminID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CreateDate' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pickings', @level2type=N'COLUMN',@level2name=N'Volume'
GO
/****** Object:  Table [dbo].[Forms]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Forms](
	[ID] [varchar](50) NOT NULL,
	[StorageID] [varchar](50) NOT NULL,
	[Quantity] [decimal](18, 7) NOT NULL,
	[NoticeID] [varchar](50) NULL,
	[Status] [int] NOT NULL,
	[SessionID] [varchar](50) NULL,
 CONSTRAINT [PK_Forms] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Index_StorageID_Quantity] ON [dbo].[Forms] 
(
	[StorageID] ASC,
	[Status] ASC,
	[Quantity] ASC
)
INCLUDE ( [ID],
[NoticeID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [StorageID] ON [dbo].[Forms] 
(
	[StorageID] ASC
)
INCLUDE ( [ID],
[NoticeID],
[Status]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Forms', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Forms', @level2type=N'COLUMN',@level2name=N'StorageID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Forms', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'通知ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Forms', @level2type=N'COLUMN',@level2name=N'NoticeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'冻结：Frozen,真实的（真正执行的）：Facted' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Forms', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'按照约定：InputID    OutputID  AdminID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Forms', @level2type=N'COLUMN',@level2name=N'SessionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Forms'
GO
/****** Object:  View [dbo].[CgNoticedTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgNoticedTopView]
AS
SELECT   input.OrderID AS vastOrderID, noticed.StorageID, noticed.Quantity, Storage.Total
FROM      (SELECT   StorageID, SUM(Quantity) AS Quantity
                 FROM      dbo.Notices AS notice WITH (nolock)
                 WHERE   (WareHouseID LIKE 'SZ%')
                 GROUP BY StorageID) AS noticed INNER JOIN
                dbo.Storages AS Storage ON noticed.StorageID = Storage.ID AND noticed.Quantity <= Storage.Total INNER JOIN
                dbo.Inputs AS input ON Storage.InputID = input.ID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "noticed"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 96
               Right = 196
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Storage"
            Begin Extent = 
               Top = 6
               Left = 234
               Bottom = 126
               Right = 407
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "input"
            Begin Extent = 
               Top = 6
               Left = 445
               Bottom = 126
               Right = 609
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgNoticedTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgNoticedTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'说明', @value=N'与荣检第二次对应并商议为：D:\Projects_vs2015\BvNew\BvnErp.Chains_SIT-分支\Needs.Chain.Customs.Services\Views\Order\CenterOrderPendingDeliveryView.cs 中
 var qtyView = from c in storagesTopView
                          where c.WareHouseID == szWarehouseName && (c.Total - c.Quantity) > 0
                          group c by c.OrderID into g
                          select new
                          {
                              mainOrderID = g.Key,
                              HasNotified = false,
                              HasExited = false,
                          };
提供专有过滤视图' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgNoticedTopView'
GO
/****** Object:  View [dbo].[CgInputReportTopView]    Script Date: 05/22/2020 18:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgInputReportTopView]
AS
SELECT     sto.SortingID, sto.WareHouseID, sort.Quantity AS EnterQuantity, sort.CreateDate AS EnterDate, sort.AdminID, ipt.ClientID, ipt.Currency, ipt.UnitPrice, nt.Quantity AS NoticeQuantity, 
                      nt.CreateDate AS NoticeDate, nt.ID AS NoticeID, nt.Source, ptv.PartNumber, ptv.Manufacturer, sto.Origin, sto.Supplier, ipt.OrderID, ipt.TinyOrderID, sort.WaybillID, client.Name, client.EnterCode, 
                      sto.InputID, sto.ProductID, sort.Summary, sto.DateCode, sto.CustomsName, sort.Weight, sort.Volume, sort.BoxCode, sto.ShelveID
FROM         dbo.Storages AS sto INNER JOIN
                      dbo.Sortings AS sort ON sto.SortingID = sort.ID INNER JOIN
                      dbo.Inputs AS ipt ON sto.InputID = ipt.ID INNER JOIN
                      dbo.ProductsTopView AS ptv ON sto.ProductID = ptv.ID LEFT OUTER JOIN
                      dbo.Notices AS nt ON sort.NoticeID = nt.ID LEFT OUTER JOIN
                      dbo.WsClientsTopView AS client ON ipt.ClientID = client.ID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入库报表视图' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgInputReportTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[21] 2[22] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "sto"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 11
         End
         Begin Table = "sort"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 146
               Right = 400
            End
            DisplayFlags = 280
            TopColumn = 8
         End
         Begin Table = "ipt"
            Begin Extent = 
               Top = 6
               Left = 438
               Bottom = 146
               Right = 598
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "ptv"
            Begin Extent = 
               Top = 6
               Left = 844
               Bottom = 146
               Right = 1011
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "nt"
            Begin Extent = 
               Top = 6
               Left = 636
               Bottom = 146
               Right = 806
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "client"
            Begin Extent = 
               Top = 6
               Left = 1049
               Bottom = 146
               Right = 1219
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 23
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
     ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgInputReportTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'    Width = 1500
         Width = 1875
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgInputReportTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgInputReportTopView'
GO
/****** Object:  View [dbo].[CgDeliveriesTopView]    Script Date: 05/22/2020 18:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgDeliveriesTopView]
AS
SELECT  storage.ID, storage.Type, storage.SortingID, storage.InputID, storage.ProductID, storage.Total, storage.Quantity, storage.Origin, storage.IsLock, storage.ShelveID, storage.Supplier, storage.DateCode, 
               storage.Summary, input.OrderID, input.TinyOrderID, input.ItemID, prodcut.PartNumber, prodcut.Manufacturer, prodcut.PackageCase, prodcut.Packaging, input.Currency, input.UnitPrice
FROM     dbo.Storages AS storage WITH (nolock) INNER JOIN
               dbo.Inputs AS input WITH (nolock) ON storage.InputID = input.ID INNER JOIN
               dbo.ProductsTopView AS prodcut WITH (nolock) ON storage.ProductID = prodcut.ID
WHERE  (storage.WareHouseID LIKE 'HK%')
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "storage"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 195
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "input"
            Begin Extent = 
               Top = 6
               Left = 233
               Bottom = 126
               Right = 381
            End
            DisplayFlags = 280
            TopColumn = 12
         End
         Begin Table = "prodcut"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 246
               Right = 192
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 897
         Table = 1168
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1345
         SortOrder = 1413
         GroupBy = 1350
         Filter = 1345
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgDeliveriesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgDeliveriesTopView'
GO
/****** Object:  View [dbo].[CgStoragesTopView]    Script Date: 05/22/2020 18:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgStoragesTopView]
AS
SELECT   storage.ID, storage.WareHouseID, storage.InputID, storage.Total, storage.Quantity, storage.Origin, storage.IsLock, 
                storage.ShelveID, storage.Supplier, storage.DateCode, storage.Summary, storage.CustomsName, input.OrderID, 
                input.TinyOrderID, input.ItemID, input.TrackerID, input.Currency, input.UnitPrice, product.PartNumber, 
                product.Manufacturer, product.PackageCase, product.Packaging, storage.ProductID, client.ID AS ClientID, 
                client.Name AS ClientName, storage.CreateDate
FROM      dbo.Storages AS storage INNER JOIN
                dbo.Inputs AS input ON storage.InputID = input.ID INNER JOIN
                dbo.ProductsTopView AS product ON storage.ProductID = product.ID INNER JOIN
                dbo.Sortings AS sort ON storage.SortingID = sort.ID INNER JOIN
                dbo.WaybillsTopView AS waybill ON sort.WaybillID = waybill.wbID INNER JOIN
                dbo.WsClientsTopView AS client ON waybill.wbEnterCode = client.EnterCode
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[43] 4[21] 2[19] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "storage"
            Begin Extent = 
               Top = 6
               Left = 42
               Bottom = 246
               Right = 210
            End
            DisplayFlags = 280
            TopColumn = 5
         End
         Begin Table = "input"
            Begin Extent = 
               Top = 6
               Left = 252
               Bottom = 238
               Right = 409
            End
            DisplayFlags = 280
            TopColumn = 5
         End
         Begin Table = "product"
            Begin Extent = 
               Top = 6
               Left = 451
               Bottom = 242
               Right = 608
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "sort"
            Begin Extent = 
               Top = 6
               Left = 646
               Bottom = 126
               Right = 791
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "waybill"
            Begin Extent = 
               Top = 126
               Left = 646
               Bottom = 246
               Right = 839
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "client"
            Begin Extent = 
               Top = 240
               Left = 248
               Bottom = 360
               Right = 403
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 40
         Width = 284
         Width = 1365
         Width = 1365
         Widt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStoragesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'h = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStoragesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStoragesTopView'
GO
/****** Object:  View [dbo].[CgStatisticsShiped]    Script Date: 05/22/2020 18:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*深圳出库 CgStatisticsShiped*/
CREATE VIEW [dbo].[CgStatisticsShiped]
AS
SELECT     'SZShiped' AS type, InputID, SUM(Total) - SUM(Quantity) AS SZOutQuantity
FROM         dbo.Storages WITH (nolock)
WHERE     (WareHouseID LIKE 'SZ%')
GROUP BY InputID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Storages"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 211
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStatisticsShiped'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStatisticsShiped'
GO
/****** Object:  View [dbo].[CgStatisticsHKDelivery]    Script Date: 05/22/2020 18:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*香港入库 CgHkHKDeliveryStatistics*/
CREATE VIEW [dbo].[CgStatisticsHKDelivery]
AS
SELECT     'HKDelivery' AS type, InputID, SUM(Total) AS HKEnterQuantity
FROM         dbo.Storages WITH (nolock)
WHERE     (WareHouseID LIKE 'HK%')
GROUP BY InputID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Storages"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 211
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStatisticsHKDelivery'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStatisticsHKDelivery'
GO
/****** Object:  View [dbo].[CgStatisticsDeclare]    Script Date: 05/22/2020 18:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgStatisticsDeclare]
AS
SELECT     'Customs' AS type, Storage.InputID, SUM(Storage.Total) AS CustomQuantity
FROM         dbo.Logs_DeclareItem AS ldi WITH (nolock) INNER JOIN
                      dbo.Storages AS Storage WITH (nolock) ON ldi.StorageID = Storage.ID
GROUP BY Storage.InputID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "ldi"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 204
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Storage"
            Begin Extent = 
               Top = 6
               Left = 242
               Bottom = 126
               Right = 415
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStatisticsDeclare'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgStatisticsDeclare'
GO
/****** Object:  View [dbo].[CgTempStoragesTopView]    Script Date: 05/22/2020 18:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgTempStoragesTopView]
AS
SELECT  storage.ID, storage.WareHouseID, storage.SortingID, storage.ProductID, storage.Total, storage.Quantity, storage.Origin, storage.IsLock, storage.CreateDate, storage.ShelveID, storage.Supplier, storage.DateCode, 
               storage.Summary, storage.CustomsName, sorting.NoticeID, sorting.WaybillID, sorting.BoxCode, sorting.AdminID, sorting.Weight, sorting.NetWeight, sorting.Volume, product.PartNumber, product.Manufacturer, 
               product.PackageCase, product.Packaging, waybill.wbEnterCode AS EnterCode
FROM     dbo.Storages AS storage INNER JOIN
               dbo.Sortings AS sorting ON storage.SortingID = sorting.ID LEFT OUTER JOIN
               dbo.ProductsTopView AS product ON storage.ProductID = product.ID INNER JOIN
               dbo.WaybillsTopView AS waybill ON sorting.WaybillID = waybill.wbID
WHERE  (storage.Type = 800)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[42] 4[20] 2[12] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "storage"
            Begin Extent = 
               Top = 26
               Left = 28
               Bottom = 314
               Right = 196
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "sorting"
            Begin Extent = 
               Top = 2
               Left = 294
               Bottom = 145
               Right = 447
            End
            DisplayFlags = 280
            TopColumn = 7
         End
         Begin Table = "product"
            Begin Extent = 
               Top = 179
               Left = 540
               Bottom = 306
               Right = 703
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "waybill"
            Begin Extent = 
               Top = 112
               Left = 745
               Bottom = 323
               Right = 953
            End
            DisplayFlags = 280
            TopColumn = 13
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 139
         Width = 284
         Width = 2214
         Width = 1358
         Width = 1358
         Width = 2174
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 135' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgTempStoragesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'8
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
         Width = 1358
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 2106
         Table = 1168
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1345
         SortOrder = 1413
         GroupBy = 1350
         Filter = 1345
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgTempStoragesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgTempStoragesTopView'
GO
/****** Object:  View [dbo].[CgSzStoragesTopView]    Script Date: 05/22/2020 18:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*select * from Notices  OPTION (MAXDOP 2);*/
CREATE VIEW [dbo].[CgSzStoragesTopView]
AS
SELECT     input.OrderID AS VastOrderid, input.TinyOrderID, product.PartNumber, product.Manufacturer, storage.ID AS UnqiueID, input.ItemID, sorts.BoxCode, sorts.Weight, sorts.NetWeight, storage.Type, 
                      ISNULL(noticed.Quantity, 0) AS Quantity, storage.DateCode, storage.WareHouseID, storage.Origin, storage.InputID AS StoInputID, storage.ID AS StorageID, input.UnitPrice, product.ID AS ProductID, 
                      sorts.Volume, storage.Total
FROM         dbo.Storages AS storage WITH (nolock) INNER JOIN
                      dbo.Inputs AS input WITH (nolock) ON storage.InputID = input.ID INNER JOIN
                      dbo.ProductsTopView AS product WITH (nolock) ON storage.ProductID = product.ID INNER JOIN
                      dbo.Sortings AS sorts WITH (nolock) ON storage.SortingID = sorts.ID LEFT OUTER JOIN
                          (SELECT     StorageID, SUM(Quantity) AS Quantity
                            FROM          dbo.Notices WITH (nolock)
                            WHERE      (WareHouseID LIKE 'SZ%') AND (Type = 200)
                            GROUP BY StorageID) AS noticed ON storage.ID = noticed.StorageID
WHERE     (storage.WareHouseID LIKE 'SZ%')
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[26] 4[16] 2[32] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "storage"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 195
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "input"
            Begin Extent = 
               Top = 6
               Left = 413
               Bottom = 126
               Right = 561
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "product"
            Begin Extent = 
               Top = 6
               Left = 599
               Bottom = 126
               Right = 753
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "sorts"
            Begin Extent = 
               Top = 6
               Left = 791
               Bottom = 126
               Right = 936
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "noticed"
            Begin Extent = 
               Top = 6
               Left = 233
               Bottom = 96
               Right = 375
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 20
         Width = 284
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
 ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgSzStoragesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'        Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
         Width = 1365
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgSzStoragesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgSzStoragesTopView'
GO
/****** Object:  View [dbo].[CgXdtDelcaresTopView]    Script Date: 05/22/2020 18:36:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
经过与董建、荣检的提示\纠正，并与苏亮交流需要完成出库使用OutputID
再次强调：装箱分拣的是要完成箱号与称重相关数据的！

表示已经装箱可以申报

经过与董建、荣检的提示\纠正，并与苏亮交流需要完成出库使用OutputID
再次强调：装箱分拣的是要完成箱号与称重相关数据的！


经过与董建、荣检的提示\纠正，并与苏亮交流需要完成出库使用OutputID
再次强调：装箱分拣的是要完成箱号与称重相关数据的！
*/
CREATE VIEW [dbo].[CgXdtDelcaresTopView]
AS
SELECT     ldi.ID AS UnqiueID, ld.TinyOrderID, ldi.OrderItemID AS ItemID, ld.EnterCode, ldi.BoxCode, ld.LotNumber, ld.CreateDate AS BoxingDate, ld.AdminID AS Packer, ld.Summary, ldi.OrderItemID, 
                      ldi.StorageID, ldi.Quantity, p.PartNumber, p.Manufacturer, s.Origin, ldi.Weight, ldi.Weight * 0.7 AS NetWeight, s.InputID, ldi.OutputID,
                          (SELECT     TOP (1) PackageType
                            FROM          dbo.Boxes AS box
                            WHERE      (ldi.BoxCode = Series)) AS PackageType
FROM         dbo.Logs_Declare AS ld WITH (nolock) INNER JOIN
                      dbo.Logs_DeclareItem AS ldi WITH (nolock) ON ld.TinyOrderID = ldi.TinyOrderID INNER JOIN
                      dbo.Storages AS s WITH (nolock) ON ldi.StorageID = s.ID INNER JOIN
                      dbo.ProductsTopView AS p WITH (nolock) ON s.ProductID = p.ID
WHERE     (ld.Status = 30)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = -11
      End
      Begin Tables = 
         Begin Table = "ld"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 188
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ldi"
            Begin Extent = 
               Top = 6
               Left = 226
               Bottom = 126
               Right = 376
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "s"
            Begin Extent = 
               Top = 6
               Left = 414
               Bottom = 126
               Right = 570
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "p"
            Begin Extent = 
               Top = 6
               Left = 608
               Bottom = 126
               Right = 762
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgXdtDelcaresTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgXdtDelcaresTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'说明', @value=N'华芯通申报视图' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgXdtDelcaresTopView'
GO
/****** Object:  View [dbo].[CgXdtProcessesTopView]    Script Date: 05/22/2020 18:36:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*where a.OrderID ='NL02020200413001'*/
CREATE VIEW [dbo].[CgXdtProcessesTopView]
AS
SELECT     ID, Type, SortingID, InputID, ProductID, Total, Quantity, Origin, IsLock, ShelveID, Supplier, DateCode, Summary, OrderID, TinyOrderID, ItemID, PartNumber, Manufacturer, PackageCase, Packaging, 
                      BoxCode
FROM         (SELECT     storage.ID, storage.Type, storage.SortingID, storage.InputID, storage.ProductID, storage.Total, storage.Quantity, storage.Origin, storage.IsLock, storage.ShelveID, storage.Supplier, 
                                              storage.DateCode, storage.Summary, input.OrderID, input.TinyOrderID, input.ItemID, prodcut.PartNumber, prodcut.Manufacturer, prodcut.PackageCase, prodcut.Packaging, 
                                              sorting.BoxCode
                       FROM          dbo.Storages AS storage WITH (nolock) INNER JOIN
                                              dbo.Inputs AS input WITH (nolock) ON storage.InputID = input.ID INNER JOIN
                                              dbo.ProductsTopView AS prodcut WITH (nolock) ON storage.ProductID = prodcut.ID INNER JOIN
                                              dbo.Sortings AS sorting WITH (nolock) ON storage.SortingID = sorting.ID LEFT OUTER JOIN
                                              dbo.Notices AS notice WITH (nolock) ON sorting.NoticeID = notice.ID
                       WHERE      (notice.Type = 100 OR
                                              notice.Type IS NULL) AND (storage.WareHouseID LIKE 'HK%')
                       UNION ALL
                       SELECT     storage.ID, storage.Type, storage.SortingID, storage.InputID, storage.ProductID, storage.Total, storage.Quantity, storage.Origin, storage.IsLock, storage.ShelveID, storage.Supplier, 
                                             storage.DateCode, storage.Summary, output.OrderID, output.TinyOrderID, output.ItemID, prodcut.PartNumber, prodcut.Manufacturer, prodcut.PackageCase, prodcut.Packaging, 
                                             picking.BoxCode
                       FROM         dbo.Storages AS storage WITH (nolock) INNER JOIN
                                             dbo.Pickings AS picking WITH (nolock) ON storage.ID = picking.StorageID INNER JOIN
                                             dbo.ProductsTopView AS prodcut WITH (nolock) ON storage.ProductID = prodcut.ID INNER JOIN
                                             dbo.Outputs AS output WITH (nolock) ON picking.OutputID = output.ID INNER JOIN
                                             dbo.Notices AS notice WITH (nolock) ON picking.NoticeID = notice.ID
                       WHERE     (notice.Type = 300) AND (storage.WareHouseID LIKE 'HK%')) AS a
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 192
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgXdtProcessesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgXdtProcessesTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'说明', @value=N'与荣检沟通：
华芯通在转报关与代报关流程中期望提供一个统一的视图完成箱号的展示，特地建立此视图' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgXdtProcessesTopView'
GO
/****** Object:  View [dbo].[CgXdtBillTopView]    Script Date: 05/22/2020 18:36:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgXdtBillTopView]
AS
SELECT     OrderID, TinyOrderID, wbCode, total, ClearanceDate, CargoValue, CarrierName, Type
FROM         (SELECT     input.OrderID, input.TinyOrderID, waybill.wbCode, SUM(sorting.Quantity) AS total, SUM(input.UnitPrice * sorting.Quantity) AS CargoValue, MIN(storage.CreateDate) AS ClearanceDate,
                                                  (SELECT     Name
                                                    FROM          dbo.CarriersTopView AS carrier
                                                    WHERE      (ID = waybill.wbCarrierID)) AS CarrierName, 'In' AS Type
                       FROM          dbo.Inputs AS input INNER JOIN
                                              dbo.Storages AS storage ON input.ID = storage.InputID INNER JOIN
                                              dbo.Sortings AS sorting ON storage.SortingID = sorting.ID INNER JOIN
                                              dbo.WaybillsTopView AS waybill ON sorting.WaybillID = waybill.wbID
                       WHERE      (waybill.wbCode IS NOT NULL)
                       GROUP BY input.TinyOrderID, input.OrderID, waybill.wbCode, waybill.wbCarrierID
                       UNION ALL
                       SELECT     output.OrderID, output.TinyOrderID, waybill.wbCode, SUM(picking.Quantity) AS total, SUM(output.Price * picking.Quantity) AS CargoValue, MIN(storage.CreateDate) AS ClearanceDate,
                                                 (SELECT     Name
                                                   FROM          dbo.CarriersTopView AS carrier
                                                   WHERE      (ID = waybill.wbCarrierID)) AS CarrierName, 'Out' AS Type
                       FROM         dbo.Pickings AS picking INNER JOIN
                                             dbo.Outputs AS output ON picking.OutputID = output.ID INNER JOIN
                                             dbo.Storages AS storage ON picking.StorageID = storage.ID INNER JOIN
                                             dbo.Notices AS notice ON picking.NoticeID = notice.ID INNER JOIN
                                             dbo.WaybillsTopView AS waybill ON notice.WaybillID = waybill.wbID
                       WHERE     (waybill.wbCode IS NOT NULL)
                       GROUP BY output.TinyOrderID, output.OrderID, waybill.wbCode, waybill.wbCarrierID) AS A
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "A"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgXdtBillTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgXdtBillTopView'
GO
/****** Object:  View [dbo].[CgOutputReportTopView]    Script Date: 05/22/2020 18:36:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgOutputReportTopView]
AS
SELECT   pk.ID, pk.NoticeID, pk.OutputID, pk.AdminID, pk.CreateDate AS PickingDate, pk.Quantity AS PickingQuantity, pk.BoxCode, 
                pk.Summary, sto.WareHouseID, opt.OrderID, opt.TinyOrderID, opt.InputID, sto.Origin, sto.Supplier, sto.DateCode, 
                nt.Quantity AS NoticeQuantity, nt.CreateDate AS NoticeDate, nt.WaybillID, ptv.PartNumber, ptv.Manufacturer, ipt.ClientID, 
                ipt.UnitPrice AS inUnitPrice, ipt.Currency AS inCurrency, opt.Price AS outUnitPrice, opt.Currency AS outCurrency, 
                client.Name AS ClientName, client.EnterCode, opt.ReviewerID, nt.Source, nt.ProductID, sto.CustomsName
FROM      dbo.Pickings AS pk INNER JOIN
                dbo.Storages AS sto ON pk.StorageID = sto.ID INNER JOIN
                dbo.Outputs AS opt ON pk.OutputID = opt.ID INNER JOIN
                dbo.Notices AS nt ON pk.NoticeID = nt.ID INNER JOIN
                dbo.Inputs AS ipt ON opt.InputID = ipt.ID INNER JOIN
                dbo.ProductsTopView AS ptv ON nt.ProductID = ptv.ID LEFT OUTER JOIN
                dbo.WsClientsTopView AS client ON ipt.ClientID = client.ID
WHERE   (nt.Type = 200)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "pk"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 192
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "sto"
            Begin Extent = 
               Top = 6
               Left = 230
               Bottom = 146
               Right = 404
            End
            DisplayFlags = 280
            TopColumn = 13
         End
         Begin Table = "opt"
            Begin Extent = 
               Top = 6
               Left = 442
               Bottom = 146
               Right = 641
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "nt"
            Begin Extent = 
               Top = 6
               Left = 679
               Bottom = 146
               Right = 853
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ipt"
            Begin Extent = 
               Top = 6
               Left = 891
               Bottom = 146
               Right = 1051
            End
            DisplayFlags = 280
            TopColumn = 6
         End
         Begin Table = "ptv"
            Begin Extent = 
               Top = 6
               Left = 1089
               Bottom = 146
               Right = 1256
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "client"
            Begin Extent = 
               Top = 150
               Left = 38
               Bottom = 290
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgOutputReportTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'       End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 32
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgOutputReportTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgOutputReportTopView'
GO
/****** Object:  View [dbo].[CgClientIOReportTopView]    Script Date: 05/22/2020 18:36:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CgClientIOReportTopView]
AS
SELECT     InputID, Origin, OutputID, WareHouseID, CustomsName, PartNumber, Manufacturer, Quantity, Date, OutUnitPrice, OutCurrency, inUnitPrice, inCurrency, ClientName, EnterCode, PastInQuantity, 
                      PastOutQuantity, profit
FROM         (SELECT     sto.InputID, sto.Origin, NULL AS OutputID, sto.WareHouseID, sto.CustomsName, ptv.PartNumber, ptv.Manufacturer, sort.Quantity, sort.CreateDate AS Date, NULL AS OutUnitPrice, NULL 
                                              AS OutCurrency, ipt.UnitPrice AS inUnitPrice, ipt.Currency AS inCurrency, client.Name AS ClientName, waybill.wbEnterCode AS EnterCode, ISNULL
                                                  ((SELECT     SUM(Quantity) AS Expr1
                                                      FROM         dbo.Sortings AS spiq
                                                      WHERE     (CreateDate < sort.CreateDate) AND (InputID = sto.InputID)), 0) AS PastInQuantity, ISNULL
                                                  ((SELECT     SUM(poqp.Quantity) AS Expr1
                                                      FROM         dbo.Pickings AS poqp INNER JOIN
                                                                            dbo.Storages AS poqsto ON poqp.StorageID = poqsto.ID
                                                      WHERE     (poqp.CreateDate < sort.CreateDate) AND (ipt.ID = poqsto.InputID)), 0) AS PastOutQuantity, 0 AS profit
                       FROM          dbo.Sortings AS sort INNER JOIN
                                              dbo.Storages AS sto ON sort.ID = sto.SortingID INNER JOIN
                                              dbo.Inputs AS ipt ON sto.InputID = ipt.ID INNER JOIN
                                              dbo.ProductsTopView AS ptv ON sto.ProductID = ptv.ID INNER JOIN
                                              dbo.WaybillsTopView AS waybill ON sort.WaybillID = waybill.wbID INNER JOIN
                                              dbo.WsClientsTopView AS client ON waybill.wbEnterCode = client.EnterCode
                       UNION ALL
                       SELECT     sto.InputID, sto.Origin, pick.OutputID, sto.WareHouseID, sto.CustomsName, ptv.PartNumber, ptv.Manufacturer, pick.Quantity, pick.CreateDate AS Date, opt.Price AS OutUnitPrice, 
                                             opt.Currency AS OutCurrency, ipt.UnitPrice AS inUnitPrice, ipt.Currency AS inCurrency, client.Name AS ClientName, waybill.wbEnterCode AS EnterCode, ISNULL
                                                 ((SELECT     SUM(Quantity) AS Expr1
                                                     FROM         dbo.Sortings AS spiq
                                                     WHERE     (CreateDate < pick.CreateDate) AND (InputID = sto.InputID)), 0) AS PastInQuantity, ISNULL
                                                 ((SELECT     SUM(Quantity) AS Expr1
                                                     FROM         dbo.Pickings AS ppiq
                                                     WHERE     (CreateDate < pick.CreateDate) AND (ID <> pick.ID) AND (OutputID = pick.OutputID)), 0) AS PastOutQuantity, (opt.Price - ipt.UnitPrice) * pick.Quantity AS profit
                       FROM         dbo.Pickings AS pick INNER JOIN
                                             dbo.Storages AS sto ON pick.StorageID = sto.ID INNER JOIN
                                             dbo.Inputs AS ipt ON sto.InputID = ipt.ID INNER JOIN
                                             dbo.Outputs AS opt ON pick.OutputID = opt.ID INNER JOIN
                                             dbo.ProductsTopView AS ptv ON sto.ProductID = ptv.ID INNER JOIN
                                             dbo.Notices AS n ON pick.NoticeID = n.ID INNER JOIN
                                             dbo.WaybillsTopView AS waybill ON n.WaybillID = waybill.wbID INNER JOIN
                                             dbo.WsClientsTopView AS client ON waybill.wbEnterCode = client.EnterCode) AS a
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgClientIOReportTopView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CgClientIOReportTopView'
GO
/****** Object:  ForeignKey [FK_Notices_Inputs]    Script Date: 05/22/2020 18:36:08 ******/
ALTER TABLE [dbo].[Notices]  WITH CHECK ADD  CONSTRAINT [FK_Notices_Inputs] FOREIGN KEY([InputID])
REFERENCES [dbo].[Inputs] ([ID])
GO
ALTER TABLE [dbo].[Notices] CHECK CONSTRAINT [FK_Notices_Inputs]
GO
/****** Object:  ForeignKey [FK_Notices_Outputs]    Script Date: 05/22/2020 18:36:08 ******/
ALTER TABLE [dbo].[Notices]  WITH CHECK ADD  CONSTRAINT [FK_Notices_Outputs] FOREIGN KEY([OutputID])
REFERENCES [dbo].[Outputs] ([ID])
GO
ALTER TABLE [dbo].[Notices] CHECK CONSTRAINT [FK_Notices_Outputs]
GO
/****** Object:  ForeignKey [FK_Sortings_Inputs]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Sortings]  WITH CHECK ADD  CONSTRAINT [FK_Sortings_Inputs] FOREIGN KEY([InputID])
REFERENCES [dbo].[Inputs] ([ID])
GO
ALTER TABLE [dbo].[Sortings] CHECK CONSTRAINT [FK_Sortings_Inputs]
GO
/****** Object:  ForeignKey [FK_Sortings_Notices]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Sortings]  WITH CHECK ADD  CONSTRAINT [FK_Sortings_Notices] FOREIGN KEY([NoticeID])
REFERENCES [dbo].[Notices] ([ID])
GO
ALTER TABLE [dbo].[Sortings] CHECK CONSTRAINT [FK_Sortings_Notices]
GO
/****** Object:  ForeignKey [FK_Storages_Inputs]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Storages]  WITH CHECK ADD  CONSTRAINT [FK_Storages_Inputs] FOREIGN KEY([InputID])
REFERENCES [dbo].[Inputs] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Storages] CHECK CONSTRAINT [FK_Storages_Inputs]
GO
/****** Object:  ForeignKey [FK_Storages_Shelves]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Storages]  WITH CHECK ADD  CONSTRAINT [FK_Storages_Shelves] FOREIGN KEY([ShelveID])
REFERENCES [dbo].[Shelves] ([ID])
GO
ALTER TABLE [dbo].[Storages] CHECK CONSTRAINT [FK_Storages_Shelves]
GO
/****** Object:  ForeignKey [FK_Storages_Sortings]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Storages]  WITH CHECK ADD  CONSTRAINT [FK_Storages_Sortings] FOREIGN KEY([SortingID])
REFERENCES [dbo].[Sortings] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Storages] CHECK CONSTRAINT [FK_Storages_Sortings]
GO
/****** Object:  ForeignKey [FK_Pickings_Notices]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Pickings]  WITH CHECK ADD  CONSTRAINT [FK_Pickings_Notices] FOREIGN KEY([NoticeID])
REFERENCES [dbo].[Notices] ([ID])
GO
ALTER TABLE [dbo].[Pickings] CHECK CONSTRAINT [FK_Pickings_Notices]
GO
/****** Object:  ForeignKey [FK_Pickings_Outputs]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Pickings]  WITH CHECK ADD  CONSTRAINT [FK_Pickings_Outputs] FOREIGN KEY([OutputID])
REFERENCES [dbo].[Outputs] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Pickings] CHECK CONSTRAINT [FK_Pickings_Outputs]
GO
/****** Object:  ForeignKey [FK_Pickings_Storages]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Pickings]  WITH CHECK ADD  CONSTRAINT [FK_Pickings_Storages] FOREIGN KEY([StorageID])
REFERENCES [dbo].[Storages] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Pickings] CHECK CONSTRAINT [FK_Pickings_Storages]
GO
/****** Object:  ForeignKey [FK_Forms_Storages]    Script Date: 05/22/2020 18:36:15 ******/
ALTER TABLE [dbo].[Forms]  WITH CHECK ADD  CONSTRAINT [FK_Forms_Storages] FOREIGN KEY([StorageID])
REFERENCES [dbo].[Storages] ([ID])
GO
ALTER TABLE [dbo].[Forms] CHECK CONSTRAINT [FK_Forms_Storages]
GO
