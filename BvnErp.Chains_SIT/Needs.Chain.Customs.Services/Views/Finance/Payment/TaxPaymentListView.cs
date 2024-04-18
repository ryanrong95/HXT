using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 税金处理列表（未付款税金列表）视图
    /// </summary>
    public class TaxPaymentListView : QueryView<TaxPaymentListViewModel, ScCustomsReponsitory>
    {
        public TaxPaymentListView()
        {
        }

        protected TaxPaymentListView(ScCustomsReponsitory reponsitory, IQueryable<TaxPaymentListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<TaxPaymentListViewModel> GetIQueryable()
        {
            var decTaxFlows = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var iQuery = from decTaxFlow in decTaxFlows
                         join decHead in decHeads on decTaxFlow.DecTaxID equals decHead.ID
                         where decTaxFlow.PayDate == null
                         select new TaxPaymentListViewModel
                         {
                             DecTaxFlowID = decTaxFlow.ID,
                             DecHeadID = decTaxFlow.DecTaxID,
                             TaxNumber = decTaxFlow.TaxNumber,
                             TaxType = (Enums.DecTaxType)decTaxFlow.TaxType,
                             Amount = decTaxFlow.Amount,
                             //PayDate = decTaxFlow.PayDate,
                             //BankName = decTaxFlow.BankName,
                             ContrNo = decHead.ContrNo,
                             DDate = decHead.DDate,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<TaxPaymentListViewModel> iquery = this.IQueryable.Cast<TaxPaymentListViewModel>().OrderBy(item => item.DDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDecTaxFlows = iquery.ToArray();

            var decHeadIDs = ienum_myDecTaxFlows.Select(t => t.DecHeadID);

            var ienums_linq = from decTaxFlow in ienum_myDecTaxFlows
                              select new TaxPaymentListViewModel
                              {
                                  DecTaxFlowID = decTaxFlow.DecTaxFlowID,
                                  DecHeadID = decTaxFlow.DecHeadID,
                                  TaxNumber = decTaxFlow.TaxNumber,
                                  TaxType = (Enums.DecTaxType)decTaxFlow.TaxType,
                                  Amount = decTaxFlow.Amount,
                                  //PayDate = decTaxFlow.PayDate,
                                  //BankName = decTaxFlow.BankName,
                                  ContrNo = decTaxFlow.ContrNo,
                                  DDate = decTaxFlow.DDate,
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<TaxPaymentListViewModel, object> convert = item => new
            {
                DecTaxFlowID = item.DecTaxFlowID,
                DecHeadID = item.DecHeadID,
                TaxNumber = item.TaxNumber,
                TaxTypeInt = (int)item.TaxType,
                TaxTypeName = item.TaxType.GetDescription(),
                Amount = item.Amount,
                //PayDate = item.PayDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                //BankName = item.BankName,
                ContrNo = item.ContrNo,
                DDate = item.DDate?.ToString("yyyy-MM-dd HH:mm:ss"),
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }



        /// <summary>
        /// 根据费用类型查询
        /// </summary>
        /// <param name="decTaxType">费用类型</param>
        /// <returns></returns>
        public TaxPaymentListView SearchByDecTaxType(Enums.DecTaxType decTaxType)
        {
            var linq = from query in this.IQueryable
                       where query.TaxType == decTaxType
                       select query;

            var view = new TaxPaymentListView(this.Reponsitory, linq);
            return view;
        }

        ///// <summary>
        ///// 根据缴税日期开始时间查询
        ///// </summary>
        ///// <param name="begin"></param>
        ///// <returns></returns>
        //public TaxPaymentListView SearchByPayDateStartDate(DateTime begin)
        //{
        //    var linq = from query in this.IQueryable
        //               where query.PayDate >= begin
        //               select query;

        //    var view = new TaxPaymentListView(this.Reponsitory, linq);
        //    return view;
        //}

        ///// <summary>
        ///// 根据缴税日期结束时间查询
        ///// </summary>
        ///// <param name="end"></param>
        ///// <returns></returns>
        //public TaxPaymentListView SearchByPayDateEndDate(DateTime end)
        //{
        //    var linq = from query in this.IQueryable
        //               where query.PayDate < end
        //               select query;

        //    var view = new TaxPaymentListView(this.Reponsitory, linq);
        //    return view;
        //}

        /// <summary>
        /// 根据报关日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public TaxPaymentListView SearchByDDateStartDate(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= begin
                       select query;

            var view = new TaxPaymentListView(this.Reponsitory, linq);
            return view;
        }


        /// <summary>
        /// 根据报关日期结束时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public TaxPaymentListView SearchByDDateEndDate(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.DDate < end
                       select query;

            var view = new TaxPaymentListView(this.Reponsitory, linq);
            return view;
        }

    }



    public class TaxPaymentListViewModel
    {
        /// <summary>
        /// DecTaxFlowID
        /// </summary>
        public string DecTaxFlowID { get; set; }

        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 海关发票号
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// 税费类型
        /// </summary>
        public Enums.DecTaxType TaxType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        ///// <summary>
        ///// 缴税日期
        ///// </summary>
        //public DateTime? PayDate { get; set; }

        ///// <summary>
        ///// 银行名称
        ///// </summary>
        //public string BankName { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }
    }

}
