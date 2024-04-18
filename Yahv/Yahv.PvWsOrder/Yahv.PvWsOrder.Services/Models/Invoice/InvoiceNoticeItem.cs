using Layers.Data.Sqls;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models
{
    public class InvoiceNoticeItem : IUnique
    {
        #region 属性
        public string ID { get; set; }
        public string InvoiceNoticeID { get; set; }
        public string BillID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal? Difference { get; set; }
        public string InvoiceNo { get; set; }
        public GeneralStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string Summary { get; set; }
        #endregion
        public InvoiceNoticeItem()
        {
            this.CreateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }
    }
}
