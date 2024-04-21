
use  PvbCrm;

--*******************************企业信息[dbo].[Enterprises] Corporation RegAddress    Uscc******************************
--********Corporation*******
if (not exists (select * from syscolumns where ID=object_id('Enterprises') and name='Corporation'))
begin
Alter TABLE [dbo].[Enterprises] add Corporation nvarchar(50) null;
end
GO
---********RegAddress******
if (not exists (select * from syscolumns where ID=object_id('Enterprises') and name='RegAddress'))
begin
Alter TABLE [dbo].[Enterprises] add RegAddress nvarchar(150) null;
end

----********Uscc******
if (not exists (select * from syscolumns where ID=object_id('Enterprises') and name='Uscc'))
begin
Alter TABLE [dbo].[Enterprises] add Uscc varchar(150) null;
end

GO

--*******************************[dbo].[Consignees]--添加了PlateCode,Province,City,Land********************************
--*************PlateCode*********
if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='PlateCode'))
begin
Alter TABLE [dbo].[Consignees] add PlateCode nvarchar(50) null;
end
GO
--*************Province*********
if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='Province'))
begin
Alter TABLE [dbo].[Consignees] add Province nvarchar(50) null;
end
GO
--*************City*********
if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='City'))
begin
Alter TABLE [dbo].[Consignees] add City nvarchar(50) null;
end

GO
--*************Land*********
if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='Land'))
begin
Alter TABLE [dbo].[Consignees] add Land nvarchar(50) null;
end
GO
--************************************************[dbo].[Contacts]--添加了Fax*******************************************

if (not exists (select * from syscolumns where ID=object_id('Contacts') and name='Fax'))
begin
Alter TABLE [dbo].[Contacts] add Fax varchar(50) null;
end

GO


--**************************[dbo].[Invoices]--添加了Province,City,Land,DiliveryType,CompanyTel******************************


--*************Province*********
if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='Province'))
begin
Alter TABLE [dbo].[Invoices] add Province nvarchar(50) null;
end
GO
--*************City*********
if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='City'))
begin
Alter TABLE [dbo].[Invoices] add City nvarchar(50) null;
end


--*************Land*********
if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='Land'))
begin
Alter TABLE [dbo].[Invoices] add Land nvarchar(50) null;
end

GO
--*************DeliveryType*********
if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='DeliveryType'))
begin
Alter TABLE [dbo].[Invoices] add DeliveryType int not null default 0;
end

GO
--*************CompanyTel*********
if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='CompanyTel'))
begin
Alter TABLE [dbo].[Invoices] add CompanyTel varchar(50) null;
end



--*******************************************************[dbo].[Invoices]发票添加企业电话******************************


--***********************************-************[dbo].[ContactsTopView]添加Fax
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[ContactsTopView]'))
DROP VIEW [dbo].[ContactsTopView]
GO

CREATE VIEW [dbo].[ContactsTopView]
AS
SELECT   ID, EnterpriseID, Type, Name, Tel, Mobile, Email, Status, Fax
FROM      dbo.Contacts

GO
--**************************************修改发票视图,添加企业信息中的纳税人识别号和企业电话******************************

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[InvoicesTopView]'))
DROP VIEW [dbo].[InvoicesTopView]
GO
Create VIEW [dbo].[InvoicesTopView]
AS
SELECT   dbo.Invoices.ID, dbo.Invoices.Type, dbo.Invoices.Bank, dbo.Invoices.BankAddress, dbo.Invoices.Account, 
                dbo.Invoices.TaxperNumber, dbo.Invoices.Name, dbo.Invoices.Tel, dbo.Invoices.Mobile, dbo.Invoices.Email, 
                dbo.Invoices.District, dbo.Invoices.Postzip, dbo.Invoices.Status, dbo.Invoices.Address, dbo.Invoices.EnterpriseID, 
                dbo.Invoices.CreateDate, dbo.Invoices.UpdateDate, dbo.Invoices.AdminID, dbo.Invoices.DeliveryType, 
                dbo.Enterprises.Uscc, dbo.Invoices.CompanyTel
FROM      dbo.Invoices INNER JOIN
                dbo.Enterprises ON dbo.Invoices.EnterpriseID = dbo.Enterprises.ID

GO

