using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    /// <summary>
    /// 订单中的费用信息列表视图
    /// </summary>
    public class FeeListInOrderView : QueryView<FeeListInOrderViewModel, PsOrderRepository>
    {
        private string _OrderID { get; set; }

        public FeeListInOrderView(string orderID)
        {
            this._OrderID = orderID;
        }

        protected FeeListInOrderView(PsOrderRepository reponsitory, IQueryable<FeeListInOrderViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<FeeListInOrderViewModel> GetIQueryable()
        {
            var payeeLeftsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.PayeeLefts>();

            var iQuery = from payeeLeft in payeeLeftsTopView
                         where payeeLeft.FormID == this._OrderID
                         select new FeeListInOrderViewModel
                         {
                             CutDateIndex = payeeLeft.CutDateIndex,
                             Subject = payeeLeft.Subject,
                             Total = payeeLeft.Total,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<T[], int> ToMyPage<T>(Func<FeeListInOrderViewModel, T> convert, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<FeeListInOrderViewModel> iquery = this.IQueryable.Cast<FeeListInOrderViewModel>().OrderBy(item => item.CutDateIndex);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_payeeLefts = iquery.ToArray();

            var ienums_linq = from payeeLeft in ienum_payeeLefts
                              orderby payeeLeft.CutDateIndex ascending
                              select new FeeListInOrderViewModel
                              {
                                  CutDateIndex = payeeLeft.CutDateIndex,
                                  Subject = payeeLeft.Subject,
                                  Total = payeeLeft.Total,
                              };

            var results = ienums_linq;

            return new Tuple<T[], int>(results.Select(convert).ToArray(), total);
        }
    }

    public class FeeListInOrderViewModel
    {
        /// <summary>
        /// 期号
        /// </summary>
        public int? CutDateIndex { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Total { get; set; }
    }
}
