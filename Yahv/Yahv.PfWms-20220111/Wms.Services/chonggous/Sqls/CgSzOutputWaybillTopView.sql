SELECT     waybill.wbID, waybill.wbType, waybill.wbCarrierID, waybill.wbCreateDate, waybill.corCompany, waybill.corPlace, waybill.corAddress, waybill.corContact, waybill.corPhone, waybill.coeCompany, 
                      waybill.coePlace, waybill.coeAddress, waybill.coeContact, waybill.coePhone, waybill.coeZipcode, waybill.coeEmail, waybill.wbTotalParts, waybill.wbExcuteStatus AS IsModify, waybill.coeIDNumber, 
                      waybill.wbSummary, waybill.AppointTime, waybill.ExType, waybill.ExPayType, notice.Quantity, notice.DateCode, notice.Origin, notice.Weight, notice.NetWeight, notice.Volume, notice.BoxCode, 
                      notice.ShelveID, product.PartNumber, product.Manufacturer, product.PackageCase, product.Packaging, outputs.OrderID AS Expr6, outputs.TinyOrderID, outputs.ItemID, outputs.TrackerID, 
                      waybill.wbEnterCode, waybill.corEmail, waybill.corZipcode, waybill.corIDNumber, waybill.corIDType, waybill.coeIDType
FROM         dbo.WaybillsTopView AS waybill INNER JOIN
                      dbo.Notices AS notice ON waybill.wbID = notice.WaybillID INNER JOIN
                      dbo.ProductsTopView AS product ON notice.ProductID = product.ID INNER JOIN
                      dbo.Outputs AS outputs ON notice.OutputID = outputs.ID
WHERE notice.WareHouseID LIKE 'SZ%'  and notice.[Type]=200
  
  
  
 
   