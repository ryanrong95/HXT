using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Services.Models;
using Yahv.Utils.Serializers;
using Layers.Data;
using Wms.Services.Models;
using Yahv.Payments;
using Yahv.Utils.Converters.Contents;
using Wms.Services.Models_chenhan;

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 用于深圳的出库单的送货文件的上传
    /// </summary>
    public class CgSzPickingsView : QueryView<object, PvWmsRepository>
    {
        #region 构造器
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public CgSzPickingsView()
        {
        }

        /// <summary>
        /// 有参构造函数，外部调用使用
        /// </summary>
        /// <param name="reponsitory">数据库实例</param>
        protected CgSzPickingsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 有参构造函数，条件查询使用
        /// </summary>
        /// <param name="reponsitory">数据库实例</param>
        /// <param name="iQueryable">查询结果集</param>
        protected CgSzPickingsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<object> GetIQueryable()
        {
            var clientView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>();
            var carriersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>();
            var filesView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.FilesDescriptionTopView>().Where(x => x.Type == (int)FileType.SendGoods);

            var sources = new int[] { (int)CgNoticeSource.AgentSend, (int)CgNoticeSource.Transfer, (int)CgNoticeSource.AgentCustomsFromStorage, (int)CgNoticeSource.AgentBreakCustomsForIns, (int)CgNoticeSource.AgentBreakCustoms }.ToList();
            var linqs = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                        join _client in clientView on waybill.wbEnterCode equals _client.EnterCode into clients
                        from client in clients.DefaultIfEmpty()
                        join _carrier in carriersTopView on waybill.wbCarrierID equals _carrier.ID into carriers
                        from carrier in carriers.DefaultIfEmpty()
                        join file in filesView on waybill.wbID equals file.WaybillID into files
                        from file in files.DefaultIfEmpty()
                        where (waybill.NoticeType == (int)CgNoticeType.Out
                          || waybill.NoticeType == (int)CgNoticeType.Boxing)

                        orderby waybill.wbCreateDate descending
                        select new MyWaybill
                        {
                            ID = waybill.wbID,
                            CreateDate = waybill.wbCreateDate,
                            EnterCode = waybill.wbEnterCode,
                            ClientName = client == null ? null : client.Name,
                            ExcuteStatus = (CgPickingExcuteStatus)waybill.wbExcuteStatus,
                            Type = (WaybillType)waybill.wbType,
                            Code = waybill.wbCode,
                            CarrierID = waybill.wbCarrierID,
                            CarrierName = carrier != null ? carrier.Name : null,
                            OrderID = waybill.OrderID,
                            NoticeType = (CgNoticeType)waybill.NoticeType,
                            Driver = waybill.wldDriver,
                            CarNumberID = waybill.wldCarNumber1,
                            CarNumber1 = waybill.chcdCarNumber1,
                            Source = (CgNoticeSource)waybill.Source,
                            File = new CenterFileDescription
                            {
                                ID = file.ID,
                                WaybillID = file.WaybillID,
                                NoticeID = file.NoticeID,
                                StorageID = file.StorageID,
                                CustomName = file.CustomName,
                                //Type = file.Type,
                                Url = CenterFile.Web + file.Url,
                                CreateDate = file.CreateDate,
                                ClientID = file.ClientID,
                                AdminID = file.AdminID,
                                InputID = file.InputID,
                                //Status = (FileDescriptionStatus)file.Status
                            },
                            IsUpload =file==null? IsUpload.Uploading:IsUpload.Uploaded
                        };
            return linqs;
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object GetPagelistData(int? pageIndex = 1, int? pageSize = 50)
        {
            var iquery = this.IQueryable.Cast<MyWaybill>();
            int total = iquery.Count();
            //执行sql语句
            var waybills = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).ToArray();
            var result = waybills.Select(waybill => new
            {
                ID = waybill.ID,
                CreateDate = waybill.CreateDate,
                EnterCode = waybill.EnterCode,
                ClientName = waybill.ClientName,
                ExcuteStatus = waybill.ExcuteStatus,
                ExcuteStatusDescription = waybill.ExcuteStatus.GetDescription(),
                Type = waybill.Type,
                WaybillTypeDescription = waybill.Type.GetDescription(),
                Code = waybill.Code,
                CarrierID = waybill.CarrierID,
                CarrierName = waybill.CarrierName,
                OrderID = waybill.OrderID,
                NoticeType = waybill.NoticeType,
                Driver = waybill.Driver,
                CarNumber1 = waybill.CarNumber1,
                Files = waybill.File,
                Status =waybill.IsUpload  //是否上传
            }).OrderByDescending(x => x.OrderID);

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = result.ToArray(),
            };
        }


        #region Helper Class
        /// <summary>
        /// 符合Picking视图头部定义的内部类
        /// </summary>
        private class MyWaybill
        {
            public string ID { get; set; }
            public DateTime CreateDate { get; set; }
            public string EnterCode { get; set; }
            public string ClientName { get; set; }
            public CgPickingExcuteStatus ExcuteStatus { get; set; }
            public WaybillType Type { get; set; }
            public string Code { get; set; }
            public string CarrierID { get; set; }
            public string CarrierName { get; set; }
            public string OrderID { get; set; }
         
            public CgNoticeSource Source { get; set; }
            /// <summary>
            /// 业务类型名称
            /// </summary>
            public string SourceName { get; set; }
            public CgNoticeType NoticeType { get; set; }
         
            public string Driver { get; set; }
            public string CarNumberID { get; set; }
            public string CarNumber1 { get; set; }

            public CgLoadingExcuteStauts? LoadingExcuteStatus { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 经过商议暂时增加，等与库房前端对通后，我们可以没有这个传递与处理了。
            /// </remarks>
            public string[] Files { get; set; }

            /// <summary>
            /// 收件地址
            /// </summary>
            public string CoeAddress { get; set; }
            /// <summary>
            /// 提货证件
            /// </summary>
            public string IDNumber { get; set; }
            /// <summary>
            /// 证件类型
            /// </summary>
            public IDType? IDType { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string IDTypeName { get; set; }
            /// <summary>
            /// 提货联系人
            /// </summary>
            public string TakingContact { get; set; }
            /// <summary>
            /// 联系电话
            /// </summary>
            public string TakingPhone { get; set; }

            /// <summary>
            /// 快递类型
            /// </summary>
            public int? Extype { get; set; }
            /// <summary>
            /// 快递支付方式
            /// </summary>

            public int? ExPayType { get; set; }
            /// <summary>
            /// 总货值
            /// </summary>

            public decimal? chgTotalPrice { get; set; }

            /// <summary>
            /// 收货人姓名
            /// </summary>
            public string coeContact { get; set; }
            /// <summary>
            /// 收货电话
            /// </summary>

            public string coePhone { get; set; }

            /// <summary>
            /// 送货人
            /// </summary>
            public string corContact { get; set; }
            /// <summary>
            /// 送货电话
            /// </summary>
            public string corPhone { get; set; }

            public string corAddress { get; set; }

            public string corCompany { get; set; }

            /// <summary>
            ///总件数
            /// </summary>
            public int? TotalParts { get; set; }
            /// <summary>
            /// 总重量
            /// </summary>
            public decimal? TotalWeight { get; set; }
            /// <summary>
            public string Condition { get; set; }

            /// <summary>
            /// 上传状态
            /// </summary>
            public IsUpload IsUpload { get; set; }

            public CenterFileDescription File { get; set; }

        }
        #endregion

        #region 查询条件搜索
        /// <summary>
        /// 根据承运商搜索
        /// </summary>
        /// <param name="承运商"></param>
        /// <returns></returns>
        public CgSzPickingsView SearchByCarrier(string carrier)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.CarrierName.Contains(carrier)
                       orderby waybill.ID descending
                       select waybill;

            var view = new CgSzPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }


        /// <summary>
        /// 根据客户搜索
        /// </summary>
        /// <param name="Client">客户名称</param>
        /// <returns></returns>
        public CgSzPickingsView SearchByClient(string Client)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var clientEnterCodes = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>().Where(item => item.Name.Contains(Client)).Select(item => item.EnterCode);

            var linq = from waybill in waybillView
                       join entercode in clientEnterCodes on waybill.EnterCode equals entercode
                       select waybill;

            var view = new CgSzPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 根据入仓号搜索
        /// </summary>
        /// <param name="Code">入仓号</param>
        /// <returns></returns>
        public CgSzPickingsView SearchByCode(string Code)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var clientEnterCodes = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>().Where(item => item.EnterCode.Contains(Code)).Select(item => item.EnterCode);

            var linq = from waybill in waybillView
                       join entercode in clientEnterCodes on waybill.EnterCode equals entercode
                       select waybill;

            var view = new CgSzPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        public CgSzPickingsView SearchByStatus(int Status)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var linq = from waybill in waybillView
                       where waybill.IsUpload==(IsUpload)Status
                       select waybill;

            var view = new CgSzPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }


        string wareHouseID;

        /// <summary>
        /// 根据库房ID搜索
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public CgSzPickingsView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;

            var waybillView = this.IQueryable.Cast<MyWaybill>();

            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgSzPickingsView(this.Reponsitory, waybillView);
            }

            var linq_waybillIDs = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  where notice.WareHouseID.StartsWith(this.wareHouseID) && (notice.Type == (int)CgNoticeType.Out || notice.Type == (int)CgNoticeType.Boxing)
                                  select notice.WaybillID;

            var ienum_waybillIDs = linq_waybillIDs.Distinct();/*.Take(1000)*/

            var linq = from waybill in waybillView
                       join id in ienum_waybillIDs on waybill.ID equals id
                       orderby waybill.ID descending
                       select waybill;

            var view = new CgSzPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }



        #endregion
    }
}
