


use bv3crm


--ѯ�۱���ӱ���ʱ�� ReportDate
if (not exists (select * from syscolumns where ID=object_id('ProductItemEnquiries') and name='MOQ'))
begin
	Alter TABLE [dbo].[ProductItemEnquiries] alter column MOQ int null;
end
GO