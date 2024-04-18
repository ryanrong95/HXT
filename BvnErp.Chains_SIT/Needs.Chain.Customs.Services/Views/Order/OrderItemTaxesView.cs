using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单税率的视图
    /// </summary>
    public class OrderItemTaxesView : UniqueView<Models.OrderItemTax, ScCustomsReponsitory>
    {
        public OrderItemTaxesView()
        {
        }

        internal OrderItemTaxesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderItemTax> GetIQueryable()
        {
            return from orderTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>()
                   select new Models.OrderItemTax
                   {
                       ID = orderTax.ID,
                       OrderItemID = orderTax.OrderItemID,
                       Type = (Enums.CustomsRateType)orderTax.Type,
                       Rate = orderTax.Rate,
                       ReceiptRate = orderTax.ReceiptRate,
                       Value = orderTax.Value,
                       Status = (Enums.Status)orderTax.Status,
                       CreateDate = orderTax.CreateDate,
                       UpdateDate = orderTax.UpdateDate,
                       Summary = orderTax.Summary
                   };
        }
    }

}


