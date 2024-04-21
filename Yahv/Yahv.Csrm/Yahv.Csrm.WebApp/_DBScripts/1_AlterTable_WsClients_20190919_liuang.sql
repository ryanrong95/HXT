use PvbCrm;
--改表名

if exists(select * from sys.objects where object_id=OBJECT_ID(N'[dbo].[Files]') AND type in (N'U'))
begin
EXEC sp_rename Files , FilesDescription ;
end


--[dbo].[SiteUsers],加EnterpriseID，主外键关系，删除SsoID
if (exists (select * from syscolumns where ID=object_id('SiteUsers') and name='SsoID'))
begin
Alter TABLE [dbo].[SiteUsers] drop column SsoID;
end;
GO

if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='EnterpriseID'))
begin
Alter TABLE [dbo].[SiteUsers] add EnterpriseID varchar(50) not null;
end;


declare  @a int
set @a=(SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE where constraint_name='FK_SiteUsers_Enterprises')
if @a=0
begin
alter table [dbo].[SiteUsers] add constraint FK_SiteUsers_Enterprises foreign key(ID) references [dbo].[Enterprises](ID)
end


GO

--[dbo].[SiteUsers] UpdateDate  not null
if (exists (select * from syscolumns where ID=object_id('SiteUsers') and name='UpdateDate'))
begin
Alter TABLE [dbo].[SiteUsers] alter column UpdateDate datetime not null;
end;

GO

--[dbo].[SiteUsers],加是否主账号字段IsMain bit 
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='IsMain'))
begin
Alter TABLE [dbo].[SiteUsers] ADD IsMain bit not null;
end;

GO

--[dbo].[SiteUsers],加RealName字段，varchar(50) null
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='RealName'))
begin
Alter TABLE [dbo].[SiteUsers] ADD RealName varchar(50) null;
end;
GO

--[dbo].[SiteUsers],加密码Password字段，varchar(50) not null
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Password'))
begin
Alter TABLE [dbo].[SiteUsers] ADD [Password] varchar(50) not null;
end;
GO

--[dbo].[SiteUsers],加密码Summary字段，varchar(50) not null
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Summary'))
begin
Alter TABLE [dbo].[SiteUsers] ADD Summary nvarchar(300) null;
end;

GO
--[dbo].[SiteUsers],增加Status
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Status'))
begin
Alter TABLE [dbo].[SiteUsers] ADD Status int not null;
end;

--[dbo].[SiteUsers],删除From
if ( exists (select * from syscolumns where ID=object_id('SiteUsers') and name='From'))
begin
Alter TABLE [dbo].[SiteUsers] drop column [From] ;
end;



--[dbo].[FilesDescription]

--[dbo].[FilesDescription]  增加文件格式字段 FileFormat
if (not exists (select * from syscolumns where ID=object_id('FilesDescription') and name='FileFormat'))
begin
Alter TABLE [dbo].[FilesDescription] ADD FileFormat varchar(50) not null;
end;
GO

--[dbo].[Files]  增加上传人字段 AdminID ,varchar(50) not null
if (not exists (select * from syscolumns where ID=object_id('FilesDescription') and name='AdminID'))
begin
Alter TABLE [dbo].[FilesDescription] ADD AdminID varchar(50) not null;
end;
GO

--[dbo].[Files]  增加备注字段 Summary ,nvarchar(300) null
if (not exists (select * from syscolumns where ID=object_id('FilesDescription') and name='Summary'))
begin
Alter TABLE [dbo].[FilesDescription] ADD Summary nvarchar(300) not null;
end;
GO

--[dbo].[Files]，CreateDate  not null
if ( exists (select * from syscolumns where ID=object_id('FilesDescription') and name='CreateDate'))
begin
Alter TABLE [dbo].[FilesDescription] alter column CreateDate datetime not null;
end;
GO

--[dbo].[Files],Status not null
if ( exists (select * from syscolumns where ID=object_id('FilesDescription') and name='Status'))
begin
Alter TABLE [dbo].[FilesDescription] alter column [Status] int not null;
end;
GO

--[dbo].[Files],Url not null
if ( exists (select * from syscolumns where ID=object_id('FilesDescription') and name='Url'))
begin
Alter TABLE [dbo].[FilesDescription] alter column Url varchar(200) not null;
end;
GO


if (not exists (select * from syscolumns where ID=object_id('FilesDescription') and name='Name'))
begin
Alter TABLE [dbo].[FilesDescription] add Name varchar(150) not null;
end;
GO






