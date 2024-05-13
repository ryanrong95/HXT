SELECT     waybill.wbID AS WaybillID, waybill.wbType AS WaybillType, waybill.wbTotalParts, waybill.OrderID, waybill.wbExcuteStatus AS IsModify, waybill.AppointTime, waybill.wbCreateDate
FROM         dbo.WaybillsTopView AS waybill WITH (nolock) INNER JOIN
  (SELECT     WaybillID
    FROM          dbo.Notices AS notice WITH (nolock)
    WHERE      (WareHouseID LIKE 'SZ%') AND (Type = 200)
    GROUP BY WareHouseID, WaybillID) AS nsv ON waybill.wbID = nsv.WaybillID