using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Models
{
    public interface IStandardPartnumberForShow
    {
        /// <summary>
        /// 型号
        /// </summary>
        string Partnumber { get; set; }

        /// <summary>
        /// 是否为冷偏型号
        /// </summary>
        bool IsUnpopular { get; set; }
    }

    /// <summary>
    /// 有完整归类信息的
    /// </summary>
    public class StandardPartnumberForShow : IStandardPartnumberForShow
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string Partnumber { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }
        
        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 3C
        /// </summary>
        public bool IsCcc { get; set; }

        /// <summary>
        /// 禁运
        /// </summary>
        public bool IsEmbargo { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal CIQprice { get; set; }

        /// <summary>
        /// Eccn
        /// </summary>
        /// <remarks>
        /// 直接返回Eccn编码
        /// </remarks>
        public string Eccn { get; set; }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal TariffRate { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal VATRate { get; set; }

        /// <summary>
        /// 附加关税率
        /// </summary>
        public decimal AddedTariffRate { get; set; }

        /// <summary>
        /// 是否为冷偏型号
        /// </summary>
        public bool IsUnpopular { get; set; }
    }

    /// <summary>
    /// 没有归类信息，仅从[PvData].[dbo].[StandardPartnumbers]返回标准型号
    /// </summary>
    public class StandardPartnumberOnly : IStandardPartnumberForShow
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string Partnumber { get; set; }

        /// <summary>
        /// 是否为冷偏型号
        /// </summary>
        public bool IsUnpopular { get; set; }
    }
}
