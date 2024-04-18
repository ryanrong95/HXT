using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Ccs.Services.Views
{
    public class StorageFeeReportView : QueryView<Models.StorageFeeReport, ScCustomsReponsitory>
    {
        public StorageFeeReportView()
        {
        }

        protected StorageFeeReportView(ScCustomsReponsitory reponsitory, IQueryable<StorageFeeReport> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<Models.StorageFeeReport> GetIQueryable()
        {
            var iQuery = from orderPremium in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StorageFeeReport>()                      
                         select new StorageFeeReport
                         {
                            ID = orderPremium.OrderID,
                            OrderID = orderPremium.OrderID,
                            Business = orderPremium.Business,
                            Catalog = orderPremium.Catalog,
                            Subject = orderPremium.Subject,
                            ReceivableAmount = orderPremium.Price,
                            ReceivableCNYAmount = orderPremium.Price1,
                            PayableAmount = orderPremium.LeftPrice,
                            CreateTime = orderPremium.LeftDate,
                            ReceiptsAmount = orderPremium.RightPrice,
                            AdminName = orderPremium.RealName,
                            Currency = orderPremium.Currency,
                            payCompanyName = orderPremium.payCompanyName,
                            recCompanyName = orderPremium.recCompanyName,
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.StorageFeeReport> iquery = this.IQueryable.Cast<Models.StorageFeeReport>().OrderByDescending(item => item.CreateTime);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue) //如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myBill = iquery.ToArray();

         

            var results = ienum_myBill;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<StorageFeeReport, object> convert = orderPremium => new
            {
                ID = orderPremium.OrderID,
                OrderID = orderPremium.OrderID,
                Business = orderPremium.Business,
                Catalog = orderPremium.Catalog,
                Subject = orderPremium.Subject,
                ReceivableAmount = orderPremium.ReceivableAmount,
                ReceivableCNYAmount = orderPremium.ReceivableCNYAmount == null ? "0" : orderPremium.ReceivableCNYAmount.Value.ToString("0.00"),
                PayableAmount = Math.Round(orderPremium.PayableAmount,2),
                PayableTaxedAmount = orderPremium.PayableTaxedAmount,
                CreateTime = orderPremium.CreateTime.ToString("yyyy-MM-dd"),
                ReceiptsAmount = orderPremium.ReceiptsAmount==null?0:Math.Round(orderPremium.ReceiptsAmount.Value,2),
                Discount = orderPremium.Discount,
                OwedMoney = orderPremium.OwedMoney,
                AdminName = orderPremium.AdminName,
                Currency = orderPremium.Currency,
                payCompanyName = orderPremium.payCompanyName,
                recCompanyName = orderPremium.recCompanyName,
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.CreateTime).Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 查询客户名称
        /// </summary>
        /// <param name="ownerName">客户名称</param>
        /// <returns>视图</returns>
        public StorageFeeReportView SearchByOwnerName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.payCompanyName.Contains(ClientName)
                       select query;

            var view = new StorageFeeReportView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询报告日期
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public StorageFeeReportView SearchByStartDate(DateTime startTime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime >= startTime
                       select query;

            var view = new StorageFeeReportView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期结束时间查询
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public StorageFeeReportView SearchByEndDate(DateTime endTime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime < endTime
                       select query;

            var view = new StorageFeeReportView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询订单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>视图</returns>
        public StorageFeeReportView SearchByType(string itype)
        {
            var linq = from query in this.IQueryable
                       where query.Subject.Contains(itype)
                       select query; ;

            var view = new StorageFeeReportView(this.Reponsitory, linq);
            return view;
        }

    }
}
