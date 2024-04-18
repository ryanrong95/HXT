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

    public class CgDelcareSZPriceView : UniqueView<Models.SZPrice, ScCustomsReponsitory>
    {
        public CgDelcareSZPriceView()
        {
        }

        internal CgDelcareSZPriceView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZPrice> GetIQueryable()
        {
           
            var orderItemView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderView = new Origins.OrdersOrigin(this.Reponsitory).Where(item => item.Status == Enums.Status.Normal);
            var taxesView = new Origins.OrderItemTaxsOrigin(this.Reponsitory).Where(item => item.Type == Enums.CustomsRateType.ImportTax);//取关税率

            return from dec in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                   join orderItem in orderItemView on dec.OrderItemID equals orderItem.ID into orderItems
                   from orderItem in orderItems.DefaultIfEmpty()
                   join order in orderView on orderItem.OrderID equals order.ID
                   join tax in taxesView on orderItem.ID equals tax.OrderItemID
                   select new Models.SZPrice
                   {
                       MainOrderId = order.MainOrderID,
                       OrderItemID = dec.OrderItemID,
                       CustomsExchangeRate = order.CustomsExchangeRate.Value,
                       OrderID = dec.OrderID,
                       DeclTotal = dec.DeclTotal,
                       Qty = dec.GQty,
                       ReceiptRate = tax.ReceiptRate,
                   };
        }



    }


}
