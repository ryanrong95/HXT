use PvbCrm;
--������[dbo].[Drivers]�����ֶ�IsChcd
if (not exists (select * from syscolumns where ID=object_id('Drivers') and name='IsChcd'))
begin
alter table [dbo].[Drivers] add IsChcd bit null;

end
go
if (exists (select * from syscolumns where ID=object_id('Drivers') and name='IsChcd'))
begin
update [dbo].[Drivers] set IsChcd=0;
alter table [dbo].[Drivers] alter column IsChcd bit not null;
end
--������˾��[dbo].[DriversTopView],��IsChcd
--IF EXISTS (SELECT * FROM sys.views WHERE object_id =
--OBJECT_ID(N'[dbo].[DriversTopView]'))
--DROP VIEW [dbo].[DriversTopView]
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO

--CREATE VIEW [dbo].[DriversTopView]
--AS
--SELECT   ID as ID, EnterpriseID as EnterpriseID, 
--Name as Name, IDCard as IDCard, Mobile as Mobile, Mobile2 as Mobile2, Status as Status, CustomsCode as CustomsCode, CardCode as CardCode, PortCode as PortCode, LBPassword as LBPassword,IsChecd as IsChecd
--FROM      dbo.Drivers

--[dbo].[Drivers]Mobile��Ϊ��Ϊnull
if (exists (select * from syscolumns where ID=object_id('Drivers') and name='Mobile'))
begin
alter table [dbo].[Drivers] alter column Mobile varchar(50) null;
end


--[dbo].[Enterprises][AdminCode]��Ϊ��Ϊnull
if (exists (select * from syscolumns where ID=object_id('Enterprises') and name='AdminCode'))
begin
alter table [dbo].[Enterprises] alter column AdminCode varchar(50) null;
end