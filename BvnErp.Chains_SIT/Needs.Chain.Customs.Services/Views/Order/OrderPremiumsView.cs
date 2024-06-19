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
    /// 代理订单附加费用的视图
    /// </summary>
    public class OrderPremiumsView : UniqueView<Models.OrderPremium, ScCustomsReponsitory>
    {
        public OrderPremiumsView()
        {
        }

        internal OrderPremiumsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderPremium> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var orderReceivedsView = new OrderReceivedsView(this.Reponsitory);

            return from orderPremium in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                   join admin in adminsView on orderPremium.AdminID equals admin.ID
                   join orderReceived in orderReceivedsView on orderPremium.ID equals orderReceived.FeeSourceID into orderReceiveds
                   where orderPremium.Status == (int)Enums.Status.Normal
                   select new Models.OrderPremium
                   {
                       ID = orderPremium.ID,
                       OrderID = orderPremium.OrderID,
                       OrderItemID = orderPremium.OrderItemID,
                       Admin = admin,
                       Type = (Enums.OrderPremiumType)orderPremium.Type,
                       Name = orderPremium.Name,
                       Count = orderPremium.Count,
                       UnitPrice = orderPremium.UnitPrice,
                       Currency = orderPremium.Currency,
                       Rate = orderPremium.Rate,
                       StandardID = orderPremium.StandardID,
                       StandardPrice = orderPremium.StandardPrice,
                       StandardRemark = orderPremium.StandardRemark,
                       Status = (Enums.Status)orderPremium.Status,
                       CreateDate = orderPremium.CreateDate,
                       UpdateDate = orderPremium.UpdateDate,
                       Summary = orderPremium.Summary,
                       //杂费(非商检费)实收
                       OrderReceiveds = orderReceiveds.OrderByDescending(item=>item.CreateDate)
                   };
        }

        public IQueryable<OrderPremium> GetOrderPremiums() 
        {
            return from orderPremium in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                   select new Models.OrderPremium
                   {
                       ID = orderPremium.ID,
                       OrderID = orderPremium.OrderID,
                       OrderItemID = orderPremium.OrderItemID,
                       //Admin = admin,
                       Type = (Enums.OrderPremiumType)orderPremium.Type,
                       Name = orderPremium.Name,
                       Count = orderPremium.Count,
                       UnitPrice = orderPremium.UnitPrice,
                       Currency = orderPremium.Currency,
                       Rate = orderPremium.Rate,
                       StandardID = orderPremium.StandardID,
                       StandardPrice = orderPremium.StandardPrice,
                       StandardRemark = orderPremium.StandardRemark,
                       Status = (Enums.Status)orderPremium.Status,
                       CreateDate = orderPremium.CreateDate,
                       UpdateDate = orderPremium.UpdateDate,
                       Summary = orderPremium.Summary,
                       //杂费(非商检费)实收
                       //OrderReceiveds = orderReceiveds.OrderByDescending(item => item.CreateDate)
                   };
        }
    }
}
