using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class OrderPayExchangeSuppliersView : View<Models.OrderPayExchangeSupplier, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderPayExchangeSuppliersView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Models.OrderPayExchangeSupplier> GetIQueryable()
        {
            return from trace in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers>()
                   join supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on trace.ClientSupplierID equals supplier.ID
                   orderby trace.CreateDate descending
                   where trace.OrderID == this.OrderID && trace.Status == (int)Enums.Status.Normal
                   select new Models.OrderPayExchangeSupplier
                   {
                       ID = trace.ID,
                       OrderID = trace.OrderID,
                       ClientSupplierID = trace.ClientSupplierID,
                       Name = supplier.Name,
                       ChineseName = supplier.ChineseName,
                       CreateDate = trace.CreateDate,
                       Summary = trace.Summary
                   };
        }
    }
}