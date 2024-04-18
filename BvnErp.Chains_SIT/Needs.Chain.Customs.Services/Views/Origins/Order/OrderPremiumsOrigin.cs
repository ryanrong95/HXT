using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class OrderPremiumsOrigin : UniqueView<Models.OrderPremium, ScCustomsReponsitory>
    {
        public OrderPremiumsOrigin()
        {
        }

        public OrderPremiumsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderPremium> GetIQueryable()
        {
            return from orderPremium in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                   select new Models.OrderPremium
                   {
                       ID = orderPremium.ID,
                       OrderID = orderPremium.OrderID,
                       OrderItemID = orderPremium.OrderItemID,
                       AdminID = orderPremium.AdminID,
                       Type = (Enums.OrderPremiumType)orderPremium.Type,
                       Name = orderPremium.Name,
                       Count = orderPremium.Count,
                       UnitPrice = orderPremium.UnitPrice,
                       Currency = orderPremium.Currency,
                       Rate = orderPremium.Rate,
                       Status = (Enums.Status)orderPremium.Status,
                       CreateDate = orderPremium.CreateDate,
                       UpdateDate = orderPremium.UpdateDate,
                       Summary = orderPremium.Summary,
                   };
        }
    }
}
