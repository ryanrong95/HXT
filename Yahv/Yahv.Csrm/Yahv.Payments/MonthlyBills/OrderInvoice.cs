using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 后期一定向   Underly.InvoiceType 进行统一
    /// </remarks>
    public enum InvoiceType
    {
        /// <summary>
        /// 增值税全额发票
        /// </summary>
        VATInvoice = 0,

        /// <summary>
        /// 服务费发票
        /// </summary>
        Servicing = 1
    }

    /// <summary>
    /// 月结相关【票据】
    /// </summary>
    public class RelatedInvoice
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }
        /// <summary>
        /// 应开金额
        /// </summary>
        public decimal? LeftPrice { get; set; }
        /// <summary>
        /// 差额（可正可负）
        /// </summary>
        public decimal? Difference { get; set; }
        /// <summary>
        /// 实开金额
        /// </summary>
        public decimal? RightPrice { get; set; }


        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public InvoiceType InvoiceType { get; set; }

        public bool IsOpened { get; set; }
        public string ClientName { get; set; }

        public decimal? CustomsLeft { get; set; }
        public decimal? CustomsRight { get; set; }

        public DateTime? CustomsInvoiceDate { get; set; }
       

    }
}
