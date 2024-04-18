using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class PreProductsView : View<Models.PreProduct, ScCustomsReponsitory>
    {
        public PreProductsView()
        {
        }

        internal PreProductsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PreProduct> GetIQueryable()
        {
            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>()
                   join clients in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on para.ClientID equals clients.ID
                   select new Models.PreProduct
                   {
                       ID = para.ID,
                       ClientID = para.ClientID,
                       ProductUnionCode = para.ProductUnionCode,
                       Model = para.Model,
                       Manufacturer = para.Manufacturer,
                       BatchNo = para.BatchNo,
                       Price = para.Price,
                       Currency = para.Currency,
                       Supplier = para.Supplier,
                       Status = (int)para.Status,
                       CreateDate = para.CreateDate,
                       UpdateDate = para.UpdateDate
                   };
        }
    }
}