--新建 [dbo].[MapsBEnter]表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[MapsBEnter]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[MapsBEnter](
	[ID] [varchar](50) NOT NULL,
	[Bussiness] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[SubID] [varchar](50) NOT NULL,
	[CtreatorID] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_MapsBEnter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'md5(EnterpriseID+SubID+Bussiness+Type)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter', @level2type=N'COLUMN',@level2name=N'ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'业务:贸易（Trading） 10,
代报关服务（CustombrokerServicing） 20, 
代仓储服务（WarehouseServicing ）30, 
代理线服务（AagentServicing） 40' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter', @level2type=N'COLUMN',@level2name=N'Bussiness'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Supplier\Beneficiary\Consigee\Contact\Invoice' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter', @level2type=N'COLUMN',@level2name=N'Type'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'EnterpriseID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter', @level2type=N'COLUMN',@level2name=N'EnterpriseID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Supplier\Beneficiary\Consigee\Contact\Invoice等ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter', @level2type=N'COLUMN',@level2name=N'SubID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'添加人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter', @level2type=N'COLUMN',@level2name=N'CtreatorID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'默认：报价人、询价人等（在一个业务中应该只有一个默认人员）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter', @level2type=N'COLUMN',@level2name=N'IsDefault'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'企业的业务下关系' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsBEnter'

END

--删除表[dbo].[Files]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Files]') AND type in (N'U'))
BEGIN
drop table [dbo].[Files];
END

