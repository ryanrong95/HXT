using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 税务信息
    /// </summary>
    public class TaxRule : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxFirstCategory { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxSecondCategory { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxThirdCategory { get; set; }

        /// <summary>
        /// 数据状态：正常、删除
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        #endregion

        /// <summary>
        /// 构造器，内部查询使用
        /// </summary>
        internal TaxRule()
        {
        }
    }
}
