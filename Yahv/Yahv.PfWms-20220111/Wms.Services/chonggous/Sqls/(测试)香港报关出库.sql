SELECT storage.[ID]
      ,storage.[Type]
      ,storage.[WareHouseID]
      ,storage.[SortingID]
      ,storage.[InputID]
      ,storage.[ProductID]
      ,storage.[Total]
      ,storage.[Quantity]
      ,storage.[Origin]
      ,[IsLock]
      ,storage.[CreateDate]
      ,storage.[Status]
      ,storage.[ShelveID]
      ,storage.[Supplier]
      ,storage.[DateCode]
      ,waybill.wbTotalParts
  FROM [PvWms].[dbo].[Storages] as  storage
  inner join [PvWms].[dbo].[Pickings] as  picking on storage.ID = picking.StorageID
  inner join [PvWms].dbo.Notices as notice on picking.NoticeID = notice.ID
  inner join [PvWms].dbo.WaybillsTopView as waybill on   notice.WaybillID = waybill.wbID 
  where waybill.chcdLotNumber = '1202004141628'
GO


  


----http://hv.warehouse.b1b.com/wmsapi/cgDelcare/AutoHkExit?lotNumber=1202004141551