GO
--新建[dbo].[FilesDescription]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[FilesDescription]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FilesDescription](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Type] [int] NULL,
	[FileFormat] [varchar](50) NOT NULL,
	[Url] [varchar](200) NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[Summary] [nvarchar](300) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[PaysID] [varchar](50) NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[FilesDescription]  WITH CHECK ADD  CONSTRAINT [FK_Files_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[FilesDescription] CHECK CONSTRAINT [FK_Files_Enterprises]
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'企业ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'EnterpriseID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'营业执照' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'Type'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'正常、删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'Status'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'财务ID（应收ID、应付ID、实收ID、实付ID）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'PaysID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription'

END


--新建[dbo].[MapsTracker]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[MapsTracker]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[MapsTracker](
	[ID] [varchar](50) NOT NULL,
	[Bussiness] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[RealID] [varchar](50) NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_MapsTracker] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'md5(AdminID+RealID+Bussiness+Type)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsTracker', @level2type=N'COLUMN',@level2name=N'ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'贸易（Trading） 10,
代报关服务（CustombrokerServicing） 20, 
代仓储服务（WarehouseServicing ）30, 
代理线服务（AagentServicing） 40' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsTracker', @level2type=N'COLUMN',@level2name=N'Bussiness'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'80网站客户  10供应商 20客户 71来源 72送达地址 73联系人 74发票 75 芯达通业务员  76 跟单员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsTracker', @level2type=N'COLUMN',@level2name=N'Type'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'管理员ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsTracker', @level2type=N'COLUMN',@level2name=N'AdminID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'如果要求的(BeneficiaryID,ContactID) Suppliers ,Client' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsTracker', @level2type=N'COLUMN',@level2name=N'RealID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'默认：报价人、询价人等（在一个业务中应该只有一个默认人员）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsTracker', @level2type=N'COLUMN',@level2name=N'IsDefault'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'业务员与其他主体关系' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MapsTracker'
END

--新建[dbo].[SiteUsersXdt]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[SiteUsersXdt]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[SiteUsersXdt](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[IsMain] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_SiteUserXdt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[SiteUsersXdt]  WITH CHECK ADD  CONSTRAINT [FK_SiteUsersXdt_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[SiteUsersXdt] CHECK CONSTRAINT [FK_SiteUsersXdt_Enterprises]
ALTER TABLE [dbo].[SiteUsersXdt]  WITH CHECK ADD  CONSTRAINT [FK_SiteUsersXdt_SiteUsers] FOREIGN KEY([ID])
REFERENCES [dbo].[SiteUsers] ([ID])
ALTER TABLE [dbo].[SiteUsersXdt] CHECK CONSTRAINT [FK_SiteUsersXdt_SiteUsers]
END


--新建[dbo].[SiteUsers]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[SiteUsers]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[SiteUsers](
	[ID] [varchar](50) NOT NULL,
	[UserName] [nvarchar](150) NOT NULL,
	[RealName] [varchar](50) NULL,
	[Password] [varchar](50) NOT NULL,
	[Mobile] [varchar](50) NULL,
	[Email] [nvarchar](150) NULL,
	[QQ] [varchar](50) NULL,
	[Wx] [varchar](50) NULL,
	[Summary] [nvarchar](300) NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Sites] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers', @level2type=N'COLUMN',@level2name=N'ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers', @level2type=N'COLUMN',@level2name=N'UserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'手机' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers', @level2type=N'COLUMN',@level2name=N'Mobile'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers', @level2type=N'COLUMN',@level2name=N'Email'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'QQ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers', @level2type=N'COLUMN',@level2name=N'QQ'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'微信' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers', @level2type=N'COLUMN',@level2name=N'Wx'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外部(网站)用户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers'
EXEC sys.sp_addextendedproperty @name=N'ObjectName', @value=N'SiteUser' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SiteUsers'
END
--新建[dbo].[WsClients]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[WsClients]') AND type in (N'U'))
BEGIN

SET ANSI_NULLS ON
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
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入仓号：代仓储客户编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'EnterCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'海关编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'CustomsCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'Status'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'Summary'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients', @level2type=N'COLUMN',@level2name=N'UpdateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'代仓储客户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsClients'

END
--新建[dbo].[WsSuppliers]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[WsSuppliers]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[WsSuppliers](
	[ID] [varchar](50) NOT NULL,
	[EnglishName] [nvarchar](150) NULL,
	[ChineseName] [nvarchar](150) NULL,
	[Grade] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[Summary] [varchar](500) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_WsSuppliers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[WsSuppliers]  WITH CHECK ADD  CONSTRAINT [FK_WsSuppliers_Enterprises] FOREIGN KEY([ID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[WsSuppliers] CHECK CONSTRAINT [FK_WsSuppliers_Enterprises]
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'等级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'Grade'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'Status'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'Summary'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers', @level2type=N'COLUMN',@level2name=N'UpdateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'代仓储供应商' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsSuppliers'
END


--新建[dbo].[Carriers]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Carriers]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[Carriers](
	[ID] [varchar](50) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Icon] [varchar](200) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Summary] [varchar](500) NULL,
	[Creator] [varchar](50) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Carriers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[Carriers]  WITH CHECK ADD  CONSTRAINT [FK_Carriers_Enterprises] FOREIGN KEY([ID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[Carriers] CHECK CONSTRAINT [FK_Carriers_Enterprises]
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'企业ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Carriers', @level2type=N'COLUMN',@level2name=N'ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'承运商编号（做到全局唯一的判断）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Carriers', @level2type=N'COLUMN',@level2name=N'Code'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'快递的图片（仓百度上copy就可以）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Carriers', @level2type=N'COLUMN',@level2name=N'Icon'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Carriers', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Carriers', @level2type=N'COLUMN',@level2name=N'UpdateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'摘要（备注）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Carriers', @level2type=N'COLUMN',@level2name=N'Summary'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'正常：Normal,黑名单：black,停用：closed,删除：Deleted' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Carriers', @level2type=N'COLUMN',@level2name=N'Status'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'承运商(暂时做非企业化管理)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Carriers'
END


--新建[dbo].[Drivers]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Drivers]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[Drivers](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[IDCard] [varchar](50) NULL,
	[Mobile] [varchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Creator] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Drivers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[Drivers]  WITH CHECK ADD  CONSTRAINT [FK_Drivers_Carries] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[Drivers] CHECK CONSTRAINT [FK_Drivers_Carries]
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'(EnterpriseID,Name,IDCard,Mobile).MD5()' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Drivers', @level2type=N'COLUMN',@level2name=N'ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Drivers', @level2type=N'COLUMN',@level2name=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身份证号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Drivers', @level2type=N'COLUMN',@level2name=N'IDCard'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'移动电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Drivers', @level2type=N'COLUMN',@level2name=N'Mobile'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Drivers', @level2type=N'COLUMN',@level2name=N'Status'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Drivers', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Drivers', @level2type=N'COLUMN',@level2name=N'UpdateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'司机' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Drivers'
END
--新建[dbo].[Transports]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Transports]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[Transports](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[CarNumber1] [varchar](50) NOT NULL,
	[CarNumber2] [varchar](50) NULL,
	[Weight] [nvarchar](50) NULL,
	[Creator] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Transports] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[Transports]  WITH CHECK ADD  CONSTRAINT [FK_Transports_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[Transports] CHECK CONSTRAINT [FK_Transports_Enterprises]
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'(EnterpriseID,TypeCarNumber).MD5()' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transports', @level2type=N'COLUMN',@level2name=N'ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'车辆类型:面包车、厢式货车、七座小客车,3吨车,5吨车,8吨车,10吨车' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transports', @level2type=N'COLUMN',@level2name=N'Type'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'车牌号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transports', @level2type=N'COLUMN',@level2name=N'CarNumber1'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'临时车牌号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transports', @level2type=N'COLUMN',@level2name=N'CarNumber2'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'车重（KGS）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transports', @level2type=N'COLUMN',@level2name=N'Weight'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transports', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transports', @level2type=N'COLUMN',@level2name=N'UpdateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transports', @level2type=N'COLUMN',@level2name=N'Status'
END

--新建[dbo].[Contracts]

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Contracts]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[Contracts](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[ExchangeMode] [int] NOT NULL,
	[AgencyRate] [decimal](18, 4) NOT NULL,
	[MinAgencyFee] [decimal](18, 4) NOT NULL,
	[InvoiceType] [int] NOT NULL,
	[InvoiceTaxRate] [decimal](18, 4) NOT NULL,
	[Status] [int] NOT NULL,
	[Creator] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](400) NULL,
 CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[Contracts]  WITH CHECK ADD  CONSTRAINT [FK_Contracts_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[Contracts] CHECK CONSTRAINT [FK_Contracts_Enterprises]
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开始日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'StartDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'结束日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'EndDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'代理费率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'AgencyRate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最低代理费' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'MinAgencyFee'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发票对应税点' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'InvoiceTaxRate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'Status'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Admin' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'Creator'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'UpdateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'摘要备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'Summary'

END
--新建[dbo].[FilesDescription]

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[FilesDescription]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[FilesDescription](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Type] [int] NULL,
	[FileFormat] [varchar](50) NOT NULL,
	[Url] [varchar](200) NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[Summary] [nvarchar](300) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[PaysID] [varchar](50) NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[FilesDescription]  WITH CHECK ADD  CONSTRAINT [FK_Files_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[FilesDescription] CHECK CONSTRAINT [FK_Files_Enterprises]
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'企业ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'EnterpriseID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'营业执照' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'Type'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'正常、删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'Status'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'财务ID（应收ID、应付ID、实收ID、实付ID）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription', @level2type=N'COLUMN',@level2name=N'PaysID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FilesDescription'
END

--新建表[dbo].[Consignors]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Consignors]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[Consignors](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[DyjCode] [varchar](50) NOT NULL,
	[Postzip] [varchar](50) NOT NULL,
	[Province] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[Land] [nvarchar](50) NULL,
	[Address] [nvarchar](150) NOT NULL,
	[Status] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Tel] [varchar](50) NULL,
	[Mobile] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_Consignors_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[Consignors]  WITH CHECK ADD  CONSTRAINT [FK_Consignors_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])
ALTER TABLE [dbo].[Consignors] CHECK CONSTRAINT [FK_Consignors_Enterprises]
END
--视图新增[dbo].[CarriersTopView]

IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[CarriersTopView]'))
DROP VIEW [dbo].[CarriersTopView]
GO
CREATE VIEW [dbo].[CarriersTopView]
AS
SELECT   dbo.Carriers.ID, dbo.Carriers.Code, dbo.Carriers.Icon, dbo.Carriers.CreateDate, dbo.Carriers.UpdateDate, 
                dbo.Carriers.Summary, dbo.Carriers.Creator, dbo.Carriers.Status, dbo.Enterprises.Name, dbo.Enterprises.District, 
                dbo.Enterprises.RegAddress, dbo.Enterprises.Uscc, dbo.Enterprises.Corporation
FROM      dbo.Carriers INNER JOIN
                dbo.Enterprises ON dbo.Carriers.ID = dbo.Enterprises.ID

GO
--视图新增[dbo].[WarehousePlatesTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[WarehousePlatesTopView]'))
DROP VIEW [dbo].[WarehousePlatesTopView]
GO
CREATE VIEW [dbo].[WarehousePlatesTopView]
AS
SELECT   cnge.ID, cnge.Title, cnge.Address, cnge.PlateCode AS Code, cnge.Postzip, cnge.Status, ep.ID AS EnterpriseID, 
                ep.Name AS WarehouseName, wh.Address AS WarehouseAddress, wh.WsCode, wh.Status AS WarehouseStatus
FROM      dbo.WareHouses AS wh INNER JOIN
                dbo.Enterprises AS ep ON wh.ID = ep.ID INNER JOIN
                dbo.Consignees AS cnge ON wh.ID = cnge.EnterpriseID
GO
--视图新增[dbo].[WsSuppliersTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[WsSuppliersTopView]'))
DROP VIEW [dbo].[WsSuppliersTopView]
GO
CREATE VIEW [dbo].[WsSuppliersTopView]
AS
SELECT   dbo.Enterprises.Name, dbo.Enterprises.ID, dbo.Enterprises.AdminCode, dbo.Enterprises.Corporation, 
                dbo.Enterprises.RegAddress, dbo.Enterprises.Uscc, dbo.WsSuppliers.Grade, dbo.WsSuppliers.Status, 
                dbo.WsSuppliers.AdminID, dbo.WsSuppliers.CreateDate, dbo.WsSuppliers.UpdateDate, 
                dbo.WsSuppliers.ChineseName, dbo.WsSuppliers.EnglishName
FROM      dbo.Enterprises INNER JOIN
                dbo.WsSuppliers ON dbo.Enterprises.ID = dbo.WsSuppliers.ID

GO

--新建[dbo].[WareHouses]  ID, 新增：WsCode,Status,CreateDate,UpdateDate
--WsCode
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='WsCode'))
begin
Alter TABLE [dbo].[WareHouses] ADD WsCode varchar(50) not null;
end
GO
--Status
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='Status'))
begin
Alter TABLE [dbo].[WareHouses]  ADD [Status] int not null;
end

GO
--CreateDate
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='CreateDate'))
begin
Alter TABLE [dbo].[WareHouses]  ADD CreateDate datetime not null;
end

GO
--UpdateDate
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='UpdateDate'))
begin
Alter TABLE [dbo].[WareHouses]  ADD UpdateDate datetime not null;
end

