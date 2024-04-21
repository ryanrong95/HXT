USE [PvbCrm]
GO

/****** Object:  Table [dbo].[WsClients]    Script Date: 2019/8/20 星期二 10:16:14 ******/
--新增表WsClients-带仓储客户
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[WsClients]') AND type in (N'U'))
begin
SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[WsClients](
	[ID] [varchar](50) NOT NULL,
	[Grade] [int] NOT NULL,
	[Vip] [bit] NOT NULL,
	[EnterCode] [varchar](50) NULL,
	[CustomsCode] [varchar](150) NULL,
	[Status] [int] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[Summary] [varchar](500) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_WsClients] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF

ALTER TABLE [dbo].[WsClients]  WITH CHECK ADD  CONSTRAINT [FK_WsClients_Enterprises] FOREIGN KEY([ID])
REFERENCES [dbo].[Enterprises] ([ID])

ALTER TABLE [dbo].[WsClients] CHECK CONSTRAINT [FK_WsClients_Enterprises]

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'等级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'Grade'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否Vip' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'Vip'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入仓号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'EnterCode'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'Status'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'Summary'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'CreateDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'UpdateDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'代仓储客户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients'

end;

--WsClients索引
if(not exists(select * from sysindexes where id=object_id('WsClients') and name='Index_Grade_Vip'))
BEGIN
CREATE NONCLUSTERED INDEX [Index_Grade_Vip] ON [dbo].[WsClients]
(
	[Grade] ASC,
	[Vip] ASC
)
INCLUDE ( 	[ID],
	[EnterCode],
	[CustomsCode],
	[Status],
	[AdminID],
	[Summary],
	[CreateDate],
	[UpdateDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

end
GO
if(not exists(select * from sysindexes where id=object_id('WsClients') and name='Index_Grade_Vip_Status'))
BEGIN
CREATE NONCLUSTERED INDEX [Index_Grade_Vip_Status] ON [dbo].[WsClients]
(
	[Grade] ASC,
	[Vip] ASC,
	[Status] ASC
)
INCLUDE ( 	[ID],
	[EnterCode],
	[CustomsCode],
	[Summary],
	[CreateDate],
	[UpdateDate],
	[AdminID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO
if(not exists(select * from sysindexes where id=object_id('WsClients') and name='Index_Grade_Vip_Status_AdminID'))
BEGIN
CREATE NONCLUSTERED INDEX [Index_Grade_Vip_Status_AdminID] ON [dbo].[WsClients]
(
	[Grade] ASC,
	[Vip] ASC,
	[Status] ASC,
	[AdminID] ASC
)
INCLUDE ( 	[ID],
	[EnterCode],
	[CustomsCode],
	[Summary],
	[CreateDate],
	[UpdateDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO
--新增表WsSuppliers-带仓储供应商
if not exists(select * from sys.objects where object_id=OBJECT_ID(N'[dbo].[WsSuppliers]') AND type in (N'U'))

begin
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[WsSuppliers](
	[ID] [varchar](50) NOT NULL,
	[Grade] [int] NOT NULL,
	
	[Status] [int] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[Summary] [varchar](500) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[ChineseName] [nvarchar](150) NULL,
	[EnglishName] [nvarchar](150) NULL,
 CONSTRAINT [PK_WsSuppliers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'等级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'Grade'


EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'Status'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'Summary'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'CreateDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'UpdateDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'代仓储供应商' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers'

end

Go
--WsSuppliers索引

if(not exists(select * from sysindexes where id=object_id('WsSuppliers') and name='Index_Grade_Status_AdminID'))
BEGIN
CREATE NONCLUSTERED INDEX [Index_Grade_Status_AdminID] ON [dbo].[WsSuppliers]
(
	[Grade] ASC,
	[Status] ASC,
	[AdminID] ASC
)
INCLUDE ( 	[ID],
	[Summary],
	[CreateDate],
	[UpdateDate],
	[ChineseName],
	[EnglishName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
end
GO
if(not exists(select * from sysindexes where id=object_id('WsSuppliers') and name='Index_Grade_Status'))
BEGIN
CREATE NONCLUSTERED INDEX [Index_Grade_Status] ON [dbo].[WsSuppliers]
(
	[Grade] ASC,
	[Status] ASC
)
INCLUDE ( 	[ID],
	[AdminID],
	[Summary],
	[CreateDate],
	[UpdateDate],
	[ChineseName],
	[EnglishName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO
--新增表File
if not exists(select * from sys.objects where object_id=OBJECT_ID(N'[dbo].[Files]') AND type in (N'U'))
begin
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON

CREATE TABLE [dbo].[Files](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[Type] [int] NULL,
	[Url] [varchar](200) NULL,
	[CreateDate] [datetime] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF

ALTER TABLE [dbo].[Files]  WITH CHECK ADD  CONSTRAINT [FK_Files_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])

ALTER TABLE [dbo].[Files] CHECK CONSTRAINT [FK_Files_Enterprises]

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'企业ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Files', @level2type=N'COLUMN',@level2name=N'EnterpriseID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'营业执照' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Files', @level2type=N'COLUMN',@level2name=N'Type'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Files', @level2type=N'COLUMN',@level2name=N'CreateDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'正常、删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Files', @level2type=N'COLUMN',@level2name=N'Status'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Files'

end

GO


--Enterprises表增加Corporation(法人).注册地址、统一社会信用代码
if (not exists (select * from syscolumns where ID=object_id('Enterprises') and name='Corporation'))
begin
Alter TABLE [dbo].[Enterprises] ADD Corporation nvarchar(50) null ;
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公司法人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Enterprises', @level2type=N'COLUMN',@level2name=N'Corporation'
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Enterprises') and name='RegAddress'))
begin
Alter TABLE [dbo].[Enterprises] ADD RegAddress nvarchar(150) null ;
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'注册地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Enterprises', @level2type=N'COLUMN',@level2name=N'RegAddress'
end;

GO
if (not exists (select * from syscolumns where ID=object_id('Enterprises') and name='Uscc'))
begin
Alter TABLE [dbo].[Enterprises] ADD Uscc nvarchar(150) null ;
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'统一社会信用代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Enterprises', @level2type=N'COLUMN',@level2name=N'Uscc'
End

GO

--到货地址、发票、联系人、受益人、
if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='FromType'))
begin
Alter TABLE [dbo].[Consignees] ADD FromType int not null default 1;
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Contacts') and name='FromType'))
begin
Alter TABLE [dbo].[Contacts] ADD FromType int not null default 1;
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='FromType'))
begin
Alter TABLE [dbo].[Invoices] ADD FromType int not null default 1;
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Beneficiaries') and name='FromType'))
begin
Alter TABLE [dbo].[Beneficiaries] ADD FromType int not null default 1;
end;


--视图修改
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[BeneficiariesTopView]'))
DROP VIEW [dbo].[BeneficiariesTopView]
GO
CREATE VIEW [dbo].[BeneficiariesTopView]
AS
SELECT   ID, EnterpriseID, InvoiceType, RealName, RealID, Bank, BankAddress, Account, SwiftCode, Methord, Currency, District, 
                Name, Tel, Mobile, Email, Status
FROM      dbo.Beneficiaries
WHERE   (FromType = 1)
GO

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[ConsigneesTopView]'))
DROP VIEW [dbo].[ConsigneesTopView]
GO
CREATE VIEW [dbo].[ConsigneesTopView]
AS
SELECT   ID, EnterpriseID, DyjCode, District, Address, Postzip, Status, Name, Tel, Mobile, Email
FROM      dbo.Consignees
WHERE   (FromType = 1)
GO

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[ContactsTopView]'))
DROP VIEW [dbo].[ContactsTopView]
GO
CREATE VIEW [dbo].[ContactsTopView]
AS
SELECT   ID, EnterpriseID, Type, Name, Tel, Mobile, Email, Status
FROM      dbo.Contacts
WHERE   (FromType = 1)
GO


--库房
--if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='FatherID'))
--begin
--Alter TABLE [dbo].[WareHouses] ADD FatherID varchar(50) null;
--end;

--到货地址
if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='Title'))
begin
Alter TABLE [dbo].[Consignees] ADD Title nvarchar(50) null;
end;


--客户Vip类型bit变更为int ,null
if EXISTS(select * from  syscolumns where id=object_id('Clients')
and name='Vip')
BEGIN
if(exists(select * from sysindexes where id=object_id('Clients') and name='Index_Status'))
drop index Index_Status on Clients;
alter table Clients alter column Vip int not null
END

GO
if(not exists(select * from sysindexes where id=object_id('Clients') and name='Index_Status'))
BEGIN
CREATE NONCLUSTERED INDEX [Index_Status] ON [dbo].[Clients]
(
	[Status] ASC
)
INCLUDE ( 	[ID],
	[Nature],
	[Grade],
	[AreaType],
	[DyjCode],
	[TaxperNumber],
	[Vip]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


end
GO
--客户视图ClientsTopView更新
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[ClientsTopView]'))
DROP VIEW [dbo].[ClientsTopView]
GO
CREATE VIEW [dbo].[ClientsTopView]
AS
SELECT   dbo.Enterprises.Name, dbo.Enterprises.District, dbo.Clients.ID, dbo.Clients.Nature, dbo.Clients.Grade, 
                dbo.Clients.DyjCode, dbo.Clients.TaxperNumber, dbo.Clients.Vip, dbo.Clients.Status, dbo.Clients.AdminID, 
                dbo.Clients.CreateDate, dbo.Clients.UpdateDate, dbo.Clients.AreaType
FROM      dbo.Clients INNER JOIN
                dbo.Enterprises ON dbo.Clients.ID = dbo.Enterprises.ID
GO

--Vip原来是Bool类型，是Vip的值变为-1
update Clients set Vip=-1 where Vip=1