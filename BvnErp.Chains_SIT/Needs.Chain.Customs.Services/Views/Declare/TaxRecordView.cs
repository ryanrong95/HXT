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
    /// 客户端使用的缴费流水
    /// </summary>
    public class TaxRecordsView : Needs.Linq.Generic.Unique1Classics<DecTaxFlowForUser, ScCustomsReponsitory>
    {
        public TaxRecordsView() : base()
        {
        }
        internal TaxRecordsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecTaxFlowForUser> GetIQueryable(Expression<Func<DecTaxFlowForUser, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from decTaxFlow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                       select new Models.DecTaxFlowForUser
                       {
                           ID = decTaxFlow.ID,
                           DecTaxID=decTaxFlow.DecTaxID,
                           Amount=decTaxFlow.Amount,
                           TaxNumber=decTaxFlow.TaxNumber,
                           TaxType = (DecTaxType)decTaxFlow.TaxType,
                           PayDate= decTaxFlow.PayDate,
                           CreateDate = decTaxFlow.CreateDate,
                       };
            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<DecTaxFlowForUser, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<DecTaxFlowForUser> OnReadShips(DecTaxFlowForUser[] results)
        {
            var decHeadView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(item => results.Select(c=>c.DecTaxID).ToArray().Contains(item.ID)).ToArray();
            var decTaxView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().Where(item => results.Select(c => c.DecTaxID).ToArray().Contains(item.ID)).ToArray();
            var orderView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(item=> decHeadView.Select(c=>c.OrderID).ToArray().Contains(item.ID)).ToArray();
            var fileView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>().Where(item=> decHeadView.Select(c=>c.ID).ToArray().Contains(item.DecHeadID)&& item.Status==(int)Status.Normal).ToArray();

            return from decTaxFlow in results
                   join dechead in decHeadView on decTaxFlow.DecTaxID equals dechead.ID
                   join dectax in decTaxView on decTaxFlow.DecTaxID equals dectax.ID
                   join order in orderView on dechead.OrderID equals order.ID
                   join file in fileView on decTaxFlow.DecTaxID equals file.DecHeadID into files
                   select new DecTaxFlowForUser
                   {
                       ID = decTaxFlow.ID,
                       OrderID = dechead.OrderID,
                       ContrNo = dechead.ContrNo,
                       Amount = decTaxFlow.Amount,
                       ClientID = order.ClientID,
                       UserID = order.UserID,
                       files = files.Select(item=>new DecHeadFile { FileType=(FileType)item.FileType,Url=item.Url,Name=item.Name}),
                       TaxNumber = decTaxFlow.TaxNumber,
                       InvoiceType = (InvoiceType)dectax.InvoiceType,
                       TaxType = (DecTaxType)decTaxFlow.TaxType,
                       PayDate= decTaxFlow.PayDate,
                       CreateDate = decTaxFlow.CreateDate,
                   };
        }
    }
}
