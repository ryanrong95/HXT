using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PayExchangeDetailView : UniqueView<Models.PayExchangeDetail, ScCustomsReponsitory>
    {
        public PayExchangeDetailView()
        { }

        protected PayExchangeDetailView(ScCustomsReponsitory reponsitory, IQueryable<Models.PayExchangeDetail> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.PayExchangeDetail> GetIQueryable()
        {
            //客户名称 //订单号 //合同号
            var orderViews = new OrdersView(this.Reponsitory);
            var decheadViews = new DecHeadsView(this.Reponsitory).Where(t=>t.IsSuccess);

            var results = from payitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                          join payapply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on payitem.PayExchangeApplyID equals payapply.ID
                          join order in orderViews on payitem.OrderID equals order.ID
                          join dechead in decheadViews on order.ID equals dechead.OrderID into t_dechead
                          from dechead in t_dechead.DefaultIfEmpty()
                          where payapply.Status == (int)Enums.Status.Normal
                          && payapply.PayExchangeApplyStatus != (int)Enums.PayExchangeApplyStatus.Cancled
                          && payapply.PayExchangeApplyStatus != (int)Enums.PayExchangeApplyStatus.Auditing
                          //orderby payapply.CreateDate descending
                          select new Models.PayExchangeDetail
                          {
                              ID = payitem.ID,
                              PayExchangeApplyID = payapply.ID,
                              OrderID = order.ID,
                              ClientName = order.Client.Company.Name,
                              PayExchangeRate = payapply.ExchangeRate,
                              CreateDate = payapply.CreateDate,
                              ContrNo = dechead == null ? "" : dechead.ContrNo,
                              Currency = order.Currency,
                              Amount = payitem.Amount,
                              SupplierName = payapply.SupplierEnglishName
                          };

            return results;
        }

        /// <summary>
        ///  根据客户名称查询
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public PayExchangeDetailView SearchByClientName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName == ClientName
                       select query;

            var view = new PayExchangeDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        ///  根据订单号查询
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public PayExchangeDetailView SearchByOrderID(string OrderID)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName == OrderID
                       select query;

            var view = new PayExchangeDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        ///  根据合同号查询
        /// </summary>
        /// <param name="ContrNo"></param>
        /// <returns></returns>
        public PayExchangeDetailView SearchByContrNo(string ContrNo)
        {
            var linq = from query in this.IQueryable
                       where query.ContrNo == ContrNo
                       select query;

            var view = new PayExchangeDetailView(this.Reponsitory, linq);
            return view;
        }

        public PayExchangeDetailView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= fromtime
                       select query;

            var view = new PayExchangeDetailView(this.Reponsitory, linq);
            return view;
        }

        public PayExchangeDetailView SearchByTo(DateTime totime)
        {
            var to = totime.AddDays(1);
            var linq = from query in this.IQueryable
                       where query.CreateDate <= to
                       select query;

            var view = new PayExchangeDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.PayExchangeDetail> iquery = this.IQueryable.Cast<Models.PayExchangeDetail>().OrderByDescending(item => item.CreateDate);

            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var results = iquery.ToArray();


            Func<Models.PayExchangeDetail, object> convert = item => new
            {
                item.ID,
                item.PayExchangeApplyID,
                item.OrderID,
                item.ClientName,
                item.PayExchangeRate,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                item.ContrNo,
                item.Currency,
                item.Amount,
                item.SupplierName,
                item.AmountRMB
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,             
                rows = results.Select(convert).ToArray(),
            };
        }
    }
}
