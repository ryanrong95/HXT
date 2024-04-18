using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 开票明细列表视图
    /// </summary>
    public class InvoiceDetailView : QueryView<InvoiceDetailViewModel, PvWsOrderReponsitory>
    {
        public InvoiceDetailView()
        {
        }

        protected InvoiceDetailView(PvWsOrderReponsitory reponsitory, IQueryable<InvoiceDetailViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<InvoiceDetailViewModel> GetIQueryable()
        {
            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems>();
            var invoiceNotices = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>();
            var wsClientsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WsClientsTopView>();

            var iQuery = from invoiceNoticeItem in invoiceNoticeItems
                         join invoiceNotice in invoiceNotices on invoiceNoticeItem.InvoiceNoticeID equals invoiceNotice.ID
                         join client in wsClientsTopView on invoiceNotice.ClientID equals client.ID into wsClientsTopView2
                         from client in wsClientsTopView2.DefaultIfEmpty()
                         //where invoiceNotice.Status == (int)InvoiceEnum.Invoiced
                         select new InvoiceDetailViewModel
                         {
                             InvoiceNoticeItemID = invoiceNoticeItem.ID,
                             InvoiceNoticeID = invoiceNoticeItem.InvoiceNoticeID,
                             BillID = invoiceNoticeItem.BillID,
                             CompanyName = invoiceNotice.Title,
                             InvoiceTime = invoiceNotice.UpdateDate,
                             InvoiceNoticeItemCreateDate = invoiceNoticeItem.CreateDate,
                             InvoiceNo = invoiceNoticeItem.InvoiceNo,
                             Difference = invoiceNoticeItem.Difference,
                             Amount = invoiceNoticeItem.Amount,
                             InvoiceNoticeStatus = (InvoiceEnum)invoiceNotice.Status,
                             UnitPrice = invoiceNoticeItem.UnitPrice,
                             FromType = (InvoiceFromType)invoiceNotice.FromType,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<T[], int> ToMyPage<T>(Func<InvoiceDetailViewModel, T> convert, int? pageIndex = null, int? pageSize = null) where T : class
        {
            IQueryable<InvoiceDetailViewModel> iquery = this.IQueryable.Cast<InvoiceDetailViewModel>().OrderByDescending(t => t.InvoiceNoticeItemCreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_invoiceNoticeItems = iquery.ToArray();

            //BillID
            var billsID = ienum_invoiceNoticeItems.Select(item => item.BillID);

            #region OrderIDs(订单号 IDs（逗号间隔）)

            var billItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.BillItems>();

            var linq_billItems = from billItem in billItems
                                 where billsID.Contains(billItem.BillID)
                                 select new
                                 {
                                     BillID = billItem.BillID,
                                     OrderID = billItem.OrderID,
                                 };

            var ienums_billItems = linq_billItems.ToArray();
            var groups_billItems = from item in ienums_billItems
                                   group item by item.BillID into g
                                   select new
                                   {
                                       BillID = g.Key,
                                       OrderIDs = string.Join(", ", g.Select(t => t.OrderID).ToArray()),
                                   };

            #endregion

            var ienums_linq = from invoiceNoticeItem in ienum_invoiceNoticeItems
                              join ogroups_billItems in groups_billItems on invoiceNoticeItem.BillID equals ogroups_billItems.BillID into groups_billItems2
                              from ogroups_billItems in groups_billItems2.DefaultIfEmpty()
                              //let ogroups_billItems = groups_billItems.FirstOrDefault(item => item.BillID == invoiceNoticeItem.BillID)
                              select new InvoiceDetailViewModel
                              {
                                  InvoiceNoticeItemID = invoiceNoticeItem.InvoiceNoticeItemID,
                                  InvoiceNoticeID = invoiceNoticeItem.InvoiceNoticeID,
                                  BillID = invoiceNoticeItem.BillID,
                                  OrderIDs = ogroups_billItems?.OrderIDs,
                                  DetailAmount = invoiceNoticeItem.Amount + (invoiceNoticeItem.Difference ?? 0),
                                  CompanyName = invoiceNoticeItem.CompanyName,
                                  InvoiceNo = invoiceNoticeItem.InvoiceNo,
                                  InvoiceTime = invoiceNoticeItem.InvoiceTime,
                                  //TaxAmount = 返回给页面时计算, (item.DetailSalesTotalPrice * item.InvoiceTaxRate(1.06)).ToRound(2),
                                  DetailSalesUnitPrice = (invoiceNoticeItem.Amount + (invoiceNoticeItem.Difference ?? 0)) / (decimal)1.06,
                                  DetailSalesTotalPrice = (invoiceNoticeItem.Amount + (invoiceNoticeItem.Difference ?? 0)) / (decimal)1.06,
                                  Difference = invoiceNoticeItem.Difference,
                                  InvoiceNoticeItemCreateDate = invoiceNoticeItem.InvoiceNoticeItemCreateDate,
                                  Amount = invoiceNoticeItem.Amount,
                                  InvoiceNoticeStatus = invoiceNoticeItem.InvoiceNoticeStatus,
                                  UnitPrice = invoiceNoticeItem.UnitPrice,
                                  FromType = invoiceNoticeItem.FromType,
                              };

            var results = ienums_linq;

            return new Tuple<T[], int>(results.Select(convert).ToArray(), total);
        }

        /// <summary>
        /// 根据订单号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public InvoiceDetailView SearchByOrderID(string orderID)
        {
            var linq = this.IQueryable;

            var bills = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Bills>();
            var billItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.BillItems>();

            var linq_billItems = from bill in bills
                                 join billItem in billItems on bill.ID equals billItem.BillID
                                 where billItem.OrderID.Contains(orderID)
                                 select bill.ID;

            string[] billIDs = linq_billItems.Distinct().ToArray();

            if (billIDs != null && billIDs.Length > 0)
            {
                linq = from item in linq
                       where billIDs.Contains(item.BillID)
                       select item;
            }

            var view = new InvoiceDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据开票公司查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public InvoiceDetailView SearchByCompanyName(string companyName)
        {
            var linq = from query in this.IQueryable
                       where query.CompanyName.Contains(companyName)
                       select query;

            var view = new InvoiceDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据开票日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public InvoiceDetailView SearchByInvoiceTimeBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceTime >= begin
                       select query;

            var view = new InvoiceDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据开票日期结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public InvoiceDetailView SearchByInvoiceTimeEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceTime < end
                       select query;

            var view = new InvoiceDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 InvoiceNoticeIDs 查询
        /// </summary>
        /// <param name="invoiceNoticeIDs"></param>
        /// <returns></returns>
        public InvoiceDetailView SearchByInvoiceNoticeIDs(string[] invoiceNoticeIDs)
        {
            var linq = from query in this.IQueryable
                       where invoiceNoticeIDs.Contains(query.InvoiceNoticeID)
                       select query;

            var view = new InvoiceDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据开票通知状态查询已开票的
        /// </summary>
        /// <returns></returns>
        public InvoiceDetailView SearchByInvoiceNoticeStatusInvoiced()
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNoticeStatus == InvoiceEnum.Invoiced
                       select query;

            var view = new InvoiceDetailView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 InvoiceNoticeItemID 更新 InvoiceNo
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceNoticeItemID"></param>
        public static void UpdateInvoiceNo(string invoiceNo, string invoiceNoticeItemID)
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems>(new
                {
                    InvoiceNo = invoiceNo,
                }, item => item.ID == invoiceNoticeItemID);
            }
        }

        /// <summary>
        /// 确认开票
        /// </summary>
        public static void ConfirmInvoice(List<InvoiceSubmitModel> InvoiceModelList, string invoiceNoticeID, string adminID)
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                //1.更行通知项的信息
                foreach (var item in InvoiceModelList)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems>(new
                    {
                        InvoiceNo = item.InvoiceNo,
                        //Difference = item.Difference
                    }, x => x.ID == item.ID);
                }

                //2.更新通知状态,和开票人
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>(new
                {
                    Status = (int)InvoiceEnum.Invoiced,
                    //AdminID = adminID,
                    InvoiceDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                }, x => x.ID == invoiceNoticeID);
            }
        }
    }

    public class InvoiceDetailViewModel
    {
        /// <summary>
        /// InvoiceNoticeItemID
        /// </summary>
        public string InvoiceNoticeItemID { get; set; }

        /// <summary>
        /// InvoiceNoticeID
        /// </summary>
        public string InvoiceNoticeID { get; set; }

        /// <summary>
        /// 账单ID
        /// </summary>
        public string BillID { get; set; }

        /// <summary>
        /// 订单号 IDs（逗号间隔）
        /// </summary>
        public string OrderIDs { get; set; }

        /// <summary>
        /// 含税单价
        /// </summary>
        public decimal DetailUnitPrice { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal DetailAmount { get; set; }

        /// <summary>
        /// 开票公司
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceTime { get; set; }

        ///// <summary>
        ///// 税额
        ///// </summary>
        //public decimal TaxAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal DetailSalesUnitPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal DetailSalesTotalPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? Difference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime InvoiceNoticeItemCreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 开票通知状态
        /// </summary>
        public InvoiceEnum InvoiceNoticeStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 开票申请来源
        /// </summary>
        public InvoiceFromType FromType { get; set; }
    }

    /// <summary>
    /// 数据Model（前端上传）
    /// </summary>
    public class InvoiceSubmitModel
    {
        /// <summary>
        /// 通知项ID
        /// </summary>
        public string ID { get; set; }

        public string OrderID { get; set; }

        public decimal Difference { get; set; }

        public string InvoiceNo { get; set; }
        public decimal InvoiceTaxRate { get; set; }

        //public decimal Amount { get; set; }

        public string TaxCode { get; set; }
    }
}
