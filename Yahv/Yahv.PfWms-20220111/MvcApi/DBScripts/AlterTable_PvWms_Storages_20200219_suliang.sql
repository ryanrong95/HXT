use PvWms
GO

-- 删除表Storages中NoticeID字段
if(exists(select * from syscolumns where ID=object_id('Storages') and name='NoticeID'))
begin
	-- 删除NoticeID字段
	Alter table [dbo].[Storages] Drop Column NoticeID
end 
GO

-- Storages表中新增Total字段
if(not exists(select * from syscolumns where ID=object_id('Storages') and name='Total'))
begin
	-- 新增Total字段
	Alter Table [dbo].[Storages] Add Total decimal(18,7) null
end
GO
