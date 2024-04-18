SELECT COUNT(1)
FROM [dbo].[ClassifiedHistoriesTopView] AS [ch]
INNER JOIN [dbo].[Tariffs] AS [tariff] ON [ch].[HSCode] = [tariff].[ID]
--{QueryCriteria}