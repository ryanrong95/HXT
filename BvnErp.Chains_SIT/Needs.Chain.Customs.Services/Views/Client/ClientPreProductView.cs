using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientPreProductView : UniqueView<Models.ClientPreProduct, ScCustomsReponsitory>
    {
        public ClientPreProductView()
        {

        }
        internal ClientPreProductView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ClientPreProduct> GetIQueryable()
        {
            var admin = new AdminsTopView(this.Reponsitory);
            var classifyView =this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>();
            return from preoduct in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>()
                   join classify in classifyView on preoduct.ID equals classify.PreProductID
                   join adminfirst in admin on classify.ClassifyFirstOperator equals adminfirst.ID into first
                   from firstadmins in first.DefaultIfEmpty()
                   join adminsecond in admin on classify.ClassifySecondOperator equals adminsecond.ID into second
                   from secondadmins in second.DefaultIfEmpty()
                   select new Models.ClientPreProduct
                   {
                       ID = preoduct.ID,
                       ClientID = preoduct.ClientID,
                       Model = classify.Model,
                       Manufacturer = classify.Manufacture,
                       ProductName = classify.ProductName,
                       HSCode = classify.HSCode,
                       TariffRate = classify.TariffRate,
                       AddedValueRate = classify.AddedValueRate,
                       ExciseTaxRate = classify.ExciseTaxRate,
                       TaxCode = classify.TaxCode,
                       TaxName=classify.TaxName,
                       Type = (ItemCategoryType)classify.Type,
                       ClassifyStatus = (ClassifyStatus)classify.ClassifyStatus,
                       InspectionFee = classify.InspectionFee,
                       Unit1 = classify.Unit1,
                       Unit2 = classify.Unit2,
                       CIQCode = classify.CIQCode,
                       Elements = classify.Elements,
                       Status = (Status)classify.Status,
                       Createtime = classify.CreateDate,
                       Updatetime = classify.UpdateDate,
                       Price = preoduct.Price,
                       Currency = preoduct.Currency,
                       ProductUnionCode = preoduct.ProductUnionCode,
                       Supplier = preoduct.Supplier,
                       ClassifyFirstOperator = firstadmins.ID,
                       ClassifySecondOperator= secondadmins.ID,
                   };
        }
    }
}