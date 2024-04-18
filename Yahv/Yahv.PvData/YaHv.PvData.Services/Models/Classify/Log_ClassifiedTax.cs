using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 税务归类历史记录
    /// </summary>
    public class Log_ClassifiedTax : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        ///// <summary>
        ///// 排序
        ///// </summary>
        //public int OrderIndex { get; set; }

        #endregion

        /// <summary>
        /// 构造器，内部查询使用
        /// </summary>
        internal Log_ClassifiedTax()
        {
        }
    }
}
