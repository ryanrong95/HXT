/*
Description: 修改归类品牌对应的中文或外文名称后，需要修改该品牌相应的产品预归类信息
CreateDate: 2021-01-19
*/

----------------------------------------------数据处理 Begin------------------------------------------------------------------------

----未下单的产品预归类数据
--select ppc.[ID], ppc.[HSCode], ppc.[Elements], ppc.[Manufacture] into [dbo].[#temp_PreProductCategories]
--from [foricScCustoms].[dbo].[PreProductCategories] as ppc
--join [foricScCustoms].[dbo].[PreProducts] as pp on ppc.[PreProductID] = pp.ID
--where pp.[ProductUnionCode] not in (
--	select [ProductUniqueCode] from [foricScCustoms].[dbo].[OrderItems] as item
--	where item.ProductUniqueCode is not null and item.ProductUniqueCode != ''
--)
--and rtrim(ltrim(ppc.Manufacture)) = @paramMfr;

----部分下单的产品预归类数据
--insert into [dbo].[#temp_PreProductCategories]
--select ppc.[ID], ppc.[HSCode], ppc.[Elements], ppc.[Manufacture]
--from [foricScCustoms].[dbo].[PreProductCategories] as ppc
--join [foricScCustoms].[dbo].[PreProducts] as pp on ppc.[PreProductID] = pp.ID
--join [foricScCustoms].[dbo].[OrderItems] as item on pp.[ProductUnionCode] = item.ProductUniqueCode and pp.Qty != item.Quantity
--where rtrim(ltrim(ppc.Manufacture)) = @paramMfr;

select [ID], [HSCode], [Elements], [Manufacture] into [dbo].[#temp_PreProductCategories]
from [foricScCustoms].[dbo].[PreProductCategories]
where rtrim(ltrim(Manufacture)) = @paramMfr;

update [dbo].[#temp_PreProductCategories] 
set Elements = replace(ppc.Elements, @paramFrom + '牌', @paramTo)
from [dbo].[#temp_PreProductCategories] as ppc;

update [dbo].[#temp_PreProductCategories] 
set Elements = replace(ppc.Elements, @paramFrom, @paramTo)
from [dbo].[#temp_PreProductCategories] as ppc;

----------------------------------------------数据处理 End--------------------------------------------------------------------------

----------------------------------------------数据更新 Begin------------------------------------------------------------------------

update [foricScCustoms].[dbo].[PreProductCategories]
set [Elements] = temp.[Elements]
from [foricScCustoms].[dbo].[PreProductCategories] as ppc
join [dbo].[#temp_PreProductCategories] as temp on ppc.ID = temp.ID;

----------------------------------------------数据更新 End--------------------------------------------------------------------------

drop table [dbo].[#temp_PreProductCategories];