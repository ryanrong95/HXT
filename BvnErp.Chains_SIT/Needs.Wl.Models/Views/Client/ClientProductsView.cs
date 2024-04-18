using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class ClientProductsView : View<ClientProducts, ScCustomsReponsitory>
    {
        public ClientProductsView()
        {
           
        }

        internal ClientProductsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ClientProducts> GetIQueryable()
        {
            return from product in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientProducts>()
                   where product.Status == (int)Enums.Status.Normal
                   select new Models.ClientProducts
                   {
                       ID = product.ID,
                       ClientID = product.ClientID,
                       Name = product.Name,
                       Model = product.Model,
                       Manufacturer = product.Manufacturer,
                       Batch = product.Batch,
                       UpdateDate = product.UpdateDate,
                       CreateDate = product.CreateDate,
                       Status = product.Status
                   };
        }
    }
}