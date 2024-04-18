using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class XDTOrderInvoiceTopView : QueryView<XDTOrderInvoice, ScCustomReponsitory>
    {
        string ClientID;

        protected XDTOrderInvoiceTopView()
        {

        }

        public XDTOrderInvoiceTopView(string Enterpriseid)
        {
            this.ClientID = Enterpriseid;
        }

        private XDTOrderInvoiceTopView(ScCustomReponsitory reponsitory, IQueryable<XDTOrderInvoice> iquery) : base(reponsitory, iquery)
        {

        }

        /// <summary>
        /// 获取结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<XDTOrderInvoice> GetIQueryable()
        {
            #region 原先代码

            //var linq = from dechead in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>()
            //           join taxflow in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>() on dechead.ID equals taxflow.DecTaxID
            //           where dechead.IsSuccess == true
            //           select new CustomsTaxReport
            //           {
            //               ID = taxflow.ID,
            //               OrderID = dechead.OrderID,
            //               DecTaxID = taxflow.DecTaxID,
            //               Amount = taxflow.Amount,
            //               TaxNumber = taxflow.TaxNumber,
            //               TaxType = (DecTaxType)taxflow.TaxType,
            //           };

            //return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderInvoiceTopViewRight>()
            //       join taxflow in linq on entity.TinyOrderID equals taxflow.OrderID into taxflows
            //       join dechead in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>().Where(item => item.IsSuccess == true)
            //       on entity.TinyOrderID equals dechead.OrderID into decheads
            //       from _dechead in decheads.DefaultIfEmpty()
            //       where entity.ClientID == this.ClientID
            //       orderby entity.TinyOrderID descending
            //       select new XDTOrderInvoice
            //       {
            //           ID = entity.ID,
            //           InvoiceNo = entity != null ? entity.InvoiceNo : null,
            //           InvoiceStatus = entity != null ? (XDTModels.InvoiceStatus?)entity.Status : null,
            //           InvoiceType = entity != null ? (XDTInvoiceType?)entity.InvoiceType : null,
            //           OrderID = entity != null ? entity.OrderID : null,
            //           TinyOrderID = entity != null ? entity.TinyOrderID : null,
            //           InvoiceDate = entity != null ? entity.InvoiceDate : null,
            //           Amount = entity != null ? entity.Amount : null,
            //           EntryID = _dechead == null ? "" : _dechead.ID,
            //           Taxs = taxflows.ToArray(),
            //       };

            #endregion

            var orderInvoiceTopViewRight = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderInvoiceTopViewRight>();

            var iQuery = from entity in orderInvoiceTopViewRight
                         where entity.ClientID == this.ClientID
                         select new XDTOrderInvoice
                         {
                             ID = entity.ID,
                             InvoiceNo = entity != null ? entity.InvoiceNo : null,
                             InvoiceStatus = entity != null ? (XDTModels.InvoiceStatus?)entity.Status : null,
                             InvoiceType = entity != null ? (XDTInvoiceType?)entity.InvoiceType : null,
                             OrderID = entity != null ? entity.OrderID : null,
                             TinyOrderID = entity != null ? entity.TinyOrderID : null,
                             InvoiceDate = entity != null ? entity.InvoiceDate : null,
                             Amount = entity != null ? entity.Amount : null,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(Func<XDTOrderInvoice, object> convert, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<XDTOrderInvoice> iquery = this.IQueryable.Cast<XDTOrderInvoice>().OrderByDescending(item => item.TinyOrderID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myOrderInvoices = iquery.ToArray();

            var tinyOrderIDs = ienum_myOrderInvoices.Select(item => item.TinyOrderID);

            var decHeads = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>();
            var decTaxFlows = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>();

            #region TaxFlows

            var linq_taxFlows = from dechead in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>()
                                join taxflow in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>() on dechead.ID equals taxflow.DecTaxID
                                where dechead.IsSuccess == true && tinyOrderIDs.Contains(dechead.OrderID)
                                select new CustomsTaxReport
                                {
                                    ID = taxflow.ID,
                                    OrderID = dechead.OrderID,
                                    DecTaxID = taxflow.DecTaxID,
                                    Amount = taxflow.Amount,
                                    TaxNumber = taxflow.TaxNumber,
                                    TaxType = (DecTaxType)taxflow.TaxType,
                                };

            var ienums_taxFlows = linq_taxFlows.ToArray();

            #endregion

            #region DecHead

            var linq_decHeads = from decHead in decHeads
                                where decHead.IsSuccess == true && tinyOrderIDs.Contains(decHead.OrderID)
                                select new
                                {
                                    DecHeadID = decHead.ID,
                                    OrderID = decHead.OrderID,
                                };

            var ienums_decHeads = linq_decHeads.ToArray();

            #endregion

            var ienums_linq = from entity in ienum_myOrderInvoices
                              join taxflow in ienums_taxFlows on entity.TinyOrderID equals taxflow.OrderID into taxflows
                              join dechead in ienums_decHeads on entity.TinyOrderID equals dechead.OrderID into decheads
                              from _dechead in decheads.DefaultIfEmpty()
                              orderby entity.TinyOrderID descending
                              select new XDTOrderInvoice
                              {
                                  ID = entity.ID,
                                  InvoiceNo = entity != null ? entity.InvoiceNo : null,
                                  InvoiceStatus = entity != null ? (XDTModels.InvoiceStatus?)entity.InvoiceStatus : null,
                                  InvoiceType = entity != null ? (XDTInvoiceType?)entity.InvoiceType : null,
                                  OrderID = entity != null ? entity.OrderID : null,
                                  TinyOrderID = entity != null ? entity.TinyOrderID : null,
                                  InvoiceDate = entity != null ? entity.InvoiceDate : null,
                                  Amount = entity != null ? entity.Amount : null,
                                  EntryID = _dechead == null ? "" : _dechead.DecHeadID,
                                  Taxs = taxflows.ToArray(),
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
        /// 开票时间起始
        /// </summary>
        /// <param name="StartDate">开始日期</param>
        /// <returns></returns>
        public XDTOrderInvoiceTopView SearchByStartDate(DateTime StartDate)
        {
            var linq = this.IQueryable.Where(item => item.InvoiceDate == null || item.InvoiceDate >= StartDate);

            return new XDTOrderInvoiceTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 开票时间结束
        /// </summary>
        /// <param name="EndDate">结束日期</param>
        /// <returns></returns>
        public XDTOrderInvoiceTopView SearchByEndDate(DateTime EndDate)
        {
            var linq = this.IQueryable.Where(item => item.InvoiceDate == null || item.InvoiceDate <= EndDate);

            return new XDTOrderInvoiceTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 型号查询
        /// </summary>
        /// <param name="PartNumber">型号</param>
        /// <returns></returns>
        public XDTOrderInvoiceTopView SearchByPartNumber(string PartNumber)
        {
            var linq = from entity in this.IQueryable
                       join item in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderItems>() on entity.TinyOrderID equals item.OrderID
                       where item.Model.Contains(PartNumber)
                       select entity;

            return new XDTOrderInvoiceTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 开票状态查询
        /// </summary>
        /// <param name="InvoiceStatus">开票状态</param>
        /// <returns></returns>
        public XDTOrderInvoiceTopView SearchByInvoiceStatus(string InvoiceStatus)
        {
            var status = (XDTModels.InvoiceStatus)int.Parse(InvoiceStatus);

            var linq = this.IQueryable.Where(item => item.InvoiceStatus == status);

            return new XDTOrderInvoiceTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 订单号查询
        /// </summary>
        /// <param name="OrderID">订单号</param>
        /// <returns></returns>
        public XDTOrderInvoiceTopView SearchByOrderID(string OrderID)
        {
            var linq = this.IQueryable.Where(item => item.OrderID == OrderID);

            return new XDTOrderInvoiceTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 订单类型查询
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public XDTOrderInvoiceTopView SearchByOrderType(OrderType orderType)
        {
            var pvWsOrderBaseOrderView = new ClientViews.PvWsOrderBaseOrderView(this.Reponsitory);

            var linq = from entity in this.IQueryable
                       join pvOrder in pvWsOrderBaseOrderView on entity.OrderID equals pvOrder.ID
                       where pvOrder.Type == orderType
                       select entity;

            return new XDTOrderInvoiceTopView(this.Reponsitory, linq);
        }

        ///// <summary>
        ///// 按照 InvoiceDate 倒叙排列
        ///// </summary>
        ///// <returns></returns>
        //public XDTOrderInvoiceTopView OrderByDescendingInvoiceDate()
        //{
        //    var linq = this.IQueryable.OrderByDescending(t => t.InvoiceDate);

        //    return new XDTOrderInvoiceTopView(this.Reponsitory, linq);
        //}

        ///// <summary>
        ///// 按照 TinyOrderID 倒叙排列
        ///// </summary>
        ///// <returns></returns>
        //public XDTOrderInvoiceTopView OrderByDescendingTinyOrderID()
        //{
        //    var linq = this.IQueryable.OrderByDescending(t => t.TinyOrderID);

        //    return new XDTOrderInvoiceTopView(this.Reponsitory, linq);
        //}

        #endregion
    }


    public class XDTOrderInvoice : IUnique
    {
        /// <summary>
        /// 发票详情主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 发票类型
        /// 0：全额发票;1: 服务费发票
        /// </summary>
        public XDTInvoiceType? InvoiceType { get; set; }

        /// <summary>
        /// 发票通知ID
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderType? orderType
        {
            get
            {
                using (ClientViews.OrderBaseOrigin orders = new ClientViews.OrderBaseOrigin())
                {
                    return orders[this.OrderID]?.Type ?? OrderType.Declare;
                }
            }
        }


        /// <summary>
        /// 开票金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 发票状态
        /// </summary>
        public XDTModels.InvoiceStatus? InvoiceStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string EntryID { get; set; }

        /// <summary>
        /// 税单信息
        /// </summary>
        public CustomsTaxReport[] Taxs { get; set; }

    }

}
