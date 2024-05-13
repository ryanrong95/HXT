--CgSzOutputNoticesTopView

SELECT     waybill.wbID AS WaybillID, 
waybill.wbType AS WaybillType, 
n.Quantity, 
outputs.OrderID,
waybill.wbExcuteStatus AS IsModify,
 n.CreateDate
FROM dbo.WaybillsTopView AS waybill WITH (nolock)INNER JOIN
  dbo.Notices AS n WITH (nolock) ON waybill.wbID = n.WaybillID INNER JOIN
  dbo.Outputs AS outputs WITH (nolock) ON n.OutputID = outputs.ID
  where n.WareHouseID  LIKE 'SZ%'