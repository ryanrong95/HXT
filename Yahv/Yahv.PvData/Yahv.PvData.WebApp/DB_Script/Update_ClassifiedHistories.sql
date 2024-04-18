/*
Description: 修改归类品牌对应的中文或外文名称后，需要修改该品牌相应的归类历史记录
CreateDate: 2021-01-19
*/

----------------------------------------------数据处理 Begin------------------------------------------------------------------------

select * into [dbo].[#temp_ClassifiedPartNumbers]
from
(select *,rn = ROW_NUMBER()over( partition by partnumber, manufacturer order by orderdate desc )
from [PvData].[dbo].[ClassifiedPartNumbers]
) as data
where data.rn =1 and rtrim(ltrim(data.Manufacturer)) = @paramMfr;

alter table [dbo].[#temp_ClassifiedPartNumbers]
add MD5ID varchar(50);

update [dbo].[#temp_ClassifiedPartNumbers] 
set Elements = replace(cpn.Elements, @paramFrom + '牌', @paramTo),
orderdate = getdate()
from [dbo].[#temp_ClassifiedPartNumbers] as cpn;

update [dbo].[#temp_ClassifiedPartNumbers] 
set Elements = replace(cpn.Elements, @paramFrom, @paramTo),
orderdate = getdate()
from [dbo].[#temp_ClassifiedPartNumbers] as cpn;

update [dbo].[#temp_ClassifiedPartNumbers]
  set MD5ID = UPPER(SubString(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(150),[PartNumber])
				+convert(varchar(150),lower([Manufacturer]))
				+[HSCode]
				+convert(varchar(150),[Name])
				+convert(varchar(150),[LegalUnit1])
				+convert(varchar(150),isnull([LegalUnit2],''))
				+rtrim(convert(float, [VATRate]))
				+rtrim(convert(float,[ImportPreferentialTaxRate]))
				+rtrim(convert(float,[ExciseTaxRate]))
				+convert(varchar(150),[Elements])
				+isnull([SupervisionRequirements],'')
				+isnull([CIQC],'')
				+isnull([CIQCode],'')
				+isnull([TaxCode],'')
				+convert(varchar(150),isnull([TaxName],'')))),3,32));

select id, hscode, partnumber, manufacturer, elements into [dbo].[#temp_Logs_ClassifiedPartNumber]
from
(select *,rn = ROW_NUMBER()over( partition by partnumber, manufacturer order by createdate desc )
from [PvData].[dbo].[Logs_ClassifiedPartNumber]
) as data
where data.rn =1 and rtrim(ltrim(data.Manufacturer)) = @paramMfr;

update [dbo].[#temp_Logs_ClassifiedPartNumber] 
set Elements = replace(cpn.Elements, @paramFrom + '牌', @paramTo)
from [dbo].[#temp_Logs_ClassifiedPartNumber] as cpn;

update [dbo].[#temp_Logs_ClassifiedPartNumber] 
set Elements = replace(cpn.Elements, @paramFrom, @paramTo)
from [dbo].[#temp_Logs_ClassifiedPartNumber] as cpn;

----------------------------------------------数据处理 End--------------------------------------------------------------------------

----------------------------------------------数据更新 Begin------------------------------------------------------------------------

update [PvData].[dbo].[ClassifiedPartNumbers]
set orderdate = temp.orderdate
from [PvData].[dbo].[ClassifiedPartNumbers] as cpn
join [PvData].[dbo].[#temp_ClassifiedPartNumbers] as temp on cpn.id = temp.md5id;

insert into [PvData].[dbo].[ClassifiedPartNumbers]
           ([ID]
           ,[PartNumber]
           ,[Manufacturer]
           ,[HSCode]
           ,[Name]
           ,[LegalUnit1]
           ,[LegalUnit2]
           ,[VATRate]
           ,[ImportPreferentialTaxRate]
           ,[ExciseTaxRate]
           ,[Elements]
           ,[SupervisionRequirements]
           ,[CIQC]
           ,[CIQCode]
           ,[TaxCode]
           ,[TaxName]
           ,[CreateDate]
           ,[OrderDate])
	select [Md5ID]
		  ,[PartNumber]
		  ,[Manufacturer]
		  ,[HSCode]
		  ,[Name]
		  ,[LegalUnit1]
		  ,[LegalUnit2]
		  ,[VATRate]
		  ,[ImportPreferentialTaxRate]
		  ,[ExciseTaxRate]
		  ,[Elements]
		  ,[SupervisionRequirements]
		  ,[CIQC]
		  ,[CIQCode]
		  ,[TaxCode]
		  ,[TaxName]
		  ,[CreateDate]
		  ,[OrderDate]
	from [dbo].[#temp_ClassifiedPartNumbers]
	where [Md5ID] not in (
		select [ID] from [PvData].[dbo].[ClassifiedPartNumbers]
	);

update [PvData].[dbo].[Logs_ClassifiedPartNumber]
set [Elements] = temp.[Elements]
from [PvData].[dbo].[Logs_ClassifiedPartNumber] as log
join [dbo].[#temp_Logs_ClassifiedPartNumber] as temp on log.ID = temp.ID;

----------------------------------------------数据更新 End--------------------------------------------------------------------------

drop table [dbo].[#temp_ClassifiedPartNumbers];
drop table [dbo].[#temp_Logs_ClassifiedPartNumber];