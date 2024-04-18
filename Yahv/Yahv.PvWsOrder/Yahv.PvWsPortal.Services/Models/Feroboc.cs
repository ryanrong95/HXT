using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsPortal.Services.Models
{
    /// <summary>
    /// 实时汇率
    /// </summary>
    public class Feroboc : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 汇率类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public int Currency { get; set; }

        /// <summary>
        /// 现汇买入
        /// </summary>
        public decimal? Xhmr { get; set; }

        /// <summary>
        /// 现钞买入
        /// </summary>
        public decimal? Xcmr { get; set; }

        /// <summary>
        /// 现汇卖出
        /// </summary>
        public decimal? Xhmc { get; set; }

        /// <summary>
        /// 现钞卖出
        /// </summary>
        public decimal? Xcmc { get; set; }

        /// <summary>
        /// 中行折算价
        /// </summary>
        public decimal? Zhzsj { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishDate { get; set; }
        #endregion
    }
}
