IF @paramMfr IS NULL OR lEN(LTRIM(RTRIM(@paramMfr))) = 0
BEGIN
	-- 如果型号可以精确匹配到数据，则使用精确匹配的；否则进行模糊匹配
    IF EXISTS(SELECT * FROM [PvData].[dbo].[StandardPastQuotes] WITH(NOLOCK) WHERE PartNumber = @paramPN)
    BEGIN
	    SELECT PartNumber, Manufacturer, Currency, UnitPrice, Quantity, CreateDate
        FROM [PvData].[dbo].[StandardPastQuotes] WHERE PartNumber = @paramPN;
    END
    ELSE
    BEGIN
        SELECT PartNumber, Manufacturer, Currency, UnitPrice, Quantity, CreateDate
        FROM [PvData].[dbo].[StandardPastQuotes] WHERE PartNumber LIKE (@paramPN + '%');
    END
END
ELSE
BEGIN
	-- 如果型号、品牌可以精确匹配到数据，则使用精确匹配的；否则进行模糊匹配
    IF EXISTS(SELECT * FROM [PvData].[dbo].[StandardPastQuotes] WITH(NOLOCK) WHERE PartNumber = @paramPN AND Manufacturer = @paramMfr)
    BEGIN
	    SELECT PartNumber, Manufacturer, Currency, UnitPrice, Quantity, CreateDate
        FROM [PvData].[dbo].[StandardPastQuotes] WHERE PartNumber = @paramPN AND Manufacturer = @paramMfr;
    END
    ELSE
    BEGIN
        SELECT PartNumber, Manufacturer, Currency, UnitPrice, Quantity, CreateDate
        FROM [PvData].[dbo].[StandardPastQuotes] WHERE PartNumber LIKE (@paramPN + '%') AND Manufacturer LIKE (@paramMfr + '%');
    END
END