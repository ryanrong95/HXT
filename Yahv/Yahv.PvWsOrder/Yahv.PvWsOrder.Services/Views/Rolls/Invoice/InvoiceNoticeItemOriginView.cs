using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    public class InvoiceNoticeItemOriginView : QueryView<InvoiceNoticeItemOriginViewModel, PvWsOrderReponsitory>
    {
        public InvoiceNoticeItemOriginView()
        {
        }

        protected InvoiceNoticeItemOriginView(PvWsOrderReponsitory reponsitory, IQueryable<InvoiceNoticeItemOriginViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<InvoiceNoticeItemOriginViewModel> GetIQueryable()
        {
            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems>();

            var iQuery = from invoiceNoticeItem in invoiceNoticeItems          
                         select new InvoiceNoticeItemOriginViewModel
                         {
                             ID = invoiceNoticeItem.ID,
                             InvoiceNoticeID = invoiceNoticeItem.InvoiceNoticeID,
                             BillID = invoiceNoticeItem.BillID,
                             UnitPrice = invoiceNoticeItem.UnitPrice,
                             Quantity = invoiceNoticeItem.Quantity,
                             Amount = invoiceNoticeItem.Amount,
                             Difference = invoiceNoticeItem.Difference,
                             InvoiceNo = invoiceNoticeItem.InvoiceNo,
                             Status = (GeneralStatus)invoiceNoticeItem.Status,
                             CreateDate = invoiceNoticeItem.CreateDate,
                             Summary = invoiceNoticeItem.Summary,
                         };

            return iQuery;
        }
    }

    public class InvoiceNoticeItemOriginViewModel
    {
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
    }
}
