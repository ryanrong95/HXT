using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views
{
    public class FeeReportDownloadView : QueryView<Models.FeeReport, ScCustomsReponsitory>
    {
        public FeeReportDownloadView()
        {
        }

        protected FeeReportDownloadView(ScCustomsReponsitory reponsitory, IQueryable<FeeReport> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<Models.FeeReport> GetIQueryable()
        {          
            var orderPayables = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t=>t.Type== (int)Enums.OrderReceiptType.Receivable);
            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t => t.Type == (int)Enums.OrderReceiptType.Received);
            var orderPremiums = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientView = new ClientsView(this.Reponsitory);
            var adminTopView = new AdminsTopView2(this.Reponsitory);
            var ienums_rates = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRateHistoriesTopView>().Where(t => t.Code == "HKD");

            var iQuery = from orderPremium in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                         join orderPayable in orderPayables on orderPremium.ID equals orderPayable.FeeSourceID
                         join orderReceipt in orderReceipts on orderPremium.ID equals orderReceipt.FeeSourceID into Receipts
                         from receipt in Receipts.DefaultIfEmpty()
                         join order in orders on orderPremium.OrderID equals order.ID
                         join client in clientView on order.ClientID equals client.ID
                         join admin in adminTopView on orderPremium.AdminID equals admin.OriginID
                         //join rate in ienums_rates on new { orderPremium.CreateDate.Year, orderPremium.CreateDate.Month, orderPremium.CreateDate.Day }
                         //    equals new { rate.Date.Value.Year, rate.Date.Value.Month, rate.Date.Value.Day } into rates
                         //from rate in rates.DefaultIfEmpty()
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
                             FeeCreator = admin,
                             ClientName = client.Company.Name,
                             ClientID = client.ID,
                             Client = client,
                             //exchangeRate = rate==null?0:rate.Rate
                         };
            return iQuery;
        }

        public List<Models.FeeReport> GetDownload(params LambdaExpression[] expressions)
        {
            var orderPayables = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t => t.Type == (int)Enums.OrderReceiptType.Receivable);
            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t => t.Type == (int)Enums.OrderReceiptType.Received);
            var orderPremiums = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientView = new ClientsView(this.Reponsitory);
            var adminTopView = new AdminsTopView2(this.Reponsitory);
            var ienums_rates = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRateHistoriesTopView>().Where(t => t.Code == "HKD");

            var iQuery = from orderPremium in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                         join orderPayable in orderPayables on orderPremium.ID equals orderPayable.FeeSourceID
                         join orderReceipt in orderReceipts on orderPremium.ID equals orderReceipt.FeeSourceID into Receipts
                         from receipt in Receipts.DefaultIfEmpty()
                         join order in orders on orderPremium.OrderID equals order.ID
                         join client in clientView on order.ClientID equals client.ID
                         join admin in adminTopView on orderPremium.AdminID equals admin.OriginID
                         //join rate in ienums_rates on new { orderPremium.CreateDate.Year, orderPremium.CreateDate.Month, orderPremium.CreateDate.Day }
                         //    equals new { rate.Date.Value.Year, rate.Date.Value.Month, rate.Date.Value.Day } into rates
                         //from rate in rates.DefaultIfEmpty()
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
                             ReceiptsAmount = receipt == null ? 0 : -receipt.Amount,
                             FeeCreatorID = orderPremium.AdminID,
                             FeeCreator = admin,
                             ClientName = client.Company.Name,
                             ClientID = client.ID,
                             Client = client,
                             //exchangeRate = rate==null?0:rate.Rate
                         };
            

            foreach (var expression in expressions)
            {
                iQuery = iQuery.Where(expression as Expression<Func<Needs.Ccs.Services.Models.FeeReport, bool>>);
            }

            var data = iQuery.ToList();

            var ienums_exchangeRateLog = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRateHistoriesTopView>().Where(t => t.Code == "HKD").ToList();

            //对每一条数据，先用日志表的值，再用当前实时汇率表中的值
            for (int i = 0; i < data.Count; i++)
            {
                //匹配报关日期汇率
                var theExchangeRateLogDDate = ienums_exchangeRateLog
                        .Where(t => t.Date.Value.Date==data[i].Date.Date)
                        .OrderByDescending(t => t.Date).FirstOrDefault();

                if (theExchangeRateLogDDate != null)
                {
                    data[i].exchangeRate = theExchangeRateLogDDate.Rate;
                }
            }

            return data;

        }



        /// <summary>
        /// 查询客户名称
        /// </summary>
        /// <param name="ownerName">客户名称</param>
        /// <returns>视图</returns>
        public FeeReportDownloadView SearchByOwnerName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new FeeReportDownloadView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询报告日期
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public FeeReportDownloadView SearchByStartDate(DateTime startTime)
        {
            var linq = from query in this.IQueryable
                       where query.Date >= startTime
                       select query;

            var view = new FeeReportDownloadView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期结束时间查询
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public FeeReportDownloadView SearchByEndDate(DateTime endTime)
        {
            var linq = from query in this.IQueryable
                       where query.Date < endTime
                       select query;

            var view = new FeeReportDownloadView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询订单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>视图</returns>
        public FeeReportDownloadView SearchByType(int itype)
        {
            var linq = from query in this.IQueryable
                       where query.Type == (Enums.OrderPremiumType)itype
                       select query; ;

            var view = new FeeReportDownloadView(this.Reponsitory, linq);
            return view;
        }

    }
}
