use PVbCrm

--�ⷿ��ͼ�����ִ�ʹ��
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
FROM      dbo.Consignees INNER JOIN
                dbo.Enterprises ON dbo.Consignees.EnterpriseID = dbo.Enterprises.ID INNER JOIN
                dbo.WareHouses ON dbo.Enterprises.ID = dbo.WareHouses.ID
GO

--��ϵ����ͼ����[dbo].[ContactsTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[ContactsTopView]'))
DROP VIEW [dbo].[ContactsTopView]
GO
CREATE VIEW [dbo].[ContactsTopView]
AS
SELECT   ID, EnterpriseID, Type, Name, Tel, Mobile, Email, Status
FROM      dbo.Contacts

GO

--��ϵ�ˣ������ˣ���Ʊ��������ַȥ��FromType
--ɾ����ϵ��FromType
if ( exists (select * from syscolumns where ID=object_id('Contacts') and name='FromType'))
begin
	declare @name varchar(50)
	select  @name =b.name from sysobjects b join syscolumns a
	on b.id = a.cdefault
	where a.id = object_id('Contacts')
	and a.name ='FromType'
	if (@name is not null) 
	begin
		exec('alter table Contacts drop constraint ' + @name)
	end
Alter TABLE [dbo].[Contacts] drop column  FromType; 

end;
GO

--ɾ��������FromType
if ( exists (select * from syscolumns where ID=object_id('Beneficiaries') and name='FromType'))
begin
declare @name varchar(50)
	select  @name =b.name from sysobjects b join syscolumns a
	on b.id = a.cdefault
	where a.id = object_id('Beneficiaries')
	and a.name ='FromType'
	if (@name is not null) 
	begin
		exec('alter table Beneficiaries drop constraint ' + @name)
	end
Alter TABLE [dbo].[Beneficiaries] drop column  FromType; 
end;

GO
--ɾ����ƱFromType
if ( exists (select * from syscolumns where ID=object_id('Invoices') and name='FromType'))
begin
	declare @name varchar(50)
	select  @name =b.name from sysobjects b join syscolumns a
	on b.id = a.cdefault
	where a.id = object_id('Invoices')
	and a.name ='FromType'
	if (@name is not null) 
	begin
		exec('alter table Invoices drop constraint ' + @name)
	end
Alter TABLE [dbo].[Invoices] drop column  FromType; 
end;
GO
--ɾ��������ַFromtype
if ( exists (select * from syscolumns where ID=object_id('Consignees') and name='FromType'))
begin
	declare @name varchar(50)
	select  @name =b.name from sysobjects b join syscolumns a
	on b.id = a.cdefault
	where a.id = object_id('Consignees')
	and a.name ='FromType'
	if (@name is not null) 
	begin
		exec('alter table Consignees drop constraint ' + @name)
	end
	Alter TABLE [dbo].[Consignees] drop column  FromType; 
end;
GO