
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
详细解释：
查看京东的支付就可以看出，支付是有来源，有去向的。
因此，当我们开发的时候面向[[B端客户]]。我们理应展示：我们收款银行账户、支付宝、paypal、现金等信息。
并展示我是如何收到客户的款，就是客户支付的手段：银行账户、支付宝、paypal、现金
类型:支付宝、paypal、银行转账、商兑汇票、支票、现金等。
因此还需要流水单号，可能是我们自己的也可能是流水单号

总结：
Payer、Payee等可能都是负载类型。
*/

namespace NtErp.Wss.Sales.Services.Model.Orders
{
    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PaymentMethod
    {
        [Naming("余额")]
        Wallet = 1,
        [Naming("信用")]
        Credit = 2,
        [Naming("Paypal")]
        Paypal = 3,
        [Naming("TT")]
        TT = 4,
        [Naming("Alipay")]
        Alipay = 5,
    }

    /// <summary>
    /// 收据
    /// </summary>
    public class Receipt
    {
        protected Order father;

        /// <summary>
        /// 方式
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string Drawee { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Amount { get; set; }

        Currency currency;
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency
        {
            get
            {
                if (father != null)
                {
                    this.currency = father.Currency;
                }
                return this.currency;
            }
            set
            {
                this.currency = value;
            }
        }

        /// <summary>
        /// 国际化时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string Summary { get; set; }

        public Receipt()
        {
            this.CreateTime = DateTime.Now;
        }

    }
}
