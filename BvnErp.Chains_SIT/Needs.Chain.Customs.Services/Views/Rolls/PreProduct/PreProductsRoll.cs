using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Rolls
{
    internal class PreProductsRoll : UniqueView<Models.PreProduct, ScCustomsReponsitory>
    {
        internal PreProductsRoll()
        {
        }

        internal PreProductsRoll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PreProduct> GetIQueryable()
        {
            var preproductsView = new Origins.PreProductsOrigin(this.Reponsitory);
            var clientsView = new ClientsRoll(this.Reponsitory);

            return from preproduct in preproductsView
                   join client in clientsView on preproduct.ClientID equals client.ID
                   select new Models.PreProduct
                   {
                       ID = preproduct.ID,
                       ClientID = preproduct.ClientID,
                       Client = client,
                       ProductUnionCode = preproduct.ProductUnionCode,
                       Model = preproduct.Model,
                       Manufacturer = preproduct.Manufacturer,
                       Qty = preproduct.Qty,
                       Price = preproduct.Price,
                       Currency = preproduct.Currency,
                       Supplier = preproduct.Supplier,
                       CompanyType = preproduct.CompanyType,
                       BatchNo = preproduct.BatchNo,
                       Description = preproduct.Description,
                       Pack = preproduct.Pack,
                       AreaOfProduction = preproduct.AreaOfProduction,
                       UseFor = preproduct.UseFor,
                       UseType = preproduct.UseType,
                       Source = preproduct.Source,
                       Status = preproduct.Status,
                       DueDate = preproduct.DueDate,
                       CreateDate = preproduct.CreateDate,
                       UpdateDate = preproduct.UpdateDate,
                       Summary = preproduct.Summary,
                       AdminID = preproduct.AdminID,
                       IcgooAdmin = preproduct.IcgooAdmin
                   };
        }
    }
}
