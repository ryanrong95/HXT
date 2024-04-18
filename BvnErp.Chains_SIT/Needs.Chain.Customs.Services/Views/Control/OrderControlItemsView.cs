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
    public class OrderControlItemsAllsView : UniqueView<Models.OrderControlItem, ScCustomsReponsitory>
    {
        public OrderControlItemsAllsView()
        {
        }

        internal OrderControlItemsAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderControlItem> GetIQueryable()
        {
            var orderItemsView = new OrderItemsView(this.Reponsitory);

            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                   join orderItem in orderItemsView on entity.OrderItemID equals orderItem.ID
                   select new Models.OrderControlItem
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       OrderItem = orderItem,
                       ControlType = (Enums.OrderControlType)entity.ControlType,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }

    public sealed class OrderControlItemsView : OrderControlItemsAllsView
    {
        Enums.OrderControlStep Step;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="step">订单审核步骤/审核层级</param>
        public OrderControlItemsView(Enums.OrderControlStep step)
        {
            this.Step = step;
        }

        protected override IQueryable<Models.OrderControlItem> GetIQueryable()
        {
            var controlStepsView = new OrderControlStepsView(this.Reponsitory);

            return from entity in base.GetIQueryable()
                   join controlStep in controlStepsView on entity.ID equals controlStep.OrderControlID
                   where controlStep.Step == this.Step && controlStep.ControlStatus == Enums.OrderControlStatus.Auditing
                   select entity;
        }
    }

    /// <summary>
    /// 总部审批管控项
    /// </summary>
    public sealed class HQControlItemsView : OrderControlItemsAllsView
    {
        public HQControlItemsView()
        {

        }

        protected override IQueryable<Models.OrderControlItem> GetIQueryable()
        {
            var controlStepsView = new OrderControlStepsView(this.Reponsitory);

            return from entity in base.GetIQueryable()
                   join controlStep in controlStepsView on entity.ID equals controlStep.OrderControlID
                   where controlStep.Step == Enums.OrderControlStep.Headquarters && controlStep.ControlStatus == Enums.OrderControlStatus.Auditing
                   select entity;
        }
    }

    /// <summary>
    /// 跟单员管控审批项
    /// </summary>
    public sealed class MerchandiserControlItemsView : OrderControlItemsAllsView
    {
        public MerchandiserControlItemsView()
        {

        }

        protected override IQueryable<Models.OrderControlItem> GetIQueryable()
        {
            var controlStepsView = new OrderControlStepsView(this.Reponsitory);

            return from entity in base.GetIQueryable()
                   join controlStep in controlStepsView on entity.ID equals controlStep.OrderControlID
                   where controlStep.ControlStatus == Enums.OrderControlStatus.Auditing
                   select entity;
        }
    }
}
