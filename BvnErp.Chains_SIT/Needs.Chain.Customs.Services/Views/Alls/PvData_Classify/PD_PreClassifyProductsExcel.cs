using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    /// <summary>
    /// 预归类产品视图(导出Excel使用)
    /// </summary>
    public class PD_PreClassifyProductsExcel : Needs.Linq.View<Models.ClassifyDoneAllModels, ScCustomsReponsitory>
    {
        protected override IQueryable<Models.ClassifyDoneAllModels> GetIQueryable()
        {
            var preproducts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>();
            var precategorys = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>().Where(t => t.ClassifyStatus == (int)Enums.ClassifyStatus.Done);
            var orderitems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.Status == (int)Enums.Status.Normal);
            var admins = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            var clients = from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                          join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                          select new
                          {
                              client.ID,
                              company.Name
                          };

            return from category in precategorys
                   join preproduct in preproducts on category.PreProductID equals preproduct.ID
                   join adminfirst in admins on category.ClassifyFirstOperator equals adminfirst.ID into admin_first
                   from adminfirst in admin_first.DefaultIfEmpty()
                   join adminsecond in admins on category.ClassifySecondOperator equals adminsecond.ID into admin_second
                   from adminsecond in admin_second.DefaultIfEmpty()
                   join client in clients on preproduct.ClientID equals client.ID
                   join orderitem in orderitems on preproduct.ProductUnionCode equals orderitem.ProductUniqueCode into order_pre
                   from orderitem in order_pre.DefaultIfEmpty()
                   orderby preproduct.CreateDate descending
                   select new Models.ClassifyDoneAllModels
                   {
                       ID = category.ID,
                       Manufacturer = category.Manufacture,
                       Model = category.Model,
                       ProductName = category.ProductName,
                       HSCode = category.HSCode,
                       Elements = category.Elements,
                       TariffRate = category.TariffRate,
                       UnitPrice = preproduct.Price,
                       UseType = (Enums.PreProductUserType)preproduct.UseType,
                       ClassifyStatus = (Enums.ClassifyStatus)category.ClassifyStatus,
                       ClientName = client.Name,
                       CreateDate = preproduct.CreateDate,
                       CompleteTime = category.UpdateDate,
                       ClassifyFirstOperatorName = adminfirst.RealName,
                       ClassifySecondOperatorName = adminsecond.RealName,
                       TaxCode = category.TaxCode,
                       TaxName = category.TaxName,
                       IsOrdered = orderitem == null ? true : false,
                   };
        }

    }
}
