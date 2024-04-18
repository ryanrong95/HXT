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
    /// 订单管控记录的视图
    /// </summary>
    public class OrderControlRecordsAllsView : UniqueView<Models.OrderControlRecord, ScCustomsReponsitory>
    {
        public OrderControlRecordsAllsView()
        {
        }

        internal OrderControlRecordsAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderControlRecord> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var clientsView = new ClientsView(this.Reponsitory);
            var orderItemsView = new OrderItemsView(this.Reponsitory);
            var controlStepsView = new OrderControlStepsView(this.Reponsitory);

            return from entity in controlStepsView
                   join control in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>() on entity.OrderControlID equals control.ID
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on control.OrderID equals order.ID
                   join client in clientsView on order.ClientID equals client.ID
                   join admin in adminsView on entity.AdminID equals admin.ID
                   join orderItem in orderItemsView on control.OrderItemID equals orderItem.ID into items
                   from orderItem in items.DefaultIfEmpty()
                   where entity.ControlStatus != Enums.OrderControlStatus.Auditing
                   && (control.ControlType != (int)Enums.OrderControlType.DeleteModel && control.ControlType != (int)Enums.OrderControlType.ChangeQuantity)
                   orderby entity.UpdateDate descending
                   select new Models.OrderControlRecord
                   {
                       ID = entity.ID,
                       OrderControlID = entity.OrderControlID,
                       OrderID = control.OrderID,
                       Client = client,
                       OrderItem = orderItem,
                       ControlType = (Enums.OrderControlType)control.ControlType,
                       Step = entity.Step,
                       ControlStatus = entity.ControlStatus,
                       Admin = admin,
                       Status = entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                       ApproveSummary =control.ApproveSummary
                   };
        }
    }

    /// <summary>
    /// 北京管控记录的视图
    /// </summary>
    public class HQControlRecordsView : OrderControlRecordsAllsView
    {
        protected override IQueryable<OrderControlRecord> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Step == Enums.OrderControlStep.Headquarters
                   select entity;
        }
    }

    /// <summary>
    /// 跟单员管控记录的视图
    /// </summary>
    public class MerchandiserControlRecordsView : OrderControlRecordsAllsView
    {
        protected override IQueryable<OrderControlRecord> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Step == Enums.OrderControlStep.Merchandiser
                   select entity;
        }
    }
}
