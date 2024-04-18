using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.ReportManage
{
    public class BillSummaryView : QueryView<Models.BillSummary, ScCustomsReponsitory>
    {
        public BillSummaryView()
        {
        }

        protected BillSummaryView(ScCustomsReponsitory reponsitory, IQueryable<BillSummary> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<Models.BillSummary> GetIQueryable()
        {

            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var decheads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t=>t.IsSuccess);
            var clientagreements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var orderReceiptView = new OrderReceiptView(this.Reponsitory);
            var clientSuppliers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>();
            var orderConsignees = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>(); 
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var iQuery = from order in orders
                         where order.OrderStatus >= (int)Enums.OrderStatus.QuoteConfirmed && order.OrderStatus <= (int)Enums.OrderStatus.Completed
                         join orderConsignee in orderConsignees on order.ID equals orderConsignee.OrderID
                         join clientSupplier in clientSuppliers on orderConsignee.ClientSupplierID equals clientSupplier.ID
                         join client in clients on order.ClientID equals client.ID
                         join dechead in decheads on order.ID equals dechead.OrderID
                         join clientagreement in clientagreements on order.ClientAgreementID equals clientagreement.ID
                         join receiptview in orderReceiptView on order.ID equals receiptview.OrderID 
                         join company in companies on client.CompanyID equals company.ID
                         select new BillSummary
                         {
                             ID = order.ID,
                             OrderID = order.ID,
                             MainOrderID = order.MainOrderId,
                             ClientID = client.ID,
                             //ClientName = dechead.OwnerName,
                             ClientName = company.Name,
                             ContrNo = dechead.ContrNo,
                             DDate = dechead.DDate,
                             DeclarePrice = order.DeclarePrice,
                             Currency = order.Currency,
                             RealExchangeRate = order.RealExchangeRate,
                             RMBDeclarePrice = order.RealExchangeRate * order.DeclarePrice,
                             CustomsExchangeRate = order.CustomsExchangeRate,
                             AgencyRate = clientagreement.AgencyRate,
                             AddedValueTax = receiptview.AddedValueTax,
                             Incidental = receiptview.Incidental,
                             Tariff = receiptview.Tariff,
                             AgencyFee = receiptview.AgencyFee,
                             TotalTariff = receiptview.AddedValueTax + receiptview.Incidental + receiptview.Tariff + receiptview.AgencyFee,
                             TotalDeclare = order.RealExchangeRate * order.DeclarePrice + receiptview.AddedValueTax + receiptview.Incidental + receiptview.Tariff + receiptview.AgencyFee,
                             ClientType = (Enums.ClientType)client.ClientType,
                             SupplierName = clientSupplier.Name,
                             InvoiceType = (Enums.InvoiceType)clientagreement.InvoiceType
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.BillSummary> iquery = this.IQueryable.Cast<Models.BillSummary>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue) //如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myBill = iquery.ToArray();

            #region 发票信息

            var ordersID = ienum_myBill.Select(t => t.OrderID);
            var clientID = ienum_myBill.Select(t => t.ClientID);

            var invoiceNoticesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();
            var invoiceNoticeItemsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();
            var invoiceWaybillsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>();

            //借助ClientID做第一次过滤
            //var linq_invoice = from item in invoiceNoticeItemsView
            //                   join invoice in invoiceNoticesView on item.InvoiceNoticeID equals invoice.ID
            //                   join waybill in invoiceWaybillsView on invoice.ID equals waybill.InvoiceNoticeID into temp_waybill
            //                   from waybill in temp_waybill.DefaultIfEmpty()
            //                   where clientID.Contains(invoice.ClientID) && invoice.Status == (int)Enums.InvoiceNoticeStatus.Confirmed
            //                   select new
            //                   {
            //                       ID = invoice.ID,
            //                       OrderID = item.OrderID,
            //                       InvoiceType = invoice.InvoiceType,
            //                       DeliveryType = invoice.DeliveryType,
            //                       InvoiceNo = item.InvoiceNo,
            //                       WaybillCode = waybill.WaybillCode,
            //                   };
            //var arr_invoice = linq_invoice.ToArray();


            //foreach (var orderid in ordersID)
            //{
            //    var inv = arr_invoice.Where(t => t.OrderID.Contains(orderid)).FirstOrDefault();

            //}


            #endregion




            var ienums_linq = from bill in ienum_myBill
                              select new BillSummary
                              {
                                  ID = bill.ID,
                                  OrderID = bill.ID,
                                  MainOrderID = bill.MainOrderID,
                                  ClientName = bill.ClientName,
                                  ContrNo = bill.ContrNo,
                                  DDate = bill.DDate,
                                  DeclarePrice = bill.DeclarePrice,
                                  Currency = bill.Currency,
                                  RealExchangeRate = bill.RealExchangeRate,
                                  CustomsExchangeRate = bill.CustomsExchangeRate,
                                  RMBDeclarePrice = bill.RealExchangeRate * bill.DeclarePrice,
                                  AgencyRate = bill.AgencyRate,
                                  AddedValueTax = bill.AddedValueTax,
                                  Incidental = bill.Incidental,
                                  Tariff = bill.Tariff,
                                  AgencyFee = bill.AgencyFee,
                                  TotalTariff = bill.AddedValueTax + bill.Incidental + bill.Tariff + bill.AgencyFee,
                                  TotalDeclare = bill.RealExchangeRate * bill.DeclarePrice + bill.AddedValueTax + bill.Incidental + bill.Tariff + bill.AgencyFee,
                                  ClientType = (Enums.ClientType)bill.ClientType,
                                  SupplierName = bill.SupplierName,
                                  InvoiceType = bill.InvoiceType
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

            Func<BillSummary, object> convert = billsummsry => new
            {
                ID = billsummsry.ID,
                OrderID = billsummsry.ID,
                MainOrderID = billsummsry.MainOrderID,
                ClientName = billsummsry.ClientName,
                ContrNo = billsummsry.ContrNo,
                DDate = billsummsry.DDate.Value.ToString("yyyy-MM-dd"),
                DeclarePrice = billsummsry.DeclarePrice.ToRound(2),
                Currency = billsummsry.Currency,
                RealExchangeRate = billsummsry.RealExchangeRate,
                CustomsExchangeRate = billsummsry.CustomsExchangeRate,
                RMBDeclarePrice = (billsummsry.RealExchangeRate * billsummsry.DeclarePrice).Value.ToRound(2),
                AgencyRate = billsummsry.AgencyRate,
                // AddedValueTax = billsummsry.AddedValueTax,修改为增值税金额小于50，显示为0 by 2020-09-27 yeshuangshuang
                AddedValueTax = (int)billsummsry.ClientType == 1? billsummsry.AddedValueTax.Value.CompareTo(50) == -1? 0: billsummsry.AddedValueTax : billsummsry.AddedValueTax,
                Incidental = billsummsry.Incidental,
                //Tariff = billsummsry.Tariff, 修改为关税金额小于50，显示为0 by 2020-09-27 yeshuangshuang
                Tariff = (int)billsummsry.ClientType == 1? billsummsry.Tariff.Value.CompareTo(50) == -1? 0: billsummsry.Tariff : billsummsry.Tariff,
                AgencyFee = billsummsry.AgencyFee,
                TotalTariff = (((int)billsummsry.ClientType == 1 ? billsummsry.AddedValueTax.Value.CompareTo(50) == -1 ? 0 : billsummsry.AddedValueTax : billsummsry.AddedValueTax) + billsummsry.Incidental + ((int)billsummsry.ClientType == 1 ? billsummsry.Tariff.Value.CompareTo(50) == -1 ? 0 : billsummsry.Tariff : billsummsry.Tariff) + billsummsry.AgencyFee).Value.ToRound(2),
                TotalDeclare = ((billsummsry.RealExchangeRate * billsummsry.DeclarePrice) + (((int)billsummsry.ClientType == 1 ? billsummsry.AddedValueTax.Value.CompareTo(50) == -1 ? 0 : billsummsry.AddedValueTax : billsummsry.AddedValueTax) + billsummsry.Incidental + ((int)billsummsry.ClientType == 1 ? billsummsry.Tariff.Value.CompareTo(50) == -1 ? 0 : billsummsry.Tariff : billsummsry.Tariff) + billsummsry.AgencyFee)).Value.ToRound(2),
                ClientType = (Enums.ClientType)billsummsry.ClientType,
                SupplierName = billsummsry.SupplierName,
                InvoiceType = billsummsry.InvoiceType.GetDescription()
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item=>item.DDate).Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderID"> 订单ID</param>
        /// <returns>视图</returns>
        public BillSummaryView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new BillSummaryView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询客户名称
        /// </summary>
        /// <param name="ownerName">客户名称</param>
        /// <returns>视图</returns>
        public BillSummaryView SearchByOwnerName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new BillSummaryView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询报告日期
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public BillSummaryView SearchByStartDate(DateTime startTime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= startTime
                       select query;

            var view = new BillSummaryView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期结束时间查询
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public BillSummaryView SearchByEndDate(DateTime endTime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate < endTime
                       select query;

            var view = new BillSummaryView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 查询订单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>视图</returns>
        public BillSummaryView SearchByType(int itype)
        {
            var linq = from query in this.IQueryable
                       where query.ClientType == (Enums.ClientType)itype
                       select query; ;

            var view = new BillSummaryView(this.Reponsitory, linq);
            return view;
        }

    }
}
