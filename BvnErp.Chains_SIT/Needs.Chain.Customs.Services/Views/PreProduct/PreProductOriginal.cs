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
    public class PreProductOriginalView : UniqueView<Models.IcgooPreProduct, ScCustomsReponsitory>
    {
        public PreProductOriginalView()
        {
        }

        internal PreProductOriginalView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.IcgooPreProduct> GetIQueryable()
        {

            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>()                  
                   select new Models.IcgooPreProduct
                   {
                       ID = para.ID,
                       ClientID = para.ClientID,
                       sale_orderline_id = para.ProductUnionCode,
                       partno = para.Model,
                       mfr = para.Manufacturer,
                       price = para.Price,
                       currency_code = para.Currency,
                       supplier = para.Supplier,
                       Status = (int)para.Status,
                       CreateTime = para.CreateDate,
                       UpdateTime = para.UpdateDate,                     
                       CompanyType = (CompanyTypeEnums)para.CompanyType,
                       BatchNo = para.BatchNo,
                       Pack = para.Pack,
                       Description = para.Description,
                       UseFor = para.UseFor,
                       AraeOfProduction = para.AreaOfProduction,
                   };
        }
    }
}
