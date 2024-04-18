using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Alls
{
    public class OrderItemChangeLogsAll : UniqueView<Models.OrderItemChangeLog, ScCustomsReponsitory>
    {
        public OrderItemChangeLogsAll()
        {
        }

        internal OrderItemChangeLogsAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderItemChangeLog> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var logsView = new Origins.OrderItemChangeLogsOrigin(this.Reponsitory);

            return from log in logsView
                   join admin in adminsView on log.AdminID equals admin.ID
                   select new Models.OrderItemChangeLog
                   {
                       ID = log.ID,
                       OrderID = log.OrderID,
                       OrderItemID = log.OrderItemID,
                       AdminID = log.AdminID,
                       Admin = admin,
                       Type = log.Type,
                       CreateDate = log.CreateDate,
                       Summary = log.Summary
                   };
        }
    }
}
