using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Views
{
    public class WayBillCodeUpdater
    {
        private string _wayBillCode { get; set; }

        private string _carrier { get; set; }

        private string[] _invoiceNoticeIDs { get; set; }

        public WayBillCodeUpdater(string wayBillCode, string carrier, string[] invoiceNoticeIDs)
        {
            this._wayBillCode = wayBillCode;
            this._carrier = carrier;
            this._invoiceNoticeIDs = invoiceNoticeIDs;
        }

        public void Update()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>(new
                {
                    WayBillCode = this._wayBillCode,
                    Carrier = this._carrier,
                }, item => this._invoiceNoticeIDs.Contains(item.ID));
            }
        }
    }
}
