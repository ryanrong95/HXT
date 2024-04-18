SELECT [data].[ID], [data].[PartNumber], [data].[Manufacturer], [data].[HSCode], [data].[Name] AS [TariffName], [data].[TaxCode], [data].[TaxName], [data].[LegalUnit1], [data].[LegalUnit2], [data].[value] AS [VATRate], [data].[value2] AS [ImportPreferentialTaxRate], [data].[value3] AS [ImportControlTaxRate], [data].[value4] AS [ExciseTaxRate], [data].[CIQCode], [data].[Elements], [data].[SupervisionRequirements], [data].[CIQC], [data].[OrderDate], [data].[Ccc], [data].[Embargo], [data].[HkControl], [data].[Coo], [data].[CIQ], [data].[CIQprice], [data].[Summary]
FROM (
        SELECT ROW_NUMBER() OVER (ORDER BY [ch].[PartNumber], [ch].[Manufacturer]) AS [ROW_NUMBER], [ch].[ID], [ch].[PartNumber], [ch].[Manufacturer], [ch].[HSCode], [ch].[Name], [ch].[TaxCode], [ch].[TaxName], [tariff].[LegalUnit1], [tariff].[LegalUnit2], [tariff].[VATRate] / 100 AS [value], [tariff].[ImportPreferentialTaxRate] / 100 AS [value2], 
            (CASE 
                WHEN [tariff].[ImportControlTaxRate] IS NULL THEN CONVERT(Decimal(18,7),NULL)
                ELSE ([tariff].[ImportControlTaxRate]) / 100
             END) AS [value3], (COALESCE([tariff].[ExciseTaxRate],0)) / 100 AS [value4], [ch].[CIQCode], [ch].[Elements], [ch].[Ccc], [ch].[Embargo], [ch].[HkControl], [ch].[Coo], [ch].[CIQ], [ch].[CIQprice], [ch].[Summary], [ch].[SupervisionRequirements], [ch].[CIQC], [ch].[OrderDate]
        FROM [dbo].[ClassifiedHistoriesTopView] AS [ch]
        INNER JOIN [dbo].[Tariffs] AS [tariff] ON [ch].[HSCode] = [tariff].[ID]
		--{QueryCriteria}
        ) AS [data]
WHERE [data].[ROW_NUMBER] BETWEEN (@pageIndex - 1) * @pageSize + 1 AND (@pageIndex - 1) * @pageSize + @pageSize
ORDER BY [data].[ROW_NUMBER];