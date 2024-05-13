--获取最新ID
SELECT  max([ID] )
  FROM [PvWms].[dbo].[Logs_DeclareItem]
GO

--插入段（对申报数据进行补充）
INSERT INTO [PvWms].[dbo].[Logs_DeclareItem]
           ([ID]
           ,[TinyOrderID]
           ,[OrderItemID]
           ,[StorageID]
           ,[Quantity]
           ,[BoxCode]
           ,[AdminID]
           ,[OutputID]
           ,[Weight]
           ,[NetWeight]
           ,[Volume])
           
           
--可以单独用于查询
SELECT [ID] = 'LDI'+ CONVERT(varchar(50) , (20200623001366 + row_number()over(order by n.id desc)))
           ,[TinyOrderID]= 'WL43020200623002-01'
           ,[OrderItemID]= input.ItemID
           ,[StorageID] = s.ID
           ,[Quantity] =  s.Quantity
           ,[BoxCode] = so.BoxCode
           ,[AdminID] = 'Admin00973'
           ,[OutputID] =null 
           ,[Weight] = so.[Weight]
           ,[NetWeight] = null 
           ,[Volume] = null 
	        , s.ProductID
	        ,n.ProductID
  FROM [PvWms].[dbo].[Notices] AS N
  JOIN [PvWms].[dbo].[Sortings] AS SO ON N.ID = SO.NoticeID
  JOIN [PvWms].[dbo].[Storages] AS S ON SO.ID = S.SortingID
  join [PvWms].[dbo].[Inputs] as input on N.InputID = input.ID 
  WHERE n.WaybillID ='Waybill202006230004'   
  and s.ProductID = '743C1159900EAACA9364D17BE4816B0E' --修改了产品数据后理应更新tinyorderid与itemID，这是修改后的库存产品ID是最准确的
  
  AND  S.ID  NOT IN (
    SELECT [StorageID]
  FROM [PvWms].[dbo].[Logs_DeclareItem] WHERE TinyOrderID = 'WL43020200623002-01'
  )

--历史产品修改效验
select * from [PvWms].[dbo].ProductsTopView where ID in (
'27E4F80565786E88DDC93132B0DEBCF7',
'ECA5CA3685DFF42B29BBEA250C243613'
)


select * from [PvWms].[dbo].ProductsTopView where ID in (
'6B642EA161D730B85683530A5EFA93BE',
'3F50D85F2C3C16DCB94426574612671C'
)

select * from [PvWms].[dbo].ProductsTopView where ID in (
'509D853A75B8851B718A286DE358A9D8',
'7EDBEA382195D5B963D1EE3D7B737EC3'
)

select * from [PvWms].[dbo].ProductsTopView where ID in (
'743C1159900EAACA9364D17BE4816B0E',
'023CE10F4FBBA4478778767192DB000A'
)
	

--修改数据基础

SELECT [ID]
      ,[Code]
      ,[OriginID]
      ,[OrderID]
      ,[TinyOrderID]
      ,[ItemID]
      ,[ProductID]
      ,[ClientID]
      ,[PayeeID]
      ,[ThirdID]
      ,[TrackerID]
      ,[SalerID]
      ,[PurchaserID]
      ,[Currency]
      ,[UnitPrice]
      ,[CreateDate]
  FROM [PvWms].[dbo].[Inputs]
 where  ID in (
SELECT     input.id 
FROM         dbo.Storages AS storage WITH (nolock) INNER JOIN
                      dbo.Inputs AS input WITH (nolock) ON storage.InputID = input.ID INNER JOIN
                      dbo.ProductsTopView AS product WITH (nolock) ON storage.ProductID = product.ID INNER JOIN
                      dbo.Sortings AS sorts WITH (nolock) ON storage.SortingID = sorts.ID LEFT OUTER JOIN
                          (SELECT     StorageID, SUM(Quantity) AS Quantity
                            FROM          dbo.Notices WITH (nolock)
                            WHERE      (WareHouseID LIKE 'SZ%') AND (Type = 200)
                            GROUP BY StorageID) AS noticed ON storage.ID = noticed.StorageID
WHERE     (storage.WareHouseID LIKE 'SZ%') and  input.OrderID = 'WL43020200623002'
)