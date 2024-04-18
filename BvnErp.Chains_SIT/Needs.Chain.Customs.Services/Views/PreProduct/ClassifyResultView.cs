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
    public class ClassifyResultView : UniqueView<Models.ClassifyResult, ScCustomsReponsitory>
    {
        public ClassifyResultView()
        {
        }

        internal ClassifyResultView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClassifyResult> GetIQueryable()
        {
            var admin = new AdminsTopView(this.Reponsitory);
            var icgooPre = new IcgooPreProductView(this.Reponsitory);
            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>()
                   join pre in icgooPre on para.PreProductID equals pre.ID
                   join adminfirst in admin on para.ClassifyFirstOperator equals adminfirst.ID into g
                   from admins in g.DefaultIfEmpty()
                   join adminsecond in admin on para.ClassifySecondOperator equals adminsecond.ID into m
                   from secondadmins in m.DefaultIfEmpty()
                   select new Models.ClassifyResult
                   {
                      ID = para.ID,
                      PreProductID=para.PreProductID,
                      PreProduct = pre,
                      Model = para.Model,
                      Manufacturer = para.Manufacture,
                      ProductName = para.ProductName,
                      HSCode = para.HSCode,
                      TariffRate = para.TariffRate,
                      AddedValueRate = para.AddedValueRate,
                      TaxCode = para.TaxCode,
                      TaxName = para.TaxName,
                      Type = (ItemCategoryType)para.Type,
                      ClassifyStatus = (ClassifyStatus)para.ClassifyStatus,
                      InspectionFee = para.InspectionFee,
                      Unit1 = para.Unit1,
                      Unit2 = para.Unit2,
                      CIQCode = para.CIQCode,
                      Elements = para.Elements,
                      Status = (Status)para.Status,
                      CreateDate = para.CreateDate,
                      UpdateDate = para.UpdateDate,
                      ClassifyFirstOperator = admins, 
                      ClassifySecondOperator=secondadmins,
                   };
        }
    }
}
