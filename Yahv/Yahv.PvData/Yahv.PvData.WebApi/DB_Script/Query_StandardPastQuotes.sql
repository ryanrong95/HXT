IF @paramMfr IS NULL OR lEN(LTRIM(RTRIM(@paramMfr))) = 0
BEGIN
	-- ����ͺſ��Ծ�ȷƥ�䵽���ݣ���ʹ�þ�ȷƥ��ģ��������ģ��ƥ��
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
	-- ����ͺš�Ʒ�ƿ��Ծ�ȷƥ�䵽���ݣ���ʹ�þ�ȷƥ��ģ��������ģ��ƥ��
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