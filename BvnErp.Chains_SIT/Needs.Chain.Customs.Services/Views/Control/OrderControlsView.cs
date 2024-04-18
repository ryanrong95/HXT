using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 订单管控项的视图
    /// </summary>
    public class OrderControlsView : UniqueView<Models.OrderControlData, ScCustomsReponsitory>
    {
        public OrderControlsView()
        {
        }

        internal OrderControlsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderControlData> GetIQueryable()
        {
            var orderItemsView = new OrderItemsView(this.Reponsitory);

            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                   select new Models.OrderControlData
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       OrderItemID = entity.OrderItemID,
                       ControlType = (Enums.OrderControlType)entity.ControlType,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }

}
