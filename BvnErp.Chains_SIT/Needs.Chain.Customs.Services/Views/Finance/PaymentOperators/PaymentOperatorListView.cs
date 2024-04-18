using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PaymentOperatorListView : QueryView<PaymentOperatorListViewModel, ScCustomsReponsitory>
    {
        public PaymentOperatorListView()
        {
        }

        protected PaymentOperatorListView(ScCustomsReponsitory reponsitory, IQueryable<PaymentOperatorListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<PaymentOperatorListViewModel> GetIQueryable()
        {
            var financePaymentOperators = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePaymentOperators>();

            var iQuery = from financePaymentOperator in financePaymentOperators
                         where financePaymentOperator.Status == (int)Enums.Status.Normal
                         orderby financePaymentOperator.CreateDate descending
                         select new PaymentOperatorListViewModel
                         {
                             FinancePaymentOperatorID = financePaymentOperator.ID,
                             AdminID = financePaymentOperator.AdminID,
                             Type = (Enums.PaymentOperatorType)financePaymentOperator.Type,
                             CreateDate = financePaymentOperator.CreateDate,
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<PaymentOperatorListViewModel> iquery = this.IQueryable.Cast<PaymentOperatorListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_financePaymentOperators = iquery.ToArray();

            var adminIDs = ienum_financePaymentOperators.Select(t => t.AdminID);

            #region AdminName

            var adminsTopView2 = new AdminsTopView2(this.Reponsitory);

            var linq_adminName = from admin in adminsTopView2
                                 where adminIDs.Contains(admin.OriginID)
                                 group admin by new { admin.OriginID } into g
                                 select new
                                 {
                                     AdminID = g.Key.OriginID,
                                     AdminName = g.FirstOrDefault().RealName,
                                 };

            var ienums_adminName = linq_adminName.ToArray();

            #endregion

            var ienums_linq = from financePaymentOperator in ienum_financePaymentOperators
                              join admin in ienums_adminName on financePaymentOperator.AdminID equals admin.AdminID into ienums_adminName2
                              from admin in ienums_adminName2.DefaultIfEmpty()
                              select new PaymentOperatorListViewModel
                              {
                                  FinancePaymentOperatorID = financePaymentOperator.FinancePaymentOperatorID,
                                  AdminID = financePaymentOperator.AdminID,
                                  CreateDate = financePaymentOperator.CreateDate,

                                  AdminName = admin != null ? admin.AdminName : "",
                              };

            var results = ienums_linq;

            Func<PaymentOperatorListViewModel, object> convert = item => new
            {
                FinancePaymentOperatorID = item.FinancePaymentOperatorID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminID = item.AdminID,
                AdminName = item.AdminName,
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    item.FinancePaymentOperatorID,
                    item.AdminID,
                    item.AdminName,
                };

                return results.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 查出可用的但不是实际的
        /// </summary>
        /// <returns></returns>
        public PaymentOperatorListView SearchByAvaiableButNotReal()
        {
            var financePaymentOperators = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePaymentOperators>();

            var realAdminIDs = financePaymentOperators
                .Where(t => t.Type == (int)Enums.PaymentOperatorType.PaymentOperator
                         && t.Status == (int)Enums.Status.Normal)
                .Select(t => t.AdminID)
                .Distinct().ToArray();

            var linq = from query in this.IQueryable
                       where query.Type == Enums.PaymentOperatorType.Avaiable
                          && !realAdminIDs.Contains(query.AdminID)
                       select query;

            var view = new PaymentOperatorListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查出实际付款操作人
        /// </summary>
        /// <returns></returns>
        public PaymentOperatorListView SearchByPaymentOperator()
        {
            var linq = from query in this.IQueryable
                       where query.Type == Enums.PaymentOperatorType.PaymentOperator
                       select query;

            var view = new PaymentOperatorListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class PaymentOperatorListViewModel
    {
        /// <summary>
        /// FinancePaymentOperatorID
        /// </summary>
        public string FinancePaymentOperatorID { get; set; }

        /// <summary>
        /// AdminID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// AdminName
        /// </summary>
        public string AdminName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Enums.PaymentOperatorType Type { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }
    }

    
}
