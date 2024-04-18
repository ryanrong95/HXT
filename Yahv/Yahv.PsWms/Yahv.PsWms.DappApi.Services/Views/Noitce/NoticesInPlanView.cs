using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Views
{
    /// <summary>
    /// 入库提货单视图
    /// </summary>
    public class NoticesInPlanView : Linq.UniqueView<MyNotice, PsWmsRepository>
    {
        #region 构造函数
        public NoticesInPlanView()
        {
        }

        protected NoticesInPlanView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected NoticesInPlanView(PsWmsRepository reponsitory, IQueryable<MyNotice> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        #endregion

        protected override IQueryable<MyNotice> GetIQueryable()
        {
            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory);

            var noticeView = from notice in new NoticesView(this.Reponsitory)
                             where notice.NoticeType == NoticeType.Inbound && (notice.Status == NoticeStatus.Arranging || notice.Status == NoticeStatus.Processing || notice.Status == NoticeStatus.Completed)
                             orderby notice.CreateDate descending
                             select new MyNotice
                             {
                                 ID = notice.ID,
                                 ClientID = notice.ClientID,
                                 CompanyID = notice.CompanyID,
                                 ConsigneeID = notice.ConsigneeID,
                                 ConsignorID = notice.ConsignorID,
                                 FormID = notice.FormID,
                                 NoticeType = notice.NoticeType,
                                 Status = notice.Status,
                                 WarehouseID = notice.WarehouseID,
                                 WaybillID = notice.WaybillID,
                                 TrackerID = notice.TrackerID,
                                 CreateDate = notice.CreateDate,
                                 ModifyDate = notice.ModifyDate,
                                 Summary = notice.Summary,
                                 Exception = notice.Exception,
                             };
            
            var view = from notice in noticeView
                       join _noticeTransport in noticeTransportsView
                       on notice.ConsignorID equals _noticeTransport.ID into noticeTransports
                       from noticeTransport in noticeTransports.DefaultIfEmpty()
                       where noticeTransport.TransportMode == TransportMode.PickUp
                       orderby notice.CreateDate descending
                       select notice;

            return view;
        }

        /// <summary>
        /// 补全数据
        /// </summary>
        /// <returns></returns>
        public object[] ToMyArray()
        {
            return this.ToMyPage() as object[];
        }

        /// <summary>
        /// 获取单个的详情信息
        /// </summary>
        /// <returns></returns>
        public object Single()
        {
            var results = this.ToMyPage(1) as object[];
            return results?.FirstOrDefault();
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<MyNotice>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_iquery = iquery.ToArray();

            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory);

            #region 补充完整对象

            var transportIDs = ienum_iquery.Select(item => item.ConsignorID);
            var ienum_transports = noticeTransportsView.Where(item => transportIDs.Contains(item.ID));

            var clientIDs = ienum_iquery.Select(item => item.ClientID);
            var clientView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
                             where clientIDs.Contains(client.ID)
                             select new
                             {
                                 client.ID,
                                 client.Name,
                             };

            var ienum_clients = clientView.ToArray();
            var linq = from notice in ienum_iquery
                       join _client in ienum_clients on notice.ClientID equals _client.ID into clients
                       from client in clients.DefaultIfEmpty()
                       join _consignor in ienum_transports on notice.ConsignorID equals _consignor.ID into consignors
                       from consignor in consignors.DefaultIfEmpty()
                       join _admin in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.AdminsTopView>() on notice.TrackerID equals _admin.ID into admins
                       from admin in admins.DefaultIfEmpty()
                       select new
                       {
                           ClientName = client?.Name,
                           Address = consignor?.Address,
                           consignor,
                           TransportMode = consignor?.TransportMode,
                           notice.TrackerID,
                           notice.NoticeType,
                           TrackerName = admin?.RealName,
                           TakingTime = consignor?.TakingTime,
                           CreateDate = notice.CreateDate,
                           FormID = notice.FormID,
                           ID = notice.ID,
                           notice.Summary,
                           notice.Exception,
                           notice.ClientID,
                       };

            #endregion

            var ienum_linq = linq.ToArray();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                var result_fenye = ienum_linq.Select(item => new
                {
                    item.ClientID,
                    item.ClientName,
                    item.Address,
                    item.TransportMode,
                    TransportModeDes = item.TransportMode?.GetDescription(),
                    item.TrackerID,
                    item.TrackerName,
                    item.TakingTime,
                    item.CreateDate,
                    item.FormID,
                    item.ID
                });

                return new
                {
                    Total = total,
                    Size = pageSize,
                    Index = pageIndex,
                    Data = result_fenye,
                };
            }
            else
            {
                var result = ienum_linq.Select(item => new
                {
                    item.ClientID,
                    item.NoticeType,
                    NoticeTypeDes = item.NoticeType.GetDescription(),
                    item.ClientName,
                    item.Address,
                    item.TransportMode,
                    TransportModeDes = item.TransportMode?.GetDescription(),
                    item.TrackerID,
                    item.TrackerName,
                    item.TakingTime,
                    ConsignorID = item.consignor?.ID,
                    Contact = item.consignor?.Contact,
                    Phone = item.consignor?.Phone,
                    TakerName = item.consignor?.TakerName,
                    TakerLicense = item.consignor?.TakerLicense,
                    TakerPhone = item.consignor?.TakerPhone,
                    TakerIDCode = item.consignor?.TakerIDCode,
                    TakerIDType = item.consignor?.TakerIDType,                    
                    TakerIDTypeDes = item.consignor?.TakerIDType?.GetDescription(),
                    item.Summary,
                    item.Exception,
                    item.CreateDate,
                    item.FormID,
                    item.ID
                });

                return result.ToArray();
            }
        }        

        #region 搜索方法
        /// <summary>
        /// 根据状态搜索
        /// </summary>
        /// <param name="status">status = true 默认为未安排 TakerName == NULL , false 已安排 TakerName != null</param>
        /// <returns></returns>
        public NoticesInPlanView SearchByStatus(bool status = true)
        {
            var noticeInPlanView = this.IQueryable.Cast<MyNotice>();
            
            var transportsID = noticeInPlanView.Select(item => item.ConsignorID).Distinct().ToArray();
            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory).Where(item => transportsID.Contains(item.ID));

            var linq = from notice in noticeInPlanView
                       join _noticeTransport in noticeTransportsView
                       on notice.ConsignorID equals _noticeTransport.ID into noticeTransports
                       from noticeTransport in noticeTransports.DefaultIfEmpty()
                       where (noticeTransport.TakerName == null) == status
                       select notice;            

            var view = new NoticesInPlanView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据WarehouseID搜索
        /// </summary>
        /// <param name="whid"></param>
        /// <returns></returns>
        public NoticesInPlanView SearchByWarehouseID(string whid)
        {
            var noticeInPlanView = this.IQueryable.Cast<MyNotice>();
            var linq = from notice in noticeInPlanView
                       where notice.WarehouseID.ToLower().StartsWith(whid)
                       select notice;

            var view = new NoticesInPlanView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据ClientName搜索
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public NoticesInPlanView SearchByClientName(string clientName)
        {
            var noticeInPlanView = this.IQueryable.Cast<MyNotice>();

            var linq = from notice in noticeInPlanView
                       join client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
                       on notice.ClientID equals client.ID                       
                       where client.Name.Contains(clientName)
                       select notice;

            var view = new NoticesInPlanView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据通知ID搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public NoticesInPlanView SearchByNoticeID(string noticeID)
        {
            var noticeInPlanView = this.IQueryable.Cast<MyNotice>();
            var linq = from notice in noticeInPlanView
                       where notice.ID == noticeID
                       select notice;

            var view = new NoticesInPlanView(this.Reponsitory, linq)
            {
            };

            return view;
        }

        /// <summary>
        /// 根据FormID搜索
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public NoticesInPlanView SearchByFormID(string formID)
        {
            var noticeInPlanView = this.IQueryable.Cast<MyNotice>();
            var linq = from notice in noticeInPlanView
                       where notice.FormID == formID
                       select notice;

            var view = new NoticesInPlanView(this.Reponsitory, linq)
            {
            };

            return view;
        }

        /// <summary>
        /// 根据起止时间搜索
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public NoticesInPlanView SearchByDate(DateTime? start, DateTime? end)
        {
            Expression<Func<MyNotice, bool>> predicate = notice => (start.HasValue ? notice.CreateDate >= start.Value : true)
                && (end.HasValue ? notice.CreateDate < end.Value.AddDays(1) : true);
            
            var noticeInPlanView = this.IQueryable.Cast<MyNotice>();
            var transportsID = noticeInPlanView.Select(item => item.ConsignorID).Distinct().ToArray();
            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory).Where(item => transportsID.Contains(item.ID));

            //var linq = noticeInPlanView.Where(predicate);

            var linq = from notice in noticeInPlanView
                       join _noticeTransport in noticeTransportsView
                       on notice.ConsignorID equals _noticeTransport.ID into noticeTransports
                       from noticeTransport in noticeTransports.DefaultIfEmpty()
                       where noticeTransport.TakingTime >= start.Value && noticeTransport.TakingTime < end.Value.AddDays(1)
                       orderby notice.CreateDate descending
                       select notice;

            var view = new NoticesInPlanView(this.Reponsitory, linq);
            return view;
        }
        #endregion

        /// <summary>
        /// 入库自提安排
        /// </summary>
        /// <param name="jobject"></param>
        public void Arrange(JObject jobject)
        {
            string noticeID = jobject["NoticeID"].Value<string>();
            string consignorID = jobject["ConsignorID"].Value<string>();
            string takerName = jobject["TakerName"].Value<string>();
            string takerPhone = jobject["TakerPhone"].Value<string>();
            string takerLicence = jobject["TakerLicence"].Value<string>();
            string exception = jobject["Exception"]?.Value<string>();

            if (string.IsNullOrWhiteSpace(noticeID))
            {
                throw new ArgumentException("参数NoticeID不能为空!");
            }

            if (string.IsNullOrWhiteSpace(consignorID))
            {
                throw new ArgumentException("参数ConsignorID不能为空!");
            }

            var notice = new NoticesView().Single(item => item.ID == noticeID);

            if (notice.ConsignorID != consignorID)
            {
                throw new Exception( $"参数 ConsignorID:{consignorID} 与 NoticeID: {noticeID}对应的通知中的收货人信息不匹配");
            }

            var noticeTransportsView = new NoticeTransportsView();

            if (noticeTransportsView.Any(item => item.ID == consignorID))
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeTransports>(
                    new
                    {
                        TakerName = takerName,
                        TakerPhone = takerPhone,
                        TakerLicense = takerLicence,
                    }, item => item.ID == consignorID);
            }
            else
            {
                throw new Exception("没有找到对应的ConsignorID:{consignorID}，不能保存修改对应的信息");
            }

            if (!string.IsNullOrWhiteSpace(exception))
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Notices>(new
                {
                    Exception = exception,
                }, item => item.ID == noticeID);
            }
        }
    }
}
