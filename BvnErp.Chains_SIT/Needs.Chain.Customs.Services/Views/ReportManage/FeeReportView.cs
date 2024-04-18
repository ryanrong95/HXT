using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Ccs.Services.Views
{
    public class FeeReportView : QueryView<Models.FeeReport, ScCustomsReponsitory>
    {
        public FeeReportView()
        {
        }

        protected FeeReportView(ScCustomsReponsitory reponsitory, IQueryable<FeeReport> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<Models.FeeReport> GetIQueryable()
        {          
            var orderPayables = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t=>t.Type== (int)Enums.OrderReceiptType.Receivable);
            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t => t.Type == (int)Enums.OrderReceiptType.Received);
            var orderPremiums = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var iQuery = from orderPremium in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                         join orderPayable in orderPayables on orderPremium.ID equals orderPayable.FeeSourceID
                         join orderReceipt in orderReceipts on orderPremium.ID equals orderReceipt.FeeSourceID into Receipts
                         from receipt in Receipts.DefaultIfEmpty()
                         join order in orders on orderPremium.OrderID equals order.ID
                         join client in clients on order.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         where orderPremium.Status == (int)Enums.Status.Normal
                         select new FeeReport
                         {
                             ID = orderPremium.ID,
                             Date = orderPremium.CreateDate,
                             OrderID = orderPremium.OrderID,
                             Type = (Enums.OrderPremiumType)orderPremium.Type,
                             PayableUnitPrice = orderPremium.UnitPrice,
                             PayableCount = orderPremium.Count,
                             PayableCurrency = orderPremium.Currency,
                             PayableTaxedAmount = orderPayable.Amount,
                             ReceivableAmount = orderPremium.StandardPrice,
                             ReceivableCurrency = orderPremium.StandardCurrency,
                             ReceiptsAmount = receipt == null?0:-receipt.Amount,
                             FeeCreatorID = orderPremium.AdminID,
                             ClientName = company.Name,
                             ClientID = client.ID,
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.FeeReport> iquery = this.IQueryable.Cast<Models.FeeReport>().OrderByDescending(item => item.Date);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue) //如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myBill = iquery.ToArray();

            var clientIDs = ienum_myBill.Select(t => t.ClientID).Distinct();
            var clientView = new ClientsView(this.Reponsitory);
            var clients = clientView.Where(t => clientIDs.Contains(t.ID)).ToArray();

            var adminIDs = ienum_myBill.Select(t => t.FeeCreatorID).Distinct();
            var adminTopView = new AdminsTopView2(this.Reponsitory);
            var admins = adminTopView.Where(t => adminIDs.Contains(t.OriginID)).ToArray();
            var ienums_rates =  this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRateHistoriesTopView>().Where(t => t.Code == "HKD").OrderByDescending(t=>t.Date).ToList();

            var ienums_linq = from orderPremium in ienum_myBill                              
                              join client in clients on orderPremium.ClientID equals client.ID   
                              join admin in admins on orderPremium.FeeCreatorID equals admin.OriginID
                              //join rate in ienums_rates on new { orderPremium.Date.Year, orderPremium.Date.Month, orderPremium.Date.Day} 
                              //equals new {rate.Date.Value.Year,rate.Date.Value.Month,rate.Date.Value.Day} into rates
                              //from rate in rates.DefaultIfEmpty()
                              select new FeeReport
                              {
                                  ID = orderPremium.ID,                                  
                                  Date = orderPremium.Date,
                                  OrderID = orderPremium.OrderID,
                                  Type = orderPremium.Type,
                                  PayableUnitPrice = orderPremium.PayableUnitPrice,
                                  PayableCount = orderPremium.PayableCount,
                                  PayableCurrency = orderPremium.PayableCurrency,
                                  PayableTaxedAmount = orderPremium.PayableTaxedAmount,
                                  ReceivableAmount = orderPremium.ReceivableAmount,
                                  ReceivableCurrency = orderPremium.ReceivableCurrency,
                                  ReceiptsAmount = orderPremium.ReceiptsAmount,
                                  FeeCreatorID = orderPremium.FeeCreatorID,
                                  Client = client,
                                  FeeCreator = admin,                                 
                              };

            var results = ienums_linq.ToList();

            for (int i= 0;i < results.Count();i++)
            {
                var rate = ienums_rates.Where(t => t.Date.Value.Date == results[i].Date.Date).FirstOrDefault();
                if (rate != null)
                {
                    results[i].exchangeRate = rate.Rate;
                }
            }

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<FeeReport, object> convert = orderPremium => new
            {
                ID = orderPremium.ID,
                ClientName = orderPremium.Client.Company.Name,
                Date = orderPremium.Date.ToString("yyyy-MM-dd"),
                OrderID = orderPremium.OrderID,
                Type = orderPremium.Type.GetDescription(),
                PayableAmount = orderPremium.PayableAmount,
                PayableCurrency = orderPremium.PayableCurrency,
                PayableTaxedAmount = orderPremium.PayableTaxedAmount,
                ReceivableAmount = orderPremium.ReceivableAmount,
                ReceivableCurrency = orderPremium.ReceivableCurrency,
                ReceiptsAmount = orderPremium.ReceiptsAmount,
                OwedMoney = orderPremium.OwedMoney,
                Discount = orderPremium.Discount,
                FeeCreator = orderPremium.FeeCreator.RealName,               
                Salesman = orderPremium.Client.ServiceManager.RealName,
                Mechandiser = orderPremium.Client.Merchandiser.RealName,
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.Date).Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 查询客户名称
        /// </summary>
        /// <param name="ownerName">客户名称</param>
        /// <returns>视图</returns>
        public FeeReportView SearchByOwnerName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new FeeReportView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询报告日期
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public FeeReportView SearchByStartDate(DateTime startTime)
        {
            var linq = from query in this.IQueryable
                       where query.Date >= startTime
                       select query;

            var view = new FeeReportView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期结束时间查询
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public FeeReportView SearchByEndDate(DateTime endTime)
        {
            var linq = from query in this.IQueryable
                       where query.Date < endTime
                       select query;

            var view = new FeeReportView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询订单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>视图</returns>
        public FeeReportView SearchByType(int itype)
        {
            var linq = from query in this.IQueryable
                       where query.Type == (Enums.OrderPremiumType)itype
                       select query; ;

            var view = new FeeReportView(this.Reponsitory, linq);
            return view;
        }

    }
}
