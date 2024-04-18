using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Wl.Environment.Models
{
    /// <summary>
    /// 单据前缀
    /// </summary>
    public class PreFix
    {
        /// <summary>
        /// 合同号前缀
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 商检合同号前缀
        /// </summary>
        public string SJContractNo { get; set; }

        /// <summary>
        /// 提运单号前缀
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 商检提运单号前缀
        /// </summary>
        public string SJBillNo { get; set; }

        /// <summary>
        /// 报关单表头ID前缀
        /// </summary>
        public string DecHeadID { get; set; }
    }
}