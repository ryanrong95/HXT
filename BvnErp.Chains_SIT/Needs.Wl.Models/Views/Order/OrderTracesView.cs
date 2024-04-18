using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 代理订单轨迹的视图
    /// </summary>
    public class OrderTracesView : View<Models.OrderTrace, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderTracesView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<OrderTrace> GetIQueryable()
        {
            return from trace in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
                   orderby trace.CreateDate descending
                   where trace.OrderID == this.OrderID
                   select new Models.OrderTrace
                   {
                       ID = trace.ID,
                       OrderID = trace.OrderID,
                       Step = (Enums.OrderTraceStep)trace.Step,
                       CreateDate = trace.CreateDate,
                       Summary = trace.Summary
                   };
        }
    }
}
