using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 北京总部管控的视图
    /// </summary>
    public class HQControlsView : UniqueView<Models.HQControl, ScCustomsReponsitory>
    {
        public HQControlsView()
        {
        }

        internal HQControlsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<HQControl> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var controlStepsView = new OrderControlStepsView(this.Reponsitory);

            var controls = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                           join order in ordersView on entity.OrderID equals order.ID
                           join controlStep in controlStepsView on entity.ID equals controlStep.OrderControlID
                           where controlStep.Step == Enums.OrderControlStep.Headquarters && controlStep.ControlStatus == Enums.OrderControlStatus.Auditing &&
                           (order.OrderStatus >= Enums.OrderStatus.Classified && order.OrderStatus <= Enums.OrderStatus.QuoteConfirmed) &&
                           order.IsHangUp
                           && (entity.ControlType != (int)Enums.OrderControlType.DeleteModel && entity.ControlType != (int)Enums.OrderControlType.ChangeQuantity)
                           orderby entity.ID
                           select new Models.HQControl
                           {
                               ID = entity.ID,
                               Order = order,
                               ControlType = (Enums.OrderControlType)entity.ControlType,
                               Status = (Enums.Status)entity.Status
                           };

            return from entity in controls
                   group entity by new { entity.Order.ID, entity.ControlType } into entities
                   select entities.FirstOrDefault();
        }
    }
}