GO
--删除Name
if (exists (select * from syscolumns where ID=object_id('WareHouses') and name='Name'))
begin
alter table [dbo].[WareHouses] drop column Name;
end
GO

declare  @a int
set @a=(SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE where constraint_name='FK_WareHouses_Enterprises')
if @a=0
begin
ALTER TABLE [dbo].[WareHouses]  WITH CHECK ADD  CONSTRAINT [FK_WareHouses_Enterprises] FOREIGN KEY([ID])
REFERENCES [dbo].[Enterprises] ([ID])
end

--视图新增[dbo].[WarehousesTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[WarehousesTopView]'))
DROP VIEW [dbo].[WarehousesTopView]
GO
CREATE VIEW [dbo].[WarehousesTopView]
AS
SELECT   dbo.WareHouses.ID, dbo.WareHouses.Grade, dbo.WareHouses.District, dbo.WareHouses.Address, 
                dbo.WareHouses.DyjCode, dbo.Enterprises.Name, dbo.Enterprises.AdminCode, dbo.Enterprises.RegAddress, 
                dbo.Enterprises.Corporation, dbo.Enterprises.Uscc, dbo.WareHouses.AdminID, dbo.WareHouses.CreateDate, 
                dbo.WareHouses.UpdateDate, dbo.WareHouses.Status, dbo.WareHouses.WsCode
FROM      dbo.WareHouses INNER JOIN
                dbo.Enterprises ON dbo.WareHouses.ID = dbo.Enterprises.ID

GO

--视图新增[dbo].[WsClientsTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[WsClientsTopView]'))
DROP VIEW [dbo].[WsClientsTopView]
GO
CREATE VIEW [dbo].[WsClientsTopView]
AS
SELECT   dbo.Enterprises.ID, dbo.Enterprises.Name, dbo.Enterprises.AdminCode, dbo.Enterprises.Corporation, 
                dbo.Enterprises.RegAddress, dbo.Enterprises.Uscc, dbo.WsClients.Grade, dbo.WsClients.Vip, 
                dbo.WsClients.EnterCode, dbo.WsClients.CustomsCode, dbo.WsClients.Status, dbo.WsClients.CreateDate, 
                dbo.WsClients.UpdateDate, dbo.WsClients.AdminID
FROM      dbo.Enterprises INNER JOIN
                dbo.WsClients ON dbo.Enterprises.ID = dbo.WsClients.ID
GO
--视图新增[dbo].[SubsWarehousesTopView]  -???
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[SubsWarehousesTopView]'))
DROP VIEW [dbo].[SubsWarehousesTopView]
GO
CREATE VIEW [dbo].[SubsWarehousesTopView]
AS
SELECT   dbo.Consignees.ID, dbo.Consignees.Title, dbo.WareHouses.ID AS EnterpriseID, dbo.Enterprises.Name, 
                dbo.WareHouses.WsCode, dbo.Consignees.Address, dbo.Consignees.Postzip, dbo.WareHouses.Grade, 
                dbo.WareHouses.DyjCode, dbo.WareHouses.District, dbo.Enterprises.Corporation, dbo.Enterprises.Uscc, 
                dbo.Enterprises.RegAddress, dbo.WareHouses.Status, dbo.Consignees.Status AS ConsinStatus
FROM      dbo.Enterprises INNER JOIN
                dbo.WareHouses ON dbo.Enterprises.ID = dbo.WareHouses.ID INNER JOIN
                dbo.Consignees ON dbo.Consignees.EnterpriseID = dbo.Enterprises.ID
GO

--[dbo].[SiteUsers]增加 RealName,Password,Summary;
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='RealName'))
begin
Alter TABLE [dbo].[SiteUsers] ADD [RealName] varchar(50) null;
end

