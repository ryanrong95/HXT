using Yahv.Underly;

namespace Yahv.Payments.Models
{
    public class XdtFee
    {
        public XdtFee()
        {
        }

        public XdtFee(string catalog, string subject, Currency currency, decimal price)
        {
            Catalog = catalog;
            Subject = subject;
            Currency = currency;
            Price = price;
        }

        public XdtFee(string catalog, string subject, Currency currency, decimal price, string itemID)
        {
            Catalog = catalog;
            Subject = subject;
            Currency = currency;
            Price = price;
            ItemID = itemID;
        }

        public string Catalog { get; set; }
        public string Subject { get; set; }

        /// <summary>
        /// 币种 ， 就是我们记录发生币种。具体结算币种还是依据Order获取。
        /// </summary>
        public Currency Currency { get; set; }

        public decimal Price { get; set; }
        /// <summary>
        /// 型号ID
        /// </summary>
        public string ItemID { get; set; }
    }

    /// <summary>
    /// 申请费用
    /// </summary>
    public class ApplyFee
    {
        public ApplyFee() { }

        public ApplyFee(string orderId, string catalog, string subject, decimal? price)
        {
            this.OrderID = orderId;
            this.Catalog = catalog;
            this.Subject = subject;
            this.Price = price;
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Price { get; set; }
    }
}
