USE PvbErm
go
-----------------------------------------------------WageItems�޸�-------------------------------------------
--�޸�����
EXEC sp_rename '[dbo].[WageItems].[IsCalc]','Type'
GO
--�޸�����
ALTER TABLE dbo.WageItems ALTER COLUMN type INT NOT null
GO

--���¼����
ALTER TABLE [dbo].[WageItems]
    ADD [InputerId] VARCHAR (50) NULL;

go


--1��ͨ�� 2������
UPDATE dbo.WageItems SET type=1;
UPDATE dbo.WageItems SET type=2 WHERE name IN('���ʺϼ�','ʵ������');
UPDATE dbo.WageItems SET type=18 WHERE Name='���¸�˰';

go
-----------------------------------------------------PayItems�޸�-------------------------------------------
ALTER TABLE [dbo].[PayItems]
    ADD [UpdateAdminID] VARCHAR (50) NULL,
        [Status]        INT          NULL;

go
--�޸�����״̬Ϊ���� 100
UPDATE dbo.PayItems SET [status]=100;

go
-----------------------------------------------------MapsItem����-------------------------------------------
IF NOT EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[MapsItem]'))
BEGIN
	CREATE TABLE [dbo].[MapsItem] (
    [WageItemID] VARCHAR (50) NOT NULL,
    [StaffID]    VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MapsItem] PRIMARY KEY CLUSTERED ([WageItemID] ASC, [StaffID] ASC)
);
END

GO

-----------------------------------------------------Pasts_WageItem����-------------------------------------------
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

-----------------------------------------------------Oplogs�޸�-------------------------------------------
ALTER TABLE dbo.Oplogs ALTER COLUMN Remark NVARCHAR(max)
go


-----------------------------------------------------�˵����IsLocal�ֶ�-------------------------------------------
IF NOT EXISTS(SELECT * FROM syscolumns WHERE id=OBJECT_ID('Menus') AND name='IsLocal')
BEGIN
	ALTER TABLE dbo.Menus ADD IsLocal BIT NULL DEFAULT 1;	
END


-----------------------------------------------------���admin�Ϳⷿ��ϵ��-------------------------------------------
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

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ⷿID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsWarehose', @level2type=N'COLUMN',@level2name=N'WarehoseID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'admin�Ϳⷿӳ���ϵ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsWarehose'
GO


