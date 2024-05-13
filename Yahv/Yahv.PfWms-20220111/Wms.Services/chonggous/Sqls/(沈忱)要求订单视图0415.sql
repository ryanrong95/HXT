-- 香港收货
select a.*,input.orderID,input.ItemID, product.PartNumber, product.Manufacturer, input.CreateDate 
from 
(select [type]='HKIns',InputID ,HKEnterQuantity = SUM(Total)  from dbo.Storages
where WareHouseID like 'HK%' 
group by InputID) as a 
inner  join dbo.Inputs as input on a.InputID = input.ID
inner join dbo.ProductsTopView as product on input.ProductID = product.ID


--香港报关
select b.*,input.orderID, input.ItemID, product.PartNumber, product.Manufacturer
from
(select [type]='Customs', InputID , CustomQuantity = SUM(Total) from dbo.Logs_DeclareItem as ldi
join dbo.Storages as Storage on ldi.StorageID = Storage.ID
group by Storage.InputID ) as b
inner join dbo.Inputs as input on b.InputID = input.ID
inner join dbo.ProductsTopView as product on input.ProductID = product.ID

--深圳出库
select c.*, input.OrderID, input.ItemID, product.PartNumber, product.Manufacturer
from 
(select [type]='SZOuts', InputID , SZOutQuantity = SUM(Total) - SUM(Quantity)  from dbo.Storages
where WareHouseID like 'SZ%' 
group by InputID ) as c
inner join dbo.Inputs as input on c.InputID = input.ID
inner join dbo.ProductsTopView as product on input.ProductID = product.ID

--������ CgStatisticsHKDelivery
select [type]='HKDelivery',InputID ,HKEnterQuantity = SUM(Total)  from dbo.Storages with(nolock)
where WareHouseID like 'HK%' 
group by InputID

--���ڱ��� CgStatisticsDeclare
select [type]='Customs', InputID , CustomQuantity = SUM(Total) from dbo.Logs_DeclareItem as ldi  with(nolock)
join dbo.Storages as Storage  with(nolock)on ldi.StorageID = Storage.ID
group by Storage.InputID

--���ڳ��� CgStatisticsShiped
select [type]='SZShiped', InputID , SZOutQuantity = SUM(Total) - SUM(Quantity)  from dbo.Storages  with(nolock)
where WareHouseID like 'SZ%' 
group by InputID


select * from CgStatisticsHKDelivery
select * from CgStatisticsDeclare
select * from CgStatisticsShiped
