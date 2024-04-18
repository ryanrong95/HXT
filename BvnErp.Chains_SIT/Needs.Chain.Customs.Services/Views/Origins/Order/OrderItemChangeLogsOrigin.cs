using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class OrderItemChangeLogsOrigin : UniqueView<Models.OrderItemChangeLog, ScCustomsReponsitory>
    {
        internal OrderItemChangeLogsOrigin()
        {
        }

        internal OrderItemChangeLogsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderItemChangeLog> GetIQueryable()
        {
            return from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemChangeLogs>()
                   select new Models.OrderItemChangeLog
                   {
                       ID = log.ID,
                       OrderID = log.OrderID,
                       OrderItemID = log.OrderItemID,
                       AdminID = log.AdminID,
                       Type = (Enums.OrderItemChangeType)log.Type,
                       CreateDate = log.CreateDate,
                       Summary = log.Summary
                   };
        }
    }
}
