using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Enums;
using Yahv.PvRoute.Services.Models;
using Yahv.PvRoute.Services.Views.Origins;
using Yahv.Underly;

namespace Yahv.PvRoute.Services.Views.Rolls
{
    public class BillsRoll : QueryView<Bill, PvRouteReponsitory>
    {
        public BillsRoll()
        {

        }

        public BillsRoll(PvRouteReponsitory reponsitory, IQueryable<Bill> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<Bill> GetIQueryable()
        {
            var billOrigin = new BillOrigin(this.Reponsitory);

            return billOrigin;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)                                     
        {
            IQueryable<Bill> iquery = this.IQueryable.Cast<Bill>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_bill = iquery.ToArray();

            Func<Bill, object> convert = item => new
            {
                ID = item.ID,
                FaceOrderID = item.FaceOrderID,
                Quantity = item.Quantity,
                Weight = item.Weight,
                Price = item.Price,
                Currency = item.Currency,
                Carrier = item.Carrier,

                FeeDetail = item.FeeDetail,
                Checker = item.Checker,
                CheckTime = item.CheckTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                Reviewer = item.Reviewer,
                ReviewTime = item.ReviewTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                Cashier = item.Cashier,
                CashierTime = item.CashierTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                DateIndex = item.DateIndex,
                Source = item.Source,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                CarrierName=item.Carrier.GetDescription(),
                CurrencyDes=item.Currency.GetDescription(),
                SourceDes=item.Source.GetDescription()
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {

                };

                return ienum_bill.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = ienum_bill.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 面单ID搜索
        /// </summary>
        /// <param name="faceOrderID"></param>
        /// <returns></returns>
        public BillsRoll SearchByFaceOrderID(string faceOrderID)
        {

            var linq = from query in this.IQueryable
                       where query.FaceOrderID.Contains(faceOrderID)
                       select query;

            var view = new BillsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据状态查询
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public BillsRoll SearchByCarrier(PrintSource source)
        {
            var linq = from query in this.IQueryable
                       where query.Carrier == source
                       select query;

            var view = new BillsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 期号搜索
        /// </summary>
        /// <param name="dateIndex"></param>
        /// <returns></returns>
        public BillsRoll SearchByDateIndex(int dateIndex)
        {

            var linq = from query in this.IQueryable
                       where query.DateIndex.Equals(dateIndex)
                       select query;

            var view = new BillsRoll(this.Reponsitory, linq);
            return view;
        }

    }
}
