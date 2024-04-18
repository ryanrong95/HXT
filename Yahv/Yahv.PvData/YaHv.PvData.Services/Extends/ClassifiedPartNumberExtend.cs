using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;

namespace YaHv.PvData.Services.Extends
{
    public static class ClassifiedPartNumberExtend
    {
        /// <summary>
        /// 填充自动归类信息
        /// </summary>
        /// <param name="cpn">税则归类信息</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="isVerifyPriceFluctuation">是否验证价格浮动</param>
        /// <param name="highPriceLimit">价格高于历史平均价格时的浮动百分比</param>
        /// <param name="lowPriceLimit">价格低于历史平均价格时的浮动百分比</param>
        /// <param name="origin">产地</param>
        /// <returns></returns>
        public static HistoryClassified FillAutoClassified(this Models.ClassifiedPartNumber cpn, decimal unitPrice, bool isVerifyPriceFluctuation, decimal highPriceLimit, decimal lowPriceLimit, string origin)
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                #region 基础数据查询

                //根据归类的海关编码获取对应的税则信息
                var tariff = new Views.Alls.TariffsAll(reponsitory)[cpn.HSCode];
                //原产地加征关税率
                decimal originRate = 0;
                if (!string.IsNullOrEmpty(origin))
                {
                    var originATRate = new Views.Alls.OriginsATRateAll(reponsitory)[cpn.HSCode, origin];
                    originRate = originATRate?.Rate ?? 0m;
                }

                #endregion

                #region 特殊类型验证

                //根据型号、品牌/制造商获取产品归类的特殊类型
                var other = new Views.Alls.OthersAll(reponsitory)[cpn.PartNumber, cpn.Manufacturer];
                //更新特殊类型的排序时间
                if (other != null)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvData.Others>(new
                    {
                        OrderDate = DateTime.Now
                    }, a => a.PartNumber == cpn.PartNumber && a.Manufacturer == cpn.Manufacturer);
                }
                bool ccc = other?.Ccc ?? false;
                bool embargo = other?.Embargo ?? false;
                bool hkControl = other?.HkControl ?? false;
                bool coo = other?.Coo ?? false;
                bool ciq = other?.CIQ ?? false;
                bool remarked = other == null ? false : !string.IsNullOrEmpty(other?.Summary);

                //系统管控(Ccc、禁运)
                var sysCcc = new Views.Alls.ProductControlsAll(reponsitory)[cpn.PartNumber, ControlType.Ccc];
                var sysEmbargo = new Views.Alls.ProductControlsAll(reponsitory)[cpn.PartNumber, ControlType.Embargo];
                bool isSysCcc = sysCcc == null ? false : true;
                bool isSysEmbargo = sysEmbargo == null ? false : true;

                //原产地消毒/检疫
                bool disinfected = false;
                if (!string.IsNullOrEmpty(origin))
                {
                    var curDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                    var originDisinfection = new Views.Alls.OriginsDisinfectionAll(reponsitory).FirstOrDefault(item => item.Origin == origin &&
                                                    item.StartDate <= curDate && (item.EndDate == null || item.EndDate >= curDate));
                    disinfected = originDisinfection != null ? true : false;
                }

                //无效价格，不能完成自动归类
                bool isInvalidPrice = unitPrice == 0;

                //高价值产品验证
                bool isHighPrice = cpn.HighPrice(unitPrice, reponsitory);

                //价格浮动验证
                bool isPriceFluctuation = cpn.PriceFluctuation(unitPrice, isVerifyPriceFluctuation, highPriceLimit, lowPriceLimit, reponsitory);

                //申报要素校验
                bool isInconsistentElements = cpn.InconsistentElements(tariff);

                //是否需要海关验估(卡控海关编码)
                bool isCustomsInspection = new Views.Alls.CustomsControlsAll(reponsitory)[cpn.HSCode, CustomsControlType.HSCode] != null;

                //是否是卡控型号
                bool isPartNumberControl = new Views.Alls.CustomsControlsAll(reponsitory)[cpn.PartNumber, cpn.TariffName, CustomsControlType.Partnumber] != null;

                //是否缺失归类信息
                bool isMissingInfo = string.IsNullOrEmpty(cpn.TaxCode) || string.IsNullOrEmpty(cpn.TaxName) || string.IsNullOrEmpty(cpn.CIQCode);

                //是否属于特殊类型
                bool isSpecialType = other == null || ccc || embargo || hkControl || coo || ciq || isSysCcc || isSysEmbargo ||
                                     isInvalidPrice || isHighPrice || isPriceFluctuation ||
                                     isInconsistentElements || isCustomsInspection || isPartNumberControl || isMissingInfo;

