using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ApprovedListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public ApprovedListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public ApprovedListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<ApprovedListViewModel> GetResults(out int totalCount, int pageIndex, int pageSize, string approveAdminID, LambdaExpression[] expressions)
        {
            var costApplies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplies>();
            var costApplyLogs = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplyLogs>();

            var relateLogs = from costApplyLog in costApplyLogs
                             where (costApplyLog.AdminID == approveAdminID || approveAdminID == "SA01")
                             orderby costApplyLog.CreateDate descending
                             group costApplyLog by new { costApplyLog.CostApplyID, } into g
                             select new ApprovedListViewModel
                             {
                                 CostApplyID = g.Key.CostApplyID,
                                 ApproveTime = g.FirstOrDefault().CreateDate,
                             };

            var allInfoRelateLogs = from relateLog in relateLogs
                                    join costApply in costApplies on relateLog.CostApplyID equals costApply.ID
                                    orderby costApply.CreateDate descending
                                    select new ApprovedListViewModel
                                    {
                                        CostApplyID = relateLog.CostApplyID,
                                        PayeeName = costApply.PayeeName,
                                        //CostType = (Enums.CostTypeEnum)costApply.CostType,
                                        //FeeType = (Enums.FeeTypeEnum)costApply.FeeType,
                                        //FeeDesc = costApply.FeeDesc,
                                        Amount = costApply.Amount,
                                        Currency = costApply.Currency,
                                        CostStatus = (Enums.CostStatusEnum)costApply.CostStatus,
                                        CostStatusInt = costApply.CostStatus,
                                        PayTime = costApply.PayTime,
                                        CreateDate = costApply.CreateDate,
                                        AdminID = costApply.AdminID,
                                        ApproveTime = relateLog.ApproveTime,
                                    };

            foreach (var expression in expressions)
            {
                allInfoRelateLogs = allInfoRelateLogs.Where(expression as Expression<Func<ApprovedListViewModel, bool>>);
            }

            totalCount = allInfoRelateLogs.Count();

            var resultList = allInfoRelateLogs.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return resultList;
        }

    }

    public class ApprovedListViewModel
    {
        /// <summary>
        /// CostApplyID
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 收款人姓名
        /// </summary>
        public string PayeeName { get; set; } = string.Empty;

        /// <summary>
        /// 费用类型
        /// </summary>
        public Enums.CostTypeEnum CostType { get; set; }

        /// <summary>
        /// 费用名称
        /// </summary>
        public Enums.FeeTypeEnum FeeType { get; set; }

        /// <summary>
        /// 其它
        /// </summary>
        public string FeeDesc { get; set; } = string.Empty;

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 申请费用状态
        /// </summary>
        public Enums.CostStatusEnum CostStatus { get; set; }

        /// <summary>
        /// 申请费用状态(int)
        /// </summary>
        public int CostStatusInt { get; set; }

        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime ApproveTime { get; set; }
    }
}