GO

if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Password'))
begin
Alter TABLE [dbo].[SiteUsers] ADD [Password] varchar(50) not null;
end

GO
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Summary'))
begin
Alter TABLE [dbo].[SiteUsers] ADD [Summary] nvarchar(300) null;
end

GO

if (exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Summary'))
begin
Alter TABLE [dbo].[SiteUsers] alter column [Summary] nvarchar(300) null;
end

GO
--[dbo].[SiteUsers]删除字段SsoID，From UpdateDate
if (exists (select * from syscolumns where ID=object_id('SiteUsers') and name='SsoID'))
begin
Alter TABLE [dbo].[SiteUsers] drop column SsoID;
end
GO

if (exists (select * from syscolumns where ID=object_id('SiteUsers') and name='From'))
begin
Alter TABLE [dbo].[SiteUsers] drop column [From];
end
GO

if (exists (select * from syscolumns where ID=object_id('SiteUsers') and name='UpdateDate'))
begin
Alter TABLE [dbo].[SiteUsers] drop column UpdateDate;
end
GO
--视图新增[dbo].[SiteUserXdtTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[SiteUserXdtTopView]'))
DROP VIEW [dbo].[SiteUserXdtTopView]
GO
CREATE VIEW [dbo].[SiteUserXdtTopView]
AS
SELECT   dbo.SiteUsers.ID, dbo.SiteUsers.UserName, dbo.SiteUsers.RealName, dbo.SiteUsers.Password, dbo.SiteUsers.Mobile, 
                dbo.SiteUsers.Email, dbo.SiteUsers.QQ, dbo.SiteUsers.Wx, dbo.SiteUsers.Summary, dbo.SiteUsersXdt.EnterpriseID, 
                dbo.SiteUsersXdt.IsMain, dbo.SiteUsersXdt.Status
FROM      dbo.SiteUsers INNER JOIN
                dbo.SiteUsersXdt ON dbo.SiteUsers.ID = dbo.SiteUsersXdt.ID
GO
--视图新增[dbo].[PlateTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[PlateTopView]'))
DROP VIEW [dbo].[PlateTopView]
GO

CREATE VIEW [dbo].[PlateTopView]
AS
SELECT   ID, Title, Address, Postzip, Status AS ConsinStatus, EnterpriseID, PlateCode AS Code
FROM      dbo.Consignees AS csgn
WHERE   (EnterpriseID IN
                    (SELECT   ID
                     FROM      dbo.WareHouses))
 GO   
--视图新增[dbo].[MapsTrackerTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[MapsTrackerTopView]'))
DROP VIEW [dbo].[MapsTrackerTopView]
GO
CREATE VIEW [dbo].[MapsTrackerTopView]
AS
SELECT   ID, Bussiness, Type, AdminID, IsDefault, RealID
FROM      dbo.MapsTracker

 GO
--视图新增[dbo].[MapsBEnterTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[MapsBEnterTopView]'))
DROP VIEW [dbo].[MapsBEnterTopView]
GO

CREATE VIEW [dbo].[MapsBEnterTopView]
AS
SELECT   ID, Bussiness, Type, EnterpriseID, SubID, CtreatorID, CreateDate, IsDefault
FROM      dbo.MapsBEnter

 GO
--视图新增[dbo].[FilesDescriptionTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[FilesDescriptionTopView]'))
DROP VIEW [dbo].[FilesDescriptionTopView]
GO
CREATE VIEW [dbo].[FilesDescriptionTopView]
AS
SELECT     ID, EnterpriseID, Name, Type, FileFormat, Url, AdminID, Summary, CreateDate, Status, PaysID
FROM         dbo.FilesDescription

 GO
--修改视图[dbo].[InvoicesTopView] 新增字段Uscc,CompanyTel,RegAddress,CompanyName
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[InvoicesTopView]'))
DROP VIEW [dbo].[InvoicesTopView]
GO
CREATE VIEW [dbo].[InvoicesTopView]
AS
SELECT   dbo.Invoices.ID, dbo.Invoices.Type, dbo.Invoices.Bank, dbo.Invoices.BankAddress, dbo.Invoices.Account, 
                dbo.Invoices.TaxperNumber, dbo.Invoices.Name, dbo.Invoices.Tel, dbo.Invoices.Mobile, dbo.Invoices.Email, 
                dbo.Invoices.District, dbo.Invoices.Postzip, dbo.Invoices.Status, dbo.Invoices.Address, dbo.Invoices.EnterpriseID, 
                dbo.Invoices.CreateDate, dbo.Invoices.UpdateDate, dbo.Invoices.AdminID, dbo.Invoices.DeliveryType, 
                dbo.Enterprises.Uscc, dbo.Invoices.CompanyTel, dbo.Enterprises.RegAddress, 
                dbo.Enterprises.Name AS CompanyName
FROM      dbo.Invoices INNER JOIN
                dbo.Enterprises ON dbo.Invoices.EnterpriseID = dbo.Enterprises.ID


 GO
 --修改数据视图[dbo].[BeneficiariesTopView]  去掉  条件  WHERE   (FromType = 1)
 IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[BeneficiariesTopView]'))
DROP VIEW [dbo].[BeneficiariesTopView]
GO
CREATE VIEW [dbo].[BeneficiariesTopView]
AS
SELECT   ID, EnterpriseID, InvoiceType, RealName, RealID, Bank, BankAddress, Account, SwiftCode, Methord, Currency, District, 
                Name, Tel, Mobile, Email, Status
FROM      dbo.Beneficiaries




 GO

 --删除表[dbo].[Customsbrokers]
 IF  EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Customsbrokers]') AND type in (N'U'))
BEGIN
drop table [dbo].[Customsbrokers];
END

--[dbo].[Enterprises]  Uscc类型 由nvarchar(150)改为 varchar(150)
if (exists (select * from syscolumns where ID=object_id('Enterprises') and name='Uscc'))
begin
Alter TABLE [dbo].[Enterprises] alter column Uscc varchar(150) null;
end
GO




--********************************王辉*******************************************
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[FlowAccounts]') AND type in (N'U'))
begin
CREATE TABLE [dbo].[FlowAccounts](
	[ID] [varchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[Payer] [varchar](50) NOT NULL,
	[Payee] [varchar](50) NOT NULL,
	[Business] [nvarchar](50) NOT NULL,
	[Catalog] [nvarchar](50) NULL,
	[Subject] [nvarchar](50) NULL,
	[Currency] [int] NOT NULL,
	[Price] [decimal](18, 7) NOT NULL,
	[Currency1] [int] NOT NULL,
	[Price1] [decimal](18, 7) NOT NULL,
	[ERate1] [decimal](18, 7) NULL,
	[Bank] [nvarchar](50) NULL,
	[Account] [varchar](50) NULL,
	[FormCode] [varchar](50) NULL,
	[OrderID] [varchar](50) NULL,
	[WaybillID] [varchar](50) NULL,
	[AdminID] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[OriginIndex] [int] NULL,
	[ChangeIndex] [int] NULL,
	[OriginalDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_Balances] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[DebtTerms]') AND type in (N'U'))
begin
CREATE TABLE [dbo].[DebtTerms](
	[ID] [VARCHAR](50) NOT NULL,
	[Payer] [VARCHAR](50) NOT NULL,
	[Payee] [VARCHAR](50) NOT NULL,
	[Business] [NVARCHAR](50) NOT NULL,
	[Catalog] [NVARCHAR](50) NOT NULL,
	[SettlementType] [INT] NOT NULL,
	[Months] [INT] NOT NULL,
	[Days] [INT] NOT NULL,
	[CreateDate] [DATETIME] NOT NULL,
	[AdminID] [VARCHAR](50) NOT NULL,
	[ERateType] [INT] NOT NULL,
 CONSTRAINT [PK_DebtTerms] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

end

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[DebtTermsTopView]'))
DROP VIEW [dbo].[DebtTermsTopView]
GO

CREATE VIEW [dbo].[DebtTermsTopView]
AS
SELECT     Payer, Payee, Business, Catalog, SettlementType, Months, Days, ERateType
FROM         dbo.DebtTerms

GO


USE [PvbCrm]
GO

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[FlowAccountsTopView]'))
DROP VIEW [dbo].[FlowAccountsTopView]
GO

CREATE VIEW [dbo].[FlowAccountsTopView]
AS
SELECT     ID, Type, Payer, Payee, Business, Catalog, Subject, Currency, Price, Currency1, Price1, ERate1, Bank, FormCode, OrderID, WaybillID, AdminID, CreateDate, OriginIndex, ChangeIndex, OriginalDate, 
                      ChangeDate
FROM         dbo.FlowAccounts

GO

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[EnterprisesTopView]'))
DROP VIEW [dbo].[EnterprisesTopView]
GO

CREATE VIEW dbo.EnterprisesTopView
AS
SELECT   ID, Name, AdminCode, Status, Corporation, RegAddress, Uscc, District
FROM      dbo.Enterprises

GO

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[CreditCostsTopView]'))
DROP VIEW [dbo].[CreditCostsTopView]
GO

CREATE VIEW dbo.CreditCostsTopView
AS
SELECT     Payer, Payee, Business, Catalog, Currency, SUM(Price) AS Total
FROM         dbo.FlowAccounts AS fa
WHERE     (Type = 20)
GROUP BY Payer, Payee, Business, Catalog, Currency

GO

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[CreditGrantsStatisticsView]'))
DROP VIEW [dbo].[CreditGrantsStatisticsView]
GO

CREATE VIEW [dbo].[CreditGrantsStatisticsView]
AS
SELECT     Payer, Payee, Business, Catalog, Currency, SUM(Price) AS Total
FROM         dbo.FlowAccounts AS fa
WHERE     (Type = 10)
GROUP BY Payer, Payee, Business, Catalog, Currency

GO

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[CreditCostsStatisticsView]'))
DROP VIEW [dbo].[CreditCostsStatisticsView]
GO

CREATE VIEW [dbo].[CreditCostsStatisticsView]
AS
SELECT     Payer, Payee, Business, Catalog, Currency, SUM(Price) AS Total
FROM         dbo.FlowAccounts AS fa
WHERE     (Type = 20)
GROUP BY Payer, Payee, Business, Catalog, Currency

GO

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[CreditsStatisticsView]'))
DROP VIEW [dbo].[CreditsStatisticsView]
GO

CREATE VIEW dbo.CreditsStatisticsView
AS
SELECT     cr.Payer, cr.Payee, cr.Business, cr.Catalog, cr.Currency, ISNULL(cr.Total, 0) AS Total, ISNULL(cc.Total, 0) AS Cost
FROM         dbo.CreditGrantsStatisticsView AS cr LEFT OUTER JOIN
                      dbo.CreditCostsStatisticsView AS cc ON cr.Payer = cc.Payer AND cc.Business = cr.Business AND cc.Catalog = cr.Catalog AND cc.Currency = cr.Currency AND cc.Payee = cr.Payee

GO