--联系人增加传真字段
if (not exists (select * from syscolumns where ID=object_id('Contacts') and name='Fax'))
begin
Alter TABLE [dbo].[Contacts] add Fax varchar(50)  null;
end;
GO



--SubsWarehousesTopView代仓储库房更新
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
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

--[dbo].[Consignees]加索引
if(not exists(select * from sysindexes where id=object_id('Consignees') and name='Index_EnterpriseID'))
BEGIN
CREATE NONCLUSTERED INDEX [Index_EnterpriseID] ON [dbo].[Consignees]
(
	[EnterpriseID] ASC
)
INCLUDE ( 	[ID],
	[Title],
	[DyjCode],
	[District],
	[Address],
	[Postzip],
	[Status],
	[Name],
	[Tel],
	[Mobile],
	[Email],
	[CreateDate],
	[UpdateDate],
	[AdminID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
end
GO



--[dbo].[SiteUsers]删除字段IsMain,AdminID,UpdateDate,Status

if ( exists (select * from syscolumns where ID=object_id('SiteUsers') and name='IsMain'))
begin
Alter TABLE [dbo].[SiteUsers] drop column IsMain ;
end;


if ( exists (select * from syscolumns where ID=object_id('SiteUsers') and name='AdminID'))
begin
Alter TABLE [dbo].[SiteUsers] drop column AdminID ;
end;

GO

if ( exists (select * from syscolumns where ID=object_id('SiteUsers') and name='UpdateDate'))
begin
Alter TABLE [dbo].[SiteUsers] drop column UpdateDate ;
end;

GO

if ( exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Status'))
begin
Alter TABLE [dbo].[SiteUsers] drop column [Status] ;
end;

GO
--[dbo].[SiteUsers]删除EnterpriseID

declare  @b int
set @b=(SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE where constraint_name='FK_SiteUsers_Enterprises')
if @b>0
begin
alter table [dbo].[SiteUsers] drop constraint  FK_SiteUsers_Enterprises;
end

GO

if (exists (select * from syscolumns where ID=object_id('SiteUsers') and name='EnterpriseID'))
begin
Alter TABLE [dbo].[SiteUsers] drop column EnterpriseID ;
end;

GO




--[dbo].[Invoices]增加交付方式DeliveryType字段 int  not  null
if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='DeliveryType'))
begin
Alter TABLE [dbo].[Invoices] add DeliveryType int not null default 1;
end;

GO
--[dbo].[Contacts]增加传真Fax字段,varchar(50) null
if (not exists (select * from syscolumns where ID=object_id('Contacts') and name='Fax'))
begin
Alter TABLE [dbo].[Contacts] add Fax varchar(50) null;
end;

GO
--电子合同设计



--关系表：客户与供应商的关系表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[MapsBEnter]') AND type in (N'U'))
begin
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

end
GO

--业务员与跟单员关系表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[MapsTracker]') AND type in (N'U'))
begin
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
end

GO
--客户与库房、供应商与供应商的关系表可以不提供,现在也就是,新增关系表MapsBEnter
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[MapsBEnter]') AND type in (N'U'))
begin
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
 CONSTRAINT [PK_MapsBEnter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
end
--发票，到货地址增加字段省市区

if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='Province'))
begin
Alter TABLE [dbo].[Invoices] add Province nvarchar(50) null;
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='City'))
begin
Alter TABLE [dbo].[Invoices] add City nvarchar(50) null;
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='Land'))
begin
Alter TABLE [dbo].[Invoices] add Land nvarchar(50) null;
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='Province'))
begin
Alter TABLE [dbo].[Consignees] add Province nvarchar(50) null;
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='City'))
begin
Alter TABLE [dbo].[Consignees] add City nvarchar(50) null;
end;

GO

if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='Land'))
begin
Alter TABLE [dbo].[Consignees] add Land nvarchar(50) null;
end;

GO
--新增表[dbo].[Consignors]提货地址
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[Consignors]') AND type in (N'U'))
begin
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

ALTER TABLE [dbo].[Consignors] CHECK CONSTRAINT [FK_Consignors_Enterprises];

end



-- 芯达通网站用户表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[SiteUsersXdt]') AND type in (N'U'))
begin

/****** Object:  Table [dbo].[SiteUsersXdt]    Script Date: 2019/9/19 星期四 18:25:18 ******/
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
end

GO

-- 发票视图提供纳税人识别号
---[dbo].[FilesDescription]   Summary可为空 
if (exists (select * from syscolumns where ID=object_id('FilesDescription') and name='Summary'))
begin
Alter TABLE [dbo].[FilesDescription] alter column Summary nvarchar(300) null;
end;