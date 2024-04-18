using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class DeclareProductAlls : UniqueView<DeclareProduct, BvCrmReponsitory>, Needs.Underly.IFkoView<DeclareProduct>
    {
        internal DeclareProductAlls()
        {

        }

        internal DeclareProductAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<DeclareProduct> GetIQueryable()
        {
            return this.GetIQueryable(this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.DeclareProducts>());
        }

        internal IQueryable<DeclareProduct> GetIQueryable(IQueryable<Layer.Data.Sqls.BvCrm.DeclareProducts> query)
        {
            CompanyAlls CompanyAlls = new CompanyAlls(this.Reponsitory);
            StandardProductAlls standardProductAlls = new StandardProductAlls(this.Reponsitory);

            return from declare in query
                   join product in standardProductAlls on declare.StandardID equals product.ID
                   join company in CompanyAlls on declare.SupplierID equals company.ID into suppliers
                   from supplier in suppliers.DefaultIfEmpty()
                   orderby declare.ID ascending,product.Name descending
                   select new DeclareProduct
                   {
                       ID = declare.ID,
                       CatelogueID = declare.CatalogueID,
                       StandardProduct = product,
                       Supplier = supplier,
                       Currency = (CurrencyType)declare.Currency,
                       Amount = declare.Amount,
                       UnitPrice = declare.UnitPrice,
                       Delivery = declare.Delivery,
                       Count = declare.Count,
                       Status = (ProductStatus)declare.Status,
                       TotalPrice = declare.TotalPrice,
                       Expect = declare.Expect,
                       ExpectDate = declare.ExpectDate,
                       CompeteManu = declare.CompeteManu,
                       CompeteModel = declare.CompeteModel,
                       CompetePrice = declare.CompetePrice,
                       OriginNumber = declare.OriginNumber,
                       ExpectTotal = declare.Expect * declare.TotalPrice / 100,
                   };
        }
    }
}
