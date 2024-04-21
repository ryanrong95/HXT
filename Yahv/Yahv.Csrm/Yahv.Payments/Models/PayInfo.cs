using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments
{
    /// <summary>
    /// 付款信息
    /// </summary>
    public class PayInfo
    {
        /// <summary>
        /// 付款人企业ID
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 付款人ID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 匿名付款人
        /// </summary>
        public string PayerAnonymous { get; set; }

        /// <summary>
        /// 收款人企业ID
        /// </summary>
        public string Payee { get; set; }

        /// <summary>
        /// 收款人ID
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 匿名收款人
        /// </summary>
        public string PayeeAnonymous { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public Inputer Inputer { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }
    }
}
