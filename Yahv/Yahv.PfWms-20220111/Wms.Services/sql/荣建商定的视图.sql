/*
建议提供了订单ID 客户信息由华芯通来展示所属人员、与入仓号
*/

SELECT WaybillID = wb.[ID]
      ,WaybillType =  wb.[Type]
	  ,Quantity = n.Quantity
	  ,outputs.OrderID 
	  ,IsModify = wb.ExcuteStatus 
	  ,CreateDate = n.CreateDate
  FROM [PvCenter].[dbo].[Waybills] as wb
  inner join [PvWms].[dbo].[Notices]  as n on wb.ID  = n.WaybillID
  inner join [PvWms].[dbo].[Outputs] as outputs  on n.OutputID = outputs.ID
  --inner join [PvbCrm].[dbo].[Enterprises] as ep  on outputs.OwnerID = ep.ID





SELECT WaybillID = wb.[ID]
      ,WaybillType =  wb.[Type]
	  ,Quantity = n.Quantity
	  ,outputs.OrderID 
	  ,wsc.EnterCode
	  ,ClientName=  ep.Name
	  ,IsModify = wb.ExcuteStatus 
	  ,CreateDate = n.CreateDate
  FROM [PvCenter].[dbo].[Waybills] as wb
  inner join [PvWms].[dbo].[Notices]  as n on wb.ID  = n.WaybillID
  inner join [PvWms].[dbo].[Outputs] as outputs  on n.OutputID = outputs.ID
  inner join [PvbCrm].[dbo].[Enterprises] as ep  on outputs.OwnerID = ep.ID
  inner join [PvbCrm].[dbo].[WsClients] as wsc  on  ep.ID =wsc.ID
  --inner join [PvbErm].[dbo].[Admins] as admins  on outputs.   admins.ID 

