--SELECT TOP 100 xbj.PartNumber, xbj.HSCode, xbj.TaxCode, xbj.TaxName,
--			   xbj.Ccc, xbj.Embargo, xbj.CIQ, xbj.CIQprice,
--			(CASE WHEN eccn.Code IS NULL THEN '' ELSE eccn.Code END) AS Eccn
--INTO #tempStandardPartnumbers
--FROM [PvData].[dbo].[XbjsTopView] AS xbj WITH(NOLOCK)
--LEFT JOIN [PvData].[dbo].[Eccn] AS eccn ON xbj.PartNumber = eccn.PartNumber
--WHERE xbj.PartNumber LIKE (@paramPN + '%')
--ORDER BY xbj.OrderDate DESC;

--IF @@ROWCOUNT > 0
--SELECT temp.*, 
----如果有暂定税率，且暂定税率小于优惠税率，则取暂定税率为关税率；否则取优惠税率为关税率
--        (CASE 
--			WHEN tariff.ImportControlTaxRate IS NOT NULL AND tariff.ImportControlTaxRate < tariff.ImportPreferentialTaxRate 
--			THEN tariff.ImportControlTaxRate
--			ELSE tariff.ImportPreferentialTaxRate
--		END) AS TariffRate, tariff.VATRate,
--(CASE WHEN atRate.Rate IS NULL THEN 0 ELSE atRate.Rate END) AS AddedTariffRate
--from #tempStandardPartnumbers as temp
--JOIN [PvData].[dbo].[Tariffs] AS tariff ON temp.HSCode = tariff.HSCode
--LEFT JOIN [PvData].[dbo].[OriginsATRate] AS atRate ON temp.HSCode = atRate.TariffID AND atRate.StartDate <= GETDATE() AND (atRate.EndDate IS NULL OR atRate.EndDate >= GetDate()) AND atRate.Status = 200;

--ELSE
--SELECT TOP 100 NAME as PartNumber FROM [PvData].[dbo].[StandardPartnumbers] WHERE NAME LIKE (@paramPN + '%');

--IF EXISTS(SELECT 1 FROM [PvData].[dbo].[StandardPartnumbersForPlugin] WHERE PartNumber LIKE (@paramPN + '%'))
SELECT TOP 50 [PartNumber],[HSCode],[TaxCode],[TaxName],[Ccc],[Embargo],[CIQ],[CIQprice],
(CASE WHEN Eccn IS NULL THEN '' ELSE Eccn END) AS Eccn,
[TariffRate],[VATRate],
(CASE WHEN AddedTariffRate IS NULL THEN 0 ELSE AddedTariffRate END) AS AddedTariffRate
FROM [PvData].[dbo].[StandardPartnumbersForPlugin]
WHERE PartNumber Like (@paramPN + '%');

--ELSE
--SELECT TOP 50 NAME AS PartNumber FROM [PvData].[dbo].[StandardPartnumbers] WHERE NAME LIKE (@paramPN + '%');

--增加冷片型号