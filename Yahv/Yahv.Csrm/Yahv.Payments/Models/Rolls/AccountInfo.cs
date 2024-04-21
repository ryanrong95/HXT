using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments.Models.Rolls
{
    /// <summary>
    /// 流水账统计信息
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// 付款公司ID
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 收款公司ID
        /// </summary>
        public string Payee { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Business { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

    }
}
