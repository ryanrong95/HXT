using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ApplicantListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public ApplicantListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public ApplicantListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private IQueryable<ApplicantListViewModel> GetAll(LambdaExpression[] expressions)
        {
            var costApplies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplies>();
            var adminView1 = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            var results = from costApply in costApplies
                          orderby costApply.CreateDate descending
                          select new ApplicantListViewModel
                          {
                              CostApplyID = costApply.ID,
                              PayeeName = costApply.PayeeName,                            
                              Amount = costApply.Amount,
                              Currency = costApply.Currency,
                              CostStatus = (Enums.CostStatusEnum)costApply.CostStatus,
                              CostStatusInt = costApply.CostStatus,
                              PayTime = costApply.PayTime,
                              CreateDate = costApply.CreateDate,
                              AdminID = costApply.AdminID,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<ApplicantListViewModel, bool>>);
            }

            return results;
        }

        public List<ApplicantListViewModel> GetResult(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var allResults = GetAll(expressions);

            totalCount = allResults.Count();

            var applicantList = allResults.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return applicantList;
        }


    }

    public class ApplicantListViewModel
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
    }
}
