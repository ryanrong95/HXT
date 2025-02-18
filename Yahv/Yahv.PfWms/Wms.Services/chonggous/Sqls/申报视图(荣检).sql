SELECT     ldi.ID AS UnqiueID,
ld.TinyOrderID, 
ldi.OrderItemID AS ItemID, 
ld.WaybillID, 
ld.EnterCode, 
ld.BoxCode, 
ld.LotNumber, 
ld.CreateDate AS BoxingDate,  --封箱
ld.AdminID AS Packer, ld.Summary, 
                      ldi.OrderItemID, 
                      ldi.StorageID, 
                      ldi.Quantity, 
                      p.PartNumber, 
                      p.Manufacturer, 
                      s.Origin, 
                      ldi.Weight, 
                      ldi.Weight * 0.7 AS NetWeight, 
                      s.InputID, ldi.OutputID
FROM      dbo.Logs_Declare AS ld WITH (nolock) INNER JOIN
          dbo.Logs_DeclareItem AS ldi WITH (nolock) ON ld.TinyOrderID = ldi.TinyOrderID AND ld.BoxCode = ldi.BoxCode INNER JOIN
          dbo.Storages AS s WITH (nolock) ON ldi.StorageID = s.ID INNER JOIN
          dbo.ProductsTopView AS p WITH (nolock) ON s.ProductID = p.ID  
WHERE     (ld.Status = 30)