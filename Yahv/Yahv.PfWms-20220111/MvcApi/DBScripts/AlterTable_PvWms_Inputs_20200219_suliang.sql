use PvWms
GO

-- 删除表Inputs中DateCode字段
if(exists(select * from syscolumns where ID=object_id('Inputs') and name='DateCode'))
begin
	-- 删除DateCode字段
	Alter table [dbo].[Inputs] Drop Column DateCode
end 
GO

-- 删除表Inputs中Origin字段
if(exists(select * from syscolumns where ID=object_id('Inputs') and name='Origin'))
begin
	-- 删除Origin字段
	Alter table [dbo].[Inputs] Drop Column Origin
end
GO