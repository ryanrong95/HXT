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
    /// 开票申请视图(香港代仓储)
    /// </summary>
    public class Invoices_Show_View : QueryView<InvoiceShow, PvWsOrderReponsitory>
    {
        public Invoices_Show_View()
        {

        }

        protected Invoices_Show_View(PvWsOrderReponsitory reponsitory, IQueryable<InvoiceShow> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<InvoiceShow> GetIQueryable()
        {
            var invoices = new Origins.InvoiceNoticesOrigin(this.Reponsitory).Where(t => t.FromType == InvoiceFromType.HKStore);
            var clients = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WsClientsTopView>();
            var admins = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.AdminsTopView>();

            var iQuery = from entity in invoices
                         join client in clients on entity.ClientID equals client.ID
                         join admin in admins on entity.AdminID equals admin.ID
                         select new InvoiceShow
                         {
                             ID = entity.ID,
                             Title = entity.Title,
                             InvoiceType = entity.Type,
                             InvoiceDeliveryType = entity.DeliveryType,
                             InvoiceNoticeStatus = entity.Status,
                             AdminID = entity.AdminID,
                             CreateDate = entity.CreateDate,
                             InvoiceDate = entity.InvoiceDate,

                             EnterCode = client.EnterCode,
                             ClientName = client.Name,
                             AdminName = admin.RealName,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<IEnumerable<InvoiceShow>, int> ToMyPage(int pageIndex = 1, int pageSize = 20)
        {
            IQueryable<InvoiceShow> iquery = this.IQueryable.Cast<InvoiceShow>().OrderByDescending(t => t.CreateDate);
            int total = iquery.Count();
            iquery = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            #region 含税金额 + 发票号

            //获取开票通知ID
            var ids = iquery.Select(item => item.ID).ToArray();
            var invoiceItems = new Origins.InvoiceNoticeItemsOrigin(this.Reponsitory);

            var linq_invoiceItems = (from entity in invoiceItems
                                     where ids.Contains(entity.InvoiceNoticeID)
                                     select new
                                     {
                                         entity.InvoiceNoticeID,
                                         entity.Amount,
                                         entity.InvoiceNo,
                                     }).ToArray();
            var groups_invoiceItems = from item in linq_invoiceItems
                                      group item by item.InvoiceNoticeID into groups
                                      let invoiceNos = groups.Select(b => b.InvoiceNo).Distinct().ToArray()
                                      select new
                                      {
                                          InvoiceNoticeID = groups.Key,
                                          Amount = groups.Sum(t => t.Amount),
                                          InvoiceNos = String.Join(",", invoiceNos),
                                      };
            #endregion

            var linq = from entity in iquery.ToArray()
                       join item in groups_invoiceItems on entity.ID equals item.InvoiceNoticeID
                       select new InvoiceShow
                       {
                           ID = entity.ID,
                           EnterCode = entity.EnterCode,
                           ClientName = entity.ClientName,
                           Title = entity.Title,
                           InvoiceType = entity.InvoiceType,
                           InvoiceDeliveryType = entity.InvoiceDeliveryType,
                           InvoiceNoticeStatus = entity.InvoiceNoticeStatus,
                           AdminID = entity.AdminID,
                           AdminName = entity.AdminName,
                           CreateDate = entity.CreateDate,
                           InvoiceDate = entity.InvoiceDate,

                           Amount = item.Amount,
                           InvoiceNos = item.InvoiceNos,
                       };
            return new Tuple<IEnumerable<InvoiceShow>, int>(linq, total);
        }

        /// <summary>
        /// 根据开票状态查询
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByStatus(InvoiceEnum status)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceNoticeStatus == status
                       select query;

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据客户编号查询
        /// </summary>
        /// <param name="enterCode"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByEnterCode(string enterCode)
        {
            var linq = from query in this.IQueryable
                       where query.EnterCode.Contains(enterCode)
                       select query;

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByCompanyName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(clientName)
                       select query;

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据发票抬头查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByTitle(string title)
        {
            var linq = from query in this.IQueryable
                       where query.Title.Contains(title)
                       select query;

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据开票类型查询
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByInvoiceType(InvoiceType invoiceType)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceType == invoiceType
                       select query;

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据交付方式查询
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByInvoiceDeliveryType(InvoiceDeliveryType deliveryType)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceDeliveryType == deliveryType
                       select query;

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据申请人查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByAdminID(string adminID)
        {
            var linq = from query in this.IQueryable
                       where query.AdminID == adminID
                       select query;

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据申请时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByCreateDate(DateTime? begin, DateTime? end)
        {
            var linq = this.IQueryable;
            linq = begin == null ? linq : linq.Where(t => t.CreateDate >= begin);
            linq = end == null ? linq : linq.Where(t => t.CreateDate < ((DateTime)end).AddDays(1));

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据开票时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public Invoices_Show_View SearchByInvoiceDate(DateTime? begin, DateTime? end)
        {
            var linq = this.IQueryable;
            linq = begin == null ? linq : linq.Where(t => t.InvoiceDate >= begin);
            linq = end == null ? linq : linq.Where(t => t.InvoiceDate < ((DateTime)end).AddDays(1));

            var view = new Invoices_Show_View(this.Reponsitory, linq);
            return view;
        }
    }

    public class InvoiceShow
    {
        /// <summary>
        /// 开票编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 交付方式
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
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNos { get; set; }
    }
}
