using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Alls
{
    /// <summary>
    /// 产品归类视图(导出Excel使用)
    /// </summary>
    public class ClassifyProductsExcel : QueryView<ClassifyProductExcel, PvWsOrderReponsitory>
    {
        protected override IQueryable<ClassifyProductExcel> GetIQueryable()
        {
            var cpnsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ClassifiedPartNumbersTopView>();
            var chcdsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>().Where(chcd => chcd.SecondHSCodeID != null);
            var termsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>();

            var orderItemsView = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                                 group entity by entity.InputID into entities
                                 select entities.OrderByDescending(e => e.Type).First();

            var clientsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WsClientsTopView>();
            var productsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ProductsTopView>();
            var ordersView = new Origins.OrderOrigin(this.Reponsitory)
                .Where(o => (o.Type == OrderType.TransferDeclare || o.Type == OrderType.Declare) && o.MainStatus >= CgOrderStatus.待审核);
            var logsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Logs_ClassifyModifiedTopView>().Where(l => l.Summary.Contains("海关编码"));
            var adminsView = new AdminsAll(this.Reponsitory);

            var linq = from entity in orderItemsView
                       join product in productsView on entity.ProductID equals product.ID
                       join chcd in chcdsView on entity.ID equals chcd.ID
                       join cpn in cpnsView on chcd.SecondHSCodeID equals cpn.ID
                       join term in termsView on entity.ID equals term.ID
                       join order in ordersView on entity.OrderID equals order.ID
                       join wsClient in clientsView on order.ClientID equals wsClient.ID
                       join firstOperator in adminsView on chcd.FirstAdminID equals firstOperator.ID into firstOperators
                       from firstOperator in firstOperators.DefaultIfEmpty()
                       join secondOperator in adminsView on chcd.SecondAdminID equals secondOperator.ID into secondOperators
                       from secondOperator in secondOperators.DefaultIfEmpty()
                       join log in logsView on new { product.PartNumber, product.Manufacturer} equals new {log.PartNumber, log.Manufacturer} into logs
                       where entity.Status == (int)GeneralStatus.Normal
                       orderby entity.CreateDate descending
                       select new ClassifyProductExcel
                       {
                           ID = entity.ID,
                           TariffName = cpn.Name,
                           PartNumber = product.PartNumber,
                           Manufacturer = product.Manufacturer,
                           HSCode = cpn.HSCode,
                           Elements = cpn.Elements,
                           VATRate = cpn.VATRate,
                           ImportPreferentialTaxRate = cpn.ImportPreferentialTaxRate,
                           OriginRate = term.OriginRate,
                           UnitPrice = entity.UnitPrice,
                           ClientName = wsClient.Name,
                           CreateDate = entity.CreateDate,
                           CompleteDate =cpn.OrderDate,
                           ClassifyFirstOperatorName = firstOperator != null ? firstOperator.RealName : "Npc系统",
                           ClassifySecondOperatorName = secondOperator != null ? secondOperator.RealName : "Npc系统",
                           TaxCode = cpn.TaxCode,
                           TaxName = cpn.TaxName,
                           OrderID = entity.OrderID,
                           ClassifyLogs = logs.Select(l => l.Summary)
                       };

            return linq;
        }
    }
}
