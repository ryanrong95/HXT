using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 改写报关单视图finance/declare/customsinvoice.aspx（暂未使用）
    /// </summary>
    public class TaxRecordsView1 : Needs.Linq.Generic.Unique1Classics<DecTax, ScCustomsReponsitory>
    {
        public TaxRecordsView1() : base()
        {
        }
        internal TaxRecordsView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecTax> GetIQueryable(Expression<Func<DecTax, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from decTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>()
                       select new Models.DecTax
                       {
                           ID = decTax.ID,
                           InvoiceType = (InvoiceType)decTax.InvoiceType,
                           IsUpload = decTax.IsUpload,
                           Status = (Status)decTax.Status,
                           CreateDate=decTax.CreateDate,
                       };
            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<DecTax, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<DecTax> OnReadShips(DecTax[] results)
        {
            var IDs = results.Select(r => r.ID).ToArray();
            var decHeadView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(item => IDs.Contains(item.ID)).ToArray();
            var decListView = new Views.DecOriginListsView(this.Reponsitory).Where(item => IDs.Contains(item.DeclarationID)).ToArray();
            var fileView = new DecHeadFilesView(this.Reponsitory).Where(item => IDs.Contains(item.DecHeadID)).ToArray();
            var flowView = new DecTaxFlowsView(this.Reponsitory).Where(item => IDs.Contains(item.DecheadID)).ToArray();
            var orderView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var taxs = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                       join tax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on item.ID equals tax.OrderItemID
                       select new
                       {
                           item.OrderID,
                           tax,
                       };

            var linq = from dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                       join tax in taxs on dechead.OrderID equals tax.OrderID into _taxs
                       where IDs.Contains(dechead.ID)
                       select new
                       {
                           dechead.ID,
                           taxs = _taxs.Select(item=>item.tax),
                       };

            return from decTax in results
                   join dechead in decHeadView on decTax.ID equals dechead.ID
                   join file in fileView on decTax.ID equals file.DecHeadID into files
                   join flow in flowView on decTax.ID equals flow.DecheadID into flows
                   join declist in decListView on decTax.ID equals declist.DeclarationID into declists
                   join order in orderView on dechead.OrderID equals order.ID
                   join tax in linq on decTax.ID equals tax.ID
                   select new DecTax
                   {
                       ID = dechead.ID,
                       OrderID = dechead.OrderID,
                       ContrNo = dechead.ContrNo,
                       EntryId = dechead.EntryId,
                       DDate = dechead.DDate,
                       OwnerName = dechead.OwnerName,
                       CusDecStatus = dechead.CusDecStatus,
                       CustomMaster = dechead.CustomMaster,
                       IsSuccess = dechead.IsSuccess,
                       Currency = declists.First() == null ? "" : declists.First().TradeCurr,
                       DecAmount = declists.Sum(t => t.DeclTotal),
                       ClientID = order.ClientID,
                       UserID = order.UserID,
                       InvoiceType = (Enums.InvoiceType)decTax.InvoiceType,
                       DecTaxStatus = (Enums.DecTaxStatus)decTax.Status,
                       UploadStatus = (Enums.UploadStatus)decTax.IsUpload,
                       CreateDate = decTax.CreateDate,
                       UpdateDate = decTax.UpdateDate,
                       Summary = decTax.Summary,
                       files = files,
                       flows = flows,
                       TariffValue = tax.taxs.Where(a=>a.Type == (int)Enums.CustomsRateType.ImportTax).Sum(a=>a.Value),
                       AddedValue = tax.taxs.Where(a => a.Type == (int)Enums.CustomsRateType.AddedValueTax).Sum(a => a.Value),
                   };
        }
    }
}
