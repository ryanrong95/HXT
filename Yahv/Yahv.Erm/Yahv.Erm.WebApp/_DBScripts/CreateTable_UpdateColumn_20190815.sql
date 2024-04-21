USE PvbErm
go
-----------------------------------------------------WageItems修改-------------------------------------------
--修改名称
EXEC sp_rename '[dbo].[WageItems].[IsCalc]','Type'
GO
--修改类型
ALTER TABLE dbo.WageItems ALTER COLUMN type INT NOT null
GO

--添加录入人
ALTER TABLE [dbo].[WageItems]
    ADD [InputerId] VARCHAR (50) NULL;

go


--1普通列 2计算列
UPDATE dbo.WageItems SET type=1;
UPDATE dbo.WageItems SET type=2 WHERE name IN('工资合计','实发工资');
UPDATE dbo.WageItems SET type=18 WHERE Name='本月个税';

go
-----------------------------------------------------PayItems修改-------------------------------------------
ALTER TABLE [dbo].[PayItems]
    ADD [UpdateAdminID] VARCHAR (50) NULL,
        [Status]        INT          NULL;

go
--修改数据状态为保存 100
UPDATE dbo.PayItems SET [status]=100;

go
-----------------------------------------------------MapsItem创建-------------------------------------------
IF NOT EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[MapsItem]'))
BEGIN
	CREATE TABLE [dbo].[MapsItem] (
    [WageItemID] VARCHAR (50) NOT NULL,
    [StaffID]    VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MapsItem] PRIMARY KEY CLUSTERED ([WageItemID] ASC, [StaffID] ASC)
);
END

GO

-----------------------------------------------------Pasts_WageItem创建-------------------------------------------
IF NOT exists (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Pasts_WageItem]'))
BEGIN
CREATE TABLE [dbo].[Pasts_WageItem] (
    [ID]           VARCHAR (50)    NOT NULL,
    [DateIndex]    INT             NOT NULL,
    [WorkCityID]   VARCHAR (50)    NULL,
    [EnterpriseID] VARCHAR (50)    NOT NULL,
    [StaffID]      VARCHAR (50)    NOT NULL,
    [Type]         INT             NOT NULL,
    [Name]         NVARCHAR (50)   NULL,
    [Value]        DECIMAL (18, 5) NOT NULL,
    [Currency]     INT             NULL,
    [Accumulative] INT             NOT NULL,
    CONSTRAINT [PK_Pasts_WageItem] PRIMARY KEY CLUSTERED ([ID] ASC)
);
END
GO

-----------------------------------------------------Oplogs修改-------------------------------------------
ALTER TABLE dbo.Oplogs ALTER COLUMN Remark NVARCHAR(max)
go


-----------------------------------------------------菜单添加IsLocal字段-------------------------------------------
IF NOT EXISTS(SELECT * FROM syscolumns WHERE id=OBJECT_ID('Menus') AND name='IsLocal')
BEGIN
	ALTER TABLE dbo.Menus ADD IsLocal BIT NULL DEFAULT 1;	
END


-----------------------------------------------------添加admin和库房关系表-------------------------------------------
USE [PvbErm]
GO

/****** Object:  Table [dbo].[MapsWarehose]    Script Date: 08/26/2019 13:56:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[MapsWarehose](
	[AdminID] [VARCHAR](50) NOT NULL,
	[WarehoseID] [VARCHAR](50) NOT NULL,
 CONSTRAINT [PK_MapsWarehose] PRIMARY KEY CLUSTERED 
(
	[AdminID] ASC,
	[WarehoseID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsWarehose', @level2type=N'COLUMN',@level2name=N'WarehoseID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'admin和库房映射关系' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsWarehose'
GO


