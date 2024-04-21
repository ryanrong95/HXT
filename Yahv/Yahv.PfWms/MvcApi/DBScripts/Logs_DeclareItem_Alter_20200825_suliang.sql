use PvWms
GO

-- 向Logs_DeclareItem表中新增Status字段
if(not exists(select * from syscolumns where ID=object_id('Logs_DeclareItem') and name='Status'))
begin
	-- 新增Status字段
	Alter table [dbo].[Logs_DeclareItem] Add Status int DEFAULT (0)  not null
	update [dbo].[Logs_DeclareItem] set Status = 1
end 
GO