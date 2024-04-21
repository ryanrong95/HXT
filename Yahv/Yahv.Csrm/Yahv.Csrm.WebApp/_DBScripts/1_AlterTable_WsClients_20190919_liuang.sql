use PvbCrm;
--�ı���

if exists(select * from sys.objects where object_id=OBJECT_ID(N'[dbo].[Files]') AND type in (N'U'))
begin
EXEC sp_rename Files , FilesDescription ;
end


--[dbo].[SiteUsers],��EnterpriseID���������ϵ��ɾ��SsoID
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

--[dbo].[SiteUsers],���Ƿ����˺��ֶ�IsMain bit 
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='IsMain'))
begin
Alter TABLE [dbo].[SiteUsers] ADD IsMain bit not null;
end;

GO

--[dbo].[SiteUsers],��RealName�ֶΣ�varchar(50) null
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='RealName'))
begin
Alter TABLE [dbo].[SiteUsers] ADD RealName varchar(50) null;
end;
GO

--[dbo].[SiteUsers],������Password�ֶΣ�varchar(50) not null
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Password'))
begin
Alter TABLE [dbo].[SiteUsers] ADD [Password] varchar(50) not null;
end;
GO

--[dbo].[SiteUsers],������Summary�ֶΣ�varchar(50) not null
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Summary'))
begin
Alter TABLE [dbo].[SiteUsers] ADD Summary nvarchar(300) null;
end;

GO
--[dbo].[SiteUsers],����Status
if (not exists (select * from syscolumns where ID=object_id('SiteUsers') and name='Status'))
begin
Alter TABLE [dbo].[SiteUsers] ADD Status int not null;
end;

--[dbo].[SiteUsers],ɾ��From
if ( exists (select * from syscolumns where ID=object_id('SiteUsers') and name='From'))
begin
Alter TABLE [dbo].[SiteUsers] drop column [From] ;
end;



--[dbo].[FilesDescription]

--[dbo].[FilesDescription]  �����ļ���ʽ�ֶ� FileFormat
if (not exists (select * from syscolumns where ID=object_id('FilesDescription') and name='FileFormat'))
begin
Alter TABLE [dbo].[FilesDescription] ADD FileFormat varchar(50) not null;
end;
GO

--[dbo].[Files]  �����ϴ����ֶ� AdminID ,varchar(50) not null
if (not exists (select * from syscolumns where ID=object_id('FilesDescription') and name='AdminID'))
begin
Alter TABLE [dbo].[FilesDescription] ADD AdminID varchar(50) not null;
end;
GO

--[dbo].[Files]  ���ӱ�ע�ֶ� Summary ,nvarchar(300) null
if (not exists (select * from syscolumns where ID=object_id('FilesDescription') and name='Summary'))
begin
Alter TABLE [dbo].[FilesDescription] ADD Summary nvarchar(300) not null;
end;
GO

--[dbo].[Files]��CreateDate  not null
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






--��ϵ�����Ӵ����ֶ�
if (not exists (select * from syscolumns where ID=object_id('Contacts') and name='Fax'))
begin
Alter TABLE [dbo].[Contacts] add Fax varchar(50)  null;
end;
GO



--SubsWarehousesTopView���ִ��ⷿ����
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

--[dbo].[Consignees]������
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



--[dbo].[SiteUsers]ɾ���ֶ�IsMain,AdminID,UpdateDate,Status

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
--[dbo].[SiteUsers]ɾ��EnterpriseID

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




--[dbo].[Invoices]���ӽ�����ʽDeliveryType�ֶ� int  not  null
if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='DeliveryType'))
begin
Alter TABLE [dbo].[Invoices] add DeliveryType int not null default 1;
end;

GO
--[dbo].[Contacts]���Ӵ���Fax�ֶ�,varchar(50) null
if (not exists (select * from syscolumns where ID=object_id('Contacts') and name='Fax'))
begin
Alter TABLE [dbo].[Contacts] add Fax varchar(50) null;
end;

GO
--���Ӻ�ͬ���



--��ϵ���ͻ��빩Ӧ�̵Ĺ�ϵ��
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

--ҵ��Ա�����Ա��ϵ��

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
--�ͻ���ⷿ����Ӧ���빩Ӧ�̵Ĺ�ϵ����Բ��ṩ,����Ҳ����,������ϵ��MapsBEnter
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
--��Ʊ��������ַ�����ֶ�ʡ����

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
--������[dbo].[Consignors]�����ַ
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



-- о��ͨ��վ�û���
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =OBJECT_ID(N'[dbo].[SiteUsersXdt]') AND type in (N'U'))
begin

/****** Object:  Table [dbo].[SiteUsersXdt]    Script Date: 2019/9/19 ������ 18:25:18 ******/
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

-- ��Ʊ��ͼ�ṩ��˰��ʶ���
---[dbo].[FilesDescription]   Summary��Ϊ�� 
if (exists (select * from syscolumns where ID=object_id('FilesDescription') and name='Summary'))
begin
Alter TABLE [dbo].[FilesDescription] alter column Summary nvarchar(300) null;
end;