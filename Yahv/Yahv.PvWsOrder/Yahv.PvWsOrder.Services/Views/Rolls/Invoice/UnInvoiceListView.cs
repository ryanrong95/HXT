using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 待开票列表视图
    /// </summary>
    public class UnInvoiceListView : QueryView<UnInvoiceListViewModel, PvWsOrderReponsitory>
    {
        public UnInvoiceListView()
        {
        }

        protected UnInvoiceListView(PvWsOrderReponsitory reponsitory, IQueryable<UnInvoiceListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<UnInvoiceListViewModel> GetIQueryable()
        {
            var invoiceNotices = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>();
            var wsClientsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WsClientsTopView>();

            var iQuery = from invoiceNotice in invoiceNotices
                         join client in wsClientsTopView on invoiceNotice.ClientID equals client.ID into wsClientsTopView2
                         from client in wsClientsTopView2.DefaultIfEmpty()
                         select new UnInvoiceListViewModel
                         {
                             InvoiceNoticeID = invoiceNotice.ID,
                             EnterCode = client.EnterCode,
                             CompanyName = invoiceNotice.Title,
                             InvoiceType = (InvoiceType)invoiceNotice.Type,
                             InvoiceDeliveryType = (InvoiceDeliveryType)invoiceNotice.DeliveryType,
                             InvoiceNoticeStatus = (InvoiceEnum)invoiceNotice.Status,
                             AdminID = invoiceNotice.AdminID,
                             InvoiceNoticeCreateDate = invoiceNotice.CreateDate,
                             TaxNumber = invoiceNotice.TaxNumber,
                             RegAddress = invoiceNotice.RegAddress,
                             Tel = invoiceNotice.Tel,
                             BankName = invoiceNotice.BankName,
                             BankAccount = invoiceNotice.BankAccount,
                             Summary = invoiceNotice.Summary,
                             PostAddress = invoiceNotice.PostAddress,
                             PostRecipient = invoiceNotice.PostRecipient,
                             PostTel = invoiceNotice.PostTel,
                         };

            return iQuery;
        }


        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<T[], int> ToMyPage<T>(Func<UnInvoiceListViewModel, T> convert, int? pageIndex = null, int? pageSize = null) where T : class
        {
            IQueryable<UnInvoiceListViewModel> iquery = this.IQueryable.Cast<UnInvoiceListViewModel>().OrderByDescending(t => t.InvoiceNoticeCreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_invoiceNotices = iquery.ToArray();

            //开票通知ID
            var invoiceNoticesID = ienum_invoiceNotices.Select(item => item.InvoiceNoticeID);

            //AdminID
            var adminsID = ienum_invoiceNotices.Select(item => item.AdminID);

            #region 含税金额

            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems>();

            var linq_invoiceNoticeItems = from invoiceNoticeItem in invoiceNoticeItems
                                          where invoiceNoticesID.Contains(invoiceNoticeItem.InvoiceNoticeID)
                                          select new
                                          {
                                              InvoiceNoticeID = invoiceNoticeItem.InvoiceNoticeID,
                                              invoiceNoticeItem.Amount,
                                          };

            var ienums_invoiceNoticeItems = linq_invoiceNoticeItems.ToArray();
            var groups_invoiceNoticeItems = from item in ienums_invoiceNoticeItems
                                            group item by item.InvoiceNoticeID into groups
                                            select new
                                            {
                                                InvoiceNoticeID = groups.Key,
                                                Amount = groups.Sum(t => t.Amount),
                                            };

            #endregion

            #region 申请人姓名

            var adminsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.AdminsTopView>();

            var linq_admins = from admin in adminsTopView
                              where adminsID.Contains(admin.ID)
                              select new
                              {
                                  AdminID = admin.ID,
                                  AdminName = admin.RealName,
                              };

            var ienums_admins = linq_admins.ToArray();

            #endregion

            var ienums_linq = from invoiceNotice in ienum_invoiceNotices
                              join admin in ienums_admins on invoiceNotice.AdminID equals admin.AdminID
                              let ogroups_invoiceNoticeItems = groups_invoiceNoticeItems.FirstOrDefault(item => item.InvoiceNoticeID == invoiceNotice.InvoiceNoticeID)
                              select new UnInvoiceListViewModel
                              {
                                  InvoiceNoticeID = invoiceNotice.InvoiceNoticeID,
                                  EnterCode = invoiceNotice.EnterCode,
                                  CompanyName = invoiceNotice.CompanyName,
                                  InvoiceType = invoiceNotice.InvoiceType,
                                  Amount = ogroups_invoiceNoticeItems.Amount,
                                  InvoiceDeliveryType = invoiceNotice.InvoiceDeliveryType,
                                  InvoiceNoticeStatus = invoiceNotice.InvoiceNoticeStatus,
                                  AdminID = invoiceNotice.AdminID,
                                  AdminName = admin != null ? admin.AdminName : string.Empty,
                                  InvoiceNoticeCreateDate = invoiceNotice.InvoiceNoticeCreateDate,
                                  TaxNumber = invoiceNotice.TaxNumber,
                                  RegAddress = invoiceNotice.RegAddress,
                                  Tel = invoiceNotice.Tel,
                                  BankName = invoiceNotice.BankName,
                                  BankAccount = invoiceNotice.BankAccount,
                                  Summary = invoiceNotice.Summary,
                                  PostAddress = invoiceNotice.PostAddress,
                                  PostRecipient = invoiceNotice.PostRecipient,
                                  PostTel = invoiceNotice.PostTel,
                              };

            var results = ienums_linq;

            return new Tuple<T[], int>(results.Select(convert).ToArray(), total);
        }

        /// <summary>
        /// 根据 InvoiceNoticeStatus 查询
        /// </summary>
        /// <param name="invoiceNoticeStatus"></param>
        /// <returns></returns>
        public UnInvoiceListView SearchByInvoiceNoticeStatus(InvoiceEnum invoiceNoticeStatus)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNoticeStatus == invoiceNoticeStatus
                       select query;

            var view = new UnInvoiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据客户编号查询
        /// </summary>
        /// <param name="enterCode"></param>
        /// <returns></returns>
        public UnInvoiceListView SearchByEnterCode(string enterCode)
        {
            var linq = from query in this.IQueryable
                       where query.EnterCode.Contains(enterCode)
                       select query;

            var view = new UnInvoiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据公司名称查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public UnInvoiceListView SearchByCompanyName(string companyName)
        {
            var linq = from query in this.IQueryable
                       where query.CompanyName.Contains(companyName)
                       select query;

            var view = new UnInvoiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据开票类型查询
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public UnInvoiceListView SearchByInvoiceType(InvoiceType invoiceType)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceType == invoiceType
                       select query;

            var view = new UnInvoiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 AdminID 查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public UnInvoiceListView SearchByAdminID(string adminID)
        {
            var linq = from query in this.IQueryable
                       where query.AdminID == adminID
                       select query;

            var view = new UnInvoiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 InvoiceNoticeCreateDate 开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public UnInvoiceListView SearchByInvoiceNoticeCreateDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNoticeCreateDate >= begin
                       select query;

            var view = new UnInvoiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 InvoiceNoticeCreateDate 结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public UnInvoiceListView SearchByInvoiceNoticeCreateDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNoticeCreateDate < end
                       select query;

            var view = new UnInvoiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 InvoiceNoticeIDs 查询
        /// </summary>
        /// <param name="invoiceNoticeIDs"></param>
        /// <returns></returns>
        public UnInvoiceListView SearchByInvoiceNoticeIDs(string[] invoiceNoticeIDs)
        {
            var linq = from query in this.IQueryable
                       where invoiceNoticeIDs.Contains(query.InvoiceNoticeID)
                       select query;

            var view = new UnInvoiceListView(this.Reponsitory, linq);
            return view;
        }


        /// <summary>
        /// 更新一些 InvoiceNotice 为开票中状态
        /// </summary>
        /// <param name="invoiceNoticeIDs"></param>
        public static void UpdateStatusToApplied(string[] invoiceNoticeIDs)
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>(new
                {
                    Status = (int)InvoiceEnum.Applied,
                    UpdateDate = DateTime.Now,
                }, item => invoiceNoticeIDs.Contains(item.ID));
            }
        }
    }

    public class UnInvoiceListViewModel
    {
        /// <summary>
        /// 开票编号
        /// </summary>
        public string InvoiceNoticeID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 发票交付方式
        /// </summary>
        public InvoiceDeliveryType InvoiceDeliveryType { get; set; }

        /// <summary>
        /// 开票状态
        /// </summary>
        public InvoiceEnum InvoiceNoticeStatus { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string AdminName { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime InvoiceNoticeCreateDate { get; set; }

        /// <summary>
        /// 企业税号
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 开户行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 收票地址
        /// </summary>
        public string PostAddress { get; set; }

        /// <summary>
        /// 收票人/公司
        /// </summary>
        public string PostRecipient { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string PostTel { get; set; }
    }
}
