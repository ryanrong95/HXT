


use bv3crm


--询价表添加报备时间 ReportDate
if (not exists (select * from syscolumns where ID=object_id('ProductItemEnquiries') and name='MOQ'))
begin
	Alter TABLE [dbo].[ProductItemEnquiries] alter column MOQ int null;
end
GO