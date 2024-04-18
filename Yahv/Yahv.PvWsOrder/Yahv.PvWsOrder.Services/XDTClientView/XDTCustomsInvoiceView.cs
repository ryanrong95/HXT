using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class XDTCustomsInvoiceView : UniqueView<XDTCustomsInvoiceInfo, ScCustomReponsitory>
    {
        string ClientID;

        protected XDTCustomsInvoiceView()
        {

        }

        public XDTCustomsInvoiceView(string Enterpriseid)
        {
            this.ClientID = Enterpriseid;
        }

        private XDTCustomsInvoiceView(ScCustomReponsitory reponsitory, IQueryable<XDTCustomsInvoiceInfo> iquery) : base(reponsitory, iquery)
        {

        }

        protected override IQueryable<XDTCustomsInvoiceInfo> GetIQueryable()
        {
            var orderViews = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>();
            var decHeadsViews = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>();
            var decTaxs = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxs>();

            var result = from order in orderViews
                         join dechead in decHeadsViews on order.ID equals dechead.OrderID
                         join dectax in decTaxs on dechead.ID equals dectax.ID
                         where order.ClientID == this.ClientID
                         select new XDTCustomsInvoiceInfo
                         {
                             ID = order.ID,
                             OrderID = order.MainOrderId,
                             TinyOrderID = order.ID,
                             DecHeadID = dechead.ID,
                             DeclarePrice = order.DeclarePrice,
                             DDate = dechead.DDate,
                             InvoiceType = (XDTInvoiceType)dectax.InvoiceType,
                             ClientID = order.ClientID,
                             ContrNo = dechead.ContrNo,
                         };
            return result;
        }



        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(Func<XDTCustomsInvoiceInfo, object> convert, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<XDTCustomsInvoiceInfo> iquery = this.IQueryable.Cast<XDTCustomsInvoiceInfo>().OrderByDescending(item => item.TinyOrderID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myOrderInvoices = iquery.ToArray();

            var tinyOrderIDs = ienum_myOrderInvoices.Select(item => item.TinyOrderID);
            var decheadIDs = ienum_myOrderInvoices.Select(item => item.DecHeadID);
            var clientID = ienum_myOrderInvoices.FirstOrDefault()?.ClientID;

            var decTaxFlows = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>();
            var orderReceipts = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderReceipts>();
            var invoiceNotices = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.InvoiceNotices>();
            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.InvoiceNoticeItems>();

            #region DecTaxs

            //var linq_decTaxFlows = from decTax in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxs>()
            //                       where decheadIDs.Contains(decTax.ID)
            //                       select new
            //                       {
            //                           ID = decTax.ID,
            //                           InvoiceType = decTax.InvoiceType
            //                       };

            //var ienums_decTaxFlows = linq_decTaxFlows.ToArray();

            #endregion

            #region TaxFlows

            var linq_taxFlows = from taxflow in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>()
                                where decheadIDs.Contains(taxflow.DecTaxID)
                                select new CustomsTaxReport
                                {
                                    ID = taxflow.ID,
                                    //OrderID = dechead.OrderID,
                                    DecTaxID = taxflow.DecTaxID,
                                    Amount = taxflow.Amount,
                                    TaxNumber = taxflow.TaxNumber,
                                    TaxType = (DecTaxType)taxflow.TaxType,
                                };

            var ienums_taxFlows = linq_taxFlows.ToArray();

            #endregion

            #region 服务费

            var linq_receipts = from receipt in orderReceipts
                                where receipt.Status == (int)Yahv.Underly.GeneralStatus.Normal
                                && receipt.FeeType != (int)OrderFeeType.Product
                                && receipt.FeeType != (int)OrderFeeType.Tariff
                                && receipt.FeeType != (int)OrderFeeType.AddedValueTax
                                && receipt.Type == (int)OrderReceiptType.Receivable
                                && tinyOrderIDs.Contains(receipt.OrderID)
                                group receipt by receipt.OrderID into g_receipt
                                select new
                                {
                                    OrderID = g_receipt.Key,
                                    AgencyAmount = g_receipt.Sum(t => -t.Amount)
                                };

            var ienums_receipts = linq_receipts.ToArray();

            #endregion

            #region 服务费发票号

            var invoiceList = new List<InvoiceInfoTemp>();

            var linq_invoiceItems = from notice in invoiceNotices
                                    join noticeItem in invoiceNoticeItems on notice.ID equals noticeItem.InvoiceNoticeID
                                    where notice.ClientID == clientID
                                    select new
                                    {
                                        ID = notice.ID,
                                        OrderID = noticeItem.OrderID,
                                        InvoiceNo = noticeItem.InvoiceNo,
                                        InvoiceDate = noticeItem.InvoiceDate
                                    };

            var arry_invoiceItems = linq_invoiceItems.ToArray();

            foreach (var orderid in tinyOrderIDs)
            {
                var invoice = arry_invoiceItems.Where(t => t.OrderID.Contains(orderid)).FirstOrDefault();
                if (invoice != null)
                {
                    invoiceList.Add(new InvoiceInfoTemp
                    {
                        OrderID = orderid,
                        InvoiceNo = invoice.InvoiceNo,
                        InvoiceDate = invoice.InvoiceDate
                    });
                }
            }

            #endregion

            var ienums_linq = from entity in ienum_myOrderInvoices
                              join receipt in ienums_receipts on entity.TinyOrderID equals receipt.OrderID
                              join taxflow in ienums_taxFlows on entity.DecHeadID equals taxflow.DecTaxID into taxflows
                              join invoice in invoiceList on entity.TinyOrderID equals invoice.OrderID into t_invoice
                              from invoice in t_invoice.DefaultIfEmpty()
                              orderby entity.TinyOrderID descending
                              select new XDTCustomsInvoiceInfo
                              {
                                  InvoiceType = entity.InvoiceType,
                                  TinyOrderID = entity.TinyOrderID,
                                  OrderID = entity.OrderID,
                                  EntryID = entity.DecHeadID,
                                  DeclarePrice = entity.DeclarePrice,
                                  DDate = entity.DDate,
                                  AgencyAmount = receipt.AgencyAmount,
                                  InvoiceDate = invoice?.InvoiceDate,
                                  InvoiceNo = invoice?.InvoiceNo,
                                  Taxs = taxflows.ToArray()
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

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                list = results.Select(convert).ToArray(),
            };
        }


        #region 查询条件


        /// <summary>
        /// 过滤开票类型
        /// </summary>
        /// <param name="InvoiceType">开票类型：服务费</param>
        /// <returns></returns>
        public XDTCustomsInvoiceView SearchByInvoiceType(XDTInvoiceType InvoiceType)
        {
            var linq = this.IQueryable.Where(item => item.InvoiceType == InvoiceType);

            return new XDTCustomsInvoiceView(this.Reponsitory, linq);
        }


        /// <summary>
        /// 报关时间起始
        /// </summary>
        /// <param name="StartDate">开始日期</param>
        /// <returns></returns>
        public XDTCustomsInvoiceView SearchByStartDate(DateTime StartDate)
        {
            var linq = this.IQueryable.Where(item => item.DDate >= StartDate);

            return new XDTCustomsInvoiceView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 报关时间结束
        /// </summary>
        /// <param name="EndDate">结束日期</param>
        /// <returns></returns>
        public XDTCustomsInvoiceView SearchByEndDate(DateTime EndDate)
        {
            var linq = this.IQueryable.Where(item => item.DDate <= EndDate);

            return new XDTCustomsInvoiceView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 订单号查询
        /// </summary>
        /// <param name="OrderID">订单号</param>
        /// <returns></returns>
        public XDTCustomsInvoiceView SearchByOrderID(string OrderID)
        {
            var linq = this.IQueryable.Where(item => item.OrderID == OrderID);

            return new XDTCustomsInvoiceView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 合同号查询
        /// </summary>
        /// <param name="ContrNo">合同号</param>
        /// <returns></returns>
        public XDTCustomsInvoiceView SearchByContrNo(string ContrNo)
        {
            var linq = this.IQueryable.Where(item => item.ContrNo == ContrNo);

            return new XDTCustomsInvoiceView(this.Reponsitory, linq);
        }

        /// <summary>
        /// MultiField查询
        /// </summary>
        /// <param name="multiField"></param>
        /// <returns></returns>
        public XDTCustomsInvoiceView SearchByMultiField(string multiField)
        {
            var linq = this.IQueryable.Where(item => item.OrderID == multiField || item.ContrNo == multiField);

            return new XDTCustomsInvoiceView(this.Reponsitory, linq);
        }

        #endregion

    }


    public class XDTCustomsInvoiceInfo : IUnique
    {
        /// <summary>
        /// 发票详情主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 发票类型
        /// 0：全额发票;1: 服务费发票 
        /// </summary>
        public XDTInvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 开票总金额
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 代理费金额（代理费+杂费+商检费）
        /// </summary>
        public decimal AgencyAmount { get; set; }

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string EntryID { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 服务费发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// 税单信息
        /// </summary>
        public CustomsTaxReport[] Taxs { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvoiceInfoTemp
    {
        public string OrderID { get; set; }
        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }
    }

    /// <summary>
    /// 订单应收/实收类型
    /// </summary>
    public enum OrderReceiptType
    {
        /// <summary>
        /// 应收
        /// </summary>
        [Description("应收")]
        Receivable = 1,

        /// <summary>
        /// 实收
        /// </summary>
        [Description("实收")]
        Received = 2
    }

    /// <summary>
    /// 订单收款费用类型
    /// </summary>
    public enum OrderFeeType
    {
        /// <summary>
        /// 货款
        /// </summary>
        [Description("货款")]
        Product = 1,

        /// <summary>
        /// 关税
        /// </summary>
        [Description("关税")]
        Tariff = 2,

        /// <summary>
        /// 增值税
        /// </summary>
        [Description("增值税")]
        AddedValueTax = 3,

        /// <summary>
        /// 代理费
        /// </summary>
        [Description("代理费")]
        AgencyFee = 4,

        /// <summary>
        /// 杂费
        /// </summary>
        [Description("杂费")]
        Incidental = 5,

        /// <summary>
        /// 消费税
        /// </summary>
        [Description("消费税")]
        ExciseTax = 6,
    }
}
