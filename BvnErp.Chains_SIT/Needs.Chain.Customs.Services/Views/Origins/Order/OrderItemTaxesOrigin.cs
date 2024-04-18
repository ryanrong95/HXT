using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class OrderItemTaxsOrigin : UniqueView<Models.OrderItemTax, ScCustomsReponsitory>
    {
        internal OrderItemTaxsOrigin()
        {
        }

        internal OrderItemTaxsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderItemTax> GetIQueryable()
        {
            return from orderTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>()
                   select new Models.OrderItemTax
                   {
                       ID = orderTax.ID,
                       OrderItemID = orderTax.OrderItemID,
                       Type = (Enums.CustomsRateType)orderTax.Type,
                       Rate = orderTax.Rate,
                       Value = orderTax.Value,
                       Status = (Enums.Status)orderTax.Status,
                       CreateDate = orderTax.CreateDate,
                       UpdateDate = orderTax.UpdateDate,
                       Summary = orderTax.Summary
                   };
        }
    }
}
