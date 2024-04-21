use PvCenter
GO

-- 向WayLoadings表中新增ExcuteStatus字段
if(not exists(select * from syscolumns where ID=object_id('WayLoadings') and name='ExcuteStatus'))
begin
	-- 新增ExcuteStatus字段
	Alter table [dbo].[WayLoadings] Add ExcuteStatus int null
end 
GO