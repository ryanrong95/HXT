using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Linq.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views
{
    public class PayApproverListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public PayApproverListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public PayApproverListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }
        public PayApproverList GetResults(string costApplyID)
        {
            var costApplies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplies>();

            var result = from costApply in costApplies
                         select new PayApproverList
                         {
                             CostApplyID = costApply.ID,
                             CreateDate = costApply.CreateDate,
                             PaperNotesStatusInt = costApply.PaperNotesStatus.Value
                         };
            return result.FirstOrDefault();
        }
        private IQueryable<PayApproverList> GetAll(LambdaExpression[] expressions)
        {
            var costApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplies>();

            var results = from costApply in costApplies
                          where costApply.CostStatus >= (int)Enums.CostStatusEnum.FinanceStaffUnApprove && costApply.CostStatus <= (int)Enums.CostStatusEnum.UnPay
                          select new PayApproverList
                          {
                              CostApplyID = costApply.ID,
                              PayeeName = costApply.PayeeName,
                              CostType = (Enums.CostTypeEnum)costApply.CostType,
                              FeeType = (Enums.FeeTypeEnum)costApply.FeeType,
                              FeeDesc = costApply.FeeDesc,
                              Amount = costApply.Amount,
                              Currency = costApply.Currency,
                              CostStatus = (Enums.CostStatusEnum)costApply.CostStatus,
                              CostStatusInt = costApply.CostStatus,
                              PayTime = costApply.PayTime,
                              CreateDate = costApply.CreateDate,
                              AdminID = costApply.AdminID,
                              PaperNotesStatusInt = costApply.PaperNotesStatus.HasValue ? costApply.PaperNotesStatus.Value : 0,
                              PaperNotesStatus = costApply.PaperNotesStatus == 1 ? Enums.CheckPaperNotesEnum.PaperNotes : Enums.CheckPaperNotesEnum.UnPaperNotes,
                          };
            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<PayApproverList, bool>>);
            }

            return results;
        }
        public List<PayApproverList> GetResult(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var allResults = GetAll(expressions);

            totalCount = allResults.Count();

            var payApproverList = allResults.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return payApproverList;
        }

    }
}