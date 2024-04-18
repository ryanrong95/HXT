using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Statistics
{
    /// <summary>
    /// 客户拜访数
    /// </summary>
    public class ClientVisit : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报告创建人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 指定年月
        /// </summary>
        public int DateIndex { get; set; }

        /// <summary>
        /// 每月的有效拜访数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion
    }
}
