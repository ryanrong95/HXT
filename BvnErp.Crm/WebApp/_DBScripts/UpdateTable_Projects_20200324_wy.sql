
use bv3crm

--��Ŀ�����Ԥ�Ƴɽ����ֶ�
if (not exists (select * from syscolumns where ID=object_id('Projects') and name='ExpectTotal'))
begin
	Alter TABLE [dbo].[Projects] add ExpectTotal decimal(18,5);
end
GO

--��Ŀ��Ʒ�����Ԥ�Ƴɽ����ֶ�
if (not exists (select * from syscolumns where ID=object_id('ProductItems') and name='ExpectTotal'))
begin
	Alter TABLE [dbo].[ProductItems] add ExpectTotal decimal(18,5);
end
GO

--��Ʒ��Ԥ�Ƴɽ����ֶθ���
update [dbo].ProductItems set ExpectTotal= isnull(RefUnitPrice * ExpectQuantity * 1000,0)

--��Ŀ��Ԥ�Ƴɽ����ֶθ���
update [dbo].[Projects] set ExpectTotal = t.total
from (
select ProjectID,isnull(sum(ExpectTotal),0) as total from [dbo].[ProductItems] as p inner join [dbo].MapsProject as m on p.ID=m.ProductItemID group by ProjectID
) t where [dbo].[Projects].ID = t.ProjectID