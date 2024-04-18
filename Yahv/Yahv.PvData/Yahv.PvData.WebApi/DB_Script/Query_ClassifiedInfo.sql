-- 型号、品牌
DECLARE @PartNumber NVARCHAR(150);
DECLARE @Manufacturer NVARCHAR(150);

-- 归类信息
DECLARE @HSCode	VARCHAR(50);
DECLARE @Name NVARCHAR(150);
DECLARE @LegalUnit1 NVARCHAR(10);
DECLARE @LegalUnit2 NVARCHAR(10);
DECLARE @VATRate DECIMAL(18,7);
DECLARE @ImportPreferentialTaxRate DECIMAL(18,7);
DECLARE @ImportGeneralTaxRate DECIMAL(18,7);
DECLARE @ExciseTaxRate DECIMAL(18,7);
DECLARE @Elements NVARCHAR(500);
DECLARE @CIQCode VARCHAR(3);
DECLARE @TaxCode VARCHAR(50);
DECLARE @TaxName NVARCHAR(150);

-- 特殊类型
DECLARE @Ccc BIT;
DECLARE @Embargo BIT;
DECLARE @HkControl BIT;
DECLARE @Coo BIT;
DECLARE @CIQ BIT;
DECLARE @CIQprice DECIMAL(18,7);

-- Eccn编码
DECLARE @EccnCode VARCHAR(500);

IF @paramMfr IS NULL OR lEN(LTRIM(RTRIM(@paramMfr))) = 0
BEGIN
	-- 如果型号可以精确匹配到数据，则使用精确匹配的；否则进行模糊匹配
    IF EXISTS(SELECT * FROM [PvData].[dbo].[XbjsTopView] WITH(NOLOCK) WHERE PartNumber = @paramPN)
    BEGIN
	    SELECT TOP 1 @PartNumber = PartNumber, @Manufacturer = Manufacturer, 
               @HSCode = HSCode, @Name = Name, @Elements = Elements, @CIQCode = CIQCode, @TaxCode = TaxCode, @TaxName = TaxName,
               @Ccc = Ccc, @Embargo = Embargo, @HkControl = HkControl, @Coo = Coo, @CIQ = CIQ, @CIQprice = CIQprice
        FROM [PvData].[dbo].[XbjsTopView] WHERE PartNumber = @paramPN
        ORDER BY OrderDate DESC;
    END
    ELSE
    BEGIN
        SELECT TOP 1 @PartNumber = PartNumber, @Manufacturer = Manufacturer, 
               @HSCode = HSCode, @Name = Name, @Elements = Elements, @CIQCode = CIQCode, @TaxCode = TaxCode, @TaxName = TaxName,
               @Ccc = Ccc, @Embargo = Embargo, @HkControl = HkControl, @Coo = Coo, @CIQ = CIQ, @CIQprice = CIQprice
        FROM [PvData].[dbo].[XbjsTopView] WHERE PartNumber LIKE (@paramPN + '%') 
        ORDER BY OrderDate DESC;
    END
END
ELSE
BEGIN
	-- 如果型号、品牌可以精确匹配到数据，则使用精确匹配的；否则进行模糊匹配
    IF EXISTS(SELECT * FROM [PvData].[dbo].[XbjsTopView] WITH(NOLOCK) WHERE PartNumber = @paramPN AND Manufacturer = @paramMfr)
    BEGIN
	    SELECT TOP 1 @PartNumber = PartNumber, @Manufacturer = Manufacturer, 
               @HSCode = HSCode, @Name = Name, @Elements = Elements, @CIQCode = CIQCode, @TaxCode = TaxCode, @TaxName = TaxName,
               @Ccc = Ccc, @Embargo = Embargo, @HkControl = HkControl, @Coo = Coo, @CIQ = CIQ, @CIQprice = CIQprice
        FROM [PvData].[dbo].[XbjsTopView] WHERE PartNumber = @paramPN AND Manufacturer = @paramMfr
        ORDER BY OrderDate DESC;
    END
    ELSE
    BEGIN
        SELECT TOP 1 @PartNumber = PartNumber, @Manufacturer = Manufacturer, 
               @HSCode = HSCode, @Name = Name, @Elements = Elements, @CIQCode = CIQCode, @TaxCode = TaxCode, @TaxName = TaxName,
               @Ccc = Ccc, @Embargo = Embargo, @HkControl = HkControl, @Coo = Coo, @CIQ = CIQ, @CIQprice = CIQprice
        FROM [PvData].[dbo].[XbjsTopView] WHERE PartNumber LIKE (@paramPN + '%') AND Manufacturer LIKE (@paramMfr + '%')
        ORDER BY OrderDate DESC;
    END
END

-- 如果未查询到数据，返回Code=0
IF @PartNumber IS NULL
BEGIN
    SELECT Code = 0;
END
-- 如果查询到数据，则继续查询海关税则和Eccn，返回Code=1
ELSE
BEGIN
    -- 查询海关税则
    SELECT @VATRate = VATRate / 100, @ImportPreferentialTaxRate = ImportPreferentialTaxRate / 100, 
           @ImportGeneralTaxRate = ImportGeneralTaxRate / 100, @ExciseTaxRate = ISNULL(ExciseTaxRate, 0) / 100,
		   @LegalUnit1 = LegalUnit1, @LegalUnit2 = LegalUnit2
    FROM [PvData].[dbo].[Tariffs] WHERE HSCode = @HSCode;

    -- 查询型号的Eccn数据
    SET @EccnCode = '';
    SELECT @EccnCode = @EccnCode + Code + ',' FROM [PvData].[dbo].[EccnsTopView] WHERE PartNumber = @PartNumber;

    IF LEN(@EccnCode) > 0
    BEGIN
        SET @EccnCode = LEFT(@EccnCode, LEN(@EccnCode) - 1);
    END

    -- 返回查询结果
    SELECT Code = 1,
           PartNumber = @PartNumber, Manufacturer = @Manufacturer,
           HSCode = @HSCode, Name = @Name, Elements = @Elements, CIQCode = @CIQCode, TaxCode = @TaxCode, TaxName = @TaxName,
           VATRate = @VATRate, ImportPreferentialTaxRate = @ImportPreferentialTaxRate,
           ImportGeneralTaxRate = @ImportGeneralTaxRate, ExciseTaxRate = @ExciseTaxRate,
		   LegalUnit1 = @LegalUnit1, LegalUnit2 = @LegalUnit2,
           Ccc = @Ccc, Embargo = @Embargo, HkControl = @HkControl, Coo = @Coo, CIQ = @CIQ, CIQprice = @CIQprice,
           EccnCode = @EccnCode;
END