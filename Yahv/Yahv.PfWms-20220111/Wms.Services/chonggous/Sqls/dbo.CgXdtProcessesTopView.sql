SELECT     storage.ID, storage.Type, storage.SortingID, storage.InputID, storage.ProductID, storage.Total, storage.Quantity, storage.Origin, storage.IsLock, storage.ShelveID, storage.Supplier, 
                      storage.DateCode, storage.Summary, input.OrderID, input.TinyOrderID, input.ItemID, prodcut.PartNumber, prodcut.Manufacturer, prodcut.PackageCase, prodcut.Packaging, sorting.BoxCode
FROM         dbo.Storages AS storage WITH (nolock) INNER JOIN
  dbo.Inputs AS input WITH (nolock) ON storage.InputID = input.ID INNER JOIN
  dbo.ProductsTopView AS prodcut WITH (nolock) ON storage.ProductID = prodcut.ID INNER JOIN
  dbo.Sortings AS sorting WITH (nolock) ON storage.SortingID = sorting.ID
   INNER JOIN dbo.Notices AS notice WITH (nolock) on sorting.NoticeID = notice.ID
  where  notice.Type= 100 -- Èë¿â
  and storage.WareHouseID like 'HK%'
  union all 
  SELECT     storage.ID, storage.Type, storage.SortingID, storage.InputID, storage.ProductID, storage.Total, storage.Quantity, storage.Origin, storage.IsLock, storage.ShelveID, storage.Supplier, 
storage.DateCode, storage.Summary, [output].OrderID, [output].TinyOrderID, [output].ItemID, prodcut.PartNumber, prodcut.Manufacturer, prodcut.PackageCase, prodcut.Packaging, 
picking.BoxCode
FROM dbo.Storages AS storage WITH (nolock) INNER JOIN
  dbo.Pickings AS picking WITH (nolock) ON storage.ID = picking.StorageID  INNER JOIN
  dbo.ProductsTopView AS prodcut WITH (nolock) ON storage.ProductID = prodcut.ID INNER JOIN
  dbo.Outputs AS [output] WITH (nolock) ON picking.OutputID = [output].ID
  INNER JOIN dbo.Notices AS notice WITH (nolock) on picking.NoticeID = notice.ID
  where notice.Type= 300 -- ×°Ïä
   and storage.WareHouseID like 'HK%'