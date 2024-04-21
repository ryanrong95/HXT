
use PVbCrm;


INSERT [dbo].[Enterprises] (ID,Name,AdminCode,Status)
SELECT UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','�����ⷿ')),3,32)) ,'�����ⷿ','',200
 WHERE NOT EXISTS (SELECT *
                     FROM [dbo].[Enterprises]
                    WHERE ID =  UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','�����ⷿ')),3,32))
                  )

GO

INSERT [dbo].[Enterprises] (ID,Name,AdminCode,Status)
SELECT  UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','�Ϻ��ⷿ')),3,32)),'�Ϻ��ⷿ','',200
 WHERE NOT EXISTS (SELECT *
                     FROM [dbo].[Enterprises]
                    WHERE ID = UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','�Ϻ��ⷿ')),3,32))
                  )

GO

 INSERT [dbo].[Enterprises] (ID,Name,AdminCode,Status)
SELECT  UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','���ڿⷿ')),3,32)),'���ڿⷿ','',200
 WHERE NOT EXISTS (SELECT *
                     FROM [dbo].[Enterprises]
                    WHERE ID =  UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','���ڿⷿ')),3,32))
                  )

GO

 INSERT [dbo].[Enterprises] (ID,Name,AdminCode,Status)
SELECT  UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','��ۿⷿ')),3,32)),'��ۿⷿ','',200
 WHERE NOT EXISTS (SELECT *
                     FROM [dbo].[Enterprises]
                    WHERE ID =  UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','��ۿⷿ')),3,32))
                  )

GO
--[dbo].[WareHouses]���������
declare  @a int
set @a=(SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE where constraint_name='FK_WareHouses_Enterprises')
if @a=0
begin
alter table [dbo].[WareHouses] add constraint FK_WareHouses_Enterprises foreign key(ID) references [dbo].[Enterprises](ID)
end
 
 
GO

--�ⷿ�����
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='AdminID'))
begin
Alter TABLE [dbo].[WareHouses] ADD AdminID varchar(50) not null default 'SA01'
end;


--�ⷿ��������
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='CreateDate'))
begin
Alter TABLE [dbo].[WareHouses] ADD CreateDate datetime not null default getdate()
end;
--�ⷿ�޸�����
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='UpdateDate'))
begin
Alter TABLE [dbo].[WareHouses] ADD UpdateDate datetime not null default getdate()
end;

--�ⷿ״̬
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='Status'))
begin
Alter TABLE [dbo].[WareHouses] ADD Status int not null default 200;
end;



--�ͻ���ͼ����
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[ClientsTopView]'))
DROP VIEW [dbo].[ClientsTopView]
GO
CREATE VIEW [dbo].[ClientsTopView]
AS
SELECT   dbo.Enterprises.Name, dbo.Clients.ID, dbo.Clients.Nature, dbo.Clients.Grade, dbo.Clients.DyjCode, 
                dbo.Clients.TaxperNumber, dbo.Clients.Vip, dbo.Clients.Status, dbo.Clients.AdminID, dbo.Clients.CreateDate, 
                dbo.Clients.UpdateDate, dbo.Clients.AreaType, dbo.Enterprises.Corporation, dbo.Enterprises.RegAddress, 
                dbo.Enterprises.Uscc, dbo.Enterprises.District
FROM      dbo.Clients INNER JOIN
                dbo.Enterprises ON dbo.Clients.ID = dbo.Enterprises.ID
GO

--��Ӧ����ͼ����
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[SuppliersTopView]'))
DROP VIEW [dbo].[SuppliersTopView]
GO

CREATE VIEW [dbo].[SuppliersTopView]
AS
SELECT   s.ID, s.Status, e.Name, s.Type, s.Nature, s.Grade, s.AreaType, s.IsFactory, s.AgentCompany, s.TaxperNumber, 
                s.DyjCode, s.InvoiceType, s.Currency, s.Price, s.RepayCycle, e.Corporation, e.RegAddress, e.Uscc, e.District
FROM      dbo.Suppliers AS s INNER JOIN
                dbo.Enterprises AS e ON s.ID = e.ID
GO

--�޸Ŀⷿ�ֶ�����,����Distinct
--if (exists (select * from syscolumns where ID=object_id('WareHouses') and name='District'))
--begin
--exec sp_rename 'WareHouses.District','Region'
--end;

--֮ǰ�����ݵı�����ûɾ��
IF EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[client_query]') AND type in (N'U'))
BEGIN
drop table [dbo].[client_query];
END;


--�ⷿ���ӵ���WsCode
if (not exists (select * from syscolumns where ID=object_id('WareHouses') and name='WsCode'))
begin
Alter TABLE [dbo].[WareHouses] ADD WsCode varchar(50) not null default ''
end;

GO

if ( exists (select * from syscolumns where ID=object_id('WareHouses') and name='WsCode'))
begin
update [dbo].[WareHouses] set WsCode='SZ01',District=3 where ID=UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','���ڿⷿ')),3,32))  
update [dbo].[WareHouses] set WsCode='BJ01',District=1 where ID=UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','�����ⷿ')),3,32))  
update [dbo].[WareHouses] set WsCode='HK01',District=4 where ID=UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','��ۿⷿ')),3,32))  
update [dbo].[WareHouses] set WsCode='SH01',District=2 where ID=UPPER(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','�Ϻ��ⷿ')),3,32))
end  

GO

--�޸Ŀⷿ��ͼ

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
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

--�޸�[dbo].[ConsigneesTopView]�����Title
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[ConsigneesTopView]'))
DROP VIEW [dbo].[ConsigneesTopView]
GO
CREATE VIEW [dbo].[ConsigneesTopView]
AS
SELECT   ID, EnterpriseID, DyjCode, District, Address, Postzip, Status, Name, Tel, Mobile, Email, Title
FROM      dbo.Consignees
GO



--ɾ������-��������
if(exists(select * from sysindexes where id=object_id('WareHouses') and name='Index_Name'))
BEGIN
DROP INDEX Index_Name ON WareHouses
end

GO
if (exists (select * from syscolumns where ID=object_id('WareHouses') and name='AdminCode'))
begin
ALTER TABLE [dbo].[WareHouses] DROP COLUMN AdminCode 
end

GO

if (exists (select * from syscolumns where ID=object_id('WareHouses') and name='Name'))
begin
ALTER TABLE [dbo].[WareHouses] DROP COLUMN Name 
end

GO 

use HVRFQ;
Go
--HVRFQ�ⷿ��ͼ
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'HvRFQ.[dbo].[WarehousesTopView]'))
DROP VIEW [dbo].[WarehousesTopView]
GO
CREATE VIEW [dbo].[WarehousesTopView]
AS
SELECT   *
FROM      PvbCrm.dbo.WarehousesTopView AS WarehousesTopView_1
GO
