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
    /// 已开票 列表视图
    /// </summary>
    public class InvoicedListView : QueryView<InvoicedListViewModel, ScCustomsReponsitory>
    {
        public InvoicedListView()
        {

        }

        public InvoicedListView(ScCustomsReponsitory reponsitory, IQueryable<InvoicedListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<InvoicedListViewModel> GetIQueryable()
        {
            var invoiceNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();
            var adminView = new AdminsTopView(this.Reponsitory);
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var invoiceDate = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceDateTopView>();

            var iQuery = from invoiceNotice in invoiceNotices
                         join apply in adminView on invoiceNotice.ApplyID equals apply.ID
                         join client in clients on invoiceNotice.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         join invDate in invoiceDate on invoiceNotice.ID equals invDate.InvoiceNoticeID into invoices
                         from invdate in invoices.DefaultIfEmpty()
                         where invoiceNotice.Status == (int)Needs.Ccs.Services.Enums.InvoiceNoticeStatus.Confirmed
                         select new InvoicedListViewModel
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
                             InvoiceDate = invdate==null?null: invdate.InvoiceDate,
                             IsExStock = invoiceNotice.IsExStock,
                             VoucherNo = invoiceNotice.VoucherNo
                         };

            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<InvoicedListViewModel> iquery = this.IQueryable.Cast<InvoicedListViewModel>();
            int total = iquery.Count();

            iquery = iquery.OrderByDescending(item => item.CreateDate);

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
                                                Difference = groups.Sum(t=>t.Difference)
                                            };

            #endregion

            #region 客户发票对象

            var clientInvoices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>();

            var linq_clientInvoices = from item in clientInvoices
                                      where item.Status == (int)Enums.Status.Normal && clientsID.Contains(item.ClientID)
                                      select new// Models.ClientInvoice
                                      {
                                          //ID = item.ID,
                                          ClientID = item.ClientID,
                                          //Title = item.Title,
                                          //TaxCode = item.TaxCode,
                                          //Address = item.Address,
                                          //Tel = item.Tel,
                                          //BankName = item.BankName,
                                          //BankAccount = item.BankAccount,
                                          DeliveryType = (Enums.InvoiceDeliveryType)item.DeliveryType,
                                          //InvoiceStatus = (Enums.ClientInvoiceStatus)item.InvoiceStatus,
                                          //Status = (Enums.Status)item.Status,
                                          //CreateDate = item.CreateDate,
                                          //UpdateDate = item.UpdateDate,
                                          //Summary = item.Summary
                                      };

            var ienums_clientInvoices = linq_clientInvoices.ToArray();

            #endregion

            #region 发票运单

            var invoiceWaybills = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>();

            var linq_invoiceWaybills = from item in invoiceWaybills
                                       where invoiceNoticesID.Contains(item.InvoiceNoticeID)
                                       select new
                                       {
                                           InvoiceNoticeID = item.InvoiceNoticeID,
                                           WaybillCode = item.WaybillCode,
                                       };

            var ienums_invoiceWaybills = linq_invoiceWaybills.ToArray();

            #endregion

            #region 开票通知文件数量

            var invoiceNoticeFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles>();

            var linq_invoiceNoticeFiles = from item in invoiceNoticeFiles
                                          where item.Status == (int)Enums.Status.Normal && invoiceNoticesID.Contains(item.InvoiceNoticeID)
                                          select new
                                          {
                                              InvoiceNoticeID = item.InvoiceNoticeID,
                                              InvoiceNoticeFileID = item.ID,
                                          };

            var ienums_invoiceNoticeFiles = linq_invoiceNoticeFiles.ToArray();
            var groups_invoiceNoticeFiles = from item in ienums_invoiceNoticeFiles
                                            group item by item.InvoiceNoticeID into groups
                                            select new
                                            {
                                                InvoiceNoticeID = groups.Key,
                                                InvoiceNoticeFileCount = groups.Count(),
                                            };

            #endregion

            var ienums_linq = from invoiceNotice in ienum_myInvoiceNotices
                              let ogroups_invoiceNoticeItems = groups_invoiceNoticeItems.Single(item => item.InvoiceNoticeID == invoiceNotice.InvoiceNoticeID)
                              let oienums_clientInvoices = ienums_clientInvoices.Single(item => item.ClientID == invoiceNotice.ClientID)
                              join invoiceWaybill in ienums_invoiceWaybills on invoiceNotice.InvoiceNoticeID equals invoiceWaybill.InvoiceNoticeID into ienums_invoiceWaybills2
                              from invoiceWaybill in ienums_invoiceWaybills2.DefaultIfEmpty()
                              join ogroups_invoiceNoticeFiles in groups_invoiceNoticeFiles on invoiceNotice.InvoiceNoticeID equals ogroups_invoiceNoticeFiles.InvoiceNoticeID
                              into groups_invoiceNoticeFiles2
                              from ogroups_invoiceNoticeFiles in groups_invoiceNoticeFiles2.DefaultIfEmpty()
                              select new InvoicedListViewModel
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
                                  DeliveryType = oienums_clientInvoices.DeliveryType,
                                  WaybillCode = invoiceWaybill != null ? invoiceWaybill.WaybillCode : "",
                                  InvoiceNoticeFileCount = ogroups_invoiceNoticeFiles != null ? ogroups_invoiceNoticeFiles.InvoiceNoticeFileCount : 0,
                                  InvoiceDate = invoiceNotice.InvoiceDate,
                                  IsExStock = invoiceNotice.IsExStock,
                                  VoucherNo = invoiceNotice.VoucherNo
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

            Func<InvoicedListViewModel, object> convert = item => new
            {
                ID = item.InvoiceNoticeID,
                ClientCode = item.ClientCode,
                CompanyName = item.CompanyName,
                InvoiceType = item.InvoiceType.GetDescription(),
                Amount = item.Amount.ToRound(2),
                DeliveryType = item.DeliveryType.GetDescription(),
                WaybillCode = item.WaybillCode,
                Status = item.InvoiceNoticeStatus.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                ApplyName = item.ApplyName,
                InvoiceNoticeFileCount = item.InvoiceNoticeFileCount,
                InvoiceDate = item.InvoiceDate == null ? null : item.InvoiceDate.Value.ToString("yyyy-MM-dd"),
                IsExStock = item.IsExStock,
                VoucherNo = item.VoucherNo,
                Difference = item.Difference.ToRound(2)
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
        public InvoicedListView SearchByApplyID(string applyID)
        {
            var linq = from query in this.IQueryable
                       where query.ApplyID.Contains(applyID)
                       select query;

            var view = new InvoicedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票类型 InvoiceType 查询
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public InvoicedListView SearchByInvoiceType(int invoiceType)
        {
            var linq = from query in this.IQueryable
                       where (int)query.InvoiceType == invoiceType
                       select query;

            var view = new InvoicedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询出库状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public InvoicedListView SearchByOutStockStatus(bool status)
        {

            if (status)
            {
                var linq = from query in this.IQueryable
                           where query.IsExStock == status
                           select query;
                var view = new InvoicedListView(this.Reponsitory, linq);
                return view;
            }
            else
            {
                var linq = from query in this.IQueryable
                           where query.IsExStock == status || !query.IsExStock.HasValue
                           select query;
                var view = new InvoicedListView(this.Reponsitory, linq);
                return view;
            }

            
        }

        /// <summary>
        /// 根据 公司名称 查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public InvoicedListView SearchByCompanyName(string companyName)
        {
            var linq = from query in this.IQueryable
                       where query.CompanyName.Contains(companyName)
                       select query;

            var view = new InvoicedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 客户编号 查询
        /// </summary>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        public InvoicedListView SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode == clientCode
                       select query;

            var view = new InvoicedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 CreateDate 开始时间 查询
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public InvoicedListView SearchByStartDate(DateTime startDate)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= startDate
                       select query;

            var view = new InvoicedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 CreateDate 结束时间 查询
        /// </summary>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public InvoicedListView SearchByEndDate(DateTime endDate)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < endDate.AddDays(1)
                       select query;

            var view = new InvoicedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票 开始时间 查询
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public InvoicedListView SearchByInvStartDate(DateTime startDate)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceDate >= startDate
                       select query;

            var view = new InvoicedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票 结束时间 查询
        /// </summary>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public InvoicedListView SearchByInvEndDate(DateTime endDate)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceDate < endDate.AddDays(1)
                       select query;

            var view = new InvoicedListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class InvoicedListViewModel
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

        // 开票类型
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 交付方式
        /// </summary>
        public Enums.InvoiceDeliveryType DeliveryType { get; set; }

        /// <summary>
        /// 发票运单
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 开票状态
        /// </summary>
        public Enums.InvoiceNoticeStatus InvoiceNoticeStatus { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplyName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public int InvoiceNoticeFileCount { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; } = string.Empty;

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }
        public string OrderID { get; set; }
        public string InvoiceNo { get; set; }
        public bool? IsExStock { get; set; }

        public string VoucherNo { get; set; }
        /// <summary>
        /// 差额
        /// </summary>
        public decimal Difference { get; set; }
    }

}
