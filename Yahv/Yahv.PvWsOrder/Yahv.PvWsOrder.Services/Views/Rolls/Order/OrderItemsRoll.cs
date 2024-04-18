using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 某订单项视图
    /// </summary>
    public class OrderItemsRoll : UniqueView<OrderItem, PvWsOrderReponsitory>
    {
        string OrderID = string.Empty;

        public OrderItemsRoll(string orderID)
        {
            this.OrderID = orderID;
        }

        public OrderItemsRoll(string orderID, PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {
            this.OrderID = orderID;
        }

        protected override IQueryable<OrderItem> GetIQueryable()
        {
            var orderItemAll = new OrderItemsAlls(this.Reponsitory);
            var linq = orderItemAll.Where(item => item.OrderID == this.OrderID);

            return linq;
        }
    }
}
