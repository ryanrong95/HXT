using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 待开票、开票中 列表视图
    /// </summary>
    public class InvoiceNoticeListView : QueryView<UnInvoiceListViewModel, ScCustomsReponsitory>
    {
        public InvoiceNoticeListView()
        {

        }

        public InvoiceNoticeListView(ScCustomsReponsitory reponsitory, IQueryable<UnInvoiceListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<UnInvoiceListViewModel> GetIQueryable()
        {
            var invoiceNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();
            var adminView = new AdminsTopView(this.Reponsitory);
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var iQuery = from invoiceNotice in invoiceNotices
                         join apply in adminView on invoiceNotice.ApplyID equals apply.ID
                         join client in clients on invoiceNotice.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         //orderby company.Name ascending, invoiceNotice.CreateDate descending
                         select new UnInvoiceListViewModel
                         {
                             InvoiceNoticeID = invoiceNotice.ID,
                             ApplyID = apply.ID,
                             ApplyName = apply.RealName,
                             InvoiceType = (Enums.InvoiceType)invoiceNotice.InvoiceType,
                             CompanyName = company.Name,
                             ClientCode = client.ClientCode,
                             CreateDate = invoiceNotice.CreateDate,
                             InvoiceNoticeStatus = (Enums.InvoiceNoticeStatus)invoiceNotice.Status,
                             ClientID = invoiceNotice.ClientID,
                         };

            return iQuery;
        }


        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<UnInvoiceListViewModel> iquery = this.IQueryable.Cast<UnInvoiceListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            iquery = iquery.OrderByDescending(item => item.CreateDate).OrderBy(item => item.CompanyName);

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myInvoiceNotices = iquery.ToArray();

            //开票通知ID
            var invoiceNoticesID = ienum_myInvoiceNotices.Select(item => item.InvoiceNoticeID);

            //ClientID
            var clientsID = ienum_myInvoiceNotices.Select(item => item.ClientID);

            #region 含税金额

            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();

            var linq_invoiceNoticeItems = from item in invoiceNoticeItems
                                          where invoiceNoticesID.Contains(item.InvoiceNoticeID)
                                          select new
                                          {
                                              InvoiceNoticeID = item.InvoiceNoticeID,
                                              item.Amount,
                                              item.Difference,
                                          };

            var ienums_invoiceNoticeItems = linq_invoiceNoticeItems.ToArray();
            var groups_invoiceNoticeItems = from item in ienums_invoiceNoticeItems
                                            group item by item.InvoiceNoticeID into groups
                                            select new
                                            {
                                                InvoiceNoticeID = groups.Key,
                                                Amount = groups.Sum(t => t.Amount),
                                                Difference = groups.Sum(t => t.Difference),
                                            };

            #endregion

            #region 客户发票对象

            var clientInvoices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>();

            var linq_clientInvoices = from item in clientInvoices
                                      where item.Status == (int)Enums.Status.Normal && clientsID.Contains(item.ClientID)
                                      select new Models.ClientInvoice
                                      {
                                          ID = item.ID,
                                          ClientID = item.ClientID,
                                          Title = item.Title,
                                          TaxCode = item.TaxCode,
                                          Address = item.Address,
                                          Tel = item.Tel,
                                          BankName = item.BankName,
                                          BankAccount = item.BankAccount,
                                          DeliveryType = (Enums.InvoiceDeliveryType)item.DeliveryType,
                                          InvoiceStatus = (Enums.ClientInvoiceStatus)item.InvoiceStatus,
                                          Status = (Enums.Status)item.Status,
                                          CreateDate = item.CreateDate,
                                          UpdateDate = item.UpdateDate,
                                          Summary = item.Summary
                                      };

            var ienums_clientInvoices = linq_clientInvoices.ToArray();

            #endregion

            #region 发票张数
            var linq_InvoiceQty = from invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>()
                                  where invoiceNoticesID.Contains(invoice.InvoiceNoticeID)
                                  group invoice by invoice.InvoiceNoticeID into g
                                  select new
                                  {
                                      InvoiceNoticeID = g.Key,
                                      InvoiceQty = g.Count()
                                  };
            var ienums_InvoiceQty = linq_InvoiceQty.ToArray();

            #endregion

            var ienums_linq = from invoiceNotice in ienum_myInvoiceNotices
                              join invCount in ienums_InvoiceQty on invoiceNotice.InvoiceNoticeID equals invCount.InvoiceNoticeID into invCounts
                              from invcount in invCounts.DefaultIfEmpty()
                              let ogroups_invoiceNoticeItems = groups_invoiceNoticeItems.Single(item => item.InvoiceNoticeID == invoiceNotice.InvoiceNoticeID)
                              let oienums_clientInvoices = ienums_clientInvoices.Single(item => item.ClientID == invoiceNotice.ClientID)
                              select new UnInvoiceListViewModel
                              {
                                  InvoiceNoticeID = invoiceNotice.InvoiceNoticeID,
                                  ApplyID = invoiceNotice.ApplyID,
                                  ApplyName = invoiceNotice.ApplyName,
                                  InvoiceType = (Enums.InvoiceType)invoiceNotice.InvoiceType,
                                  CompanyName = invoiceNotice.CompanyName,
                                  ClientCode = invoiceNotice.ClientCode,
                                  CreateDate = invoiceNotice.CreateDate,
                                  InvoiceNoticeStatus = invoiceNotice.InvoiceNoticeStatus,

                                  Amount = ogroups_invoiceNoticeItems.Amount,
                                  Difference = ogroups_invoiceNoticeItems.Difference,
                                  ClientInvoice = oienums_clientInvoices,
                                  DeliveryType = oienums_clientInvoices.DeliveryType,
                                  InvoiceQty = invcount==null?0:invcount.InvoiceQty
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

            Func<UnInvoiceListViewModel, object> convert = item => new
            {
                ID = item.InvoiceNoticeID,
                ClientCode = item.ClientCode,
                CompanyName = item.CompanyName,
                InvoiceType = item.InvoiceType.GetDescription(),
                Amount = item.Amount.ToRound(2),
                Difference = item.Difference.ToRound(2),
                DeliveryType = item.ClientInvoice.DeliveryType.GetDescription(),
                Status = item.InvoiceNoticeStatus.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                item.ClientInvoice,
                ApplyName = item.ApplyName,
                InvoiceQty = item.InvoiceQty
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据 ApplyID 查询
        /// </summary>
        /// <param name="applyID"></param>
        /// <returns></returns>
        public InvoiceNoticeListView SearchByApplyID(string applyID)
        {
            var linq = from query in this.IQueryable
                       where query.ApplyID.Contains(applyID)
                       select query;

            var view = new InvoiceNoticeListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票类型 InvoiceType 查询
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public InvoiceNoticeListView SearchByInvoiceType(int invoiceType)
        {
            var linq = from query in this.IQueryable
                       where (int)query.InvoiceType == invoiceType
                       select query;

            var view = new InvoiceNoticeListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 公司名称 查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public InvoiceNoticeListView SearchByCompanyName(string companyName)
        {
            var linq = from query in this.IQueryable
                       where query.CompanyName.Contains(companyName)
                       select query;

            var view = new InvoiceNoticeListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 客户编号 查询
        /// </summary>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        public InvoiceNoticeListView SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode == clientCode
                       select query;

            var view = new InvoiceNoticeListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 CreateDate 开始时间 查询
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public InvoiceNoticeListView SearchByStartDate(DateTime startDate)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= startDate
                       select query;

            var view = new InvoiceNoticeListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 CreateDate 结束时间 查询
        /// </summary>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public InvoiceNoticeListView SearchByEndDate(DateTime endDate)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < endDate.AddDays(1)
                       select query;

            var view = new InvoiceNoticeListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 InvoiceNoticeStatus 查询
        /// </summary>
        /// <param name="invoiceNoticeStatus"></param>
        /// <returns></returns>
        public InvoiceNoticeListView SearchByInvoiceNoticeStatus(Enums.InvoiceNoticeStatus invoiceNoticeStatus)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNoticeStatus == invoiceNoticeStatus
                       select query;

            var view = new InvoiceNoticeListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class UnInvoiceListViewModel
    {
        /// <summary>
        /// InvoiceNoticeID
        /// </summary>
        public string InvoiceNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; } = string.Empty;

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 开票类型
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 差额
        /// </summary>
        public decimal Difference { get; set; }

        /// <summary>
        /// 交付方式
        /// </summary>
        public Enums.InvoiceDeliveryType DeliveryType { get; set; }

        /// <summary>
        /// 开票状态
        /// </summary>
        public Enums.InvoiceNoticeStatus InvoiceNoticeStatus { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 客户发票对象
        /// </summary>
        public Models.ClientInvoice ClientInvoice { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplyName { get; set; } = string.Empty;

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; } = string.Empty;
        public int InvoiceQty { get; set; }
    }

}
