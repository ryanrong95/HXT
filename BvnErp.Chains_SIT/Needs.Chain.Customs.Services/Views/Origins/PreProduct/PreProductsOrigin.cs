using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class PreProductsOrigin : UniqueView<Models.PreProduct, ScCustomsReponsitory>
    {
        internal PreProductsOrigin()
        {
        }

        internal PreProductsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PreProduct> GetIQueryable()
        {
            return from preproduct in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>()
                   select new Models.PreProduct
                   {
                       ID = preproduct.ID,
                       ClientID = preproduct.ClientID,
                       Model = preproduct.Model,
                       Manufacturer = preproduct.Manufacturer,
                       Price = preproduct.Price,
                       Currency = preproduct.Currency,
                       Supplier = preproduct.Supplier,
                       CompanyType = (Enums.CompanyTypeEnums)preproduct.CompanyType,
                       BatchNo = preproduct.BatchNo,
                       Description = preproduct.Description,
                       Pack = preproduct.Pack,
                       AreaOfProduction = preproduct.AreaOfProduction,
                       UseFor = preproduct.UseFor,
                       UseType = (Enums.PreProductUserType)preproduct.UseType,
                       Status = (Enums.Status)preproduct.Status,
                       DueDate = preproduct.DueDate,
                       CreateDate = preproduct.CreateDate,
                       UpdateDate = preproduct.UpdateDate,
                       Summary = preproduct.Summary,
                       ProductUnionCode = preproduct.ProductUnionCode,  
                       Qty = preproduct.Qty,
                       Source = preproduct.Source,
                       AdminID = preproduct.AdminID,
                       IcgooAdmin = preproduct.IcgooAdmin
                   };
        }
    }
}
