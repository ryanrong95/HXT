using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PayeeListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public PayeeListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public PayeeListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private IQueryable<PayeeListViewModel> GetAll(LambdaExpression[] expressions)
        {
            var costApplyPayees = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplyPayees>();

            var results = from costApplyPayee in costApplyPayees
                          where costApplyPayee.Status == (int)Enums.Status.Normal
                          orderby costApplyPayee.CreateDate descending
                          select new PayeeListViewModel
                          {
                              CostApplyPayeeID = costApplyPayee.ID,
                              AdminID = costApplyPayee.AdminID,
                              PayeeName = costApplyPayee.PayeeName,
                              PayeeAccount = costApplyPayee.PayeeAccount,
                              PayeeBank = costApplyPayee.PayeeBank,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<PayeeListViewModel, bool>>);
            }

            return results;
        }

        public List<PayeeListViewModel> GetResults(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var allResults = GetAll(expressions);

            totalCount = allResults.Count();

            var payeeList = allResults.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return payeeList;
        }
    }

    public class PayeeListViewModel
    {
        public string CostApplyPayeeID { get; set; } = string.Empty;

        public string AdminID { get; set; } = string.Empty;

        public string PayeeName { get; set; } = string.Empty;

        public string PayeeAccount { get; set; } = string.Empty;

        public string PayeeBank { get; set; } = string.Empty;
    }

}
