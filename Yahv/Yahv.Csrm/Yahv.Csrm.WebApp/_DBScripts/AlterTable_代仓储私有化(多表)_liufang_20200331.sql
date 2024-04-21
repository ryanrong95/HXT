use PvbCrm;


GO

--新建表[dbo].[Payees]
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payees]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Payees] (
    [ID]           VARCHAR (50)   NOT NULL,
    [EnterpriseID] VARCHAR (50)   NOT NULL,
    [RealID]       VARCHAR (50)   NULL,
    [Methord]      INT            NULL,
    [Bank]         NVARCHAR (200) NULL,
    [BankAddress]  NVARCHAR (200) NULL,
    [Account]      VARCHAR (150)  NULL,
    [SwiftCode]    VARCHAR (50)   NULL,
    [Currency]     INT            NULL,
    [Contact]      VARCHAR (50)   NULL,
    [Tel]          VARCHAR (50)   NULL,
    [Mobile]       VARCHAR (50)   NULL,
    [Email]        VARCHAR (50)   NULL,
    [Status]       INT            NOT NULL,
    [CreateDate]   DATETIME       NOT NULL,
    [UpdateDate]   DATETIME       NOT NULL,
    [Creator]      VARCHAR (50)   CONSTRAINT [DF_Payees_Creator] DEFAULT ('SA01') NOT NULL,
    [Place]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_Payees_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Payees_Enterprises] FOREIGN KEY ([RealID]) REFERENCES [dbo].[Enterprises] ([ID]),
    CONSTRAINT [FK_Payees_Enterprises1] FOREIGN KEY ([EnterpriseID]) REFERENCES [dbo].[Enterprises] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'账号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Account';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开户行', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Bank';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开户行地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'BankAddress';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Contact';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Creator';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'币种', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'电子邮箱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Email';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属企业ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'EnterpriseID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'汇款方式(TT,支付宝，支票，现金)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Methord';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手机号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Mobile';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'国家/地区简称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Place';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'真实企业ID(承运商ID或其他)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'RealID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'l状态(nomal,abandon)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'银行编码 (国际)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'SwiftCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'联系电话', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'Tel';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees', @level2type = N'COLUMN', @level2name = N'UpdateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收款人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payees';
END


go


--新建表[dbo].[Payers]
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payers]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Payers] (
    [ID]           VARCHAR (50)   NOT NULL,
    [EnterpriseID] VARCHAR (50)   NOT NULL,
    [RealID]       VARCHAR (50)   NULL,
    [Methord]      INT            NOT NULL,
    [Bank]         NVARCHAR (200) NULL,
    [BankAddress]  NVARCHAR (200) NULL,
    [Account]      VARCHAR (50)   NULL,
    [SwiftCode]    VARCHAR (50)   NULL,
    [Currency]     INT            NOT NULL,
    [Contact]      VARCHAR (50)   NULL,
    [Tel]          VARCHAR (50)   NULL,
    [Mobile]       VARCHAR (50)   NULL,
    [Email]        VARCHAR (50)   NULL,
    [Status]       INT            NOT NULL,
    [CreateDate]   DATETIME       NOT NULL,
    [UpdateDate]   DATETIME       NOT NULL,
    [Creator]      VARCHAR (50)   CONSTRAINT [DF_Payers_AdminID] DEFAULT ('SA01') NOT NULL,
    [Place]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_Payers] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Payers_Enterprises] FOREIGN KEY ([EnterpriseID]) REFERENCES [dbo].[Enterprises] ([ID]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_Payers_Enterprises1] FOREIGN KEY ([RealID]) REFERENCES [dbo].[Enterprises] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'账号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Account';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开户行', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Bank';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开户行地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'BankAddress';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Contact';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Creator';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'币种', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'电子邮箱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Email';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属企业ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'EnterpriseID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Pick（Payer,4）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'汇款方式(TT,支付宝，支票，现金)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Methord';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手机号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Mobile';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'真实企业ID(承运商ID或其他)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'RealID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'l状态(nomal,abandon)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'银行编码 (国际)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'SwiftCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'联系电话', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'Tel';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers', @level2type = N'COLUMN', @level2name = N'UpdateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payers';
