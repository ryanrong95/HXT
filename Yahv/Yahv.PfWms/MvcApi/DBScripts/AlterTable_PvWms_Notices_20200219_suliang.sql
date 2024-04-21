use PvWms
GO

-- 向Notices表中新增Origin字段
if(not exists(select * from syscolumns where ID=object_id('Notices') and name='Origin'))
begin
	-- 新增Origin字段
	Alter table [dbo].[Notices] Add Origin nvarchar(50) null
end 
GO

-- 向Notices表中新增StorageID字段
if(not exists(select * from syscolumns where ID=object_id('Notices') and name='StorageID'))
begin
	-- 新增StorageID字段
	Alter table [dbo].[Notices] Add StorageID varchar(50) null
end 
GO