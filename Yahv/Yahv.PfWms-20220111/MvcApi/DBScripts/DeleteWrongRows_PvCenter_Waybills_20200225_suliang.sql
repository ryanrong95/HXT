use PvCenter
GO

--由于Waybills表中新增Source, NoticeType等字段,导致线上测试版中现有Waybills表很多数据是错误的或没有的
--删除Waybills表中错误的数据

--首先删除错误数据行的其关联外键对应的数据

--删除错误数据关联的WayCharges表中的数据
  delete from [PvCenter].[dbo].[WayCharges] 
  where ID in
  (
  select wc.ID
  from [PvCenter].[dbo].[Waybills] as wb, [PvCenter].[dbo].[WayCharges] as wc
  where wb.ID = wc.ID and wb.Source is null
  )

  -- 删除错误数据关联的WayChcd表中的数据
  delete from [PvCenter].[dbo].[WayChcd] 
  where ID in
  (
  select wc.ID
  from [PvCenter].[dbo].[Waybills] as wb, [PvCenter].[dbo].[WayChcd] as wc
  where wb.ID = wc.ID and wb.Source is null
  )

  -- 删除错误数据关联的WayLoadings表中的数据
  delete from [PvCenter].[dbo].[WayLoadings] 
  where ID in
  (
  select wl.ID
  from [PvCenter].[dbo].[Waybills] as wb, [PvCenter].[dbo].[WayLoadings] as wl
  where wb.ID = wl.ID and wb.Source is null
  )

 -- 删除Waybills表中错误的数据行
 delete from [PvCenter].[dbo].[Waybills]
 where Source is null