using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Linq.Extends;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class NoticesOut_Show_View : UniqueView<Notice, PsWmsRepository>
    {
        #region 构造函数

        public NoticesOut_Show_View()
        {

        }

        public NoticesOut_Show_View(PsWmsRepository repository) : base(repository)
        {

        }

        public NoticesOut_Show_View(PsWmsRepository reponsitory, IQueryable<Notice> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<Notice> GetIQueryable()
        {
            var notices = Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Notices>()
                .Where(item => item.NoticeType == (int)NoticeType.Outbound && item.Status != (int)NoticeStatus.Closed).OrderByDescending(t => t.CreateDate);
            var clients = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>();

            var linq = from entity in notices
                       join client in clients on entity.ClientID equals client.ID
                       select new Notice()
                       {
                           ID = entity.ID,
                           WarehouseID = entity.WarehouseID,
                           ClientID = entity.ClientID,
                           CompanyID = entity.CompanyID,
                           NoticeType = (NoticeType)entity.NoticeType,
                           FormID = entity.FormID,
                           ConsigneeID = entity.ConsigneeID,
                           ConsignorID = entity.ConsignorID,
                           Status = (NoticeStatus)entity.Status,
                           WaybillID = entity.WaybillID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                           TrackerID = entity.TrackerID,
                           Exception = entity.Exception,

                           ClientName = client.Name
                       };
            return linq;
        }

        /// <summary>
        /// 分页视图
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int pageIndex = 1, int pageSize = 20)
        {
            var iquery = this.IQueryable.Cast<Notice>();
            int total = iquery.Count();

            iquery = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            var ienum_iquery = iquery.ToArray();
            var transports = new NoticeTransportsView(this.Reponsitory);
            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.AdminsTopView>();

            var linq = from entity in ienum_iquery
                       join admin in admins on entity.TrackerID equals admin.ID
                       join consignee in transports on entity.ConsigneeID equals consignee.ID
                       select new Notice_Out_Show()
                       {
                           ID = entity.ID,
                           FormID = entity.FormID,
                           Status = entity.Status,
                           CreateDate = entity.CreateDate,
                           ClientName = entity.ClientName,
                           Consignee = consignee,
                           TrackerName = admin.RealName,
                       };
            return new
            {
                Total = total,
                PageSize = pageSize,
                PageIndex = pageIndex,
                data = linq.ToArray(),
            };
        }

        #region 查询方法

        /// <summary>
        /// 根据通知ID搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public NoticesOut_Show_View SearchByNoticeID(string noticeID)
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.ID == noticeID
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据订单ID搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public NoticesOut_Show_View SearchByFromID(string formID)
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.FormID == formID
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据客户名称搜索
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public NoticesOut_Show_View SearchByClientName(string clientName)
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.ClientName.Contains(clientName)
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据通知状态搜索
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public NoticesOut_Show_View SearchByStatus(NoticeStatus status)
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.Status == status
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 搜索运输信息条件
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public NoticesOut_Show_View SearchByTransport(Expression<Func<NoticeTransport, bool>> predicate)
        {
            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory).Where(predicate);

            var linq = from transport in noticeTransportsView
                       join notice in this.IQueryable on transport.ID equals notice.ConsigneeID
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 搜索运输信息条件
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public NoticesOut_Show_View SearchByPcFile(Expression<Func<PcFile, bool>> predicate)
        {
            var pcFilesView = new PcFilesView(this.Reponsitory).Where(predicate);

            var linq = from file in pcFilesView
                       join notice in this.IQueryable on file.MainID equals notice.ID
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据时间搜索
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public NoticesOut_Show_View SearchByDate(DateTime? start, DateTime? end)
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       select notice;
            if (start != null)
            {
                linq = linq.Where(t => t.CreateDate >= start);
            }
            if (end != null)
            {
                var endDate = ((DateTime)end).AddDays(1);
                linq = linq.Where(t => t.CreateDate < endDate);
            }

            var view = new NoticesOut_Show_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 型号查询条件
        /// </summary>
        /// <param name="predicate">型号</param>
        public NoticesOut_Show_View SearchByPartnumber(string Partnumber)
        {
            var productView = new ProductsView(this.Reponsitory).Where(item => item.Partnumber == Partnumber);
            var noticeItemsView = new NoticeItemsView(this.Reponsitory);
            var query = (from entity in noticeItemsView
                         join product in productView on entity.ProductID equals product.ID
                         select entity.NoticeID).ToArray();

            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where query.Contains(notice.ID)
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 待出库出库通知
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public NoticesOut_Show_View Notict_UnExited()
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.Status != NoticeStatus.Rejected && notice.Status != NoticeStatus.Completed && notice.Status != NoticeStatus.Arrivaling
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 已出库出库通知
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public NoticesOut_Show_View Notict_Exited()
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.Status == NoticeStatus.Rejected || notice.Status == NoticeStatus.Completed || notice.Status == NoticeStatus.Arrivaling
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        #endregion

        #region 客户自提

        /// <summary>
        /// 客户自提
        /// </summary>
        /// <returns></returns>
        public NoticesOut_Show_View NoticePickUp()
        {
            Expression<Func<NoticeTransport, bool>> predicate = item => item.TransportMode == TransportMode.PickUp;

            var view = this.SearchByTransport(predicate);
            return view;
        }

        /// <summary>
        /// 客户自提(待提货)
        /// </summary>
        /// <returns></returns>
        public NoticesOut_Show_View NoticePickUp_NotArranged()
        {
            var notices = this.NoticePickUp();
            var pcFilesView = new PcFilesView(this.Reponsitory).Where(t => t.Type == Enums.FileType.CustomSign);

            var linq = from notice in notices
                       join file in pcFilesView on notice.ID equals file.MainID into files
                       where files.Count() == 0
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 客户自提(已提货)
        /// </summary>
        /// <returns></returns>
        public NoticesOut_Show_View NoticePickUp_Arranged()
        {
            var notices = this.NoticePickUp();
            var pcFilesView = new PcFilesView(this.Reponsitory).Where(t => t.Type == Enums.FileType.CustomSign);

            var linq = from notice in notices
                       join file in pcFilesView on notice.ID equals file.MainID into files
                       where files.Count() > 0
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        #endregion

        #region 送货安排

        public NoticesOut_Show_View NoticeDelivery()
        {
            Expression<Func<NoticeTransport, bool>> predicate = item => item.TransportMode == TransportMode.Dtd;

            var view = this.SearchByTransport(predicate);
            return view;
        }

        /// <summary>
        /// 送货安排(待安排)
        /// </summary>
        /// <returns></returns>
        public NoticesOut_Show_View NoticeDelivery_NotArranged()
        {
            var notices = this.NoticeDelivery();
            var pcFilesView = new PcFilesView(this.Reponsitory).Where(t => t.Type == Enums.FileType.DriverSign);

            var linq = from notice in notices
                       join file in pcFilesView on notice.ID equals file.MainID into files
                       where files.Count() == 0
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 送货安排(已安排)
        /// </summary>
        /// <returns></returns>
        public NoticesOut_Show_View NoticeDelivery_Arranged()
        {
            var notices = this.NoticeDelivery();
            var driverFilesView = new PcFilesView(this.Reponsitory).Where(t => t.Type == Enums.FileType.DriverSign);
            var customFilesView = new PcFilesView(this.Reponsitory).Where(t => t.Type == Enums.FileType.CustomSign);

            var linq = from notice in notices
                       join file1 in driverFilesView on notice.ID equals file1.MainID into files1
                       join file2 in customFilesView on notice.ID equals file2.MainID into files2
                       where files1.Count() > 0 && files2.Count() == 0
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 送货安排(已完成)
        /// </summary>
        /// <returns></returns>
        public NoticesOut_Show_View NoticeDelivery_Completed()
        {
            var notices = this.NoticeDelivery();
            var driverFilesView = new PcFilesView(this.Reponsitory).Where(t => t.Type == Enums.FileType.DriverSign);
            var customFilesView = new PcFilesView(this.Reponsitory).Where(t => t.Type == Enums.FileType.CustomSign);

            var linq = from notice in notices
                       join file1 in driverFilesView on notice.ID equals file1.MainID into files1
                       join file2 in customFilesView on notice.ID equals file2.MainID into files2
                       where files1.Count() > 0 && files2.Count() > 0
                       select notice;

            var view = new NoticesOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }

    public class Notice_Out_Show
    {

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 通知状态
        /// </summary>
        public NoticeStatus Status { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 收货人（收货人 + 提送货信息）
        /// </summary>
        public NoticeTransport Consignee { get; set; }

        /// <summary>
        /// 客服人员
        /// </summary>
        public string TrackerName { get; set; }

        public string StatusDec
        {
            get
            {
                return this.Status.GetDescription();
            }
        }

        public string TransportModeDec
        {
            get
            {
                if (this.Consignee != null)
                {
                    return this.Consignee.TransportMode.GetDescription();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }
}
