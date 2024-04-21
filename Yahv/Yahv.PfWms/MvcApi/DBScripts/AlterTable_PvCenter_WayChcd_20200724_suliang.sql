use PvCenter
GO

-- 向WayChcd表中新增DriverCode字段, 司机证件号
if(not exists(select * from syscolumns where ID=object_id('WayChcd') and name='DriverCode'))
begin
	-- 新增DriverCode字段
	Alter table [dbo].[WayChcd] Add DriverCode varchar(50) null
end 
GO

-- 向WayChcd表中新增VehicleType字段, 车型
if(not exists(select * from syscolumns where ID=object_id('WayChcd') and name='VehicleType'))
begin
	-- 新增VehicleType字段
	Alter table [dbo].[WayChcd] Add VehicleType int null
end 


-- 向WayChcd表中新增DriverMobile字段, 香港电话
if(not exists(select * from syscolumns where ID=object_id('WayChcd') and name='HKPhone'))
begin
	-- 新增HKPhone字段
	Alter table [dbo].[WayChcd] Add HKPhone varchar(50) null
end 


-- 向WayChcd表中新增DriverCardNo字段, 司机卡号
if(not exists(select * from syscolumns where ID=object_id('WayChcd') and name='DriverCardNo'))
begin
	-- 新增DriverCardNo字段
	Alter table [dbo].[WayChcd] Add DriverCardNo varchar(50) null
end


-- 向WayChcd表中新增CustomsPort字段, 口岸
if(not exists(select * from syscolumns where ID=object_id('WayChcd') and name='CustomsPort'))
begin
	-- 新增CustomsPort字段
	Alter table [dbo].[WayChcd] Add CustomsPort nvarchar(50) null
end

-- 向WayChcd表中新增VehicleSize字段, 车辆尺寸
if(not exists(select * from syscolumns where ID=object_id('WayChcd') and name='VehicleSize'))
begin
	-- 新增VehicleSize字段
	Alter table [dbo].[WayChcd] Add VehicleSize nvarchar(50) null
end

-- 向WayChcd表中新增HKSealNumber字段, 香港库房封条号
if(not exists(select * from syscolumns where ID=object_id('WayChcd') and name='HKSealNumber'))
begin
	-- 新增HKSealNumber字段
	Alter table [dbo].[WayChcd] Add HKSealNumber nvarchar(50) null
end
GO