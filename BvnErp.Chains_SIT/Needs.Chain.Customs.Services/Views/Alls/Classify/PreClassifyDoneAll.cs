using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    public class PreClassifyDoneAll : View<Models.ClassifyDoneAllModels, ScCustomsReponsitory>
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

            //var logs = from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyChangeLogs>().Where(t => t.Summary.Contains("海关编码"))
            //           group log by new { log.Model, log.Manufacturer, log.Summary } into g
            //           select new
            //           {
            //               g.Key.Model,
            //               g.Key.Manufacturer,
            //               g.Key.Summary
            //           };

            var logs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyChangeLogs>().Where(t => t.Summary.Contains("海关编码"));

            return from category in precategorys
                   join preproduct in preproducts on category.PreProductID equals preproduct.ID
                   join adminfirst in admins on category.ClassifyFirstOperator equals adminfirst.ID into admin_first
                   from adminfirst in admin_first.DefaultIfEmpty()
                   join adminsecond in admins on category.ClassifySecondOperator equals adminsecond.ID into admin_second
                   from adminsecond in admin_second.DefaultIfEmpty()
                   join client in clients on preproduct.ClientID equals client.ID
                   join orderitem in orderitems on preproduct.ProductUnionCode equals orderitem.ProductUniqueCode into order_pre
                   from orderitem in order_pre.DefaultIfEmpty()
                   //join log in logs on new { Model = category.Model, Manufacture = category.Manufacture } equals new { Model = log.Model, Manufacture = log.Manufacturer } into log_temp
                   //from log in log_temp.DefaultIfEmpty()
                   orderby preproduct.CreateDate descending
                   select new Models.ClassifyDoneAllModels
                   {
                       Manufacturer = category.Manufacture,
                       Model = category.Model,
                       ProductName = category.ProductName,
                       HSCode = category.HSCode,
                       Elements = category.Elements,
                       TariffRate = category.TariffRate,
                       UnitPrice = preproduct.Price,
                       ClassifyStatus = (Enums.ClassifyStatus)category.ClassifyStatus,
                       ClientName = client.Name,
                       CreateDate = preproduct.CreateDate,
                       CompleteTime = category.UpdateDate,
                       ClassifyFirstOperatorName = adminfirst.RealName,
                       ClassifySecondOperatorName = adminsecond.RealName,
                       TaxCode = category.TaxCode,
                       TaxName = category.TaxName,
                       IsOrdered = orderitem == null ? true : false,
                       ClassifyLog = logs.Where(t=>t.Model == category.Model && t.Manufacturer == category.Manufacture).Select(t=>t.Summary)
                   };
        }

    }
}
