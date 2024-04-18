-- ExitNoticeFiles
--		23: 客户收货确认单 -- 大订单

INSERT INTO [ls_PvCenter].[PvCenter].[dbo].[FilesDescription]
           ([ID]
           ,[WsOrderID]
		   ,[WaybillID]
           ,[AdminID]
           ,[CustomName]
		   ,[Type]
		   ,[Url]
		   ,[CreateDate]
		   ,[Status])
SELECT 'XDT' + xdtfile.[ID] as [ID],
       en.[OrderID] as [WsOrderID],
	   CASE WHEN waybill.[ID] IS NOT NULL THEN waybill.[ID] ELSE ed.[Code] END AS [WaybillID],
	   xdtfile.[AdminID] AS [AdminID],
	   xdtfile.[Name] as [CustomName],
	   xdtfile.[FileType] as [Type],
	   '//foricerp.ic360.cn/wladmin/Files/' + xdtfile.[Url] as [Url],
	   xdtfile.[CreateDate] as [CreateDate],
	   xdtfile.[Status] as [Status]
FROM [foricScCustoms].[dbo].[ExitNoticeFiles] AS xdtfile
INNER JOIN [ls_PvbErm].[PvbErm].[dbo].[Admins]as admin ON xdtfile.[AdminID] = admin.[OriginID]
INNER JOIN [foricScCustoms].[dbo].[ExitNotices] as en ON xdtfile.[ExitNoticeID] = en.[ID]
INNER JOIN [foricScCustoms].[dbo].[ExitDelivers] as ed ON en.[ID] = ed.[ExitNoticeID]
LEFT JOIN [ls_PvCenter].[PvCenter].[dbo].[Waybills]as waybill ON ed.[Code] = waybill.[Code];

-- PayExchangeApplyFiles:
--		2:  付汇委托书  -- 
--		11: 付汇PI文件

INSERT INTO [ls_PvCenter].[PvCenter].[dbo].[FilesDescription]
           ([ID]
           ,[ApplicationID]
           ,[AdminID]
           ,[CustomName]
		   ,[Type]
		   ,[Url]
		   ,[CreateDate]
		   ,[Status])
SELECT 'XDT' + xdtfile.[ID] as [ID],
	   xdtfile.[PayExchangeApplyID] as [ApplicationID],
	   CASE WHEN xdtfile.[AdminID] IS NOT NULL THEN admin.[ID] ELSE xdtfile.[UserID] END AS [AdminID],
	   xdtfile.[Name] as [CustomName],
	   xdtfile.[FileType] as [Type],
	   '//foricerp.ic360.cn/wladmin/Files/' + xdtfile.[Url] as [Url],
	   xdtfile.[CreateDate] as [CreateDate],
	   xdtfile.[Status] as [Status]
FROM [foricScCustoms].[dbo].[PayExchangeApplyFiles] AS xdtfile
LEFT JOIN [ls_PvbErm].[PvbErm].[dbo].[Admins]as admin ON xdtfile.[AdminID] = admin.[OriginID];

-- MainOrderFiles
--		1:  对账单
--		3:  代理报关委托书
--		5:  合同发票

INSERT INTO [ls_PvCenter].[PvCenter].[dbo].[FilesDescription]
           ([ID]
           ,[WsOrderID]
           ,[AdminID]
           ,[CustomName]
		   ,[Type]
		   ,[Url]
		   ,[CreateDate]
		   ,[Status])
SELECT 'XDT' + xdtfile.[ID] as [ID],
	   xdtfile.[MainOrderID] as [WsOrderID],
	   CASE WHEN xdtfile.[AdminID] IS NOT NULL THEN admin.[ID] ELSE xdtfile.[UserID] END AS [AdminID],
	   xdtfile.[Name] as [CustomName],
	   xdtfile.[FileType] as [Type],
	   '//foricerp.ic360.cn/wladmin/Files/' + xdtfile.[Url] as [Url],
	   xdtfile.[CreateDate] as [CreateDate],
	   xdtfile.[Status] as [Status]
FROM [foricScCustoms].[dbo].[MainOrderFiles] AS xdtfile
LEFT JOIN [ls_PvbErm].[PvbErm].[dbo].[Admins]as admin ON xdtfile.[AdminID] = admin.[OriginID];
