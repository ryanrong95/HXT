use PvbCrm;
--�½���ͼ[dbo].[MapsTrackerTopView]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[MapsTrackerTopView]'))
DROP VIEW [dbo].[MapsTrackerTopView]
GO
CREATE VIEW [dbo].[MapsTrackerTopView]
AS
SELECT   ID, Bussiness, Type, AdminID, IsDefault, RealID
FROM      dbo.MapsTracker

GO
--[dbo].[Contracts]���ֶ�[dbo].[Contracts]
--ɾ���ֶ�IsPrePayExchange
if (exists (select * from syscolumns where ID=object_id('Contracts') and name='IsPrePayExchange'))
begin
Alter TABLE [dbo].Contracts drop column IsPrePayExchange;
end;

--ɾ���ֶ�IsLimitNinetyDays
if (exists (select * from syscolumns where ID=object_id('Contracts') and name='IsLimitNinetyDays'))
begin
Alter TABLE [dbo].Contracts drop column IsLimitNinetyDays;
end;

--����ֶ�
if (not exists (select * from syscolumns where ID=object_id('Contracts') and name='ExchangeMode'))
begin
Alter TABLE [dbo].Contracts add ExchangeMode int not null;
end


--��ӳ����̱�


