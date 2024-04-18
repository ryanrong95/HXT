using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 代理订单附加费用的视图
    /// </summary>
    public class OrderPremiumsView : View<Models.OrderPremium, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderPremiumsView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        internal OrderPremiumsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderPremium> GetIQueryable()
        {
            return from orderPremium in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on orderPremium.AdminID equals admin.ID
                   where orderPremium.Status == (int)Enums.Status.Normal
                   && orderPremium.OrderID == this.OrderID
                   select new Models.OrderPremium
                   {
                       ID = orderPremium.ID,
                       OrderID = orderPremium.OrderID,
                       OrderItemID = orderPremium.OrderItemID,
                       Type = (Enums.OrderPremiumType)orderPremium.Type,
                       Name = orderPremium.Name,
                       Count = orderPremium.Count,
                       UnitPrice = orderPremium.UnitPrice,
                       Currency = orderPremium.Currency,
                       Rate = orderPremium.Rate,
                       Admin = new Admin()
                       {
                           ID = admin.ID,
                           RealName = admin.RealName,
                           UserName = admin.UserName
                       },
                       Status = orderPremium.Status,
                       CreateDate = orderPremium.CreateDate,
                       UpdateDate = orderPremium.UpdateDate,
                       Summary = orderPremium.Summary
                   };
        }
    }
}
