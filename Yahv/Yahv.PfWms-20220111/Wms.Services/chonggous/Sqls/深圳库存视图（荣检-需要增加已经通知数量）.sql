SELECT      input.OrderID AS VastOrderid, input.TinyOrderID, product.PartNumber, product.Manufacturer, storage.ID AS UnqiueID, input.ItemID, sorts.BoxCode, sorts.Weight, sorts.NetWeight, storage.Type, 
                     --Remainings= storage.Quantity, -- 与荣检达成一致，自行处理
                     Quantity =  noticed.quantity,
                      storage.DateCode, storage.WareHouseID, storage.Origin, storage.InputID AS StoInputID, storage.ID AS StorageID, input.UnitPrice, product.ID AS ProductID, sorts.Volume, 
                      storage.Total
FROM         dbo.Storages AS storage WITH (nolock)
inner join (select StorageID, Quantity =  SUM(quantity) from Notices  WITH (nolock)
where (WareHouseID LIKE 'SZ%' and not [type]= 200)
group by StorageID 
) as  noticed  on storage.ID = noticed.StorageID
 INNER JOIN
                      dbo.Inputs AS input WITH (nolock) ON storage.InputID = input.ID INNER JOIN
                      dbo.ProductsTopView AS product WITH (nolock) ON storage.ProductID = product.ID INNER JOIN
                      dbo.Sortings AS sorts WITH (nolock) ON storage.SortingID = sorts.ID
WHERE (storage.WareHouseID LIKE 'SZ%')
OPTION (MAXDOP 10);

--子查询

SELECT      input.OrderID AS VastOrderid, input.TinyOrderID, product.PartNumber, product.Manufacturer, storage.ID AS UnqiueID, input.ItemID, sorts.BoxCode, sorts.Weight, sorts.NetWeight, storage.Type, 
storage.Total,
 Remainings= storage.Quantity,
 Quantity =  (select SUM(quantity) from Notices  WITH (nolock)
where (WareHouseID LIKE 'SZ%' and [type]= 200 and StorageID = storage.ID)
group by StorageID ),
                      storage.DateCode, storage.WareHouseID, storage.Origin, storage.InputID AS StoInputID, storage.ID AS StorageID, input.UnitPrice, product.ID AS ProductID, sorts.Volume
                      
FROM         dbo.Storages AS storage WITH (nolock) INNER JOIN
                      dbo.Inputs AS input WITH (nolock) ON storage.InputID = input.ID INNER JOIN
                      dbo.ProductsTopView AS product WITH (nolock) ON storage.ProductID = product.ID INNER JOIN
                      dbo.Sortings AS sorts WITH (nolock) ON storage.SortingID = sorts.ID
WHERE (storage.WareHouseID LIKE 'SZ%')
OPTION (MAXDOP 10);





