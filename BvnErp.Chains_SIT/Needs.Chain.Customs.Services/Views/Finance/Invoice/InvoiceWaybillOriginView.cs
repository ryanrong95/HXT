using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 开票申请的视图
    /// </summary>
    public class InvoiceWaybillOriginView : UniqueView<Models.InvoiceWaybill, ScCustomsReponsitory>
    {
        public InvoiceWaybillOriginView()
        {
        }

        internal InvoiceWaybillOriginView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceWaybill> GetIQueryable()
        {
            

            var result = from invoiceWaybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>()                      
                         select new Models.InvoiceWaybill
                         {
                             ID = invoiceWaybill.InvoiceNoticeID,                                
                             WaybillCode = invoiceWaybill.WaybillCode,
                             CreateDate= invoiceWaybill.CreateDate,
                         };
            return result;
        }
    }
}