                #endregion

                //修正自动归类记录
                string cpnID = cpn.AmendClassified(tariff, reponsitory);

                return new HistoryClassified()
                {
                    AutoHSCodeID = cpnID,
                    PartNumber = cpn.PartNumber,
                    Manufacturer = cpn.Manufacturer,

                    HSCode = cpn.HSCode,
                    TariffName = cpn.TariffName,
                    LegalUnit1 = tariff.LegalUnit1,
                    LegalUnit2 = tariff.LegalUnit2,
                    VATRate = tariff.VATRate / 100,
                    ImportPreferentialTaxRate = tariff.ImportTaxRate / 100,
                    ExciseTaxRate = tariff.ExciseTaxRate.GetValueOrDefault() / 100,
                    Elements = cpn.Elements,
                    CIQCode = cpn.CIQCode,
                    TaxCode = cpn.TaxCode,
                    TaxName = cpn.TaxName,

                    OriginRate = originRate / 100,
                    FVARate = ConstConfig.FVARate,
                    Ccc = ccc,
                    Embargo = embargo,
                    HkControl = hkControl,
                    Coo = coo,
                    CIQ = ciq,
                    CIQprice = other?.CIQprice ?? 0,
                    IsHighPrice = false,
                    IsDisinfected = disinfected,

                    IsSysCcc = isSysCcc,
                    IsSysEmbargo = isSysEmbargo,
                    IsSpecialType = isSpecialType,
                    IsPriceFluctuation = isPriceFluctuation
                };
            }
        }

        /// <summary>
        /// 填充归类历史记录
        /// </summary>
        /// <param name="cpn">税则归类信息</param>
        /// <returns></returns>
        public static HistoryClassified FillHistoryClassified(this Models.ClassifiedPartNumber cpn)
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                //根据归类的海关编码获取对应的税则信息
                var tariff = new Views.Alls.TariffsAll(reponsitory)[cpn.HSCode];
                //根据型号、品牌/制造商获取产品归类的特殊类型
                var other = new Views.Alls.OthersAll(reponsitory)[cpn.PartNumber, cpn.Manufacturer];

                return new HistoryClassified()
                {
                    AutoHSCodeID = cpn.ID,
                    PartNumber = cpn.PartNumber,
                    Manufacturer = cpn.Manufacturer,

                    HSCode = cpn.HSCode,
                    TariffName = cpn.TariffName,
                    LegalUnit1 = tariff.LegalUnit1,
                    LegalUnit2 = tariff.LegalUnit2,
                    VATRate = tariff.VATRate / 100,
                    ImportPreferentialTaxRate = tariff.ImportTaxRate / 100,
                    ExciseTaxRate = tariff.ExciseTaxRate.GetValueOrDefault() / 100,
                    Elements = cpn.Elements,
                    CIQCode = cpn.CIQCode,
                    TaxCode = cpn.TaxCode,
                    TaxName = cpn.TaxName,

                    Ccc = other?.Ccc ?? false,
                    Embargo = other?.Embargo ?? false,
                    HkControl = other?.HkControl ?? false,
                    Coo = other?.Coo ?? false,
                    CIQ = other?.CIQ ?? false,
                    CIQprice = other?.CIQprice ?? 0
                };
            }
        }

        /// <summary>
        /// 填充归类信息
        /// </summary>
        /// <param name="cpn">税则归类信息</param>
        /// <returns></returns>
        public static HistoryClassified FillClassifiedInfo(this Models.ClassifiedPartNumber cpn)
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                //根据归类的海关编码获取对应的税则信息
                var tariff = new Views.Alls.TariffsAll(reponsitory)[cpn.HSCode];
                //根据型号、品牌/制造商获取产品归类的特殊类型
                var other = new Views.Alls.OthersAll(reponsitory)[cpn.PartNumber, cpn.Manufacturer];

                return new HistoryClassified()
                {
                    AutoHSCodeID = cpn.ID,
                    PartNumber = cpn.PartNumber,
                    Manufacturer = cpn.Manufacturer,

                    HSCode = cpn.HSCode,
                    TariffName = cpn.TariffName,
                    LegalUnit1 = tariff.LegalUnit1,
                    LegalUnit2 = tariff.LegalUnit2,
                    VATRate = tariff.VATRate / 100,
                    ImportPreferentialTaxRate = tariff.ImportTaxRate / 100,
                    ImportGeneralTaxRate = tariff.ImportGeneralTaxRate / 100,
                    ExciseTaxRate = tariff.ExciseTaxRate.GetValueOrDefault() / 100,
                    ElementsDic = tariff.ElementFormatter(cpn.Elements),
                    CIQCode = cpn.CIQCode,
                    TaxCode = cpn.TaxCode,
                    TaxName = cpn.TaxName,

                    Ccc = other?.Ccc ?? false,
                    Embargo = other?.Embargo ?? false,
                    HkControl = other?.HkControl ?? false,
                    Coo = other?.Coo ?? false,
                    CIQ = other?.CIQ ?? false,
                    CIQprice = other?.CIQprice ?? 0
                };
            }
        }

        /// <summary>
        /// 高价值产品验证
        /// </summary>
        /// <param name="cpn">税则归类信息</param>
        /// <param name="unitPrice">单价</param>
        /// <returns></returns>
        private static bool HighPrice(this Models.ClassifiedPartNumber cpn, decimal unitPrice, PvDataReponsitory reponsitory)
        {
            //二极管、三极管如果单价大于10美元，需要报关员判断是否属于高价值产品
            if (unitPrice > 10)
            {
                string[] tariffNameArr = { "二极管", "三极管", "二極管", "三極管" };
                foreach (var tariffName in tariffNameArr)
                {
                    if (cpn.TariffName.IndexOf(tariffName) >= 0)
                        return true;
                }
            }

            //电阻、电容类产品如果单价超过20美元，需要报关员判断是否属于高价值产品。电阻：8533、电容：8532
            if (unitPrice > 20 && (cpn.HSCode.IndexOf("8533") >= 0 || cpn.HSCode.IndexOf("8532") >= 0))
                return true;

            return false;
        }

        /// <summary>
        /// 价格浮动验证
        /// </summary>
        /// <param name="cpn"></param>
        /// <param name="unitPrice">单价</param>
        /// <param name="isVerifyPriceFluctuation">验证价格浮动</param>
        /// <param name="highPriceLimit">价格高于历史平均价格时的浮动百分比</param>
        /// <param name="lowPriceLimit">价格低于历史平均价格时的浮动百分比</param>
        /// <returns></returns>
        private static bool PriceFluctuation(this Models.ClassifiedPartNumber cpn, decimal unitPrice, bool isVerifyPriceFluctuation, decimal highPriceLimit, decimal lowPriceLimit, PvDataReponsitory reponsitory)
        {
            if (isVerifyPriceFluctuation)
            {
                //查询最近一年的产品报价历史记录
                var productQuotes = new Views.Alls.ProductQuotesAll(reponsitory)[cpn.PartNumber, cpn.Manufacturer, -12].Select(pq => new { pq.UnitPrice });
                if (productQuotes.Count() == 0)
                {
                    productQuotes = new Views.Alls.Logs_ClassifiedPartNumberAll(reponsitory)[cpn.PartNumber, cpn.Manufacturer, -12].Select(cpl => new { cpl.UnitPrice });
                    if (productQuotes.Count() == 0)
                        return true;
                }

                //如果价格浮动超过限制，验证不通过
                var avgPrice = productQuotes.Average(pq => pq.UnitPrice);
                var percentValue = Math.Abs(unitPrice - avgPrice) / avgPrice;

                if ((unitPrice > avgPrice) && (percentValue > highPriceLimit))
                    return true;

                if ((unitPrice < avgPrice) && (percentValue > lowPriceLimit))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 校验申报要素
        /// </summary>
        /// <param name="cpn"></param>
        /// <param name="tariff"></param>
        /// <returns></returns>
        private static bool InconsistentElements(this Models.ClassifiedPartNumber cpn, Models.Tariff tariff)
        {
            try
            {
                string elements = tariff.DeclareElements;
                var elementArr = elements.Split(';');
                var categoryElementArr = cpn.Elements.Split('|');

                //验证税则中的要素数量与历史记录中的申报要素数量是否一致，防止每年税则更新后，税号有增减要素的情况
                //+1是因为税则的申报要素没有包含“其他”
                if ((elementArr.Length + 1) != categoryElementArr.Length)
                    return true;

                for (int i = 0; i < elementArr.Length; i++)
                {
                    var arr = elementArr[i].Split(':');
                    var element = categoryElementArr[i];

                    if (arr[1] == "品牌" || arr[1] == "品牌（中文或外文名称）")
                    {
                        var mfr = element.Split('牌')[0];
                        if (mfr.ToLower().Trim() != cpn.Manufacturer.ToLower().Trim())
                            return true;
                    }
                    if (arr[1] == "品牌（中文及外文名称）")
                    {
                        //2021年申报要素中“品牌”调整为“品牌（中文及外文名称）”
                        int index = element.IndexOf('/') + 1;
                        //不符合格式要求
                        if (index == 0)
                            return true;
                        var mfr = element.Substring(index, element.Length - index - 1);
                        //品牌不一致
                        if (mfr.ToLower().Trim() != cpn.Manufacturer.ToLower().Trim())
                            return true;
                    }
                    if (arr[1] == "型号")
                    {
                        //新需求，申报要素中移除了“型号:”, 比如将“|型号:Ad620|”修改为了“|Ad620|”
                        //为防止历史数据修改有遗漏，自动归类暂时兼容两种格式
                        string pn;
                        if (element.Contains(':'))
                            pn = element.Split(':')[1];
                        else
                            pn = element;
                        if (pn.ToLower().Trim() != cpn.PartNumber.ToLower().Trim())
                            return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 修正自动归类记录
        /// </summary>
        /// <param name="cpn">自动归类记录</param>
        /// <param name="tariff">海关税则</param>
        /// <param name="reponsitory">数据库连接</param>
        /// <returns></returns>
        private static string AmendClassified(this Models.ClassifiedPartNumber cpn, Models.Tariff tariff, PvDataReponsitory reponsitory)
        {
            string cpnID = string.Concat(cpn.PartNumber, cpn.Manufacturer.ToLower(), cpn.HSCode, cpn.TariffName,
                                        tariff.LegalUnit1, tariff.LegalUnit2,
                                        tariff.VATRate / 100,
                                        tariff.ImportTaxRate / 100,
                                        tariff.ExciseTaxRate / 100,
                                        cpn.Elements,

                                        cpn.SupervisionRequirements,
                                        cpn.CIQC,

                                        cpn.CIQCode,
                                        cpn.TaxCode,
                                        cpn.TaxName).MD5();

            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>().Any(t => t.ID == cpnID))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvData.ClassifiedPartNumbers()
                {
                    ID = cpnID,
                    PartNumber = cpn.PartNumber,
                    Manufacturer = cpn.Manufacturer,
                    HSCode = cpn.HSCode,
                    Name = cpn.TariffName,
                    LegalUnit1 = tariff.LegalUnit1,
                    LegalUnit2 = tariff.LegalUnit2,
                    VATRate = tariff.VATRate / 100,
                    ImportPreferentialTaxRate = tariff.ImportTaxRate / 100,
                    ExciseTaxRate = tariff.ExciseTaxRate.GetValueOrDefault() / 100,
                    Elements = cpn.Elements,
                    SupervisionRequirements = cpn.SupervisionRequirements,
                    CIQC = cpn.CIQC,
                    CIQCode = cpn.CIQCode,
                    TaxCode = cpn.TaxCode,
                    TaxName = cpn.TaxName,
                    CreateDate = DateTime.Now,
                    OrderDate = DateTime.Now
                });
            }
            //修改
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>(new
                {
                    OrderDate = DateTime.Now
                }, a => a.ID == cpnID);
            }

            return cpnID;
        }
    }

    /// <summary>
    /// 归类历史记录
    /// </summary>
    public class HistoryClassified
    {
        public string AutoHSCodeID { get; set; }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }

        public string HSCode { get; set; }
        public string TariffName { get; set; }
        public string LegalUnit1 { get; set; }
        public string LegalUnit2 { get; set; }
        public decimal VATRate { get; set; }
        public decimal ImportPreferentialTaxRate { get; set; }
        public decimal ImportGeneralTaxRate { get; set; }
        public decimal ExciseTaxRate { get; set; }
        public string Elements { get; set; }
        public Dictionary<string, string> ElementsDic { get; set; }
        public string CIQCode { get; set; }
        public string TaxCode { get; set; }
        public string TaxName { get; set; }

        public decimal OriginRate { get; set; }
        public decimal FVARate { get; set; }
        public bool Ccc { get; set; }
        public bool Embargo { get; set; }
        public bool HkControl { get; set; }
        public bool Coo { get; set; }
        public bool CIQ { get; set; }
        public decimal CIQprice { get; set; }
        public bool IsHighPrice { get; set; }
        public bool IsDisinfected { get; set; }


        public bool IsSysCcc { get; set; }
        public bool IsSysEmbargo { get; set; }
        public bool IsSpecialType { get; set; }
        public bool IsPriceFluctuation { get; set; }
    }
}
