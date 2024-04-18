using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 代理订单日志的视图
    /// </summary>
    public class OrderLogsView : UniqueView<Models.OrderLog, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderLogsView(string orderID)
        {
            this.OrderID = orderID;
        }

        internal OrderLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderLog> GetIQueryable()
        {
            return from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                   where log.OrderID == this.OrderID
                   orderby log.CreateDate descending
                   select new Models.OrderLog
                   {
                       ID = log.ID,
                       OrderID = log.OrderID,
                       OrderItemID = log.OrderItemID,
                       OrderStatus = (Enums.OrderStatus)log.OrderStatus,
                       CreateDate = log.CreateDate,
                       Summary = log.Summary
                   };
        }
    }
}
