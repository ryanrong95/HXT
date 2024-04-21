
/*
drop table #tinput
drop table #tinyOrderid
drop table #waybillid
*/

 

select distinct tinyOrderid into #tinyOrderid from  [PvWms].[dbo].[Inputs]  where OrderID in 
(
'WL86620200606012',
'WL86620200606011',
'WL86620200606010',
'WL86620200606009',
'WL86620200606008',
'WL86620200606005',
'WL86620200606006',
'WL86620200606007',
'WL86620200606003',
'XL00120200606514',
'XL00120200606513',
'XL00120200606510',
'XL00120200606509',
'XL00120200606508',
'XL00120200606507',
'XL00120200606506',
'XL00120200606505',
'XL00120200606504',
'XL00120200606503',
'XL00120200606502',
'XL00120200606501'
)

select distinct ID into #tinput from  [PvWms].[dbo].[Inputs]  where OrderID in 
(
'WL86620200606012',
'WL86620200606011',
'WL86620200606010',
'WL86620200606009',
'WL86620200606008',
'WL86620200606005',
'WL86620200606006',
'WL86620200606007',
'WL86620200606003',
'XL00120200606514',
'XL00120200606513',
'XL00120200606510',
'XL00120200606509',
'XL00120200606508',
'XL00120200606507',
'XL00120200606506',
'XL00120200606505',
'XL00120200606504',
'XL00120200606503',
'XL00120200606502',
'XL00120200606501'
)

--É¾³ýÉê±¨
DELETE FROM [PvWms].[dbo].[Logs_Declare]  where TinyOrderID in(select ID from #tinyOrderid)
DELETE FROM [PvWms].[dbo].[Logs_DeclareItem] where TinyOrderID in(select ID from #tinyOrderid)


--É¾³ý·Ö¼ð
DELETE FROM [PvWms].[dbo].[Sortings] where ID in (
SELECT SortingID FROM [PvWms].[dbo].[Storages] where InputID  in  (select ID from #tinput))

--É¾³ýÍ¨Öª£¨input£©
DELETE FROM [PvWms].[dbo].[Notices] where InputID in (select ID from #tinput)


--É¾³ýÍ¨Öª
DELETE FROM [PvWms].[dbo].[Notices] where ID in (
select NoticeID FROM [PvWms].[dbo].[Sortings] where ID in (
SELECT SortingID FROM [PvWms].[dbo].[Storages] where InputID  in  (select ID from #tinput))
)

--É¾³ý¿â´æ
DELETE  FROM [PvWms].[dbo].[Storages] where InputID  in  (select ID from #tinput)

--É¾³ý½øÏî
DELETE FROM [PvWms].[dbo].[Inputs]  where ID in(select ID from #tinput)
     




--É¾³ýÔËµ¥
select ID  into #waybillid FROM [PvCenter].[dbo].[Waybills] where OrderID in 
(
'WL86620200606012',
'WL86620200606011',
'WL86620200606010',
'WL86620200606009',
'WL86620200606008',
'WL86620200606005',
'WL86620200606006',
'WL86620200606007',
'WL86620200606003',
'XL00120200606514',
'XL00120200606513',
'XL00120200606510',
'XL00120200606509',
'XL00120200606508',
'XL00120200606507',
'XL00120200606506',
'XL00120200606505',
'XL00120200606504',
'XL00120200606503',
'XL00120200606502',
'XL00120200606501'
)

--select 'DELETE FROM [PvCenter].[dbo].'+ name +' where id not in (select distinct waybillid from [PvWms].[dbo].notices)'  from PvCenter.sys.tables where name like 'way%'


DELETE FROM [PvCenter].[dbo].WayExpress where id in (select id from #waybillid)
DELETE FROM [PvCenter].[dbo].WayCosts where id in (select id from #waybillid)
DELETE FROM [PvCenter].[dbo].WayLoadings where id in (select id from #waybillid)
DELETE FROM [PvCenter].[dbo].WayCharges where id in (select id from #waybillid)
DELETE FROM [PvCenter].[dbo].WayParters where id in (select id from #waybillid)
DELETE FROM [PvCenter].[dbo].WayChcd where id in (select id from #waybillid)
DELETE FROM [PvCenter].[dbo].Waybills where id in (select id from #waybillid)
DELETE FROM [PvCenter].[dbo].[Waybills] where id in (select id from #waybillid)


DELETE FROM [PvCenter].[dbo].WayExpress where id not in (select distinct waybillid from [PvWms].[dbo].notices)
DELETE FROM [PvCenter].[dbo].WayCosts where id not in (select distinct waybillid from [PvWms].[dbo].notices)
DELETE FROM [PvCenter].[dbo].WayLoadings where id not in (select distinct waybillid from [PvWms].[dbo].notices)
DELETE FROM [PvCenter].[dbo].WayCharges where id not in (select distinct waybillid from [PvWms].[dbo].notices)
DELETE FROM [PvCenter].[dbo].WayParters where id not in (select distinct waybillid from [PvWms].[dbo].notices)
DELETE FROM [PvCenter].[dbo].WayChcd where id not in (select distinct waybillid from [PvWms].[dbo].notices)
DELETE FROM [PvCenter].[dbo].Waybills where id not in (select distinct waybillid from [PvWms].[dbo].notices)
DELETE FROM [PvCenter].[dbo].[Waybills] where ID  not in (select distinct waybillid from [PvWms].[dbo].notices)