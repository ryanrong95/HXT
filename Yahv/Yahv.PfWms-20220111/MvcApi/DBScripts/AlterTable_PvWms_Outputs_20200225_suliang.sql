use PvWms
GO

-- 删除表Outputs中StorageID字段
if(exists(select * from syscolumns where ID=object_id('Outputs') and name='StorageID'))
begin
	-- 删除StorageID字段
	Alter table [dbo].[Outputs] Drop Column StorageID
end 
GO