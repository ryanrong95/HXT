/*

InputID = output.InputID,
OutputID = output.ID,
WareHouseID = storage.WareHouseID,

ProductID = storage.ProductID,
CustomsName = storage.CustomsName,
PartNumber = product.PartNumber,
Manufacturer = product.Manufacturer,

Quantity = picking.Quantity,
Date = picking.CreateDate,
UnitPrice = output.Price,
Currency = output.Currency,

ClentName = client.Name,
EnterCode = waybill.wbEnterCode,
*/


/*
select SUM(poqp.Quantity) from dbo.Pickings as poqp 
join dbo.Storages AS poqsto on poqp.StorageID = poqsto.ID 
 where  poqsto.inputiD = ''

select SUM(spiq.Quantity) from dbo.Sortings as spiq 
where spiq.CreateDate <GETDATE() and spiq .InputID =''
*/


select sto.InputID ,
OutputID =null ,
WareHouseID = sto.WareHouseID,
CustomsName = sto.CustomsName,
PartNumber = ptv.PartNumber,
Manufacturer = ptv.Manufacturer,
Quantity = sort.Quantity,
[Date] = sort.CreateDate,

OutUnitPrice = null, -- 销项
OutCurrency = null,

inUnitPrice = ipt.UnitPrice, -- 进项
inCurrency = ipt.Currency,

ClentName = client.Name,
EnterCode = waybill.wbEnterCode,

PastInQuantity =ISNULL((select SUM(spiq.Quantity) from dbo.Sortings as spiq where spiq.CreateDate <sort.CreateDate and spiq .InputID =sto.InputID ) , 0), -- 进项历史,
PastOutQuantity =ISNULL ((select SUM(poqp.Quantity) from dbo.Pickings as poqp join dbo.Storages AS poqsto on poqp.StorageID = poqsto.ID 
	where poqp.CreateDate<sort.CreateDate and ipt.ID = poqsto.inputiD  ) , 0) 
from dbo.Sortings as sort 
join dbo.Storages AS sto on sort.ID = sto.SortingID
join dbo.Inputs AS ipt ON sto.InputID = ipt.ID 
join dbo.ProductsTopView AS ptv ON sto.ProductID = ptv.ID
join dbo.WaybillsTopView AS waybill ON sort.waybillid = waybill.wbID
join dbo.WsClientsTopView AS client ON waybill.wbEnterCode = client.EnterCode

union all 

select sto.InputID ,
pick.OutputID ,
WareHouseID = sto.WareHouseID,
CustomsName = sto.CustomsName,
PartNumber = ptv.PartNumber,
Manufacturer = ptv.Manufacturer,
Quantity = pick.Quantity,
[Date] = pick.CreateDate,

OutUnitPrice = opt.Price, -- 销项
OutCurrency = opt.Currency,

inUnitPrice = ipt.UnitPrice, -- 进项
inCurrency = ipt.Currency,

ClentName = client.Name,
EnterCode = waybill.wbEnterCode,

PastInQuantity =ISNULL ((select SUM(spiq.Quantity) from dbo.Sortings as spiq where spiq.CreateDate <pick.CreateDate and spiq .InputID =sto.InputID ) , 0), -- 进项历史,
PastOutQuantity =ISNULL ((select SUM(ppiq.Quantity) from dbo.Pickings as ppiq where ppiq.CreateDate<pick.CreateDate and ppiq.ID != pick.id and ppiq.OutputID =pick.OutputID) , 0) ,--销项历史
profit =(opt.Price - ipt.UnitPrice ) * pick.Quantity
from dbo.Pickings as pick 
join dbo.Storages AS sto on pick.StorageID = sto.ID
join dbo.Inputs AS ipt ON sto.InputID = ipt.ID 
join dbo.Outputs AS opt ON pick.OutputID = opt.ID 
join dbo.ProductsTopView AS ptv ON sto.ProductID = ptv.ID
join dbo.Notices as n on pick.NoticeID = n.ID 
join dbo.WaybillsTopView AS waybill ON n.waybillid = waybill.wbID
join dbo.WsClientsTopView AS client ON waybill.wbEnterCode = client.EnterCode

