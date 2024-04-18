using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 订单的海关税费
    /// 关税、增值税、消费税
    /// </summary>
    public class OrderDecHeadTaxsView : View<Needs.Wl.Models.DecTaxFlow, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderDecHeadTaxsView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Needs.Wl.Models.DecTaxFlow> GetIQueryable()
        {
            return from decTaxFlow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                   join decTax in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>() on decTaxFlow.DecTaxID equals decTax.ID into decTaxs
                   from decTax in decTaxs.DefaultIfEmpty()
                   join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on decTaxFlow.DecTaxID equals decHead.ID
                   orderby decTaxFlow.PayDate descending
                   where decHead.OrderID == this.OrderID
                   select new Needs.Wl.Models.DecTaxFlow
                   {
                       ID = decTaxFlow.ID,
                       DecTaxID = decTaxFlow.DecTaxID,
                       Amount = decTaxFlow.Amount,
                       TaxNumber = decTaxFlow.TaxNumber,
                       TaxType = (Needs.Wl.Models.Enums.DecTaxType)decTaxFlow.TaxType,
                       PayDate = decTaxFlow.PayDate,
                       CreateDate = decTaxFlow.CreateDate
                   };
        }
    }
}