--SELECT     vastOrderid = input.OrderID, [TinyOrderID] = input.[TinyOrderID], product .PartNumber, product .Manufacturer, UnqiueID = sorts.ID, input.ItemID, sorts.BoxCode, sorts.[Weight], sorts.NetWeight, 
--                      sorts.Quantity, store.DateCode, store.WareHouseID, input.Origin, StoInputID = store.InputID, StorageID = store.ID, input.UnitPrice, ProductID = product .ID, sorts.Volume
--FROM         [PvWms].[dbo].[Sortings] AS sorts WITH (nolock) INNER JOIN
--                      [PvWms].[dbo].[Storages] AS store WITH (nolock) ON sorts.ID = store.SortingID INNER JOIN
--                      [PvData].[dbo].[Products] AS product WITH (nolock) ON store.ProductID = product .ID INNER JOIN
--                      [PvWms].[dbo].[Inputs] AS input WITH (nolock) ON store.InputID = input.ID
--UNION ALL
--SELECT     vastOrderid = outputs.[OrderID], [TinyOrderID] = outputs.[TinyOrderID], product .PartNumber, product .Manufacturer, UnqiueID = pick.ID, outputs.ItemID, pick.BoxCode, pick.[Weight], pick.NetWeight, 
--                      pick.Quantity, store.DateCode, store.WareHouseID, input.Origin, StoInputID = store.InputID, StorageID = store.ID, input.UnitPrice, ProductID = product .ID, pick.Volume
--FROM         [PvWms].[dbo].[Pickings] AS pick WITH (nolock) INNER JOIN
--                      [PvWms].[dbo].[Notices] AS n WITH (nolock) ON pick.NoticeID = n.ID INNER JOIN
--                      [PvData].[dbo].[Products] AS product WITH (nolock) ON n.ProductID = product .ID INNER JOIN
--                      [PvWms].[dbo].[Outputs] AS outputs WITH (nolock) ON n.OutputID = outputs.ID INNER JOIN
--                      [PvWms].[dbo].Storages AS store WITH (nolock) ON pick.[StorageID] = store.[ID] INNER JOIN
--                      [PvWms].[dbo].[Inputs] AS input WITH (nolock) ON store.InputID = input.ID
                      
                      
                      --CgSzStoragesTopView 

SELECT
VastOrderid = input.[OrderID], 
[TinyOrderID] = input.[TinyOrderID], 
product.PartNumber, 
product .Manufacturer, 
UnqiueID = storage.ID, 
input.ItemID, 
sorts.BoxCode, 
sorts.[Weight], 
sorts.NetWeight, 
storage.Quantity, 
storage.DateCode, 
storage.WareHouseID, 
storage.Origin, 
StoInputID = storage.InputID, 
StorageID = storage.ID, 
input.UnitPrice, 
ProductID = product .ID, 
sorts.Volume
FROM [dbo].Storages AS storage WITH (nolock)
inner join [dbo].[Inputs] AS input WITH (nolock) ON storage.InputID = input.ID
inner join [dbo].ProductsTopView AS product WITH (nolock) ON storage.productID = product.ID
inner join [dbo].[Sortings] AS sorts WITH (nolock) on  storage.sortingID = sorts.ID
where WareHouseID  LIKE 'SZ%' and storage.Quantity >0