using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 收款通知
    /// </summary>
    public class ReceiptNotice : ModelBase<Layer.Data.Sqls.ScCustoms.ReceiptNotices, ScCustomsReponsitory>, IUnique, IPersist
    {
        public string ClientID { get; set; }

        /// <summary>
        /// 已经入账金额
        /// </summary>
        public decimal ClearAmount { get; set; }
    }
}