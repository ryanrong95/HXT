using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PaidListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public PaidListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public PaidListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private IQueryable<PaidListViewModel> GetAll(LambdaExpression[] expressions)
        {
            var paymentNotices = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>();
            var payExchangeApplies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();

            var results = from paymentNotice in paymentNotices
                          join payExchangeApply in payExchangeApplies
                                on new { PayExchangeApplyID = paymentNotice.PayExchangeApplyID, PayExchangeApplyStatus = (int)Enums.Status.Normal, }
                                equals new { PayExchangeApplyID = payExchangeApply.ID, PayExchangeApplyStatus = payExchangeApply.Status, } into payExchangeApplies2
                          from payExchangeApply in payExchangeApplies2.DefaultIfEmpty()

                          where paymentNotice.Status == (int)Enums.PaymentNoticeStatus.Paid
                          orderby paymentNotice.UpdateDate descending
                          select new PaidListViewModel
                          {
                              PaymentNoticeID = paymentNotice.ID,
                              PayExchangeApplyID = payExchangeApply.ID,
                              SeqNo = paymentNotice.SeqNo,
                              PayeeName = paymentNotice.PayeeName,
                              FeeTypeInt = paymentNotice.FeeType,
                              FeeDesc = paymentNotice.FeeDesc,
                              Amount = paymentNotice.Amount,
                              Currency = paymentNotice.Currency,
                              PayDate = paymentNotice.FeeType > 10000 ? null : (DateTime?)paymentNotice.PayDate,
                              Summary = paymentNotice.Summary,
                              PayerID = paymentNotice.PayerID,
                              CostApplyID = paymentNotice.CostApplyID,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<PaidListViewModel, bool>>);
            }

            return results;
        }

        public List<PaidListViewModel> GetResult(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var allResults = GetAll(expressions);

            totalCount = allResults.Count();

            var unPaidList = allResults.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return unPaidList;
        }

    }

    public class PaidListViewModel
    {
        /// <summary>
        /// PaymentNoticeID
        /// </summary>
        public string PaymentNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// PayExchangeApplyID
        /// </summary>
        public string PayExchangeApplyID { get; set; } = string.Empty;

        /// <summary>
        /// SeqNo
        /// </summary>
        public string SeqNo { get; set; } = string.Empty;

        /// <summary>
        /// 收款人
        /// </summary>
        public string PayeeName { get; set; } = string.Empty;

        /// <summary>
        /// 费用类型(int)
        /// </summary>
        public int FeeTypeInt { get; set; }

        /// <summary>
        /// 其它费用描述
        /// </summary>
        public string FeeDesc { get; set; } = string.Empty;

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PayDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 付款财务ID
        /// </summary>
        public string PayerID { get; set; } = string.Empty;

        /// <summary>
        /// 费用ID
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;
    }
}
