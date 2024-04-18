using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Models
{
    /// <summary>
    /// 产品询价参考价
    /// </summary>
    public class EnquiryReference
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EnquiryReference()
        {

        }

        #region 属性
        /// <summary>
        /// 型号
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 原厂型号
        /// </summary>
        public string OriginModel { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 最小起订量
        /// </summary>
        public int? MOQ { get; set; }

        /// <summary>
        /// 最小包装量
        /// </summary>
        public int? MPQ { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public CurrencyType? Currency { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? Validity { get; set; }

        /// <summary>
        /// 有效数量
        /// </summary>
        public int? ValidityCount { get; set; }

        /// <summary>
        /// 参考售价
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// 特殊备注
        /// </summary>
        public string Summary { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 币种描述
        /// </summary>
        public string CurrencyDes
        {
            get
            {
                return Currency?.GetDescription();
            }
        }
        #endregion
    }
}
