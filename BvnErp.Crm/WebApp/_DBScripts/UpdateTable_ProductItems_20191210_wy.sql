


use bv3crm

--��Ʒ�ͺű���ӱ�ע�ֶ�
if (not exists (select * from syscolumns where ID=object_id('ProductItems') and name='Summary'))
begin
	Alter TABLE [dbo].[ProductItems] add Summary nvarchar(300) null;
end
GO
--��Ʒ�ͺű�ɾ��������Ϣ�ֶΣ���ӵ�ѯ�۱���

--ѯ�۱���ӱ���ʱ�� ReportDate
if (not exists (select * from syscolumns where ID=object_id('ProductItemEnquiries') and name='ReportDate'))
begin
	Alter TABLE [dbo].[ProductItemEnquiries] add ReportDate datetime null;
end
GO
--����ѯ�۱���ʱ��
update [dbo].[ProductItemEnquiries]  set ReportDate=t.ReportDate from (
	select m.ProductItemEnquiryID,p.ReportDate from MapsProductItemEnquiry as m left join ProductItems as p on m.ProductItemID = p.ID
) as t where [dbo].[ProductItemEnquiries].ID = t.ProductItemEnquiryID

--update [dbo].[ProductItemEnquiries] set ReportDate=GETDATE() where ReportDate is null

--�ٴθ��ı���ʱ�䲻����Ϊ null
if ( exists (select * from syscolumns where ID=object_id('ProductItemEnquiries') and name='ReportDate'))
begin
	Alter TABLE [dbo].[ProductItemEnquiries] alter column ReportDate datetime not null;
end
--ɾ���ͺű���״̬�ֶ�
if (exists (select * from syscolumns where ID=object_id('ProductItems') and name='IsReport'))
begin
	Alter TABLE [dbo].[ProductItems] drop column IsReport;
end
GO
--ɾ���ͺű���ʱ���ֶ�
if (exists (select * from syscolumns where ID=object_id('ProductItems') and name='ReportDate'))
begin
	Alter TABLE [dbo].[ProductItems] drop column ReportDate;
end
GO
--ɾ��ProductItemTopView
if (exists (select * from sys.views where object_id=object_id(N'ProductItemTopView')))
begin
	drop view ProductItemTopView;
end
GO

--��Ʒ�ͺ��ļ���� SubID �ֶ�
if (not exists (select * from syscolumns where ID=object_id('ProductItemFiles') and name='SubID'))
begin
	Alter TABLE [dbo].[ProductItemFiles] add SubID varchar(50) null;
end
GO

