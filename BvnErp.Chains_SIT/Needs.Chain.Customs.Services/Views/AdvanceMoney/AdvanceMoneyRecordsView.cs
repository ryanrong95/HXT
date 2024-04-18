using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AdvanceMoneyRecordsView : QueryView<AdvanceRecordModel, ScCustomsReponsitory>
    {
        public AdvanceMoneyRecordsView()
        {
        }
        internal AdvanceMoneyRecordsView(ScCustomsReponsitory reponsitory, IQueryable<AdvanceRecordModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<Models.AdvanceRecordModel> GetIQueryable()
        {
            var result = from applyRecord in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>()
                         where applyRecord.Status == (int)Status.Normal
                         select new Models.AdvanceRecordModel
                         {
                             ID = applyRecord.ID,
                             ClientID = applyRecord.ClientID,
                             OrderID = applyRecord.OrderID,
                             AdvanceID = applyRecord.AdvanceID,
                             PayExchangeID = applyRecord.PayExchangeID,
                             Amount = applyRecord.Amount,
                             PaidAmount = applyRecord.PaidAmount,
                             AdvanceTime = applyRecord.AdvanceTime,
                             LimitDays = applyRecord.LimitDays,
                             InterestRate = applyRecord.InterestRate,
                             InterestAmount = applyRecord.InterestAmount,
                             OverdueInterestAmount = applyRecord.OverdueInterestAmount,
                             CreateDate = applyRecord.CreateDate
                         };

            return result;
        }
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<AdvanceRecordModel> iquery = this.IQueryable.Cast<AdvanceRecordModel>().OrderByDescending(item => item.AdvanceTime);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum = iquery.ToArray();
            var adminsView = new AdminsTopView(this.Reponsitory).ToArray();

            var ienums_linq = from ienums in ienum
                                  //join admin in adminsView on ienums.AdminID equals admin.ID
                              select new AdvanceRecordModel
                              {
                                  ID = ienums.ID,
                                  ClientID = ienums.ClientID,
                                  OrderID = ienums.OrderID,
                                  AdvanceID = ienums.AdvanceID,
                                  PayExchangeID = ienums.PayExchangeID,
                                  Amount = ienums.Amount,
                                  PaidAmount = ienums.PaidAmount,
                                  AdvanceTime = ienums.AdvanceTime,
                                  LimitDays = ienums.LimitDays,
                                  InterestRate = ienums.InterestRate,
                                  InterestAmount = ienums.InterestAmount,
                                  OverdueInterestAmount = ienums.OverdueInterestAmount,
                                  CreateDate = ienums.CreateDate
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

            Func<AdvanceRecordModel, object> convert = item => new
            {
                ID = item.ID,
                ClientID = item.ClientID,
                OrderID = item.OrderID,
                AdvanceID = item.AdvanceID,
                PayExchangeID = item.PayExchangeID,
                Amount = item.Amount,
                PaidAmount = item.PaidAmount,
                AdvanceTime = item.AdvanceTime.ToString("yyyy-MM-dd HH:mm:ss"),
                LimitDays = item.LimitDays,
                InterestRate = item.InterestRate,
                InterestAmount = item.InterestAmount,
                OverdueInterestAmount = item.OverdueInterestAmount,
                checkOverdue = item.OverdueInterestAmount != 0 ? "是" : "否",
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.AdvanceTime).Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据某个跟单员自己的客户查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public AdvanceMoneyRecordsView SearchByClientID(string ClientID)
        {
            var linq = from query in this.IQueryable
                       where query.ClientID == ClientID
                       select query;

            var view = new AdvanceMoneyRecordsView(this.Reponsitory, linq);
            return view;
        }
    }

    public class AdvanceMoneyOrderIdView : UniqueView<Models.AdvanceRecordModel, ScCustomsReponsitory>
    {
        public AdvanceMoneyOrderIdView()
        {
        }
        internal AdvanceMoneyOrderIdView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.AdvanceRecordModel> GetIQueryable()
        {
            var result = from orders in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                         join advanceRecords in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>() on orders.ID equals advanceRecords.OrderID
                         select new Models.AdvanceRecordModel
                         {
                             ID = orders.ID,
                             OrderID =orders.MainOrderId,
                             Amount = advanceRecords.Amount,
                             AdvanceTime = advanceRecords.AdvanceTime,
                             LimitDays = advanceRecords.LimitDays

                         };
            return result;
        }
    }
}
