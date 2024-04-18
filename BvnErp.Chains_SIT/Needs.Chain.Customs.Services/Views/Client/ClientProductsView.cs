using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientProductsView : UniqueView<Models.ClientProducts, ScCustomsReponsitory>
    {
        public ClientProductsView()
        {

        }
        internal ClientProductsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ClientProducts> GetIQueryable()
        {
            //var productsView = new ProductsViews(this.Reponsitory);
            return from clientproduct in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientProducts>()
                   //join product in productsView on clientproduct.ProductID equals product.ID
                   select new Models.ClientProducts
                   {
                       ID = clientproduct.ID,
                       ClientID = clientproduct.ClientID,
                       Name=clientproduct.Name,
                       Model=clientproduct.Model,
                       Manufacturer=clientproduct.Manufacturer,
                       Batch=clientproduct.Batch,
                       UpdateDate=clientproduct.UpdateDate,
                       CreateDate=clientproduct.CreateDate,
                       Status= (Enums.Status)clientproduct.Status
                   };
        }
    }
}