END

GO

--新建表[dbo].[nConsignors],
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[nConsignors]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[nConsignors](
	[ID] [varchar](50) NOT NULL,
	[nSupplierID] [varchar](50) NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[RealID] [varchar](50) NULL,
	[Title] [nvarchar](50) NULL,
	[Postzip] [varchar](50) NULL,
	[Province] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[Land] [nvarchar](50) NULL,
	[Address] [nvarchar](150) NOT NULL,
	[Contact] [nvarchar](100) NOT NULL,
	[Tel] [varchar](50) NULL,
	[Mobile] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[IsDefault] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[Place] [varchar](50) NULL,
 CONSTRAINT [PK_nConsignors] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF
ALTER TABLE [dbo].[nConsignors]  WITH CHECK ADD  CONSTRAINT [FK_nConsignors_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[nConsignors] CHECK CONSTRAINT [FK_nConsignors_Enterprises]
ALTER TABLE [dbo].[nConsignors]  WITH CHECK ADD  CONSTRAINT [FK_nConsignors_Enterprises1] FOREIGN KEY([RealID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[nConsignors] CHECK CONSTRAINT [FK_nConsignors_Enterprises1]
ALTER TABLE [dbo].[nConsignors]  WITH CHECK ADD  CONSTRAINT [FK_nConsignors_nSuppliers] FOREIGN KEY([nSupplierID])
REFERENCES [dbo].[nSuppliers] ([ID])

ALTER TABLE [dbo].[nConsignors] CHECK CONSTRAINT [FK_nConsignors_nSuppliers]

END

--新建表[dbo].[nPayers],
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[nPayers]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[nPayers](
	[ID] [varchar](50) NOT NULL,
	[nSupplierID] [varchar](50) NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[RealID] [varchar](50) NULL,
	[Methord] [int] NULL,
	[Bank] [nvarchar](200) NULL,
	[BankAddress] [nvarchar](200) NULL,
	[Account] [varchar](50) NULL,
	[SwiftCode] [varchar](50) NULL,
	[Currency] [int] NULL,
	[Contact] [varchar](50) NULL,
	[Tel] [varchar](50) NULL,
	[Mobile] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Creator] [varchar](50) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[Place] [varchar](50) NULL,
 CONSTRAINT [PK_nPayers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF
ALTER TABLE [dbo].[nPayers] ADD  CONSTRAINT [DF_nPayers_Creator_1]  DEFAULT ('SA01') FOR [Creator]
ALTER TABLE [dbo].[nPayers]  WITH CHECK ADD  CONSTRAINT [FK_nPayers_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[nPayers] CHECK CONSTRAINT [FK_nPayers_Enterprises]
ALTER TABLE [dbo].[nPayers]  WITH CHECK ADD  CONSTRAINT [FK_nPayers_Enterprises1] FOREIGN KEY([RealID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[nPayers] CHECK CONSTRAINT [FK_nPayers_Enterprises1]
ALTER TABLE [dbo].[nPayers]  WITH CHECK ADD  CONSTRAINT [FK_nPayers_nSuppliers] FOREIGN KEY([nSupplierID])
REFERENCES [dbo].[nSuppliers] ([ID])
ALTER TABLE [dbo].[nPayers] CHECK CONSTRAINT [FK_nPayers_nSuppliers]

END
--新建表[dbo].[nPayees],
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[nPayees]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON

CREATE TABLE [dbo].[nPayees](
	[ID] [varchar](50) NOT NULL,
	[nSupplierID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[RealID] [varchar](50) NULL,
	[Methord] [int] NULL,
	[Bank] [nvarchar](200) NULL,
	[BankAddress] [nvarchar](200) NULL,
	[Account] [varchar](50) NULL,
	[SwiftCode] [varchar](50) NULL,
	[Currency] [int] NULL,
	[Contact] [varchar](50) NULL,
	[Tel] [varchar](50) NULL,
	[Mobile] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Creator] [varchar](50) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[Place] [varchar](50) NULL,
 CONSTRAINT [PK_nPayees] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF

ALTER TABLE [dbo].[nPayees] ADD  CONSTRAINT [DF_nPayees_Creator_1]  DEFAULT ('SA01') FOR [Creator]

ALTER TABLE [dbo].[nPayees]  WITH CHECK ADD  CONSTRAINT [FK_nPayees_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])

ALTER TABLE [dbo].[nPayees] CHECK CONSTRAINT [FK_nPayees_Enterprises]

ALTER TABLE [dbo].[nPayees]  WITH CHECK ADD  CONSTRAINT [FK_nPayees_Enterprises1] FOREIGN KEY([RealID])
REFERENCES [dbo].[Enterprises] ([ID])

ALTER TABLE [dbo].[nPayees] CHECK CONSTRAINT [FK_nPayees_Enterprises1]

ALTER TABLE [dbo].[nPayees]  WITH CHECK ADD  CONSTRAINT [FK_nPayees_nSuppliers] FOREIGN KEY([nSupplierID])
REFERENCES [dbo].[nSuppliers] ([ID])
ALTER TABLE [dbo].[nPayees] CHECK CONSTRAINT [FK_nPayees_nSuppliers]

END
--新建表[dbo].[nSuppliers]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[nSuppliers]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON


CREATE TABLE [dbo].[nSuppliers](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[RealID] [varchar](50) NULL,
	[FromID] [varchar](50) NULL,
	[Conduct] [int] NOT NULL,
	[ChineseName] [nvarchar](150) NULL,
	[EnglishName] [nvarchar](150) NULL,
	[Grade] [int] NOT NULL,
	[Summary] [nvarchar](200) NULL,
	[Creator] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[CHNabbreviation] [nvarchar](150) NULL,
 CONSTRAINT [PK_nSuppliers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[nSuppliers]  WITH CHECK ADD  CONSTRAINT [FK_nSuppliers_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])


ALTER TABLE [dbo].[nSuppliers] CHECK CONSTRAINT [FK_nSuppliers_Enterprises]


ALTER TABLE [dbo].[nSuppliers]  WITH CHECK ADD  CONSTRAINT [FK_nSuppliers_Enterprises1] FOREIGN KEY([RealID])
REFERENCES [dbo].[Enterprises] ([ID])


ALTER TABLE [dbo].[nSuppliers] CHECK CONSTRAINT [FK_nSuppliers_Enterprises1]


ALTER TABLE [dbo].[nSuppliers]  WITH CHECK ADD  CONSTRAINT [FK_nSuppliers_Enterprises2] FOREIGN KEY([FromID])
REFERENCES [dbo].[Enterprises] ([ID])


ALTER TABLE [dbo].[nSuppliers] CHECK CONSTRAINT [FK_nSuppliers_Enterprises2]


END

--修改表[dbo].[Consignees]添加字段Place
if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='Place'))
begin
Alter TABLE [dbo].[Consignees] add Place varchar(50) null;
end;
--修改表[dbo].[Consignors]添加字段Place
if (not exists (select * from syscolumns where ID=object_id('Consignors') and name='Place'))
begin
Alter TABLE [dbo].[Consignors] add Place varchar(50) null;
end;

--[dbo].[WsClients]添加性质（上线后发现漏了修改）
if (not exists (select * from syscolumns where ID=object_id('WsClients') and name='Nature'))
begin
Alter TABLE [dbo].[WsClients] add Nature int null ;
end;

if ( exists (select * from syscolumns where ID=object_id('WsClients') and name='Nature'))
begin
update [dbo].[WsClients] set Nature=1;
Alter TABLE [dbo].[WsClients] alter column Nature int not null ;
end;


--把Origin改为Place
--if (exists (select * from syscolumns where ID=object_id('WsClients') and name='Origin'))
--begin
--EXEC sp_rename 'WsClients.[Origin]', 'Place', 'column' 
--end
----
--if (exists (select * from syscolumns where ID=object_id('WsSuppliers') and name='Origin'))
--begin
--EXEC sp_rename 'WsSuppliers.[Origin]', 'Place', 'column' 
--end

--if (exists (select * from syscolumns where ID=object_id('Clients') and name='Origin'))
--begin
--EXEC sp_rename 'Clients.[Origin]', 'Place', 'column' 
--end


--if (exists (select * from syscolumns where ID=object_id('Suppliers') and name='Origin'))
--begin
--EXEC sp_rename 'Suppliers.[Origin]', 'Place', 'column' 
--end
--Origin没加

if(not exists(select * from syscolumns where ID=object_id('Suppliers')and name='Place'))
begin
alter table [dbo].[Suppliers] add Place varchar(50) null
end

if(not exists(select * from syscolumns where ID=object_id('Clients')and name='Place'))
begin
alter table [dbo].[Clients] add Place varchar(50) null
end
GO
if(not exists(select * from syscolumns where ID=object_id('WsSuppliers')and name='Place'))
begin
alter table [dbo].[WsSuppliers] add Place varchar(50) null
end
if(not exists(select * from syscolumns where ID=object_id('WsClients')and name='Place'))
begin
alter table [dbo].[WsClients] add Place varchar(50) null
end
--企业表添加字段Place

if(not exists(select * from syscolumns where ID=object_id('Enterprises')and name='Place'))
begin
alter table [dbo].[Enterprises] add Place varchar(50) null
end



---------------------------------------------------------------------------------------------------;
--修改视图[dbo].[EnterprisesTopView],加Place
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[EnterprisesTopView]'))
DROP VIEW [dbo].[EnterprisesTopView]
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[EnterprisesTopView]
AS
SELECT   ID, Name, AdminCode, Status, Corporation, RegAddress, Uscc, Place,District
FROM      dbo.Enterprises

GO



--修改视图[dbo].[ConsigneesTopView]加Place
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsigneesTopView]') AND type IN (N'V'))
BEGIN
EXEC('
ALTER VIEW [dbo].[ConsigneesTopView]
AS
SELECT   ID, EnterpriseID, DyjCode, District, Address, Postzip, Status, Name, Tel, Mobile, Email, Title, Place
FROM      dbo.Consignees
')
END
GO

--新建视图[dbo].[wsnSupplierConsignor]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[wsnSupplierConsignor]'))
DROP VIEW [dbo].wsnSupplierConsignor
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[wsnSupplierConsignor]
AS
SELECT   ncngor.ID, ep1.ID AS OwnID, ep1.Name AS OwnName, ep2.ID AS RealEnterpriseID, ep2.Name AS RealEnterpriseName, 
                nspplier.Grade AS nGrade, ncngor.Title, ncngor.Postzip, ncngor.Address, ncngor.Place, ncngor.Contact, ncngor.Tel, 
                ncngor.Mobile, ncngor.Email, ncngor.IsDefault, ncngor.Status, client.EnterCode, client.Grade AS ClientGrade, 
                nspplier.ID AS nSupplierID
FROM      dbo.nSuppliers AS nspplier INNER JOIN
                dbo.Enterprises AS ep1 ON nspplier.EnterpriseID = ep1.ID INNER JOIN
                dbo.nConsignors AS ncngor ON nspplier.ID = ncngor.nSupplierID LEFT OUTER JOIN
                dbo.Enterprises AS ep2 ON ncngor.RealID = ep2.ID INNER JOIN
                dbo.WsClients AS client ON ep1.ID = client.ID

GO

GO
--新建视图[dbo].[wsnSupplierPayeesTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[wsnSupplierPayeesTopView]'))
DROP VIEW [dbo].[wsnSupplierPayeesTopView]
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[wsnSupplierPayeesTopView]
AS
SELECT   npayee.ID, ep1.ID AS OwnID, ep1.Name AS OwnName, ep2.ID AS RealEnterpriseID, ep2.Name AS RealEnterpriseName, 
                nspplier.Grade AS nGrade, npayee.Methord, npayee.Bank, npayee.BankAddress, npayee.Account, npayee.SwiftCode, 
                npayee.Currency, npayee.Contact, npayee.Tel, npayee.Mobile, npayee.Email, npayee.Status, client.EnterCode, 
                client.Grade AS ClientGrade, npayee.IsDefault, npayee.Place, nspplier.ID AS nSupplierID
FROM      dbo.nSuppliers AS nspplier INNER JOIN
                dbo.Enterprises AS ep1 ON nspplier.EnterpriseID = ep1.ID INNER JOIN
                dbo.nPayees AS npayee ON nspplier.ID = npayee.nSupplierID LEFT OUTER JOIN
                dbo.Enterprises AS ep2 ON npayee.RealID = ep2.ID INNER JOIN
                dbo.WsClients AS client ON ep1.ID = client.ID


GO

--新建视图[dbo].[wsnSuppliersTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[wsnSuppliersTopView]'))
DROP VIEW [dbo].[wsnSuppliersTopView]
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[wsnSuppliersTopView]
AS
SELECT   nspplier.ID, ep1.ID AS OwnID, ep1.Name AS OwnName, ep2.ID AS RealEnterpriseID, ep2.Name AS RealEnterpriseName, 
                nspplier.Grade AS nGrade, ep1.Place, client.EnterCode, client.Grade AS ClientGrade, nspplier.Status, ep2.Corporation, 
                ep2.Uscc, ep2.RegAddress,nspplier.ChineseName,nspplier.EnglishName,nspplier.CHNabbreviation
FROM      dbo.nSuppliers AS nspplier INNER JOIN
                dbo.Enterprises AS ep1 ON nspplier.EnterpriseID = ep1.ID LEFT OUTER JOIN
                dbo.Enterprises AS ep2 ON nspplier.RealID = ep2.ID INNER JOIN
                dbo.WsClients AS client ON ep1.ID = client.ID

GO

--新建视图[dbo].[wsPayeesTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[wsPayeesTopView]'))
DROP VIEW [dbo].[wsPayeesTopView]
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[wsPayeesTopView]
AS
SELECT   pyes.ID, ep1.ID AS EnterpriseID, ep1.Name AS EnterpriseName, ep2.ID AS RealEnterpriseID, 
                ep2.Name AS RealEnterpriseName, pyes.Methord, pyes.Bank, pyes.BankAddress, pyes.Account, pyes.SwiftCode, 
                pyes.Currency, pyes.Contact, pyes.Tel, pyes.Mobile, pyes.Email, pyes.Status,pyes.Place AS Place
FROM      dbo.Payees AS pyes INNER JOIN
                dbo.Enterprises AS ep1 ON pyes.EnterpriseID = ep1.ID LEFT OUTER JOIN
                dbo.Enterprises AS ep2 ON pyes.RealID = ep2.ID

GO


--新建视图[dbo].[wsPayersTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[wsPayersTopView]'))
DROP VIEW [dbo].[wsPayersTopView]
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[wsPayersTopView]
AS
SELECT   pyers.ID, ep1.ID AS EnterpriseID, ep1.Name AS EnterpriseName, ep2.ID AS RealEnterpriseID, 
                ep2.Name AS RealEnterpriseName, pyers.Methord, pyers.Bank, pyers.BankAddress, pyers.Account, pyers.SwiftCode, 
                pyers.Currency, pyers.Contact, pyers.Tel, pyers.Mobile, pyers.Email, pyers.Status, pyers.Place
FROM      dbo.Payers AS pyers INNER JOIN
                dbo.Enterprises AS ep1 ON pyers.EnterpriseID = ep1.ID LEFT OUTER JOIN
                dbo.Enterprises AS ep2 ON pyers.RealID = ep2.ID

GO

--[dbo].[wsPayersTopView]加了pyers.CreateDate, pyers.UpdateDate, pyers.Creator(上线后，董建发现和测试库相比少这3个)
use PvbCrm;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'wsPayersTopView') AND type IN (N'V'))
BEGIN
EXEC('
ALTER VIEW [dbo].[wsPayersTopView]
AS
SELECT   pyers.ID, ep1.ID AS EnterpriseID, ep1.Name AS EnterpriseName, ep2.ID AS RealEnterpriseID, 
                ep2.Name AS RealEnterpriseName, pyers.Methord, pyers.Bank, pyers.BankAddress, pyers.Account, pyers.SwiftCode, 
                pyers.Currency, pyers.Contact, pyers.Tel, pyers.Mobile, pyers.Email, pyers.Status, pyers.Place, pyers.CreateDate, 
                pyers.UpdateDate, pyers.Creator
FROM      dbo.Payers AS pyers INNER JOIN
                dbo.Enterprises AS ep1 ON pyers.EnterpriseID = ep1.ID LEFT OUTER JOIN
                dbo.Enterprises AS ep2 ON pyers.RealID = ep2.ID
')
END

--新建表[dbo].[nContacts]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[nContacts]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[nContacts](
	[ID] [nchar](10) NOT NULL,
	[nSupplierID] [varchar](50) NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[RealID] [varchar](50) NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Tel] [varchar](50) NULL,
	[Mobile] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Fax] [varchar](50) NULL,
	[QQ] [varchar](50) NULL,
	[Summary] [nvarchar](300) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[Creator] [varchar](50) NOT NULL,
 CONSTRAINT [PK_nContacts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF

ALTER TABLE [dbo].[nContacts] ADD  CONSTRAINT [DF_nContacts_AdminID]  DEFAULT ('SA01') FOR [Creator]

ALTER TABLE [dbo].[nContacts]  WITH CHECK ADD  CONSTRAINT [FK_nContacts_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])

ALTER TABLE [dbo].[nContacts] CHECK CONSTRAINT [FK_nContacts_Enterprises]

ALTER TABLE [dbo].[nContacts]  WITH CHECK ADD  CONSTRAINT [FK_nContacts_Enterprises1] FOREIGN KEY([RealID])
REFERENCES [dbo].[Enterprises] ([ID])

ALTER TABLE [dbo].[nContacts] CHECK CONSTRAINT [FK_nContacts_Enterprises1]

ALTER TABLE [dbo].[nContacts]  WITH CHECK ADD  CONSTRAINT [FK_nContacts_nSuppliers] FOREIGN KEY([nSupplierID])
REFERENCES [dbo].[nSuppliers] ([ID])

ALTER TABLE [dbo].[nContacts] CHECK CONSTRAINT [FK_nContacts_nSuppliers]


END
--新建视图[dbo].[wsnSupplierContactsTopView]

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[wsnSupplierContactsTopView]'))
DROP VIEW [dbo].[wsnSupplierContactsTopView]
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[wsnSupplierContactsTopView]
AS
SELECT   ncontact.ID, ncontact.nSupplierID, ep1.ID AS OwnID, ep1.Name AS OwnName, ep2.ID AS RealID, 
                ep2.Name AS RealName, ncontact.Name, ncontact.Tel, ncontact.Mobile, ncontact.Email, ncontact.Fax, ncontact.QQ, 
                ncontact.Summary, ncontact.Status
FROM      dbo.nSuppliers AS nspplier INNER JOIN
                dbo.Enterprises AS ep1 ON nspplier.EnterpriseID = ep1.ID INNER JOIN
                dbo.nContacts AS ncontact ON nspplier.ID = ncontact.nSupplierID LEFT OUTER JOIN
                dbo.Enterprises AS ep2 ON nspplier.RealID = ep1.ID

GO

GO
use PvbCrm;
--新建视图[dbo].[PayeesTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[PayeesTopView]'))
DROP VIEW [dbo].[PayeesTopView]
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[PayeesTopView]
AS
SELECT   ID, EnterpriseID, RealID, Methord, Bank, BankAddress, Account, Currency, SwiftCode, Contact as Name, Tel, Mobile, Email, 
                Status
FROM      dbo.Payees

GO

--[dbo].[PayersTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[PayersTopView]'))
DROP VIEW [dbo].[PayersTopView]
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[PayersTopView]
AS
SELECT   ID, EnterpriseID, RealID, Methord, Bank, BankAddress, Account, SwiftCode, Currency, Contact as Name, Tel, Mobile, Email, 
                Status
FROM      dbo.Payers

GO


use PvbCrm;
----------------------------------------------------------------------------------------------------------------------------------------------
--承运商[dbo].[Drivers]增加字段Mobile2,CustomsCode,CardCode,PortCode,LBPassword
if (not exists (select * from syscolumns where ID=object_id('Drivers') and name='Mobile2'))
begin
alter table [dbo].[Drivers] add Mobile2 varchar(50)  null;
end


if (not exists (select * from syscolumns where ID=object_id('Drivers') and name='CustomsCode'))
begin
alter table [dbo].[Drivers] add CustomsCode varchar(50)  null;
end

if (not exists (select * from syscolumns where ID=object_id('Drivers') and name='CardCode'))
begin
alter table [dbo].[Drivers] add CardCode varchar(50)  null;
end


if (not exists (select * from syscolumns where ID=object_id('Drivers') and name='PortCode'))
begin
alter table [dbo].[Drivers] add PortCode varchar(50)  null;
end

if (not exists (select * from syscolumns where ID=object_id('Drivers') and name='LBPassword'))
begin
alter table [dbo].[Drivers] add LBPassword varchar(50)  null;
end



--新建司机视图[dbo].[DriversTopView]

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[DriversTopView]'))
DROP VIEW [dbo].[DriversTopView]
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DriversTopView]
AS
SELECT   ID as ID, EnterpriseID as EnterpriseID, 
Name as Name, IDCard as IDCard, Mobile as Mobile, Mobile2 as Mobile2, Status as Status, CustomsCode as CustomsCode, CardCode as CardCode, PortCode as PortCode, LBPassword as LBPassword
FROM      dbo.Drivers

GO
use PvbCrm;
--[dbo].[Carriers]增加Type
if (not exists (select * from syscolumns where ID=object_id('Carriers') and name='Type'))
begin
Alter TABLE [dbo].[Carriers] add Type int null;
end
GO
--Type改为not null
if ( exists (select * from syscolumns where ID=object_id('Carriers') and name='Type'))
begin
update [dbo].[Carriers] set Type=1;
Alter TABLE [dbo].[Carriers] alter column Type int not null;
 
end
GO



--[dbo].[CarriersTopView]增加承运商类型和Place
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CarriersTopView]') AND type IN (N'V'))
BEGIN
EXEC('
ALTER VIEW [dbo].[CarriersTopView]
AS
SELECT     Carrier.ID, Carrier.Code, Carrier.Icon, Carrier.CreateDate, Carrier.UpdateDate, Carrier.Summary, Carrier.Creator, Carrier.Status, ep.Name, ep.District, ep.RegAddress, ep.Uscc, ep.Corporation, 
                      ep.Place, Carrier.Type
FROM         dbo.Carriers AS Carrier INNER JOIN
                      dbo.Enterprises AS ep ON Carrier.ID = ep.ID
')
END

GO


use PvbErm;
--select * from [dbo].[Menus] where Name='代仓储供应商' and Status=200
--
update [dbo].[Menus] set RightUrl='/Csrm/Crm/nSuppliers/List.aspx' where Name='代仓储供应商' and Status=200
GO
--select * from [dbo].[Menus] where Name='供应商受益人' and Status=200

update [dbo].[Menus] set RightUrl='/Csrm/Crm/nSuppliers/nPayees/List.aspx' where Name='供应商受益人' and Status=200
GO
--select * from [dbo].[Menus] where Name='供应商提货地址' and Status=200
update [dbo].[Menus] set RightUrl='/Csrm/Crm/nSuppliers/nConsignors/List.aspx' where Name='供应商提货地址' and Status=200
GO



--发现有问题后的补充

GO
if(not exists(select * from syscolumns where ID=object_id('WsClients')and name='Place'))
begin
alter table [dbo].[WsClients] add Place varchar(50) null
end

GO

CREATE VIEW [dbo].[wsnSuppliersTopView]
AS
SELECT   nspplier.ID, ep1.ID AS OwnID, ep1.Name AS OwnName, ep2.ID AS RealEnterpriseID, ep2.Name AS RealEnterpriseName, 
                nspplier.Grade AS nGrade, ep1.Place, client.EnterCode, client.Grade AS ClientGrade, nspplier.Status, ep2.Corporation, 
                ep2.Uscc, ep2.RegAddress,nspplier.ChineseName,nspplier.EnglishName,nspplier.CHNabbreviation
FROM      dbo.nSuppliers AS nspplier INNER JOIN
                dbo.Enterprises AS ep1 ON nspplier.EnterpriseID = ep1.ID LEFT OUTER JOIN
                dbo.Enterprises AS ep2 ON nspplier.RealID = ep2.ID INNER JOIN
                dbo.WsClients AS client ON ep1.ID = client.ID 