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
    public class InvoiceWaybillView : UniqueView<Models.InvoiceWaybill, ScCustomsReponsitory>
    {
        public InvoiceWaybillView()
        {
        }

        internal InvoiceWaybillView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceWaybill> GetIQueryable()
        {
            var noticeView = new InvoiceNoticeView(this.Reponsitory);

            var result = from invoiceWaybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>()
                         join notice in noticeView on invoiceWaybill.InvoiceNoticeID equals notice.ID into notices
                         from notice in notices.DefaultIfEmpty()
                         join carriers in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>() on invoiceWaybill.CompanyName equals carriers.ID
                         select new Models.InvoiceWaybill
                         {
                             ID = invoiceWaybill.ID,
                             InvoiceNotice = notice,
                             CompanyName = carriers.Name,
                             WaybillCode = invoiceWaybill.WaybillCode,
                             CreateDate= invoiceWaybill.CreateDate,
                         };
            return result;
        }
    }
}
