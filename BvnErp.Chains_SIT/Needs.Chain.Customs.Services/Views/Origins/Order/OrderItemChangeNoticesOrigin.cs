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
    internal class OrderItemChangeNoticesOrigin : UniqueView<Models.OrderItemChangeNotice, ScCustomsReponsitory>
    {
        internal OrderItemChangeNoticesOrigin()
        {
        }

        internal OrderItemChangeNoticesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderItemChangeNotice> GetIQueryable()
        {
            return from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>()
                   select new Models.OrderItemChangeNotice
                   {
                       ID = notice.ID,
                       OrderItemID = notice.OrderItemID,
                       SorterID = notice.AdminID,
                       Type = (Enums.OrderItemChangeType)notice.Type,
                       OldValue = notice.OldValue,
                       NewValue = notice.NewValue,
                       IsSplited = notice.IsSplited,
                       ProcessState = (Enums.ProcessState)notice.ProcessStatus,
                       Status = (Enums.Status)notice.Status,
                       CreateDate = notice.CreateDate,
                       UpdateDate = notice.UpdateDate,
                       TriggerSource = (Enums.TriggerSource)notice.TriggerSource,
                   };
        }
    }
}
