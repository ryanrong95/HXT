using Needs.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 订单的发票信息
    /// </summary>
    public class OrderInvoice : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 物料号
        /// </summary>
        public string ProductUnionID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// （计量）单位
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 开票差额
        /// </summary>
        public decimal Difference { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }
    }
}
