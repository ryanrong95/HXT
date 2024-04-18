using Needs.Wl.Models.Views;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat
{
    public class OrderContextExtends
    {
        string OrderID;

        public OrderContextExtends(string orderID)
        {
            this.OrderID = orderID;
        }

        public Needs.Wl.Orders.Services.Views.OrderPayExchangeRecordsView PayExchangeRecords
        {
            get { return new Needs.Wl.Orders.Services.Views.OrderPayExchangeRecordsView(this.OrderID); }
        }

        /// <summary>
        /// 订单的日志
        /// </summary>
        /// <returns></returns>
        public OrderLogsView Logs
        {
            get { return new OrderLogsView(this.OrderID); }
        }

        /// <summary>
        /// 订单跟踪轨迹
        /// </summary>
        public OrderTracesView Traces
        {
            get { return new OrderTracesView(this.OrderID); }
        }

        /// <summary>
        /// 订单的附件
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public OrderFilesView Files
        {
            get { return new OrderFilesView(this.OrderID); }
        }

        /// <summary>
        /// 订单的深圳库房 出库单/提货单
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public OrderPickupBillsView PickupBill
        {
            get { return new OrderPickupBillsView(this.OrderID); }
        }
    }
}
