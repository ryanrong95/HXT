using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    public class InvoiceNoticeOriginView : QueryView<InvoiceNoticeOriginViewModel, PvWsOrderReponsitory>
    {
        public InvoiceNoticeOriginView()
        {
        }

        protected InvoiceNoticeOriginView(PvWsOrderReponsitory reponsitory, IQueryable<InvoiceNoticeOriginViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<InvoiceNoticeOriginViewModel> GetIQueryable()
        {
            var invoiceNotices = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>();

            var iQuery = from invoiceNotice in invoiceNotices
                         select new InvoiceNoticeOriginViewModel
                         {
                             ID = invoiceNotice.ID,
                             ClientID = invoiceNotice.ClientID,
                             IsPersonal = invoiceNotice.IsPersonal,
                             FromType = (InvoiceFromType)invoiceNotice.FromType,
                             Type = (InvoiceType)invoiceNotice.Type,
                             Title = invoiceNotice.Title,
                             TaxNumber = invoiceNotice.TaxNumber,
                             RegAddress = invoiceNotice.RegAddress,
                             Tel = invoiceNotice.Tel,
                             BankName = invoiceNotice.BankName,
                             BankAccount = invoiceNotice.BankAccount,
                             PostAddress = invoiceNotice.PostAddress,
                             PostRecipient = invoiceNotice.PostRecipient,
                             PostTel = invoiceNotice.PostTel,
                             DeliveryType = (InvoiceDeliveryType)invoiceNotice.DeliveryType,
                             Carrier = invoiceNotice.Carrier,
                             WayBillCode = invoiceNotice.WayBillCode,
                             Status = (InvoiceEnum)invoiceNotice.Status,
                             InvoiceDate = invoiceNotice.InvoiceDate,
                             AdminID = invoiceNotice.AdminID,
                             CreateDate = invoiceNotice.CreateDate,
                             UpdateDate = invoiceNotice.UpdateDate,
                             Summary = invoiceNotice.Summary,

                         };

            return iQuery;
        }
    }

    public class InvoiceNoticeOriginViewModel
    {
        public string ID { get; set; }
        public string ClientID { get; set; }
        public bool IsPersonal { get; set; }
        public InvoiceFromType FromType { get; set; }
        public InvoiceType Type { get; set; }
        public string Title { get; set; }
        public string TaxNumber { get; set; }
        public string RegAddress { get; set; }
        public string Tel { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string PostAddress { get; set; }
        public string PostRecipient { get; set; }
        public string PostTel { get; set; }
        public InvoiceDeliveryType DeliveryType { get; set; }
        public string Carrier { get; set; }
        public string WayBillCode { get; set; }
        public InvoiceEnum Status { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string AdminID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
    }
}
