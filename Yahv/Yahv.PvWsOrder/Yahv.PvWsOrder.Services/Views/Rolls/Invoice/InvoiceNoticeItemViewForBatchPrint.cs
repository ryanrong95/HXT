using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Views
{
    public class InvoiceNoticeItemViewForBatchPrint : QueryView<InvoiceNoticeItemViewForBatchPrintModel, PvWsOrderReponsitory>
    {
        private string[] _invoiceNoticeIDs { get; set; }

        public InvoiceNoticeItemViewForBatchPrint(string[] invoiceNoticeIDs)
        {
            this._invoiceNoticeIDs = invoiceNoticeIDs;
        }

        protected InvoiceNoticeItemViewForBatchPrint(PvWsOrderReponsitory reponsitory, IQueryable<InvoiceNoticeItemViewForBatchPrintModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<InvoiceNoticeItemViewForBatchPrintModel> GetIQueryable()
        {
            var invoiceNotices = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>();
            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems>();

            var iQuery = from invoiceNoticeItem in invoiceNoticeItems
                         join invoiceNotice in invoiceNotices
                               on invoiceNoticeItem.InvoiceNoticeID equals invoiceNotice.ID
                         where this._invoiceNoticeIDs.Contains(invoiceNoticeItem.InvoiceNoticeID)
                         select new InvoiceNoticeItemViewForBatchPrintModel
                         {
                             InvoiceNoticeItemID = invoiceNoticeItem.ID,
                             InvoiceNoticeID = invoiceNoticeItem.InvoiceNoticeID,
                             ClientID = invoiceNotice.ClientID,
                             InvoiceNo = invoiceNoticeItem.InvoiceNo,
                             Amount = invoiceNoticeItem.Amount,
                         };

            return iQuery;
        }
    }

    public class InvoiceNoticeItemViewForBatchPrintModel
    {
        public string InvoiceNoticeItemID { get; set; }
        public string InvoiceNoticeID { get; set; }
        public string ClientID { get; set; }
        public string InvoiceNo { get; set; }
        public decimal Amount { get; set; }
    }
}
