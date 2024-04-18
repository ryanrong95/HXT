using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 海关关税、增值税缴税报表
    /// </summary>
    public class MyCustomsTaxReportsView : View<Needs.Wl.Client.Services.Models.CustomsTaxReport, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyCustomsTaxReportsView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.Models.CustomsTaxReport> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return from decTaxFlow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                       join decTax in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>() on decTaxFlow.DecTaxID equals decTax.ID into decTaxs
                       from decTax in decTaxs.DefaultIfEmpty()
                       join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on decTaxFlow.DecTaxID equals decHead.ID
                       join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decHead.OrderID equals order.ID
                       orderby decTaxFlow.PayDate descending
                       where decTax.InvoiceType == (int)Needs.Wl.Models.Enums.InvoiceType.Service && order.ClientID == this.User.ClientID
                       select new Needs.Wl.Client.Services.Models.CustomsTaxReport
                       {
                           ID = decTaxFlow.ID,
                           DecTaxID = decTaxFlow.DecTaxID,
                           OrderID = decHead.OrderID,
                           ContractNo = decHead.ContrNo,
                           Amount = decTaxFlow.Amount,
                           TaxNumber = decTaxFlow.TaxNumber,
                           TaxType = (Needs.Wl.Models.Enums.DecTaxType)decTaxFlow.TaxType,
                           PayDate = decTaxFlow.PayDate,
                           CreateDate = decTaxFlow.CreateDate
                       };
            }
            else
            {
                return from decTaxFlow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                       join decTax in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>() on decTaxFlow.DecTaxID equals decTax.ID into decTaxs
                       from decTax in decTaxs.DefaultIfEmpty()
                       join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on decTaxFlow.DecTaxID equals decHead.ID
                       join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decHead.OrderID equals order.ID
                       orderby decTaxFlow.PayDate descending
                       where decTax.InvoiceType == (int)Needs.Wl.Models.Enums.InvoiceType.Service && order.UserID == this.User.ID
                       select new Needs.Wl.Client.Services.Models.CustomsTaxReport
                       {
                           ID = decTaxFlow.ID,
                           DecTaxID = decTaxFlow.DecTaxID,
                           OrderID = decHead.OrderID,
                           ContractNo = decHead.ContrNo,
                           Amount = decTaxFlow.Amount,
                           TaxNumber = decTaxFlow.TaxNumber,
                           TaxType = (Needs.Wl.Models.Enums.DecTaxType)decTaxFlow.TaxType,
                           PayDate = decTaxFlow.PayDate,
                           CreateDate = decTaxFlow.CreateDate
                       };
            }
        }
    }
}
