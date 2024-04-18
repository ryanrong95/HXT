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
    /// 付款通知明细的视图，也可作为订单的应收实收视图
    /// </summary>
    public class PaymentApplyLogView : UniqueView<Models.PaymentApplyLog, ScCustomsReponsitory>
    {
        public PaymentApplyLogView()
        {
        }

        internal PaymentApplyLogView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PaymentApplyLog> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var result = from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentApplyLogs>()
                         join admin in adminView on log.AdminID equals admin.ID
                         select new Models.PaymentApplyLog
                         {
                             ID = log.ID,
                             PaymentApplyID = log.PaymentApplyID,
                             Admin = admin,
                             PaymentApplyStatus = (Enums.PaymentApplyStatus)log.PaymentApplyStatus,
                             CreateDate = log.CreateDate,
                             Summary = log.Summary,
                         };
            return result;
        }
    }
}
