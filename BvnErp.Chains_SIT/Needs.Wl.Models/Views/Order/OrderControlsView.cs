using Layer.Data.Sqls;
using Needs.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 订单管控
    /// </summary>
    public  class OrderControlsView : View<Needs.Wl.Models.OrderControl, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderControlsView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<OrderControl> GetIQueryable()
        {
            return from orderControl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                   where orderControl.OrderID == this.OrderID
                      && orderControl.Status == (int)Enums.Status.Normal
                   orderby orderControl.CreateDate
                   select new Models.OrderControl
                   {
                       ID = orderControl.ID,
                       OrderID = orderControl.OrderID,
                       OrderItemID = orderControl.OrderItemID,
                       ControlType = (Enums.OrderControlType)orderControl.ControlType,
                       Status = orderControl.Status,
                       CreateDate = orderControl.CreateDate,
                       UpdateDate = orderControl.UpdateDate,
                       Summary = orderControl.Summary,
                   };
        }

        private IQueryable<Layer.Data.Sqls.ScCustoms.OrderControls> InternalQuery()
        {
            var query = from orderControl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                        where orderControl.OrderID == this.OrderID
                           && orderControl.Status == (int)Enums.Status.Normal
                        orderby orderControl.CreateDate
                        select orderControl;

            return query;
        }

        /// <summary>
        /// 用户确认显示的订单管控
        /// </summary>
        /// <returns></returns>
        public IList<OrderControl> GetUserConfirm()
        {
            return this.InternalQuery().Where(s => s.ControlType == (int)Enums.OrderControlType.DeleteModel || s.ControlType == (int)Enums.OrderControlType.ChangeQuantity).Select(
                item => new Models.OrderControl
                {
                    ID = item.ID,
                    OrderID = item.OrderID,
                    OrderItemID = item.OrderItemID,
                    ControlType = (Enums.OrderControlType)item.ControlType,
                    Status = item.Status,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    Summary = item.Summary,
                }).ToList();
        }

        /// <summary>
        /// 根据类型查找
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<OrderControl> FindByType(Enums.OrderControlType type)
        {
            return this.InternalQuery().Where(s => s.ControlType == (int)type).Select(
                item => new Models.OrderControl
                {
                    ID = item.ID,
                    OrderID = item.OrderID,
                    OrderItemID = item.OrderItemID,
                    ControlType = (Enums.OrderControlType)item.ControlType,
                    Status = item.Status,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    Summary = item.Summary,
                }).ToList();
        }
    }
}