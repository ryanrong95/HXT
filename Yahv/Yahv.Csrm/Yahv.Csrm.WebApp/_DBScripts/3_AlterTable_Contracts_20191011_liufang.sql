use PvbCrm;
--新建视图[dbo].[MapsTrackerTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[MapsTrackerTopView]'))
DROP VIEW [dbo].[MapsTrackerTopView]
GO
CREATE VIEW [dbo].[MapsTrackerTopView]
AS
SELECT   ID, Bussiness, Type, AdminID, IsDefault, RealID
FROM      dbo.MapsTracker

GO
--[dbo].[Contracts]表字段[dbo].[Contracts]
--删除字段IsPrePayExchange
if (exists (select * from syscolumns where ID=object_id('Contracts') and name='IsPrePayExchange'))
begin
Alter TABLE [dbo].Contracts drop column IsPrePayExchange;
end;

--删除字段IsLimitNinetyDays
if (exists (select * from syscolumns where ID=object_id('Contracts') and name='IsLimitNinetyDays'))
begin
Alter TABLE [dbo].Contracts drop column IsLimitNinetyDays;
end;

--添加字段
if (not exists (select * from syscolumns where ID=object_id('Contracts') and name='ExchangeMode'))
begin
Alter TABLE [dbo].Contracts add ExchangeMode int not null;
end


--添加承运商表


