using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 核销信息 给 中心
    /// </summary>
    public class CenterReceipt
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; }
        /// <summary>
        /// 核销金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 核销类型
        /// </summary>
        public string FeeType { get; set; }
        /// <summary>
        /// 核销人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string AccountNo { get; set; }

    }
}
