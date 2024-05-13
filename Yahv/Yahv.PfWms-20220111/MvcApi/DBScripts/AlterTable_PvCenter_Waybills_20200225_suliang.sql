use PvCenter
GO

-- 向Waybills表中新增OrderID字段
if(not exists(select * from syscolumns where ID=object_id('Waybills') and name='OrderID'))
begin
	-- 新增OrderID字段
	Alter table [dbo].[Waybills] Add OrderID varchar(50) null
end 
GO

-- 向Waybills表中新增Source字段
if(not exists(select * from syscolumns where ID=object_id('Waybills') and name='Source'))
begin
	-- 新增Source字段
	Alter table [dbo].[Waybills] Add Source int null
end 
GO


-- 向Waybills表中新增NoticeType字段
if(not exists(select * from syscolumns where ID=object_id('Waybills') and name='NoticeType'))
begin
	-- 新增NoticeType字段
	Alter table [dbo].[Waybills] Add NoticeType int null
end 
GO


-- 向Waybills表中新增TempEnterCode字段
if(not exists(select * from syscolumns where ID=object_id('Waybills') and name='TempEnterCode'))
begin
	-- 新增TempEnterCode字段
	Alter table [dbo].[Waybills] Add TempEnterCode varchar(50) null
end 
GO