use PvCenter
GO

--Waybills表中添加OrderID, Source, NoticeType后，更新新添加这几列的数据

SELECT  
  i.[OrderID],n.[Source] , [NoticeType] = n.[Type]
  FROM [PvCenter].[dbo].[Waybills] as wb
  join [PvWms].[dbo].[Notices] as n on wb.ID = n.WaybillID
  join [PvWms].[dbo].Inputs as i on n.InputID = i.ID
GO


update  [PvCenter].[dbo].[Waybills]
set  [OrderID] =i.[OrderID] 
      ,[Source]=n.[Source]
      ,[NoticeType]=n.[Type]
from [PvCenter].[dbo].[Waybills] as wb
  join [PvWms].[dbo].[Notices] as n on wb.ID = n.WaybillID
  join [PvWms].[dbo].Inputs as i on n.InputID = i.ID