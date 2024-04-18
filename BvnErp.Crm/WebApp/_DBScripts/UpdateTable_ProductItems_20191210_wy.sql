


use bv3crm

--产品型号表添加备注字段
if (not exists (select * from syscolumns where ID=object_id('ProductItems') and name='Summary'))
begin
	Alter TABLE [dbo].[ProductItems] add Summary nvarchar(300) null;
end
GO
--产品型号表删除报备信息字段，添加到询价表中

--询价表添加报备时间 ReportDate
if (not exists (select * from syscolumns where ID=object_id('ProductItemEnquiries') and name='ReportDate'))
begin
	Alter TABLE [dbo].[ProductItemEnquiries] add ReportDate datetime null;
end
GO
--更新询价表报备时间
update [dbo].[ProductItemEnquiries]  set ReportDate=t.ReportDate from (
	select m.ProductItemEnquiryID,p.ReportDate from MapsProductItemEnquiry as m left join ProductItems as p on m.ProductItemID = p.ID
) as t where [dbo].[ProductItemEnquiries].ID = t.ProductItemEnquiryID

--update [dbo].[ProductItemEnquiries] set ReportDate=GETDATE() where ReportDate is null

--再次更改报备时间不允许为 null
if ( exists (select * from syscolumns where ID=object_id('ProductItemEnquiries') and name='ReportDate'))
begin
	Alter TABLE [dbo].[ProductItemEnquiries] alter column ReportDate datetime not null;
end
--删除型号表报备状态字段
if (exists (select * from syscolumns where ID=object_id('ProductItems') and name='IsReport'))
begin
	Alter TABLE [dbo].[ProductItems] drop column IsReport;
end
GO
--删除型号表报备时间字段
if (exists (select * from syscolumns where ID=object_id('ProductItems') and name='ReportDate'))
begin
	Alter TABLE [dbo].[ProductItems] drop column ReportDate;
end
GO
--删除ProductItemTopView
if (exists (select * from sys.views where object_id=object_id(N'ProductItemTopView')))
begin
	drop view ProductItemTopView;
end
GO

--产品型号文件添加 SubID 字段
if (not exists (select * from syscolumns where ID=object_id('ProductItemFiles') and name='SubID'))
begin
	Alter TABLE [dbo].[ProductItemFiles] add SubID varchar(50) null;
end
GO

