using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class NoticesOutView : NoticesView
    {
        #region 构造函数

        public NoticesOutView()
        {

        }

        public NoticesOutView(PsWmsRepository repository) : base(repository)
        {

        }

        public NoticesOutView(PsWmsRepository reponsitory, IQueryable<Notice> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<Notice> GetIQueryable()
        {
            var notices = base.GetIQueryable().Where(item => item.NoticeType == Enums.NoticeType.Outbound);
            var clients = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>();
            var noticeTransports = new NoticeTransportsView(this.Reponsitory);

            var linq = from entity in notices
                       join client in clients on entity.ClientID equals client.ID
                       join consignee in noticeTransports on entity.ConsigneeID equals consignee.ID into consignees
                       from consignee in consignees.DefaultIfEmpty()
                       join consignor in noticeTransports on entity.ConsignorID equals consignor.ID into consignors
                       from consignor in consignors.DefaultIfEmpty()
                       select new Notice()
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           CompanyID = entity.CompanyID,
                           ConsigneeID = entity.ConsigneeID,
                           ConsignorID = entity.ConsignorID,
                           FormID = entity.FormID,
                           NoticeType = entity.NoticeType,
                           Status = entity.Status,
                           WarehouseID = entity.WarehouseID,
                           WaybillID = entity.WaybillID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                           Exception = entity.Exception,
                           ClientName = client.Name,
                           TrackerID = entity.TrackerID,

                           Consignor = consignor,
                           Consignee = consignee,
                       };
            return linq;
        }

        #region 查询方法

        /// <summary>
        /// 根据通知ID搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public NoticesOutView SearchByNoticeID(string noticeID)
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.ID == noticeID
                       select notice;

            var view = new NoticesOutView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据客户名称搜索
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public NoticesOutView SearchByClientName(string clientName)
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.ClientName.Contains(clientName)
                       select notice;

            var view = new NoticesOutView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据通知状态搜索
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public NoticesOutView SearchByStatus(NoticeStatus status)
        {
            var notices = this.IQueryable.Cast<Notice>();
            var linq = from notice in notices
                       where notice.Status == status
                       select notice;

            var view = new NoticesOutView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        #endregion
    }
}
