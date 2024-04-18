using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class InvoiceXmlMapView : UniqueView<Models.InvoiceNoticeXmlMap, ScCustomsReponsitory>
    {
        public InvoiceXmlMapView()
        {
        }

        public InvoiceXmlMapView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.InvoiceNoticeXmlMap> GetIQueryable()
        {
            

            var result = from invoiceWaybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlMap>()                       
                         select new Models.InvoiceNoticeXmlMap
                         {
                             ID = invoiceWaybill.ID,
                             InvoiceXmlID = invoiceWaybill.InvoiceXmlID,
                             DecListID = invoiceWaybill.DecListID,                           
                             OutQty = invoiceWaybill.OutQty,
                             Status = (Enums.Status)invoiceWaybill.Status,
                             CreateDate = invoiceWaybill.CreateDate,
                         };
            return result;
        }
    }
}
