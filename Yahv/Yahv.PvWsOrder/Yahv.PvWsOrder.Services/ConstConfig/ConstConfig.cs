using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.ConstConfig
{
    /// <summary>
    /// 导出发票XML(chaowang：目前和芯达通保持一致)
    /// </summary>
    public class InvoiceXmlConfig
    {
        //版本号
        public const string Version = "2.0";
        //复核人
        public const string Fhr = "鲁亚慧";
        //收款人
        public const string Skr = "姚安予";
        //开票人
        public const string Kpr = "郝红梅";
        //商品编码版本号(20 字节)（商品版本号是你开票机的版本号）
        public const string Spbmbbh = "33.0";
        //含税标志 0：不含税税率，1：含税税率，2：差额税;中外合作油气田（原海洋石油）5%税率、1.5%税率为 1，差额税为 2，其他为 0；
        public const string Hsbz = "1";
    }

    /// <summary>
    /// 配置 or 初始化常量信息
    /// </summary>
    public class ConstConfig
    {
        /// <summary>
        /// 海关运保杂费
        /// </summary>
        public const decimal TransPremiumInsurance = 1.002M;

        /// <summary>
        /// 增值税率
        /// </summary>
        public const decimal ValueAddedTaxRate = 0.13M;

        /// <summary>
        /// 单个型号最小装箱毛重
        /// </summary>
        public const decimal MinPackingGrossWeight = 0.02M;

        /// <summary>
        /// 单个型号最小装箱净重
        /// </summary>
        public const decimal MinPackingNetWeight = 0.01M;

        /// <summary>
        /// 报关单最小毛重
        /// </summary>
        public const decimal MinDecHeadGrossWeight = 2M;

        /// <summary>
        /// 报关单最小净重
        /// </summary>
        public const decimal MinDecHeadNetWeight = 1M;

        /// <summary>
        /// 电子随附单据问价类型
        /// </summary>
        public const string EdocFomatType = "US";

        /// <summary>
        /// 电子单据-发票
        /// </summary>
        public const string PaymentInstruction = "00000001";

        /// <summary>
        /// 电子单据-装箱单
        /// </summary>
        public const string PackingList = "00000002";

        /// <summary>
        /// 电子单据-合同
        /// </summary>
        public const string Contract = "00000004";

        /// <summary>
        /// 换汇管控国家
        /// </summary>
        public static readonly string[] SwapLimitCountry = { "AFG", "BDI", "BLR", "IRQ", "LAO", "LBN", "ROU", "SVN", "SOM", "UKR", "UGA", "VEN", "YEM", "ZWE", "SSD", "ERI", "GNB", "RUS", "COG", "COD", "CAF", "ALB", "BGR", "BIH", "GRC", "MKD", "ROU", "TUR" };

    }
}
