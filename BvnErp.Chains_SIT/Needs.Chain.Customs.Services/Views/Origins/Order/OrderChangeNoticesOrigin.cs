using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class OrderChangeNoticesOrigin : UniqueView<Models.OrderChangeNotice, ScCustomsReponsitory>
    {
        public OrderChangeNoticesOrigin()
        {
        }

        public OrderChangeNoticesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderChangeNotice> GetIQueryable()
        {
            return from orderChangeNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>()
                   select new Models.OrderChangeNotice
                   {
                       ID = orderChangeNotice.ID,
                       OrderID = orderChangeNotice.OderID,
                       Type = (Enums.OrderChangeType)orderChangeNotice.Type,
                       processState = (Enums.ProcessState)orderChangeNotice.ProcessState,
                       Status = (Enums.Status)orderChangeNotice.Status,
                       CreateDate = orderChangeNotice.CreateDate,
                       UpdateDate = orderChangeNotice.UpdateDate,
                   };
        }
    }
}
