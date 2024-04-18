using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 暂存日志View
    /// </summary>
    public class InvoiceNoticeLogView : UniqueView<Models.InvoiceNoticeLog, ScCustomsReponsitory>
    {
        public InvoiceNoticeLogView()
        {
        }

        internal InvoiceNoticeLogView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.InvoiceNoticeLog> GetIQueryable()
        {
            var adminView = new Views.AdminsTopView(this.Reponsitory);

            return from invoiceNoticeLog in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeLogs>()
                   join admin in adminView on invoiceNoticeLog.AdminID equals admin.ID
                   select new Models.InvoiceNoticeLog
                   {
                       ID = invoiceNoticeLog.ID,
                       InvoiceNoticeID = invoiceNoticeLog.InvoiceNoticeID,
                       Admin = admin,
                       Status = (Enums.InvoiceNoticeStatus)invoiceNoticeLog.Status,
                       CreateDate = invoiceNoticeLog.CreateDate,
                       Summary = invoiceNoticeLog.Summary,
                   };
        }
    }
}
