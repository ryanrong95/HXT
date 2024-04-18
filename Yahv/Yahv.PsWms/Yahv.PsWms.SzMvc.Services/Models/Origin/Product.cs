using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    /// <summary>
    /// 产品
    /// </summary>
    public class Product : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Partnumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// 批次,存储的值 202050, 19+
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 最小包装量
        /// </summary>
        public int? Mpq { get; set; }

        /// <summary>
        /// 最小起订量
        /// </summary>
        public int? Moq { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }
        #endregion
    }
}
