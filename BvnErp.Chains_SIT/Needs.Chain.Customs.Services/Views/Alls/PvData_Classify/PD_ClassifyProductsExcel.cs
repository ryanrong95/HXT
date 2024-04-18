using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    /// <summary>
    /// 归类产品视图(导出Excel使用)
    /// </summary>
    public class PD_ClassifyProductsExcel : Needs.Linq.View<Models.ClassifyDoneAllModels, ScCustomsReponsitory>
    {
        protected override IQueryable<Models.ClassifyDoneAllModels> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                .Where(t => t.OrderStatus >= (int)Enums.OrderStatus.Confirmed && t.OrderStatus != (int)Enums.OrderStatus.Returned && t.OrderStatus != (int)Enums.OrderStatus.Canceled);
            var orderitems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                .Where(t => t.ClassifyStatus == (int)Enums.ClassifyStatus.Done && t.Status == (int)Enums.Status.Normal);
            var admins = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            var importTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Where(t => t.Type == (int)Enums.CustomsRateType.ImportTax);
            var clients = from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                          join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                          select new
                          {
                              client.ID,
                              company.Name
                          };

            var query = from category in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                        join orderitem in orderitems on category.OrderItemID equals orderitem.ID
                        join importtax in importTaxs on category.OrderItemID equals importtax.OrderItemID
                        join order in orders on orderitem.OrderID equals order.ID
                        join client in clients on order.ClientID equals client.ID
                        join adminfirst in admins on category.ClassifyFirstOperator equals adminfirst.ID into admin_first
                        from adminfirst in admin_first.DefaultIfEmpty()
                        join adminsecond in admins on category.ClassifySecondOperator equals adminsecond.ID into admin_second
                        from adminsecond in admin_second.DefaultIfEmpty()
                        orderby orderitem.CreateDate descending
                        select new Models.ClassifyDoneAllModels
                        {
                            ID = orderitem.ID,
                            Manufacturer = orderitem.Manufacturer,
                            Model = orderitem.Model,
                            ProductName = category.Name,
                            HSCode = category.HSCode,
                            Elements = category.Elements,
                            TariffRate = importtax.Rate,
                            UnitPrice = orderitem.UnitPrice,
                            Quantity = orderitem.Quantity,
                            ClassifyStatus = (Enums.ClassifyStatus)orderitem.ClassifyStatus,
                            ClientName = client.Name,
                            CreateDate = orderitem.CreateDate,
                            CompleteTime = category.UpdateDate,
                            ClassifyFirstOperatorName = adminfirst.RealName,
                            ClassifySecondOperatorName = adminsecond.RealName,
                            TaxCode = category.TaxCode,
                            TaxName = category.TaxName,
                            IsOrdered = true,
                            OrderID = order.ID
                        };

            return query;
        }
    }
}
