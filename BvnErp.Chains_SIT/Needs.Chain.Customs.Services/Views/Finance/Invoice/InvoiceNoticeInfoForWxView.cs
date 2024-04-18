using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class InvoiceNoticeInfoForWxView : QueryView<InvoiceNoticeInfoForWxViewModel, ScCustomsReponsitory>
    {
        private string _invoiceNoticeID { get; set; }

        public InvoiceNoticeInfoForWxView(string invoiceNoticeID)
        {
            this._invoiceNoticeID = invoiceNoticeID;
        }

        public InvoiceNoticeInfoForWxView(ScCustomsReponsitory reponsitory, IQueryable<InvoiceNoticeInfoForWxViewModel> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<InvoiceNoticeInfoForWxViewModel> GetIQueryable()
        {
            var invoiceNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();

            var iQuery = from invoiceNotice in invoiceNotices
                         where invoiceNotice.ID == this._invoiceNoticeID
                         select new InvoiceNoticeInfoForWxViewModel
                         {
                             InvoiceNoticeID = invoiceNotice.ID,
                             InvoiceType = (Enums.InvoiceType)invoiceNotice.InvoiceType,
                             ClientID = invoiceNotice.ClientID,
                             BankName = invoiceNotice.BankName,
                             BankAccount = invoiceNotice.BankAccount,
                             Summary = invoiceNotice.Summary,
                         };

            return iQuery;
        }

        public InvoiceNoticeInfoForWxViewModel GetInvoiceNoticeInfo()
        {
            IQueryable<InvoiceNoticeInfoForWxViewModel> iquery = this.IQueryable.Cast<InvoiceNoticeInfoForWxViewModel>();

            var theInvoiceNotice = iquery.FirstOrDefault();

            #region 公司名、开户行、账号

            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var theClientModel = (from client in clients
                                  join company in companies on client.CompanyID equals company.ID
                                  where client.ID == theInvoiceNotice.ClientID
                                  select new
                                  {
                                      ClientID = client.ID,
                                      CompanyName = company.Name,
                                  }).FirstOrDefault();

            #endregion

            #region 纳税人识别号

            var clientInvoice = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>();

            var theClientInvoiceModel = clientInvoice.Where(t => t.ClientID == theInvoiceNotice.ClientID && t.Status == (int)Enums.Status.Normal)
                                                     .Select(item => new { TaxCode = item.TaxCode, }).FirstOrDefault();

            #endregion

            #region 含税金额、开票差额

            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();

            var theSumModel = (from invoiceNoticeItem in invoiceNoticeItems
                               where invoiceNoticeItem.InvoiceNoticeID == theInvoiceNotice.InvoiceNoticeID
                                  && invoiceNoticeItem.Status == (int)Enums.Status.Normal
                               group invoiceNoticeItem by new { invoiceNoticeItem.InvoiceNoticeID } into g
                               select new
                               {
                                   InvoiceNoticeID = g.Key.InvoiceNoticeID,
                                   Amount = g.Sum(t => t.Amount),
                                   Difference = g.Sum(t => t.Difference),
                               }).FirstOrDefault();

            #endregion

            theInvoiceNotice.CompanyName = theClientModel?.CompanyName;
            theInvoiceNotice.TaxCode = theClientInvoiceModel?.TaxCode;
            theInvoiceNotice.Amount = theSumModel?.Amount ?? 0;
            theInvoiceNotice.Difference = theSumModel?.Difference ?? 0;

            return theInvoiceNotice;
        }
    }

    public class InvoiceNoticeInfoForWxViewModel
    {
        public string InvoiceNoticeID { get; set; }

        public Enums.InvoiceType InvoiceType { get; set; }

        public string ClientID { get; set; }

        public string CompanyName { get; set; }

        public string BankName { get; set; }

        public string BankAccount { get; set; }

        public string TaxCode { get; set; }

        public string Summary { get; set; }

        public decimal Amount { get; set; }

        public decimal Difference { get; set; }
    }
}
