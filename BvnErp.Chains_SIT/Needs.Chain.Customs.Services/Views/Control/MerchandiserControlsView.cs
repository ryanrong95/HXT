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
    /// 跟单员管控的视图
    /// </summary>
    public class MerchandiserControlsView : UniqueView<Models.MerchandiserControl, ScCustomsReponsitory>
    {
        public MerchandiserControlsView()
        {
        }

        internal MerchandiserControlsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<MerchandiserControl> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var controlStepsView = new OrderControlStepsView(this.Reponsitory);

            //处于总部审核阶段的订单管控
            var hqControls = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                             join controlStep in controlStepsView on entity.ID equals controlStep.OrderControlID
                             where controlStep.Step == Enums.OrderControlStep.Headquarters && controlStep.ControlStatus == Enums.OrderControlStatus.Auditing
                             select new
                             {
                                 entity.OrderID,
                                 entity.ControlType
                             };

            var controls = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                           join order in ordersView on entity.OrderID equals order.ID
                           join controlStep in controlStepsView on entity.ID equals controlStep.OrderControlID
                           where controlStep.ControlStatus == Enums.OrderControlStatus.Auditing &&
                           (order.OrderStatus >= Enums.OrderStatus.Classified && order.OrderStatus <= Enums.OrderStatus.QuoteConfirmed) &&
                           order.IsHangUp
                           && (entity.ControlType != (int)Enums.OrderControlType.DeleteModel && entity.ControlType != (int)Enums.OrderControlType.ChangeQuantity)
                           orderby entity.ID
                           select new Models.MerchandiserControl
                           {
                               ID = entity.ID,
                               Order = order,
                               ControlType = (Enums.OrderControlType)entity.ControlType,
                               IsHQAuditing = hqControls.Where(item => item.OrderID == order.ID).Select(item => item.ControlType).Contains(entity.ControlType),
                               Status = (Enums.Status)entity.Status,
                               Summary = entity.Summary
                           };

            return from entity in controls
                   group entity by new { entity.Order.ID, entity.ControlType } into entities
                   select entities.FirstOrDefault();
        }
    }

    /// <summary>
    /// 跟单员管控的视图
    /// </summary>
    public class MerchandiserControlsNotHangUpView : UniqueView<Models.MerchandiserControl, ScCustomsReponsitory>
    {
        public MerchandiserControlsNotHangUpView()
        {
        }

        internal MerchandiserControlsNotHangUpView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<MerchandiserControl> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var controlStepsView = new OrderControlStepsView(this.Reponsitory);
            var clientadminview = new ClientAdminsView(this.Reponsitory);
            var departView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Departments>();

            //处于总部审核阶段的订单管控
            var hqControls = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                             join controlStep in controlStepsView on entity.ID equals controlStep.OrderControlID
                             where controlStep.Step == Enums.OrderControlStep.Headquarters && controlStep.ControlStatus == Enums.OrderControlStatus.Auditing
                             select new
                             {
                                 entity.OrderID,
                                 entity.ControlType
                             };

            var controls = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                           join order in ordersView on entity.OrderID equals order.ID
                           join admin in clientadminview on order.Client.ID equals admin.ClientID
                           join depart in departView on admin.DepartmentID equals depart.ID into t_depart
                           from depart in t_depart.DefaultIfEmpty()
                           join controlStep in controlStepsView on entity.ID equals controlStep.OrderControlID
                           where controlStep.ControlStatus == Enums.OrderControlStatus.Auditing &&
                           (order.OrderStatus >= Enums.OrderStatus.Classified && order.OrderStatus <= Enums.OrderStatus.Declared)
                           && (entity.ControlType != (int)Enums.OrderControlType.DeleteModel && entity.ControlType != (int)Enums.OrderControlType.ChangeQuantity)
                           orderby entity.ID
                           select new Models.MerchandiserControl
                           {
                               ID = entity.ID,
                               Order = order,
                               ControlType = (Enums.OrderControlType)entity.ControlType,
                               IsHQAuditing = hqControls.Where(item => item.OrderID == order.ID).Select(item => item.ControlType).Contains(entity.ControlType),
                               Status = (Enums.Status)entity.Status,
                               Summary = entity.Summary,
                               DepartmentCode = depart == null ? "" : depart.Name
                           };

            return from entity in controls
                   group entity by new { entity.Order.ID, entity.ControlType } into entities
                   select entities.FirstOrDefault();
        }
    }
}
