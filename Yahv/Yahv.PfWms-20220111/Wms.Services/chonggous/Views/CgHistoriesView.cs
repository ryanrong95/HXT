using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Wms.Services;
using Yahv.Services.Enums;
using Newtonsoft.Json.Linq;
using Yahv.Utils.Serializers;
using Yahv.Underly;
using Layers.Data;
using Layers.Data.Sqls.PvWms;
using Yahv.Utils.Converters.Contents;
using Wms.Services.Extends;
using Yahv.Underly.Enums;
using Yahv.Services.Models;

namespace Wms.Services.chonggous.Views
{
    public class CgHistoriesView : QueryView<object, PvWmsRepository>
    {
        #region 构造函数 
        string waybillID;
        public CgHistoriesView()
        {
        }

        protected CgHistoriesView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgHistoriesView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<object> GetIQueryable()
        {
            var clientView = new Yahv.Services.Views.WsClientsTopView<PvWmsRepository>(this.Reponsitory);
            var adminTopsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>();
            var waybillView = new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory);
            var linqs = from waybill in waybillView
                        join admin in adminTopsView on waybill.CreatorID equals admin.ID
                        join _client in clientView
                        on waybill.EnterCode equals _client.EnterCode into clients
                        from client in clients.DefaultIfEmpty()
                        select new MyWaybill
                        {
                            ID = waybill.ID,
                            FatherID = waybill.FatherID,
                            CreateDate = waybill.CreateDate,
                            ClientName = client != null ? client.Name : null,
                            EnterCode = waybill.EnterCode,
                            Supplier = waybill.Supplier,
                            AdminName = admin.RealName,
                            ExcuteStatus = (Yahv.Underly.CgSortingExcuteStatus)waybill.ExcuteStatus,
                            Type = waybill.Type,
                            Code = waybill.Code,
                            Carrier = waybill.CarrierName,
                            ConsignorPlace = waybill.Consignor.Place,
                        };

            return linqs;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object[] ToHistory(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<MyWaybill>();            

            // 获取符合条件的ID            
            var ienum_myWaybill = iquery.ToArray();
            var waybillIds = ienum_myWaybill.Select(item => item.ID).Distinct();

            var productView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>();
            
            var sortingView = from sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()                              
                              join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                              on sorting.ID equals storage.SortingID                             
                              join product in productView on storage.ProductID equals product.ID
                              where waybillIds.Contains(sorting.WaybillID)
                              select new
                              {
                                  sorting.ID,
                                  sorting.BoxCode,
                                  sorting.Quantity,
                                  sorting.WaybillID,
                                  sorting.NoticeID,
                                  sorting.AdminID,
                                  sorting.CreateDate,
                                  sorting.Weight,
                                  sorting.NetWeight,
                                  sorting.Volume,
                                  sorting.InputID,
                                  storage.Origin,
                                  storage.DateCode,
                                  storage.ShelveID,
                                  storageID = storage.ID,                                 
                                  Product = new
                                  {
                                      PartNumber = product.PartNumber,
                                      Manufacturer = product.Manufacturer,
                                      PackageCase = product.PackageCase,
                                      Packaging = product.Packaging,
                                  },
                              };
            var ienum_sortings = sortingView.ToArray();
            var noticeIds = ienum_sortings.Select(item => item.NoticeID).Distinct();
            var storageIds = ienum_sortings.Select(item => item.storageID).Distinct();

            var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
                            where waybillIds.Contains(file.WaybillID) || noticeIds.Contains(file.NoticeID) || storageIds.Contains(file.StorageID)
                            select new CenterFileDescription
                            {
                                ID = file.ID,
                                WaybillID = file.WaybillID,
                                NoticeID = file.NoticeID,
                                StorageID = file.StorageID,
                                CustomName = file.CustomName,
                                Type = file.Type,
                                Url = CenterFile.Web + file.Url,
                                CreateDate = file.CreateDate,
                                ClientID = file.ClientID,
                                AdminID = file.AdminID,
                                InputID = file.InputID,
                                Status = file.Status,
                            };
            var files = filesView.ToArray();

            var linqs = from waybill in iquery.ToArray()                        
                        join sorting in ienum_sortings
                          on waybill.ID equals sorting.WaybillID into sortings
                        where waybillIds.Contains(waybill.ID)
                        select new
                        {
                            Waybill = waybill,
                            Sortings = sortings
                        };

            var results = linqs.Select(item => new
            {
                Waybill = new
                {
                    item.Waybill.ID,
                    item.Waybill.CreateDate,
                    item.Waybill.EnterCode,
                    item.Waybill.Supplier,
                    item.Waybill.ExcuteStatus,
                    item.Waybill.AdminName,
                    ExcuteStatusDes = item.Waybill.ExcuteStatus.GetDescription(),
                    item.Waybill.Type,
                    TypeDes = item.Waybill.Type.GetDescription(),
                    item.Waybill.Code,
                    item.Waybill.Carrier,
                    item.Waybill.ConsignorPlace,
                    item.Waybill.ClientName,
                    SortingQuantity = item.Sortings.Sum(sorting => sorting.Quantity),
                    Files = files.ToArray(),
                },
                Sortings = item.Sortings.Select(sorting => new
                {
                    sorting.ID,
                    sorting.BoxCode,
                    sorting.Quantity,
                    sorting.WaybillID,
                    sorting.NoticeID,
                    sorting.AdminID,
                    sorting.CreateDate,
                    sorting.Weight,
                    sorting.NetWeight,
                    sorting.Volume,
                    sorting.InputID,
                    sorting.DateCode,
                    sorting.ShelveID,
                    sorting.Origin,
                    sorting.Product,
                    Files = files.Where(file => file.StorageID == sorting.storageID).ToArray(),
                })
            });

            return results.ToArray();            
        }        

        #region 搜索方法

        /// <summary>
        /// 根据运单ID搜索
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public CgHistoriesView HistoryDetail(string waybillID)
        {
            this.waybillID = waybillID;
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.ID == waybillID
                       select waybill;

            var view = new CgHistoriesView(this.Reponsitory, linq)
            {
            };

            return view;
        }

        public string[] HistoryWaybillIDs(string waybillid)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.FatherID == waybillid
                       select waybill.ID;
            return linq.Distinct().ToArray();
        }

        #endregion

        #region Helper Class
        /// <summary>
        /// 符合Sorting视图头部定义的内部类
        /// </summary>
        private class MyWaybill
        {
            public string ID { get; set; }
            public string FatherID { get; set; }
            public DateTime CreateDate { get; set; }
            public string AdminName { get; set;}
            public string ClientName { get; set; }
            public string EnterCode { get; set; }
            public string Supplier { get; set; }
            public Yahv.Underly.CgSortingExcuteStatus ExcuteStatus { get; set; }
            public Yahv.Underly.WaybillType Type { get; set; }
            public CgNoticeSource Source { get; set; }
            public string Code { get; set; }
            public string Carrier { get; set; }
            public string ConsignorPlace { get; set; }
        }
        #endregion
    }
}
