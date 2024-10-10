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
using System.Linq.Expressions;
using Yahv.Services.Models;
using Wms.Services.Enums;
using Yahv.Services;
using Yahv.Underly.Attributes;
using Wms.Services.chonggous.Models;
using Newtonsoft.Json;
using Wms.Services.chonggous.Views;
using Yahv.Linq.Extends;
//using static Wms.Services.chonggous.Journals;

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 入库分拣
    /// </summary>
    public class CgSortingsView : QueryView<object, PvWmsRepository>
    {
        #region 目前开发的错误
        /*
         * 这种写法与 SearchByWareHouseID 是重叠的
         * 这种写法与 SearchByWareHouseID 是重叠的
         * 之前的讲解说明：主要是目前开发的不正规造成的。
         * 架设有一个静态统一调用为  Warehouse.Current.WareHouseID 
         * 如果：Warehouse.Current.WareHouseID  ==null 表示没有库房限制
         * 如果有值，就直接过滤
         * 因此不应该有如下构造器
         *  
         * string wareHouseID;
         *
         * public CgSortingsView(string wareHouseID)
         * {
         *    this.wareHouseID = wareHouseID;
         * }
         */
        #endregion

        #region 构造函数 

        public CgSortingsView()
        {

        }

        protected CgSortingsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgSortingsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<object> GetIQueryable()
        {
            //var clientView = new Yahv.Services.Views.WsClientsTopView<PvWmsRepository>(this.Reponsitory);
            var clientView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>();
            var waybillViews = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();
            var carriersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>();
            var wsnSuppliersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.wsnSuppliersTopView>();


            var linqs = from waybill in waybillViews
                        join _client in clientView on waybill.wbEnterCode equals _client.EnterCode into clients
                        from client in clients.DefaultIfEmpty()
                        join _carrier in carriersTopView on waybill.wbCarrierID equals _carrier.ID into carriers
                        from carrier in carriers.DefaultIfEmpty()
                        join _supplier in wsnSuppliersTopView on new { EnterCode = waybill.wbEnterCode, SupplierName = waybill.wbSupplier } equals new { EnterCode = _supplier.EnterCode, SupplierName = _supplier.RealEnterpriseName } into Suppliers
                        from supplier in Suppliers.DefaultIfEmpty()
                        where ( waybill.Source == (int)CgNoticeSource.AgentEnter || waybill.Source == (int)CgNoticeSource.Transfer) && waybill.wbStatus != 600
                        orderby waybill.wbCreateDate descending
                        select new MyWaybill
                        {
                            ID = waybill.wbID,
                            CreateDate = waybill.wbCreateDate,
                            ModifyDate = waybill.wbModifyDate,
                            EnterCode = waybill.wbEnterCode,
                            ClientName = client == null ? null : client.Name,
                            ChargeWH = client == null ? (int?) null : client.ChargeWH,
                            Supplier = waybill.wbSupplier,
                            SupplierName = supplier.ChineseName,
                            SupplierGrade = supplier.nGrade,
                            ExcuteStatus = (Yahv.Underly.CgSortingExcuteStatus)waybill.wbExcuteStatus,
                            Type = (WaybillType)waybill.wbType,
                            Code = waybill.wbCode,
                            CarrierName = carrier.Name,
                            CarrierID = waybill.wbCarrierID,
                            ConsignorID = waybill.wbConsignorID,
                            //Place = (Origin)Enum.Parse(typeof(Origin), waybill.corPlace),
                            ConsignorPlace = waybill.corPlace,
                            Condition = waybill.wbCondition.JsonTo<WayCondition>(),
                            OrderID = waybill.OrderID,
                            Source = (CgNoticeSource)waybill.Source,
                            NoticeType = (CgNoticeType)waybill.NoticeType,
                            AppointTime = waybill.AppointTime,
                            TransferID = waybill.wbTransferID,
                            Driver = waybill.wldDriver,
                            CarNumber1 = waybill.wldCarNumber1,
                            TakingAddress = waybill.wldTakingAddress,
                            TakingContact = waybill.wldTakingContact,
                            TakingPhone = waybill.wldTakingPhone,
                            LoadingExcuteStatus = (CgLoadingExcuteStauts?)waybill.loadExcuteStatus,
                            TakingDate = waybill.wldTakingDate,
                            //Merchandiser = merchandiser.RealName,
                            //waybill.wbStatus
                        };
            return linqs;
        }


        /// <summary>
        /// 获取Waybill当前的状态
        /// </summary>
        /// <param name="waybillIds">waybillID可多个</param>
        /// <returns></returns>
        public object GetWaybillCurrentStatus(params string[] waybillIds)
        {
            var view = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                       where waybillIds.Contains(waybill.wbID)
                       select new
                       {
                           waybillID = waybill.wbID,
                           Operated = waybill.wbStatus == 600 ? false : true,
                       };

            return view.ToArray();
        }


        /// <summary>
        /// 目的：补全数据
        /// </summary>
        /// <returns></returns>
        public object[] ToMyArray(string key = "")
        {
            return this.ToMyPage(null, null, key) as object[];
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null, string key = "")
        {
            //var iquery = this.IQueryable.Cast<MyWaybill>().DistinctBy2(item => item.Supplier);
            var iquery = this.IQueryable.Cast<MyWaybill>().DistinctBy2(item => string.Concat(item.ID, item.Supplier));
            int total = iquery.Count();

            //iquery = iquery.OrderByDescending(item => item.ID);
            iquery = iquery.OrderByDescending(item => item.ModifyDate);

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            // 获取符合条件的ID            
            var ienum_myWaybill = iquery.ToArray().OrderBy(item => item.Type).ThenByDescending(item => item.ModifyDate);

            var waybillIds = ienum_myWaybill.Select(item => item.ID).Distinct();
            var ordersId = ienum_myWaybill.Select(item => item.OrderID).Distinct();

            var merchandiserTopView = from merchandiser in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderMerchandiserTopView>()
                                      where ordersId.Contains(merchandiser.OrderID)
                                      select new
                                      {
                                          merchandiser.OrderID,
                                          merchandiser.RealName,
                                      };
            var trackers = merchandiserTopView.Distinct().ToArray();

            var productView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>();
            var clientLsEndDateTopView = from clientLs in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientLSEndDateTopView>()
                                         where clientLs.EnterCode != null
                                         select new
                                         {
                                             clientLs.EnterCode,
                                             clientLs.EndDate,
                                             clientLs.ClientID,
                                             clientLs.OrderID,
                                         };

            var tempStockView = from tempStock in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.TWaybills>().Where(item => item.ForOrderID != null)
                                select new
                                {
                                    tempStock.ForOrderID,
                                    tempStock.CreateDate,
                                    tempStock.CompleteDate,
                                };
            var ienum_tempStocks = tempStockView.ToArray();

            var noticeView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                             join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
                             join product in productView on notice.ProductID equals product.ID
                             join _pWeight in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>() on product.PartNumber equals _pWeight.PartNumber into pWeights
                             from pWeight in pWeights.DefaultIfEmpty()
                             where waybillIds.Contains(notice.WaybillID) && notice.OutputID == null && (product.PartNumber.Contains(key) || product.Manufacturer.Contains(key))
                             select new
                             {
                                 #region 视图

                                 Product = new
                                 {
                                     PartNumber = product.PartNumber,
                                     Manufacturer = product.Manufacturer,
                                     PackageCase = product.PackageCase,
                                     Packaging = product.Packaging,
                                 },
                                 pWeight.AVGWeight,
                                 notice.ID,
                                 notice.WaybillID,
                                 notice.InputID,
                                 notice.DateCode,
                                 notice.Quantity,
                                 notice.Origin,
                                 Conditions = notice.Conditions,
                                 Source = (NoticeSource)notice.Source,
                                 Type = (CgNoticeType)notice.Type,
                                 notice.BoxCode,
                                 notice.Weight,
                                 notice.NetWeight,
                                 notice.Volume,
                                 notice.ShelveID,
                                 notice.Summary,
                                 input.TinyOrderID,

                                 #endregion
                             };

            var ienum_notices = noticeView.ToArray();
            var inputIds = ienum_notices.Select(item => item.InputID).Distinct();
            var noticeIds = ienum_notices.Select(item => item.ID).Distinct();

            #region 文件处理

            var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
                            where waybillIds.Contains(file.WaybillID)
                                || noticeIds.Contains(file.NoticeID)
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

            #endregion

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                var fenyeResult = ienum_myWaybill.Select(waybill => new
                {
                    Waybill = new
                    {
                        ID = waybill.ID,
                        CreateDate = waybill.CreateDate,
                        ModifyDate = waybill.ModifyDate,
                        EnterCode = waybill.EnterCode,
                        ClientName = waybill.ClientName,
                        ChargeWH = waybill.ChargeWH.HasValue ? ((ChargeWHType)waybill.ChargeWH).GetDescription() : null,
                        Supplier = waybill.Supplier,
                        SupplierName = waybill.SupplierName,
                        SupplierGrade = waybill.SupplierGrade,
                        ExcuteStatus = waybill.ExcuteStatus,
                        ExcuteStatusDes = waybill.ExcuteStatus.GetDescription(),
                        Type = waybill.Type,
                        TypeDes = waybill.Type.GetDescription(),
                        Code = waybill.Code,
                        CarrierID = waybill.CarrierID,
                        CarrierName = waybill.CarrierName,
                        ConsignorID = waybill.ConsignorID,
                        ConsignorPlace = waybill.ConsignorPlace,
                        ConsignorPlaceDes = waybill.Place.GetDescription(),
                        ConsignorPlaceText = waybill.ConsignorPlace + " " + waybill.Place.GetDescription(),
                        ConsignorPlaceID = ((int)waybill.Place).ToString(),
                        Condition = waybill.Condition,
                        Source = waybill.Source,
                        SourceDes = waybill.Source.GetDescription(),
                        NoticeType = waybill.NoticeType,
                        NoticeTypeDes = waybill.NoticeType.GetDescription(),
                        Driver = waybill.Driver ?? "",
                        CarNumber1 = waybill.CarNumber1 ?? "",
                        AppointTime = waybill.AppointTime,
                        TransferID = waybill.TransferID,
                        LoadingExcuteStatus = waybill.LoadingExcuteStatus,
                        OrderID = waybill.OrderID,
                        Files = files.Where(file => string.IsNullOrEmpty(file.NoticeID)),
                        TakingDate = waybill.TakingDate,
                        TakingAddress = waybill.TakingAddress,
                        TakingContact = waybill.TakingContact,
                        TakingPhone = waybill.TakingPhone,
                        Operated = waybill.Operated.HasValue ? waybill.Operated.Value : true,
                        Merchandiser = trackers.SingleOrDefault(item => item.OrderID == waybill.OrderID)?.RealName,
                    }
                });

                return new
                {
                    Total = total,
                    Size = pageSize ?? 20,
                    Index = pageIndex ?? 1,
                    Data = fenyeResult.ToArray(),
                };
            }


            var sortingView = from sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                              join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on sorting.NoticeID equals notice.ID
                              join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on sorting.InputID equals input.ID
                              join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on sorting.ID equals storage.SortingID
                              join product in productView on storage.ProductID equals product.ID
                              where noticeIds.Contains(sorting.NoticeID)
                              select new
                              {
                                  #region 视图

                                  sorting.ID,
                                  sorting.BoxCode,
                                  sorting.Quantity,
                                  sorting.WaybillID,
                                  sorting.AdminID,
                                  sorting.CreateDate,
                                  sorting.Weight,
                                  sorting.NetWeight,
                                  sorting.Volume,
                                  sorting.InputID,
                                  sorting.Summary,
                                  sorting.NoticeID,
                                  storage.ShelveID,
                                  input = new
                                  {
                                      ID = input.ID,
                                      input.OrderID,
                                      input.TinyOrderID,
                                      input.ItemID,
                                  },
                                  Product = new
                                  {
                                      PartNumber = product.PartNumber,
                                      Manufacturer = product.Manufacturer,
                                      PackageCase = product.PackageCase,
                                      Packaging = product.Packaging,
                                  },
                                  Source = (CgNoticeSource)notice.Source,
                                  StorageType = (CgStoragesType)storage.Type

                                  #endregion
                              };

            var ienum_sortings = sortingView.ToArray();

            var source = ienum_sortings.Select(item => item.Source).Distinct().SingleOrDefault();
            if (source == CgNoticeSource.AgentEnter)
            {
                ienum_sortings = ienum_sortings.Where(item => item.StorageType == CgStoragesType.Stores).ToArray();
            }

            var sortingWithNoNoticeView = from sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                          join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                          on sorting.InputID equals input.ID
                                          join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                          on sorting.ID equals storage.SortingID
                                          join product in productView on storage.ProductID equals product.ID
                                          join _pWeight in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>() on product.PartNumber equals _pWeight.PartNumber into pWeights
                                          from pWeight in pWeights.DefaultIfEmpty()
                                          join waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                                          on sorting.WaybillID equals waybill.wbID
                                          //where waybillIds.Contains(sorting.WaybillID) || sorting.NoticeID == null
                                          where waybillIds.Contains(waybill.wbFatherID) && sorting.NoticeID == null
                                          select new
                                          {
                                              #region 视图

                                              sorting.ID,
                                              sorting.BoxCode,
                                              sorting.NoticeID,
                                              sorting.Quantity,
                                              sorting.WaybillID,
                                              sorting.AdminID,
                                              sorting.CreateDate,
                                              sorting.Weight,
                                              sorting.NetWeight,
                                              sorting.Volume,
                                              sorting.InputID,
                                              sorting.Summary,
                                              storage.ShelveID,
                                              input = new
                                              {
                                                  ID = input.ID,
                                                  input.OrderID,
                                                  input.TinyOrderID,
                                                  input.ItemID,
                                              },
                                              Product = new
                                              {
                                                  PartNumber = product.PartNumber,
                                                  Manufacturer = product.Manufacturer,
                                                  PackageCase = product.PackageCase,
                                                  Packaging = product.Packaging,
                                              },
                                              waybill.wbID,
                                              waybill.wbFatherID,
                                              pWeight.AVGWeight,

                                              #endregion
                                          };

            var ienum_sortingswithnonotice = sortingWithNoNoticeView.ToArray();

            var ienum_noticesorting = from notice in ienum_notices
                                          //join sorting in ienum_sortings on notice.InputID equals sorting.InputID into sortings
                                      join sorting in ienum_sortings on notice.ID equals sorting.NoticeID into sortings
                                      select new
                                      {
                                          #region 视图

                                          Product = notice.Product,
                                          notice.ID,
                                          notice.WaybillID,
                                          notice.InputID,
                                          notice.DateCode,
                                          notice.Type,
                                          notice.Quantity,
                                          notice.Conditions,
                                          Source = notice.Source,
                                          notice.BoxCode,
                                          notice.Weight,
                                          notice.NetWeight,
                                          notice.Volume,
                                          notice.ShelveID,
                                          notice.TinyOrderID,
                                          notice.Summary,
                                          notice.AVGWeight,
                                          Origin = (Origin)Enum.Parse(typeof(Origin), notice.Origin ?? nameof(Origin.Unknown)),
                                          Sortings = sortings.ToArray(),

                                          #endregion
                                      };

            var requirementsView = new WayRequirementsView().Where(item => waybillIds.Contains(item.ID));

            var linq = from waybill in ienum_myWaybill
                       join notice in ienum_noticesorting on waybill.ID equals notice.WaybillID into notices
                       //join sorting in ienum_sortingswithnonotice on waybill.ID equals sorting.WaybillID into sortings
                       join sorting in ienum_sortingswithnonotice on waybill.ID equals sorting.wbFatherID into sortings
                       join _requirement in requirementsView on waybill.ID equals _requirement.ID into requirements
                       from requirement in requirements.DefaultIfEmpty()
                       join _clientLs in clientLsEndDateTopView on waybill.EnterCode equals _clientLs.EnterCode into clientLsViews
                       from clientLs in clientLsViews.DefaultIfEmpty()
                       select new
                       {
                           #region 视图

                           Waybill = new
                           {
                               ID = waybill.ID,
                               CreateDate = waybill.CreateDate,
                               ModifyDate = waybill.ModifyDate,
                               EnterCode = waybill.EnterCode,
                               ClientName = waybill.ClientName,
                               ChargeWH = waybill.ChargeWH.HasValue ? ((ChargeWHType)waybill.ChargeWH).GetDescription() : null,
                               Supplier = waybill.Supplier,
                               SupplierName = waybill.SupplierName,
                               SupplierGrade = waybill.SupplierGrade,
                               ExcuteStatus = waybill.ExcuteStatus,
                               ExcuteStatusDes = waybill.ExcuteStatus.GetDescription(),
                               Type = waybill.Type,
                               TypeDes = waybill.Type.GetDescription(),
                               Code = waybill.Code,
                               CarrierID = waybill.CarrierID,
                               CarrierName = waybill.CarrierName,
                               ConsignorID = waybill.ConsignorID,
                               ConsignorPlace = waybill.ConsignorPlace,
                               Condition = waybill.Condition,
                               waybill.Place,
                               Source = waybill.Source,
                               OrderID = waybill.OrderID,
                               NoticeType = waybill.NoticeType,
                               Driver = waybill.Driver,
                               CarNumber1 = waybill.CarNumber1,
                               AppointTime = waybill.AppointTime,
                               TransferID = waybill.TransferID,
                               LoadingExcuteStatus = waybill.LoadingExcuteStatus,
                               Merchandiser = trackers.SingleOrDefault(item => item.OrderID == waybill.OrderID)?.RealName,
                               TakingDate = waybill.TakingDate,
                               TakingAddress = waybill.TakingAddress,
                               TakingContact = waybill.TakingContact,
                               TakingPhone = waybill.TakingPhone,
                               IsPayCharge = requirement == null? null : requirement.IsPayCharge,
                               IsClientLs = clientLs == null ? false : true,
                               LsEndDate = clientLs == null ? null : (DateTime?)clientLs.EndDate,
                           },
                           Notice = notices.ToArray(),
                           Sorting = sortings.ToArray(),

                           #endregion

                       };


            // 为了计算并添加LQuantity
            var results = linq.Select(item => new
            {
                Waybill = new
                {
                    ID = item.Waybill.ID,
                    CreateDate = item.Waybill.CreateDate,
                    ModifyDate = item.Waybill.ModifyDate,
                    EnterCode = item.Waybill.EnterCode,
                    ClientName = item.Waybill.ClientName,
                    ChargeWH = item.Waybill.ChargeWH,
                    Supplier = item.Waybill.Supplier,
                    SupplierName = item.Waybill.SupplierName,
                    SupplierGrade = item.Waybill.SupplierGrade,
                    ExcuteStatus = item.Waybill.ExcuteStatus,
                    ExcuteStatusDes = item.Waybill.ExcuteStatus.GetDescription(),
                    Type = item.Waybill.Type,
                    TypeDes = item.Waybill.Type.GetDescription(),
                    Code = item.Waybill.Code,
                    CarrierID = item.Waybill.CarrierID,
                    CarrierName = item.Waybill.CarrierName,
                    ConsignorID = item.Waybill.ConsignorID,
                    ConsignorPlace = item.Waybill.ConsignorPlace,
                    ConsignorPlaceDes = item.Waybill.Place.GetDescription(),
                    ConsignorPlaceText = item.Waybill.ConsignorPlace + " " + item.Waybill.Place.GetDescription(),
                    ConsignorPlaceID = ((int)item.Waybill.Place).ToString(),
                    Condition = item.Waybill.Condition,
                    Source = item.Waybill.Source,
                    SourceDes = item.Waybill.Source.GetDescription(),
                    NoticeType = item.Waybill.NoticeType,
                    NoticeTypeDes = item.Waybill.NoticeType.GetDescription(),
                    Driver = item.Waybill.Driver ?? "",
                    CarNumber1 = item.Waybill.CarNumber1 ?? "",
                    AppointTime = item.Waybill.AppointTime,
                    TransferID = item.Waybill.TransferID,
                    LoadingExcuteStatus = item.Waybill.LoadingExcuteStatus,
                    Merchandiser = item.Waybill.Merchandiser,
                    TakingDate = item.Waybill.TakingDate,
                    TakingAddress = item.Waybill.TakingAddress,
                    TakingContact = item.Waybill.TakingContact,
                    TakingPhone = item.Waybill.TakingPhone,
                    OrderID = item.Waybill.OrderID ?? string.Join(",", item.Notice.SelectMany(notice => notice.Sortings)
                        .Select(sorting => sorting.input.OrderID).Distinct()),
                    Files = files.Where(file => string.IsNullOrEmpty(file.NoticeID)),
                    IsPayCharge = item.Waybill.IsPayCharge,
                    IsClientLs = item.Waybill.IsClientLs,
                    LsEndDate = item.Waybill.LsEndDate,
                    IsTempStock = ienum_tempStocks.Any(t => t.ForOrderID == item.Waybill.OrderID || t.ForOrderID.Contains(item.Waybill.OrderID)),
                    FirstTempDate = ienum_tempStocks.Any(t => t.ForOrderID == item.Waybill.OrderID || t.ForOrderID.Contains(item.Waybill.OrderID)) ? (DateTime?)ienum_tempStocks.FirstOrDefault(t => t.ForOrderID == item.Waybill.OrderID || t.ForOrderID.Contains(item.Waybill.OrderID)).CreateDate : null,
                },
                Notice = item.Notice.Select(notice => new
                {
                    notice.ID,
                    NoticeID = notice.ID,
                    notice.Product,
                    notice.WaybillID,
                    notice.InputID,
                    notice.DateCode,
                    notice.Quantity,
                    ArrivedQuantity = notice.Sortings.Sum(s => s.Quantity),
                    LeftQuantity = notice.Quantity - notice.Sortings.Sum(s => s.Quantity),
                    CurrentQuantity = notice.Quantity - notice.Sortings.Sum(s => s.Quantity),
                    _disabled = (notice.Quantity - notice.Sortings.Sum(s => s.Quantity)) <= 0 ? true : false,
                    Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
                    Source = notice.Source,
                    notice.BoxCode,
                    notice.Weight,
                    notice.NetWeight,
                    notice.Volume,
                    notice.ShelveID,
                    notice.Type,
                    notice.TinyOrderID,
                    AVGWeight = notice.AVGWeight.HasValue ? notice.AVGWeight.Value : 0,
                    origintext = notice.Origin.ToString() + " " + notice.Origin.GetDescription(),
                    origin = (notice.Origin).ToString(),
                    originDes = notice.Origin.GetDescription(),
                    notice.Summary,
                    SortingID = (notice.Quantity - notice.Sortings.Sum(s => s.Quantity)) == 0 ? string.Empty : PKeySigner.Pick(PkeyType.Sortings),
                    Files = files.Where(file => file.NoticeID == notice.ID).ToArray(),
                    Sortings = notice.Sortings.Select(sorting => new
                    {
                        sorting.ID,
                        sorting.NoticeID,
                        sorting.WaybillID,
                        sorting.BoxCode,
                        sorting.Quantity,
                        sorting.AdminID,
                        sorting.CreateDate,
                        sorting.Weight,
                        sorting.NetWeight,
                        sorting.Volume,
                        sorting.InputID,
                        sorting.ShelveID,
                        input = sorting.input,
                        Product = sorting.Product,
                        sorting.Summary,
                    }),
                }),
                Sortings = item.Sorting.Select(sorting => new
                {
                    sorting.ID,
                    sorting.WaybillID,
                    sorting.NoticeID, // null
                    sorting.BoxCode,
                    sorting.ShelveID,
                    sorting.Quantity,
                    sorting.AdminID,
                    sorting.CreateDate,
                    sorting.Weight,
                    sorting.NetWeight,
                    sorting.Volume,
                    sorting.InputID,
                    sorting.input,
                    sorting.Product,
                    AVGWeight = sorting.AVGWeight.HasValue ? sorting.AVGWeight.Value : 0,
                    sorting.Summary,
                })
            });

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = results.ToArray(),
            };
        }

        /// <summary>
        /// 保存产品信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        string EnterProduct(JToken product)
        {
            string partNumber = product["PartNumber"]?.Value<string>();
            string manufacturer = product["Manufacturer"]?.Value<string>();
            string packageCase = product["PackageCase"]?.Value<string>();
            string packaging = product["Packaging"]?.Value<string>();
            string productid = string.Concat(partNumber, manufacturer, packageCase, packaging).MD5();

            using (var reponsitory = new PvDataReponsitory())
            {
                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Any(item => item.ID == productid);
                if (!exist)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.Products
                    {
                        ID = productid,
                        PartNumber = partNumber,
                        Manufacturer = manufacturer,
                        PackageCase = packageCase,
                        Packaging = packaging,
                        CreateDate = DateTime.Now,
                    });
                }
            }

            return productid;
        }

        /// <summary>
        /// 保存产品信息
        /// </summary>
        /// <param name="product">产品的 jToken信息</param>
        /// <param name="productid">产品的ID</param>
        /// <returns>产品是否存在</returns>
        bool EnterProduct(JToken product, out string productid)
        {
            string partNumber = product["PartNumber"]?.Value<string>();
            string manufacturer = product["Manufacturer"]?.Value<string>();
            string packageCase = product["PackageCase"]?.Value<string>();
            string packaging = product["Packaging"]?.Value<string>();
            string productID = productid = string.Concat(partNumber, manufacturer, packageCase, packaging).MD5();

            bool exist;
            using (var reponsitory = new PvDataReponsitory())
            {
                exist = reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Any(item => item.ID == productID);
                if (!exist)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.Products
                    {
                        ID = productid,
                        PartNumber = partNumber,
                        Manufacturer = manufacturer,
                        PackageCase = packageCase,
                        Packaging = packaging,
                        CreateDate = DateTime.Now,
                    });
                }
            }

            return exist;
        }


        /// <summary>
        /// 专门用于修改waybill 信息
        /// </summary>
        void EnterWaybill(JToken waybill, string adminID, CgSortingExcuteStatus? excuteStatus)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                if (excuteStatus == null)
                {
                    excuteStatus = (CgSortingExcuteStatus)(waybill["ExcuteStatus"]?.Value<int>() ?? (int)CgSortingExcuteStatus.PartStocked);
                }

                var waybillType = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Single(item => item.ID == waybill["ID"].Value<string>()).Type;

                //处理 waybill
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ExcuteStatus = (int)excuteStatus,
                    Summary = waybill["Summary"]?.Value<string>(),
                    Code = waybill["Code"]?.Value<string>(),
                    CarrierID = waybill["CarrierID"]?.Value<string>(),
                    ModifyDate = DateTime.Now,
                    ModifierID = adminID,
                    TransferID = waybill["TransferID"]?.Value<string>(),
                    Type = waybill["Type"]?.Value<int?>() ?? waybillType,
                }, item => item.ID == waybill["ID"].Value<string>());

                int? wayLoadingExcuteStatus = null;
                if (waybillType == (int)WaybillType.PickUp)
                {
                    wayLoadingExcuteStatus = (int)CgLoadingExcuteStauts.Completed;
                }

                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadings>().Any(item => item.ID == waybill["ID"].Value<string>()))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayLoadings
                    {
                        ID = waybill["ID"].Value<string>(),
                        Driver = waybill["Driver"]?.Value<string>(),
                        CarNumber1 = waybill["CarNumber1"]?.Value<string>(),
                        CreateDate = DateTime.Now,
                        CreatorID = adminID,
                        ExcuteStatus = wayLoadingExcuteStatus,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                    {
                        Driver = waybill["Driver"]?.Value<string>(),
                        CarNumber1 = waybill["CarNumber1"]?.Value<string>(),
                        ExcuteStatus = wayLoadingExcuteStatus,
                    }, item => item.ID == waybill["ID"].Value<string>());
                }

                //向客户发送收到货物消息
                //if (excuteStatus == CgSortingExcuteStatus.Completed)
                //{
                //    PushMsg pushMsg = new PushMsg(2,(int)SpotName.GoodsArrived, waybill["OrderID"]?.Value<string>());
                //    pushMsg.push();
                //}

                /* 根据董建要求,入库出库不修改运单中的收发货人
                //更新ConsignorPlace value 
                reponsitory.Update<Layers.Data.Sqls.PvCenter.WayParters>(new
                {
                    Place = waybill["ConsignorPlace"].Value<string>(),
                }, item => item.ID == waybill["ConsignorID"].Value<string>());
                */

                //// 更新Files信息
                //var files = waybill["Files"].Values<string>();
                //if (files.Count() > 0)
                //{
                //    reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                //    {
                //        WaybillID = waybill["ID"].Value<string>(),
                //        WsOrderID = waybill["OrderID"].Value<string>(),
                //    }, item => files.Contains(item.ID));
                //}

            }
        }

        /// <summary>
        /// 根据信息的Sorting提交视图，保存进项信息Input,代仓储,代转运使用
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="orderID"></param>
        /// <param name="tinyOrderID"></param>
        /// <param name="itemID"></param>
        /// <param name="productID"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        string EnterInput(string inputID, string orderID, string tinyOrderID, string itemID, string productID, PvWmsRepository repository)
        {
            var inputView = repository.ReadTable<Layers.Data.Sqls.PvWms.Inputs>();

            if (!inputView.Any(item => item.ID == inputID))//都到数据库中查询一下
            {
                if (string.IsNullOrEmpty(inputID))
                {
                    inputID = PKeySigner.Pick(PkeyType.Inputs);
                }

                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Inputs
                {
                    ID = inputID,
                    Code = inputID,
                    OrderID = orderID,
                    TinyOrderID = tinyOrderID,
                    ItemID = itemID,
                    ProductID = productID,
                    CreateDate = DateTime.Now,
                });
            }
            else
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Inputs>(new
                {
                    OrderID = orderID,
                    TinyOrderID = tinyOrderID,
                    ItemID = itemID,
                    ProductID = productID,
                }, item => item.ID == inputID);
            }

            return inputID;
        }

        string EnterInput(Layers.Data.Sqls.PvWms.Inputs input, PvWmsRepository repository)
        {
            string inputID = input.ID ?? PKeySigner.Pick(PkeyType.Inputs);
            if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Any(item => item.ID == inputID))
            {
                repository.Insert(new Layers.Data.Sqls.PvWms.Inputs
                {
                    ID = inputID,
                    Code = inputID,
                    OriginID = input.OriginID,
                    OrderID = input.OrderID,
                    TinyOrderID = input.TinyOrderID,   // 与陆凯协商,在新增 input的时候是不用抄TinyOrderID
                    ItemID = input.ItemID,
                    ProductID = input.ProductID,
                    ClientID = input.ClientID,
                    PayeeID = input.PayeeID,
                    ThirdID = input.ThirdID,
                    TrackerID = input.TrackerID,
                    SalerID = input.SalerID,
                    PurchaserID = input.PurchaserID,
                    Currency = input.Currency,
                    UnitPrice = input.UnitPrice,
                    CreateDate = DateTime.Now,
                });
            }
            return inputID;
        }

        string EnterInput(string orderID, string productID, PvWmsRepository repository)
        {
            string inputID = PKeySigner.Pick(PkeyType.Inputs);

            repository.Insert(new Layers.Data.Sqls.PvWms.Inputs
            {
                ID = inputID,
                Code = inputID,
                OrderID = orderID,
                ProductID = productID, //ProudctID 从哪里来，要从外面传进来吧？
                CreateDate = DateTime.Now,
            });

            return inputID;
        }

        /// <summary>
        /// 保存Sorting信息
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="sortingId"></param>
        /// <param name="inputId"></param>
        /// <param name="noticeID"></param>
        /// <param name="adminID"></param>
        /// <param name="waybillID"></param>
        /// <param name="rsltID">最后生成的分拣ID</param>
        /// <param name="reponsitory"></param>
        /// <returns>是否为新增</returns>
        bool EnterSorting(JToken sorting, string sortingId, string inputId, string noticeID, string adminID, string waybillID, out string rsltID, PvWmsRepository reponsitory)
        {
            bool existSorting = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.ID == sortingId);
            var weight = sorting["Weight"]?.Value<decimal?>();
            var netWeight = sorting["NetWeight"]?.Value<decimal?>();
            if (!existSorting)
            {
                if (string.IsNullOrEmpty(sortingId))
                {
                    sortingId = PKeySigner.Pick(PkeyType.Sortings);
                }                

                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Sortings
                {
                    ID = sortingId,
                    BoxCode = sorting["BoxCode"]?.Value<string>(),
                    Quantity = sorting["Quantity"].Value<decimal>(),
                    AdminID = adminID,
                    CreateDate = DateTime.Now,
                    Weight = weight.HasValue ? (weight.Value > (decimal)0.01 ? weight.Value : (decimal)0.01) : (decimal)0.01,
                    NetWeight = netWeight,
                    Volume = sorting["Volume"]?.Value<decimal?>(),
                    InputID = inputId,
                    NoticeID = noticeID,
                    WaybillID = waybillID,
                    Summary = sorting["Summary"]?.Value<string>(),
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                {
                    BoxCode = sorting["BoxCode"]?.Value<string>(),
                    Quantity = sorting["Quantity"].Value<decimal>(),
                    AdminID = adminID,
                    CreateDate = DateTime.Now,
                    Weight = weight.HasValue ? (weight.Value > (decimal)0.01 ? weight.Value : (decimal)0.01) : (decimal)0.01,
                    NetWeight = netWeight,
                    Volume = sorting["Volume"]?.Value<decimal?>(),
                    InputID = inputId,
                    NoticeID = noticeID,
                    WaybillID = waybillID,
                    Summary = sorting["Summary"]?.Value<string>(),
                }, item => item.ID == sortingId);
            }

            rsltID = sortingId;
            return !existSorting;
        }

        /// <summary>
        /// 生成子运单
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="adminID"></param>
        /// <param name="pvcenterReponsitory"></param>
        string CreateSubWaybill(string waybillID, string adminID, PvCenterReponsitory pvcenterReponsitory)
        {
            var waybillEntity = pvcenterReponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Single(item => item.ID == waybillID);
            var newWaybillID = PKeySigner.Pick(PkeyType.Waybills);
            pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Waybills
            {
                ID = newWaybillID,
                FatherID = waybillEntity.ID,
                Code = waybillEntity.Code,
                Type = waybillEntity.Type,
                Subcodes = waybillEntity.Subcodes,
                CarrierID = waybillEntity.CarrierID,
                ConsignorID = waybillEntity.ConsignorID,
                ConsigneeID = waybillEntity.ConsigneeID,
                FreightPayer = waybillEntity.FreightPayer,
                TotalParts = waybillEntity.TotalParts,
                TotalWeight = waybillEntity.TotalWeight,
                TotalVolume = waybillEntity.TotalVolume,
                CarrierAccount = waybillEntity.CarrierAccount,
                VoyageNumber = waybillEntity.VoyageNumber,
                Condition = waybillEntity.Condition,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                EnterCode = waybillEntity.EnterCode,
                Status = waybillEntity.Status,
                CreatorID = adminID, //分拣人
                ModifierID = adminID,
                TransferID = waybillEntity.TransferID,
                IsClearance = waybillEntity.IsClearance,
                Packaging = waybillEntity.Packaging,
                Supplier = waybillEntity.Supplier,
                ExcuteStatus = waybillEntity.ExcuteStatus,
                CuttingOrderStatus = waybillEntity.CuttingOrderStatus,
                Summary = null,
                AppointTime = waybillEntity.AppointTime,
                OrderID = waybillEntity.OrderID,
                Source = waybillEntity.Source,
                NoticeType = waybillEntity.NoticeType,
                TempEnterCode = waybillEntity.TempEnterCode,
            });
            return newWaybillID;
        }


        /// <summary>
        /// 入库保存利用新的Sorting视图
        /// </summary>
        /// <param name="jobject"></param>
        public void EnterNew(JObject jobject, out string transferID)
        {
            //检查者
            Func<string, CgSortingExcuteStatus> StatusChecher = new Func<string, CgSortingExcuteStatus>(delegate (string wbID)
            {
                using (var pvcenterReponsitory = new PvCenterReponsitory())
                using (var pvwmsReponsitory = new PvWmsRepository())
                {
                    CgSortingExcuteStatus excuteStatus;
                    //历史分拣
                    var linq_sorteds = from sorting in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                       join _waybill in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>() on sorting.WaybillID equals _waybill.wbID
                                       where _waybill.wbID == wbID || _waybill.wbFatherID == wbID
                                       select new
                                       {
                                           sorting.NoticeID,
                                           sorting.Quantity
                                       };
                    var ienums_sortings = linq_sorteds.ToArray();

                    //当前的全部通知
                    var linq_notices = from notice in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                       where notice.WaybillID == wbID
                                       select new
                                       {
                                           notice.ID,
                                           notice.Quantity
                                       };
                    var ienums_notices = linq_notices.ToArray();


                    if (ienums_sortings.Any(item => item.NoticeID == null))
                    {
                        excuteStatus = CgSortingExcuteStatus.Anomalous;
                    }
                    else
                    {
                        var linq_merger = from notice in ienums_notices
                                          join sorting in ienums_sortings on notice.ID equals sorting.NoticeID into _sortings
                                          select new
                                          {
                                              NoticeID = notice.ID,
                                              noticeQuantity = notice.Quantity,
                                              sortedQuantity = _sortings.Sum(item => item.Quantity)
                                          };

                        if (linq_merger.Any(item => item.noticeQuantity < item.sortedQuantity))
                        {
                            excuteStatus = CgSortingExcuteStatus.Anomalous;
                        }
                        else if (linq_merger.Any(item => item.noticeQuantity != item.sortedQuantity))
                        {
                            excuteStatus = CgSortingExcuteStatus.PartStocked;
                        }
                        else
                        {
                            excuteStatus = CgSortingExcuteStatus.Completed;
                        }

                    }

                    return excuteStatus;
                }
            });
            transferID = string.Empty;
            var waybill = jobject["Waybill"];
            var sortings = jobject["Sortings"];
            var waybillorderid = waybill["OrderID"].Value<string>();
            var enterCode = waybill["EnterCode"].Value<string>();
            bool updateOrder = false; // 是否发生异常入库,需要更新Waybill对应的订单
            bool updateCustomOrderStatus = false;
            string waybillID = waybill["ID"].Value<string>();
            string adminID = jobject["AdminID"].Value<string>();
            var waybillType = (WaybillType)waybill["Type"].Value<int>();
            var waybillLoadingStatus = (CgLoadingExcuteStauts?)waybill["LoadingExcuteStatus"]?.Value<int?>();
            var supplier = waybill["Supplier"]?.Value<string>();
            string realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminID).RealName;
            string driver = waybill["Driver"].Value<string>();
            string carNumber = waybill["CarNumber1"].Value<string>();
            string carrierID = waybill["CarrierID"].Value<string>();
            string carrierName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>().SingleOrDefault(item => item.ID == carrierID)?.Name;
            string code = waybill["Code"]?.Value<string>();
            List<CgLogs_Operator> logsOperatorList = new List<CgLogs_Operator>();

            // 产地, 品牌变更通知List
            List<ItemChangeNotice> itemChangeNoticeList = new List<ItemChangeNotice>();
            // 当前运单的执行状态
            var waybillExcuteStatus = (CgSortingExcuteStatus)waybill["ExcuteStatus"].Value<int>();

            // 获取必要的信息
            var noticeView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                             join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
                             join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
                             join waybilltopview in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>() on notice.WaybillID equals waybilltopview.wbID
                             where notice.WaybillID == waybillID || waybilltopview.wbFatherID == waybillID
                             select new
                             {
                                 #region 视图
                                 input,
                                 input.OrderID,
                                 input.TinyOrderID,
                                 input.ItemID,
                                 InputID = input.ID,
                                 input.ClientID,
                                 input.SalerID,
                                 notice,
                                 product = product,
                                 notice.WareHouseID,
                                 NoticeID = notice.ID,
                                 notice.Supplier,
                                 Conditions = notice.Conditions,
                                 notice.BoxCode,
                                 notice.Quantity,
                                 notice.StorageID,
                                 Origin = notice.Origin,
                                 notice.DateCode,
                                 notice.OutputID,
                                 notice.WaybillID,

                                 #endregion
                             };

            var ienum_notices = noticeView.ToArray().Select(item => new
            {
                #region 视图
                item.input,
                item.OrderID,
                item.TinyOrderID,
                item.ItemID,
                InputID = item.InputID,
                item.ClientID,
                item.SalerID,
                item.notice,
                product = item.product,
                item.WareHouseID,
                NoticeID = item.notice.ID,
                item.Supplier,
                Conditions = item.Conditions.JsonTo<NoticeCondition>(),
                item.BoxCode,
                item.Quantity,
                item.StorageID,
                Origin = item.Origin == null ? "Unknown" : item.Origin,
                item.DateCode,
                item.OutputID,
                item.WaybillID,

                #endregion
            });

            var warehouseID = ienum_notices.Select(item => item.WareHouseID).FirstOrDefault(item => item != null);

            var source = (CgNoticeSource)waybill["Source"].Value<int>();
            var noticeType = (CgNoticeType)waybill["NoticeType"].Value<int>();

            if (waybillType == WaybillType.PickUp && (waybillLoadingStatus == null || waybillLoadingStatus == CgLoadingExcuteStauts.Waiting))
            {
                throw new Exception("当前运单不能进行分拣，自提的运单必须提货后才能进行分拣!");
            }

            #region 入库
            if (noticeType != CgNoticeType.Enter)
            {
                throw new Exception("非入库的接口调用！");
            }

            List<SplitOrderItemNotice> splitNoticeList = new List<SplitOrderItemNotice>();

            // 针对代报关外单进行拆项分组及检查是否进行拆项通知
            #region
            /* 根据新的要求，不再进行拆项的判断，根据前端提交的拆项标志来生成拆项
            if (source == CgNoticeSource.AgentBreakCustoms)
            {
                var sortingArr = JsonConvert.DeserializeObject<MySorting[]>(sortings.Json());

                //根据InputID, NoticeID分组
                var groupViewByInputIDNoticeID = from sorting in sortingArr
                                                 group sorting by new { sorting.InputID, sorting.NoticeID } into groups
                                                 where groups.Select(item => item.SortingID).Count() > 1
                                                 select new
                                                 {
                                                     groups.Key.InputID,
                                                     groups.Key.NoticeID,
                                                     count = groups.Select(item => item.SortingID).Count(),
                                                     TotalQuantity = groups.Sum(item => item.Quantity),
                                                     items = groups.Select(item => new
                                                     {
                                                         item.Origin,
                                                         item.Product.Manufacturer,
                                                         item.Product.PartNumber,
                                                         item.SortingID,
                                                         item.Quantity,
                                                         item.Summary,
                                                     }),
                                                 };

                if (groupViewByInputIDNoticeID != null)
                {

                    foreach (var group in groupViewByInputIDNoticeID)
                    {
                        var notice = ienum_notices.SingleOrDefault(item => item.NoticeID == group.NoticeID);
                        var infos = group.items;

                        var noticestorageview = from srtng in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                                join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                                        on srtng.ID equals storage.SortingID
                                                where srtng.NoticeID == @group.NoticeID
                                                select new
                                                {
                                                    noticeID = srtng.NoticeID,
                                                    sortingID = srtng.ID,
                                                    inputID = srtng.InputID,
                                                    sortedQuantity = storage.Total,
                                                };

                        var sortedInfo = new
                        {
                            noticeID = notice.NoticeID,
                            noticeQuantity = notice.Quantity,
                            sortedQuantity = noticestorageview.ToArray().Sum(item => item.sortedQuantity)
                        };

                        if (group.TotalQuantity <= notice.Quantity - sortedInfo.sortedQuantity)
                        {

                            // 根据PartNumber, Manufacturer分组
                            var groupViewByOriginManufacturer = from entity in infos
                                                                group entity by new { entity.Origin, entity.Manufacturer } into infosGroup
                                                                select new
                                                                {
                                                                    infosGroup.Key.Manufacturer,
                                                                    infosGroup.Key.Origin,
                                                                    count = infosGroup.Select(item => item.SortingID).Count(),
                                                                    items = infosGroup.Select(item => new
                                                                    {
                                                                        item.SortingID,
                                                                        item.Quantity,
                                                                        item.PartNumber,
                                                                        item.Summary,
                                                                    })
                                                                };
                            // 此时先判断是否有相同的品牌和产地
                            if (groupViewByOriginManufacturer.Any(item => item.count > 1))
                            {
                                throw new Exception("拆项中不能含有相同型号，品牌，产地的产品!");
                            }

                            // 拆项通知处理
                            var splitInput = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Single(item => item.ID == group.InputID);
                            foreach (var orderItem in group.items)
                            {
                                if (orderItem.Manufacturer != notice.product.Manufacturer || orderItem.Origin != notice.Origin)
                                {
                                    SplitOrderItemNotice splitNotice = new SplitOrderItemNotice();
                                    splitNotice.InputID = splitInput.ID;
                                    splitNotice.SortingID = orderItem.SortingID;
                                    splitNotice.ItemID = splitInput.ItemID;
                                    splitNotice.TinyOrderID = splitInput.TinyOrderID;
                                    splitNotice.AdminID = adminID;
                                    splitNotice.Brand = orderItem.Manufacturer;
                                    splitNotice.Origin = orderItem.Origin;
                                    splitNotice.Qty = orderItem.Quantity;
                                    splitNoticeList.Add(splitNotice);
                                }
                            }
                        }
                        else
                        {
                            if (group.items.Any(item => String.IsNullOrEmpty(item.Summary)))
                            {
                                throw new Exception("当前拆分后的总数量大于通知的应到数量，只能异常入库，且异常原因不能为空！");
                            }
                        }
                    }
                }
                //throw new Exception("正常完成");

            }
            */
            #endregion

            List<string> sortingsId = new List<string>();
            //处理本地
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            using (var pvwmsReponsitory = new PvWmsRepository())
            {
                //处理通知：Input信息、sortings（waybillid）

                #region sorting 处理                    

                foreach (var sorting in sortings)
                {
                    string noticeID = sorting["NoticeID"].Value<string>();
                    var notice = ienum_notices.SingleOrDefault(item => item.NoticeID == noticeID);
                    var inputID = sorting["InputID"].Value<string>();
                    var isSplit = sorting["IsSplit"]?.Value<bool?>();
                    var isAbnormal = false;
                    CgLogs_Operator logsOperator = new CgLogs_Operator();
                    logsOperator.MainID = waybillID;
                    logsOperator.Type = LogOperatorType.Insert;
                    logsOperator.Conduct = "分拣";
                    logsOperator.CreatorID = adminID;
                    logsOperator.CreateDate = DateTime.Now;

                    #region 保存Product信息 并 获取部分信息 
                    // 保存Product信息
                    string productID = EnterProduct(sorting["Product"]);
                    string packageCase = sorting["Product"]["PackageCase"]?.Value<string>();
                    string partNumber = sorting["Product"]["PartNumber"].Value<string>();
                    string manufacturer = sorting["Product"]["Manufacturer"].Value<string>();
                    string origin = sorting["Origin"].Value<string>();
                    string datecode = sorting["DateCode"].Value<string>();
                    var boxCode = sorting["BoxCode"]?.Value<string>();
                    string summary = sorting["Summary"]?.Value<string>();
                    summary = !string.IsNullOrEmpty(summary) ? (", 异常原因: " + summary) : "";

                    // 正常分拣, 及拆项(包括到货异常的情况)
                    var sortingQuantity = sorting["Quantity"].Value<int>();

                    #endregion

                    if (source == CgNoticeSource.AgentBreakCustoms)
                    {
                        if (string.IsNullOrEmpty(boxCode))
                        {
                            throw new Exception("报关业务箱号不能为空！");
                        }
                    }

                    // 非通知到货的处理
                    if (notice == null)
                    {
                        updateOrder = true;
                        isAbnormal = true;
                        inputID = this.EnterInput(waybillorderid, productID, pvwmsReponsitory); // 传输一个OrderID
                        if (source == CgNoticeSource.AgentBreakCustoms)
                        {
                            logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 无通知分拣 {LogOperatorType.Insert.GetDescription()}, 型号: {partNumber}, 品牌: {manufacturer}, 产地: {origin} 数量: {sortingQuantity}, 箱号:{boxCode.Substring(10)}{summary}";
                        }
                        else
                        {
                            logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 无通知分拣 {LogOperatorType.Insert.GetDescription()}, 型号: {partNumber}, 品牌: {manufacturer}, 产地: {origin} 数量: {sortingQuantity}{summary}";
                        }

                    }
                    else
                    {
                        if (source == CgNoticeSource.AgentBreakCustoms)
                        {
                            logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Insert.GetDescription()} 到货通知分拣, 型号: {partNumber}, 品牌: {manufacturer}, 产地: {origin} 数量: {sortingQuantity}, 箱号:{boxCode.Substring(10)}{summary}";
                        }
                        else
                        {
                            logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Insert.GetDescription()} 到货通知分拣, 型号: {partNumber}, 品牌: {manufacturer}, 产地: {origin} 数量: {sortingQuantity}{summary}";
                        }

                        // 以下为通知到货的处理
                        #region 查询当前库存中的数据,以便查证

                        var noticestorageview = from srtng in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                                join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                                        on srtng.ID equals storage.SortingID
                                                where srtng.NoticeID == noticeID
                                                select new
                                                {
                                                    noticeID = srtng.NoticeID,
                                                    sortingID = srtng.ID,
                                                    inputID = srtng.InputID,
                                                    sortedQuantity = storage.Total,
                                                };

                        var sortedInfo = new
                        {
                            noticeID = notice.NoticeID,
                            noticeQuantity = notice.Quantity,
                            sortedQuantity = noticestorageview.ToArray().Sum(item => item.sortedQuantity)
                        };

                        #endregion

                        #region 更新Sorting中所有的Inputs

                        //型号与数量不一致就当作异常，或者代报关 -- 产生新的Input

                        //已经与荣检约定：品牌修改暂时不考虑其他风险，之后还会有两次对单的工作
                        //if (productID != notice.product.ID
                        //    || (sortedInfo.noticeQuantity < sortedInfo.sortedQuantity + sortingQuantity))
                        if (partNumber != notice.product.PartNumber || packageCase != notice.product.PackageCase || datecode != notice.DateCode
                            || (sortedInfo.noticeQuantity < sortedInfo.sortedQuantity + sortingQuantity))
                        {
                            //生成新的InputID
                            if (source == CgNoticeSource.AgentBreakCustoms)
                            {
                                inputID = PKeySigner.Pick(PkeyType.Inputs);
                                inputID = this.EnterInput(new Layers.Data.Sqls.PvWms.Inputs
                                {
                                    ID = inputID,
                                    Code = inputID,
                                    OriginID = notice.input.OriginID,
                                    OrderID = notice.input.OrderID,
                                    TinyOrderID = null,
                                    ItemID = null,
                                    ProductID = productID,
                                    ClientID = notice.input.ClientID,
                                    PayeeID = notice.input.PayeeID,
                                    ThirdID = notice.input.ThirdID,
                                    TrackerID = notice.input.TrackerID,
                                    SalerID = notice.input.SalerID,
                                    PurchaserID = notice.input.PurchaserID,
                                    Currency = null,
                                    UnitPrice = null,
                                    CreateDate = DateTime.Now,
                                }, pvwmsReponsitory);
                            }
                            else
                            {
                                //inputID = this.EnterInput(notice.input, pvwmsReponsitory);
                                inputID = PKeySigner.Pick(PkeyType.Inputs);
                                inputID = this.EnterInput(new Layers.Data.Sqls.PvWms.Inputs
                                {
                                    ID = inputID,
                                    Code = inputID,
                                    OriginID = notice.input.OriginID,
                                    OrderID = notice.input.OrderID,
                                    TinyOrderID = notice.input.TinyOrderID,
                                    ItemID = notice.input.ItemID,
                                    ProductID = productID,
                                    ClientID = notice.input.ClientID,
                                    PayeeID = notice.input.PayeeID,
                                    ThirdID = notice.input.ThirdID,
                                    TrackerID = notice.input.TrackerID,
                                    SalerID = notice.input.SalerID,
                                    PurchaserID = notice.input.PurchaserID,
                                    Currency = notice.input.Currency,
                                    UnitPrice = notice.input.UnitPrice,
                                    CreateDate = DateTime.Now,
                                }, pvwmsReponsitory);
                            }

                            if (partNumber != notice.product.PartNumber || (sortedInfo.noticeQuantity < sortedInfo.sortedQuantity + sortingQuantity))
                                isAbnormal = true;
                        }
                        else
                        {
                            if (isSplit.HasValue && isSplit.Value)
                            {
                                inputID = PKeySigner.Pick(PkeyType.Inputs);
                                inputID = this.EnterInput(new Layers.Data.Sqls.PvWms.Inputs
                                {
                                    ID = inputID,
                                    Code = inputID,
                                    OriginID = notice.input.OriginID,
                                    OrderID = notice.input.OrderID,
                                    TinyOrderID = notice.input.TinyOrderID,
                                    ItemID = notice.input.ItemID,
                                    ProductID = productID,
                                    ClientID = notice.input.ClientID,
                                    PayeeID = notice.input.PayeeID,
                                    ThirdID = notice.input.ThirdID,
                                    TrackerID = notice.input.TrackerID,
                                    SalerID = notice.input.SalerID,
                                    PurchaserID = notice.input.PurchaserID,
                                    Currency = notice.input.Currency,
                                    UnitPrice = notice.input.UnitPrice,
                                    CreateDate = DateTime.Now,
                                }, pvwmsReponsitory);
                            }
                            else
                            {
                                inputID = this.EnterInput(notice.input, pvwmsReponsitory);
                            }                          
                        }

                        //单独判断一下 是否需要更新订单为异常
                        if (partNumber != notice.product.PartNumber || (sortedInfo.noticeQuantity < sortedInfo.sortedQuantity + sortingQuantity))
                        {
                            updateOrder = true;
                            isAbnormal = true;
                        }
                        #endregion
                    }

                    #region Sorting保存
                    // 处理Sorting
                    var sortingId = sorting["SortingID"]?.Value<string>();

                    // 之所以WaybillID 用noticeEntity.WaybillID, 是因为notice可能是重新修改后发回来的，其中的WaybillID可能已经就改过，是之前WaybillID的子运单ID                    
                    if (EnterSorting(sorting, sortingId, inputID, noticeID, adminID, waybillID, out sortingId, pvwmsReponsitory))
                    {
                        sortingsId.Add(sortingId);
                    }

                    logsOperatorList.Add(logsOperator);

                    if (isAbnormal == false && isSplit.HasValue && isSplit.Value == true)
                    {
                        var splitInput = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Single(item => item.ID == inputID);

                        SplitOrderItemNotice splitNotice = new SplitOrderItemNotice();
                        splitNotice.InputID = splitInput.ID;
                        splitNotice.SortingID = sortingId;
                        splitNotice.ItemID = splitInput.ItemID;
                        splitNotice.TinyOrderID = splitInput.TinyOrderID;
                        splitNotice.AdminID = adminID;
                        splitNotice.Brand = manufacturer;
                        splitNotice.Origin = origin;
                        splitNotice.Qty = sortingQuantity;
                        splitNoticeList.Add(splitNotice);
                    }

                    #endregion

                    #region 向Storages中存储具体入库信息
                    // 查找Storages库存中是否已经存在对应的产品库存 -- 通过sortingID来唯一确定
                    // 代报关时 是报关库 Customs
                    // 代收货时 是库存库 Stores
                    // 代转运时 是流水库 Flows
                    CgStoragesType type = CgStoragesType.Stores;

                    if (source == CgNoticeSource.AgentEnter)
                    {
                        if (notice == null)
                        {
                            type = CgStoragesType.Unknown;
                        }
                        else
                        {
                            type = CgStoragesType.Stores;
                        }
                    }
                    else
                    {
                        type = CgStoragesType.Flows;
                    }

                    string storageID;

                    #region 库存处理

                    if (string.IsNullOrWhiteSpace(notice?.StorageID))
                    {
                        storageID = PKeySigner.Pick(PkeyType.Storages);
                        pvwmsReponsitory.Insert(new Layers.Data.Sqls.PvWms.Storages
                        {
                            ID = storageID,
                            Type = (int)type,
                            WareHouseID = warehouseID,
                            SortingID = sortingId,
                            InputID = inputID,
                            ProductID = productID,
                            Total = sorting["Quantity"].Value<decimal>(),
                            Quantity = sorting["Quantity"].Value<decimal>(),
                            Origin = sorting["Origin"]?.Value<string>(),
                            IsLock = false,
                            CreateDate = DateTime.Now,
                            Status = (int)GeneralStatus.Normal,
                            ShelveID = sorting["ShelveID"].Value<string>(),
                            Supplier = notice?.Supplier ?? supplier,
                            DateCode = sorting["DateCode"].Value<string>(),
                            Summary = sorting["Summary"]?.Value<string>(),
                        });

                        pvwmsReponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Storage>(new
                        {
                            IsCurrent = false,
                        }, item => item.StorageID == storageID && item.IsCurrent == true);

                        var weight = sorting["Weight"]?.Value<decimal?>();
                        pvwmsReponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Storage
                        {
                            ID = Guid.NewGuid().ToString(),
                            AdminID = adminID,
                            BoxCode = boxCode,
                            CreateDate = DateTime.Now,
                            IsCurrent = true,
                            StorageID = storageID,
                            Summary = null,
                            Weight = weight.HasValue ? (weight.Value > (decimal)0.01 ? weight.Value : (decimal)0.01) : (decimal)0.01,
                        });
                    }
                    else
                    {
                        var tempStorage = pvwmsReponsitory.ReadTable<Storages>().SingleOrDefault(item => item.ID == notice.StorageID
                            && item.Type == (int)CgStoragesType.Flows);

                        // 如果是从暂存库移到流水库， 则从通知的流水库中扣除对应的分拣数量,
                        // 有否可能，分拣的数量大于暂存库中移动出来的流水库中的数量

                        //接收通知的时候处理库存这里 至多就是检查一下！是否库存真实存在
                        if (tempStorage == null || tempStorage.Quantity != notice.Quantity)
                        {
                            throw new Exception("暂存通知处理步骤有问题！");
                        }

                        storageID = notice.StorageID;
                        //更新分拣ID
                        pvwmsReponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                        {
                            SortingID = sortingId,
                        }, item => item.ID == storageID);
                    }

                    #endregion

                    #endregion

                    #region 申报条件
                    // IsDeclared又加回来，后面的注释去掉, ---根据封箱页面的功能要求, 当前不再根据IsDeclared作为生成申报日志, 及申报项日志的条件,所以不再生成申报及申报项日志, 不再更新订单状态
                    // 当通知的来源为代报关, 并且不是无通知到货的情况下, 通知为申请报关或者, 分拣的数量小于通知的数量情况下, 直接进行申报日志及申报项日志
                    if (source == CgNoticeSource.AgentBreakCustoms && notice != null && (notice.Conditions.IsDeclared))
                    {
                        //string boxCode = sorting["BoxCode"].Value<string>();
                        decimal? weight = sorting["Weight"]?.Value<decimal>();
                        weight = weight.HasValue ? (weight.Value > (decimal)0.01 ? weight.Value : (decimal)0.01) : (decimal)0.01;
                        if (string.IsNullOrEmpty(boxCode))
                        {
                            throw new Exception("代报关装箱箱号不能为空!");
                        }
                        this.Declaring(notice.input.TinyOrderID, enterCode, adminID, pvwmsReponsitory);
                        this.DeclaringItem(notice.OutputID, sortingQuantity, notice.input.TinyOrderID, notice.input.ItemID, boxCode, enterCode, adminID,
                            storageID, weight, pvwmsReponsitory);

                        // 如果业务来源为代报关, 并且没有更新过订单主状态则进行更新
                        if (!updateCustomOrderStatus)
                        {
                            UpdateOrderStatus(source, waybillorderid, adminID);
                            updateCustomOrderStatus = true;
                        }
                    }

                    #endregion

                    #region 产品变更通知
                    var inputChange = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Single(item => item.ID == inputID);
                    if ((source == CgNoticeSource.AgentBreakCustoms) && notice != null && inputChange.ItemID != null)
                    {
                        if (!splitNoticeList.Any(item => item.ItemID == inputChange.ItemID && item.TinyOrderID == inputChange.TinyOrderID && item.Brand == manufacturer && item.Origin == origin))
                        {
                            if (manufacturer != notice.product.Manufacturer)
                            {
                                itemChangeNoticeList.Add(new ItemChangeNotice
                                {
                                    AdminID = adminID,
                                    CreateDate = DateTime.Now,
                                    ItemID = inputChange.ItemID,
                                    TinyOrderID = inputChange.TinyOrderID,
                                    NewValue = manufacturer,
                                    OldValue = notice.product.Manufacturer,
                                    Type = OrderItemChangeType.BrandChange,
                                });
                            }

                            if (origin != notice.Origin)
                            {
                                itemChangeNoticeList.Add(new ItemChangeNotice
                                {
                                    AdminID = adminID,
                                    CreateDate = DateTime.Now,
                                    ItemID = inputChange.ItemID,
                                    TinyOrderID = inputChange.TinyOrderID,
                                    NewValue = origin,
                                    OldValue = notice.Origin,
                                    Type = OrderItemChangeType.OriginChange,
                                });
                            }

                            if (packageCase != notice.product.PackageCase)
                            {
                                itemChangeNoticeList.Add(new ItemChangeNotice
                                {
                                    AdminID = adminID,
                                    CreateDate = DateTime.Now,
                                    ItemID = inputChange.ItemID,
                                    TinyOrderID = inputChange.TinyOrderID,
                                    NewValue = packageCase,
                                    OldValue = notice.product.PackageCase,
                                    Type = OrderItemChangeType.PackageCaseChange,
                                });
                            }

                            if (datecode != notice.DateCode)
                            {
                                itemChangeNoticeList.Add(new ItemChangeNotice
                                {
                                    AdminID = adminID,
                                    CreateDate = DateTime.Now,
                                    ItemID = inputChange.ItemID,
                                    TinyOrderID = inputChange.TinyOrderID,
                                    NewValue = datecode,
                                    OldValue = notice.DateCode,
                                    Type = OrderItemChangeType.BatchChange,
                                });
                            }
                        }
                    }
                    #endregion

                    #region 拆项变更通知
                    /* 不再用以前的逻辑
                    SplitOrderItemNotice soin = splitNoticeList.SingleOrDefault(snl => snl.SortingID == sortingId);
                    if (soin != null)
                    {
                        soin.InputID = inputID;
                    }
                    */
                    #endregion
                }

                #region 操作日志
                code = !string.IsNullOrEmpty(code) ? (",运单ID: " + code) : "";
                driver = !string.IsNullOrEmpty(driver) ? (", 司机: " + driver) : "";
                carNumber = !string.IsNullOrEmpty(carNumber) ? (", 车牌号: " + carNumber) : "";
                carrierName = !string.IsNullOrEmpty(carrierName) ? (", 承运商: " + carrierName) : "";
                CgLogs_Operator logsWaybillOperator = new CgLogs_Operator();
                logsWaybillOperator.MainID = waybillID;
                logsWaybillOperator.Type = LogOperatorType.Update;
                logsWaybillOperator.Conduct = "分拣";
                logsWaybillOperator.CreatorID = adminID;
                logsWaybillOperator.CreateDate = DateTime.Now;
                logsWaybillOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Update.GetDescription()} 分拣, WaybillID: {waybillID} {code}{carrierName}{driver}{carNumber}";
                logsOperatorList.Add(logsWaybillOperator);

                if (logsOperatorList.Count() > 0)
                {
                    foreach (var log in logsOperatorList)
                    {
                        log.Enter(pvwmsReponsitory);
                    }
                }
                #endregion

                #endregion

                if (sortingsId.Count > 0)
                {
                    //先对运单的Code, CarrierID进行更新
                    this.EnterWaybill(waybill, adminID, waybillExcuteStatus);
                    //生成了子运单
                    string newWaybillID = this.CreateSubWaybill(waybillID, adminID, pvcenterReponsitory);
                    //更新运单ID
                    pvwmsReponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                    {
                        WaybillID = newWaybillID,
                    }, sorting => sortingsId.Contains(sorting.ID));
                }

                #region 报关订单每次分拣点击按钮都要去通知华芯通

                //理论上应该从分拣的结果中获取小订单
                //转报关如何处理？
                //由于只完成通知，理应写成异步的
                if (source == CgNoticeSource.AgentBreakCustoms)
                {
                    // 发生异常的情况下, 即产品型号不一致, 到货数量大于通知数量, 或者无通知到货情况下要通知
                    if (updateOrder == true)
                    {
                        // 调用华芯通服务，通知华芯通
                        string url = Wms.Services.FromType.ArrivalInfoToXDT.GetDescription();
                        Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { VastOrderID = waybillorderid });
                        //var result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { VastOrderID = waybillorderid });
                        //var message = result.JsonTo<JMessage>();
                        //if (message.code != 200)
                        //{
                        //    throw new Exception("ArrivalInfoToXDT 通知失败: " + message.data);
                        //}
                        // 通知报关员入库分拣异常，不可报关
                         PlatNotice.ByRole[PlatNotice.Declarants].Enter("代报关订单入库异常", $"代报关订单入库(不可报关):{waybillorderid}", waybillorderid, LogNoticeType.SortingAbnomaly);

                        /* 根据荣捡通知库房广播接口暂时先不增加
                        // 调用恒远服务，通知恒远
                        string urlhy = Wms.Services.FromType.ArrivalInfoToHY.GetDescription();
                        Yahv.Utils.Http.ApiHelper.Current.JPost(urlhy, new { VastOrderID = waybillorderid });

                        // 通知报关员入库分拣异常，不可报关
                        PlatNotice.ByRole[PlatNotice.Declarants].Enter("代报关订单入库异常", $"代报关订单入库(不可报关):{waybillorderid}", waybillorderid, LogNoticeType.SortingAbnomaly);
                        */
                    }

                    #region 产品变更通知
                    //#if DEBUG
                    if (itemChangeNoticeList.Count() > 0)
                    {
                        string url = Wms.Services.FromType.ItemChangeNotice.GetDescription();
                        var result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { Notices = itemChangeNoticeList });
                        var message = result.JsonTo<JMessage>();
                        if (message.code != 200)
                        {
                            throw new Exception("通知华芯通产品变更通知失败:" + message.data);
                        }
                    }
                    //#endif
                    #endregion                    
                }
                else
                {
                    string url = Wms.Services.FromType.NoticeDelivery.GetDescription();

                    Yahv.Services.Models.StoreChange sc = new Yahv.Services.Models.StoreChange();
                    sc.List.Add(new Yahv.Services.Models.ChangeItem
                    {
                        orderid = waybillorderid
                    });

                    Yahv.Utils.Http.ApiHelper.Current.JPost(url, sc);
                }

                #region 代报关拆项通知,非代报关通知也使用同样的拆项处理
                //#if TEST
                if (splitNoticeList.Count > 0)
                {
                    SplitChangeRequest splitChangeRequest = new SplitChangeRequest();
                    splitChangeRequest.Notices = splitNoticeList;

                    string url = Wms.Services.FromType.SplitChangeNotice.GetDescription();
                    var result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, splitChangeRequest);
                    var message = result.JsonTo<JMessage>();
                    if (message.code != 200)
                    {
                        throw new Exception("通知华芯通拆项变更通知失败:" + message.data);
                    }
                }
                //#endif
                #endregion

                #endregion

                //重新更新状态
                var status = StatusChecher(waybillID);
                if (waybillExcuteStatus == CgSortingExcuteStatus.Anomalous)
                {
                    waybillExcuteStatus = CgSortingExcuteStatus.Anomalous;
                }
                else
                {
                    if (updateOrder == true)
                    {
                        waybillExcuteStatus = CgSortingExcuteStatus.Anomalous;
                    }
                    else
                    {
                        waybillExcuteStatus = status;
                    }
                    
                }
                this.EnterWaybill(waybill, adminID, waybillExcuteStatus);

                #region 日志处理

                // 待提货状态是否能处理到?
                // 处理状态累加(请小辉直接指导一下)            
                //更新及保存运单日志
                pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_Waybills>(new
                {
                    IsCurrent = false,//建议询问小辉
                }, item => item.Type == (int)WaybillStatusType.ExecutionStatus && item.MainID == waybillID);

                pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Waybills
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = waybillID,
                    Type = (int)WaybillStatusType.ExecutionStatus,//建议询问小辉
                    Status = (int)waybillExcuteStatus,//建议询问小辉
                    CreateDate = DateTime.Now,
                    CreatorID = adminID,
                    IsCurrent = true,
                });

                #endregion
            }

            #region 发生异常入库，需要更新运单执行状态，并通知修改订单状态
            if (updateOrder)
            {
                waybillExcuteStatus = CgSortingExcuteStatus.Anomalous;
                this.EnterWaybill(waybill, adminID, waybillExcuteStatus);

                // 通知跟单修改订单
                foreach (var trackerID in ienum_notices.Select(item => item.input.TrackerID).Distinct())
                {
                    if (string.IsNullOrWhiteSpace(trackerID))
                    {
                        continue;
                    }
                    PlatNotice.ByAdmin[trackerID].Enter("香港到货异常", $"请处理异常到货订单:{waybillorderid}", waybillorderid, LogNoticeType.HKDeliveryError);
                }
            }
            #endregion

            #region 全部入库完成时的代转运处理
            if (waybillExcuteStatus == CgSortingExcuteStatus.Completed)
            {
                #region 更新订单主状态, 根据沈忱，董建最新要求，不再更改订单的执行状态了，只在入库完成时更新订单那主状态
                UpdateOrderStatus(source, waybillorderid, adminID);
                #endregion

                using (var reponsitory = new PvWmsRepository())
                {
                    if (source == CgNoticeSource.Transfer)
                    {
                        var noticesView = from notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                          where notice.WaybillID == waybillID && notice.Type == (int)CgNoticeType.Out
                                          select notice;

                        // 此条件为没有进行过代转运出库
                        if (noticesView.Count() == 0)
                        {
                            #region 代转运处理
                            var linq = from storage in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                       join sorting in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                                       join notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on sorting.NoticeID equals notice.ID
                                       join input in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on storage.InputID equals input.ID
                                       join waybilltop in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>() on sorting.WaybillID equals waybilltop.wbID
                                       where waybilltop.wbID == waybillID || waybilltop.wbFatherID == waybillID
                                       select new
                                       {
                                           #region 视图

                                           storage.ID,
                                           storage.InputID,
                                           sorting.WaybillID,
                                           sorting.NoticeID,
                                           sorting.BoxCode,
                                           sorting.Weight,
                                           sorting.NetWeight,
                                           sorting.Volume,
                                           notice,
                                           input,
                                           waybilltop,
                                           storage,

                                           #endregion
                                       };

                            var ienum_linq = linq.ToArray();
                            transferID = ienum_linq.FirstOrDefault().waybilltop.wbTransferID;

                            foreach (var storage in ienum_linq)
                            {
                                var outputId = PKeySigner.Pick(PkeyType.Outputs);
                                var pickingID = PKeySigner.Pick(PkeyType.Pickings);
                                var outNoticeID = PKeySigner.Pick(PkeyType.Notices);

                                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Outputs
                                {
                                    ID = outputId,
                                    InputID = storage.InputID,
                                    OrderID = storage.input.OrderID,
                                    TinyOrderID = storage.input.TinyOrderID,
                                    ItemID = storage.input.ItemID,
                                    OwnerID = storage.input.ClientID,
                                    SalerID = storage.input.SalerID,
                                    CustomerServiceID = null, // 跟单员 转运出库销项跟单员填什么
                                    PurchaserID = storage.input.PurchaserID,
                                    Currency = storage.input.Currency,
                                    Price = storage.input.UnitPrice,
                                    CreateDate = DateTime.Now,
                                    TrackerID = storage.input.TrackerID
                                });

                                //在分拣录入之后就会发出出库通知
                                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Notices
                                {
                                    ID = outNoticeID,
                                    Type = (int)CgNoticeType.Out,
                                    WareHouseID = storage.storage.WareHouseID,
                                    WaybillID = storage.waybilltop.wbTransferID,
                                    InputID = storage.InputID,
                                    OutputID = outputId,
                                    ProductID = storage.storage.ProductID,
                                    Supplier = storage.storage.Supplier,
                                    DateCode = storage.storage.DateCode,
                                    Origin = storage.storage.Origin,
                                    Quantity = storage.storage.Quantity,
                                    Conditions = storage.notice.Conditions,
                                    CreateDate = DateTime.Now,
                                    Status = (int)NoticesStatus.Waiting,  // 需要再确认，转运出库时的通知状态
                                    Source = (int)NoticeSource.Transfer,
                                    Target = (int)NoticesTarget.Default,
                                    StorageID = storage.storage.ID,
                                    BoxCode = storage.BoxCode,
                                    Weight = storage.Weight,
                                    NetWeight = storage.NetWeight,
                                    Volume = storage.Volume,
                                    ShelveID = storage.storage.ShelveID,
                                });

                            }

                            #endregion
                        }

                    }

                    if (source == CgNoticeSource.AgentBreakCustoms)
                    {
                        #region 代报关处理
                        //后面的注释取消 -- 根据封箱操作页面的要求, 不在此处进行生成申报项及申报项日志.
                        var tinyOrderIds = (from notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                            join input in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                            on notice.InputID equals input.ID
                                            join waybillView in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                                            on notice.WaybillID equals waybillView.wbID
                                            where notice.WaybillID == waybillID || waybillView.wbFatherID == waybillID
                                            select input.TinyOrderID).Distinct().ToArray();

                        foreach (var tinyOrderId in tinyOrderIds)
                        {
                            var linq_custom = from storage in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                              join input in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on storage.InputID equals input.ID
                                              join sorting in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                                              join waybillView in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>() on sorting.WaybillID equals waybillView.wbID
                                              join notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on sorting.NoticeID equals notice.ID
                                              where input.TinyOrderID == tinyOrderId
                                              select new
                                              {
                                                  storage.ProductID,
                                                  storage.Origin,
                                                  storage.DateCode,
                                                  storage.Quantity,
                                                  StorageID = storage.ID,
                                                  sorting,
                                                  input.TinyOrderID,
                                                  input.ItemID,
                                                  EnterCode = waybillView.wbEnterCode,
                                                  notice.OutputID,
                                                  notice.Conditions,
                                              };

                            var ienum_linqcustom = linq_custom.ToArray();

                            //var linq_group = from item in ienum_linqcustom
                            //                 group item by new { item.TinyOrderID } into items
                            //                 select new
                            //                 {
                            //                     TinyOrderID = items.Key,
                            //                     Quantity = items.Sum(s => s.Quantity),
                            //                     ProductID = items.First().ProductID,
                            //                     Origin = items.First().Origin,
                            //                     DateCode = items.First().DateCode,
                            //                 };

                            foreach (var item in ienum_linqcustom)
                            {
                                if (!string.IsNullOrEmpty(item.sorting.BoxCode))
                                {
                                    this.Declaring(item.TinyOrderID, item.EnterCode, adminID, reponsitory);
                                    this.DeclaringItem(item.OutputID, item.Quantity, item.TinyOrderID, item.ItemID, item.sorting.BoxCode, item.EnterCode, adminID, item.StorageID, item.sorting.Weight, reponsitory);
                                    reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                                    {
                                        Summary = string.Empty,
                                    }, sorting => sorting.ID == item.sorting.ID);
                                }
                            }

                        }

                        #endregion

                        // 如果业务来源为代报关, 并且没有更新过订单主状态则进行更新
                        // 后面的注释取消--根据封箱页面要求, 也不能在此处进行代报关的订单装更新, 必须生成申报日志, 及申报项日志后才能更新
                        if (!updateCustomOrderStatus)
                        {
                            UpdateOrderStatus(source, waybillorderid, adminID);
                        }

                    }
                }
            }

            #endregion

            #endregion
        }

        /// <summary>
        /// 更新订单主状态
        /// </summary>
        /// <param name="source"></param>
        /// <param name="waybillorderid"></param>
        /// <param name="adminID"></param>
        public void UpdateOrderStatus(CgNoticeSource source, string waybillorderid, string adminID)
        {
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                if (source == CgNoticeSource.AgentEnter || source == CgNoticeSource.Transfer)
                {
                    //保存订单日志, 非代报关业务订单的状态为已收货
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == 1 && item.MainID == waybillorderid);

                    var orderID = waybillorderid;
                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = 1, //订单主状态，  
                        Status = (int)CgOrderStatus.已交货,//建议询问小辉
                        CreateDate = DateTime.Now,
                        CreatorID = adminID,
                        IsCurrent = true,
                    });

                    if (source == CgNoticeSource.AgentEnter)
                    {
                        //保存订单日志, 代收货业务订单的支付状态为待确认
                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                        {
                            IsCurrent = false,
                        }, item => item.Type == (int)OrderStatusType.PaymentStatus && item.MainID == waybillorderid);

                        pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                        {
                            ID = Guid.NewGuid().ToString(),
                            MainID = orderID,
                            Type = (int)OrderStatusType.PaymentStatus, //订单支付状态，  
                            Status = (int)OrderPaymentStatus.Confirm,
                            CreateDate = DateTime.Now,
                            CreatorID = adminID,
                            IsCurrent = true,
                        });
                    }
                }
                else if (source == CgNoticeSource.AgentBreakCustoms)
                {
                    //保存订单日志, 非代报关业务订单的状态为已收货
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == 1 && item.MainID == waybillorderid);

                    var orderID = waybillorderid;
                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = 1, //订单主状态，  
                        Status = (int)CgOrderStatus.待报关,//建议询问小辉
                        CreateDate = DateTime.Now,
                        CreatorID = adminID,
                        IsCurrent = true,
                    });
                }

            }
        }


        /// <summary>
        /// 申报
        /// </summary>
        /// <param name="waybillID">Notice.waybillID</param>
        /// <param name="tinyOrderID">小订单</param>
        /// <param name="boxCode">箱号</param>
        /// <param name="enterCode">入仓号</param>
        /// <param name="adminID">装箱人</param>
        /// <param name="reponsitory"></param>
        public void Declaring(string tinyOrderID,
            string enterCode,
            string adminID,
            PvWmsRepository reponsitory)
        {
            bool existLogDelcare = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>().
                Any(item => item.TinyOrderID == tinyOrderID);
            if (!existLogDelcare)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Declare
                {
                    ID = PKeySigner.Pick(PkeyType.LogsDeclare),
                    TinyOrderID = tinyOrderID,
                    EnterCode = enterCode,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = (int)TinyOrderDeclareStatus.Boxed,
                    AdminID = adminID,
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Declare>(new
                {
                    EnterCode = enterCode,
                    UpdateDate = DateTime.Now,
                    AdminID = adminID,
                }, item => item.TinyOrderID == tinyOrderID);
            }

        }

        /// <summary>
        /// 申报项目
        /// </summary>
        /// <param name="outputID">销项(可空)</param>
        /// <param name="quantity">申报数量</param>
        /// <param name="tinyOrderID">小订单ID</param>
        /// <param name="itemID">订单项ID</param>
        /// <param name="boxCode"></param>
        /// <param name="enterCode"></param>
        /// <param name="adminID"></param>
        /// <param name="storageID"></param>
        /// <param name="weight"></param>
        /// <param name="reponsitory"></param>
        public void DeclaringItem(string outputID, decimal quantity,
         string tinyOrderID,
         string itemID,
         string boxCode,
         string enterCode,
         string adminID,
         string storageID,
         decimal? weight, PvWmsRepository reponsitory)
        {
            bool existLogDeclareItem = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>().Any(item => item.OrderItemID == itemID
                && item.StorageID == storageID);
            if (!existLogDeclareItem)
            {
                var storage = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().SingleOrDefault(item => item.ID == storageID);
                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_DeclareItem
                {
                    ID = PKeySigner.Pick(PkeyType.LogsDeclareItem),
                    TinyOrderID = tinyOrderID,
                    OrderItemID = itemID,
                    StorageID = storageID,
                    Quantity = quantity,
                    AdminID = adminID,
                    OutputID = outputID,
                    BoxCode = boxCode,
                    Weight = weight,
                    Status = 0,
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
                {
                    TinyOrderID = tinyOrderID,
                    Quantity = quantity,
                    AdminID = adminID,
                    OutputID = outputID,
                    BoxCode = boxCode,
                    Weight = weight,
                    //Status = 0,
                }, item => item.OrderItemID == itemID && item.StorageID == storageID);
            }
        }

        /// <summary>
        /// 封箱操作,生成对应的申报项及申报项日志
        /// </summary>
        /// <param name="cgCloseBoxes"></param>
        public void CloseBoxesOld(CgCloseBoxes cgCloseBoxes)
        {
            using (var reponsitory = new PvWmsRepository())
            {
                foreach (var closeBox in cgCloseBoxes.Items)
                {
                    this.Declaring(closeBox.TinyOrderID, closeBox.EnterCode, closeBox.AdminID, reponsitory);
                    this.DeclaringItem(null, closeBox.Quantity.Value, closeBox.TinyOrderID, closeBox.ItemID, closeBox.BoxCode, closeBox.EnterCode, closeBox.AdminID, closeBox.StorageID, closeBox.Weight, reponsitory);
                }

                var firstCloseBox = cgCloseBoxes.Items[0];

                // 更新Logs_PvWsOrder订单状态
                this.UpdateOrderStatus(CgNoticeSource.AgentBreakCustoms, firstCloseBox.OrderID, firstCloseBox.AdminID);
            }
        }

        /// <summary>
        /// 新的封箱操作,仅仅提供是否能进行封箱成功的判断
        /// </summary>
        /// <param name="adminID"></param>
        /// <param name="arry"></param>
        public void CloseBoxes(string waybillID, string adminID, string[] arry)
        {
            List<string> abnormalListSorting = new List<string>();
            List<string> normalListSorting = new List<string>(arry);
            StringBuilder deliveryWithNoticeAbnormalSB = new StringBuilder();
            StringBuilder deliveryWithWsOrderAbnormalSB = new StringBuilder();
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            using (var pvwmsReponsitory = new PvWmsRepository())
            {
                //历史分拣
                var linq_sorteds = from sorting in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                   join _waybill in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>() on sorting.WaybillID equals _waybill.wbID
                                   join input in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on sorting.InputID equals input.ID
                                   join storage in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on sorting.ID equals storage.SortingID
                                   join product in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
                                   where _waybill.wbID == waybillID || _waybill.wbFatherID == waybillID
                                   select new
                                   {
                                       sorting.NoticeID,
                                       product.Manufacturer,
                                       product.PartNumber,
                                       storage.Origin,
                                       sorting.Quantity,
                                       input.TinyOrderID,
                                   };
                var ienums_sortings = linq_sorteds.ToArray();

                //当前的全部通知
                var linq_notices = from notice in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                   join input in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                   on notice.InputID equals input.ID
                                   join product in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                                   on notice.ProductID equals product.ID
                                   where notice.WaybillID == waybillID
                                   select new
                                   {
                                       notice.ID,
                                       product.Manufacturer,
                                       product.PartNumber,
                                       notice.Origin,
                                       notice.Quantity,
                                       input.TinyOrderID,
                                   };
                var ienums_notices = linq_notices.ToArray();


                var linq_merger = from notice in ienums_notices
                                  join sorting in ienums_sortings on new { NoticeID = notice.ID, PartNumber = notice.PartNumber, Manufacturer = notice.Manufacturer, Origin = notice.Origin, TinyOrderID = notice.TinyOrderID }
                                  equals new { NoticeID = sorting.NoticeID, PartNumber = sorting.PartNumber, Manufacturer = sorting.Manufacturer, Origin = sorting.Origin, TinyOrderID = sorting.TinyOrderID } into _sortings
                                  select new
                                  {
                                      NoticeID = notice.ID,
                                      noticeQuantity = notice.Quantity,
                                      notice.PartNumber,
                                      notice.Manufacturer,
                                      notice.Origin,
                                      notice.TinyOrderID,
                                      sortedQuantity = _sortings.Sum(item => item.Quantity)
                                  };

                foreach (var tinyOrderID in arry)
                {
                    var isAbnormalForSorting = false;
                    //if (ienums_sortings.Any(item => item.TinyOrderID == null))
                    //{
                    //    var abnormalSortingArr = ienums_sortings.Where(item => item.TinyOrderID == null && item.NoticeID != null).Select(item => item.PartNumber);

                    //    deliveryWithNoticeAbnormalSB.AppendLine($"{string.Join(",", abnormalSortingArr.ToArray())}型号到货异常，需要处理");
                    //    isAbnormalForSorting = true;
                    //    abnormalListSorting.Add(tinyOrderID);
                    //    normalListSorting.Remove(tinyOrderID);
                    //    break;
                    //}

                    if (ienums_sortings.Where(item => item.TinyOrderID == tinyOrderID).Any(item => item.NoticeID == null))
                    {
                        var abnormalPartNumberArr = ienums_sortings.Where(item => item.TinyOrderID == tinyOrderID).Where(item => item.NoticeID == null).Select(item => item.PartNumber);
                        deliveryWithNoticeAbnormalSB.AppendLine(tinyOrderID + "订单号中" + string.Join(",", abnormalPartNumberArr.ToArray()) + "型号没有对应的通知");
                        isAbnormalForSorting = true;
                        abnormalListSorting.Add(tinyOrderID);
                        normalListSorting.Remove(tinyOrderID);
                        break;
                    }

                    if (linq_merger.Where(item => item.TinyOrderID == tinyOrderID).Any(item => item.noticeQuantity != item.sortedQuantity))
                    {
                        deliveryWithNoticeAbnormalSB.AppendLine(tinyOrderID + "订单号中");
                        var totalErr = linq_merger.Where(item => item.TinyOrderID == tinyOrderID).Where(item => item.noticeQuantity != item.sortedQuantity).Select(item => item);
                        foreach (var item in totalErr)
                        {
                            deliveryWithNoticeAbnormalSB.AppendLine($"型号:{item.PartNumber},产地:{item.Origin}, 通知的数量为:{item.noticeQuantity},到货的数量与通知的数量不一致");
                        }

                        isAbnormalForSorting = true;
                        abnormalListSorting.Add(tinyOrderID);
                        normalListSorting.Remove(tinyOrderID);
                    }
                }
            }

            // 处理分拣与通知不一致的情况
            if (abnormalListSorting.Count > 0)
            {
                //var abnormalArrySorting = abnormalListSorting.ToArray();
                //var sortingContent = string.Join(",", abnormalArrySorting);

                //throw new Exception($"小订单{sortingContent}中包含与入库通知不一致的分拣,请处理后再进行封箱操作!");
                throw new Exception($"检查到货与通知时发现:{deliveryWithNoticeAbnormalSB.ToString()},请处理后再进行封箱操作!");
            }

            // 检测WsOrderItemTopView 与 到货的实际情况是否完全一致
            var wsOrderItemTopViews = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WsOrderItemTopView>()
                                      select new
                                      {
                                          OrderItemID = entity.ItemID,
                                          entity.TinyOrderID,
                                          entity.Origin,
                                          PartNumber = entity.Model,
                                          entity.Manufacturer,
                                          entity.Quantity,
                                      };

            var logDeclareItemViews = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>()
                                      join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on entity.StorageID equals storage.ID
                                      join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
                                      select new
                                      {
                                          entity.OrderItemID,
                                          entity.TinyOrderID,
                                          entity.Quantity,
                                          storage.Origin,
                                          product.Manufacturer,
                                          product.PartNumber,
                                      };
            var inputView = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                            select new
                            {
                                entity.OrderID,
                                entity.TinyOrderID
                            };

            List<string> abnormalList = new List<string>();
            List<string> normalList = new List<string>(arry);

            // 申报项视图与华芯通订单项视图对比检查
            bool isAbnormal = false;
            foreach (var tinyOrderID in arry)
            {
                bool islogDeclareItemAbnormal = false;
                bool iswsOrderItemAbnormal = false;
                var wsOrderItemView = wsOrderItemTopViews.Where(item => item.TinyOrderID == tinyOrderID);
                var logDeclareItemView = logDeclareItemViews.Where(item => item.TinyOrderID == tinyOrderID);
                var logDeclareItemGroupView = from entity in logDeclareItemView
                                              group entity by new { entity.TinyOrderID, entity.OrderItemID, entity.PartNumber, entity.Manufacturer, entity.Origin } into entitygroup
                                              select new
                                              {
                                                  entitygroup.Key.TinyOrderID,
                                                  entitygroup.Key.OrderItemID,
                                                  entitygroup.Key.PartNumber,
                                                  entitygroup.Key.Manufacturer,
                                                  entitygroup.Key.Origin,
                                                  Quantity = entitygroup.Sum(item => item.Quantity),
                                              };

                var orderID = inputView.Where(item => item.TinyOrderID == tinyOrderID).Select(item => item.OrderID).Distinct().First();
                deliveryWithWsOrderAbnormalSB.AppendLine($"{tinyOrderID}订单号中");
                foreach (var logDeclareItem in logDeclareItemGroupView.ToArray())
                {
                    var wsOrderItem = wsOrderItemView.SingleOrDefault(item => item.OrderItemID == logDeclareItem.OrderItemID);
                    if (wsOrderItem == null || wsOrderItem.Manufacturer != logDeclareItem.Manufacturer || wsOrderItem.PartNumber != logDeclareItem.PartNumber || wsOrderItem.Origin != logDeclareItem.Origin || wsOrderItem.Quantity != logDeclareItem.Quantity)
                    {
                        deliveryWithWsOrderAbnormalSB.AppendLine($"{tinyOrderID}订单号中");
                        deliveryWithWsOrderAbnormalSB.AppendLine($"到货型号:{logDeclareItem.PartNumber}的信息与订单不一致");
                        islogDeclareItemAbnormal = true;
                        break;
                    }
                }

                foreach (var wsOrderItem in wsOrderItemView)
                {
                    var logDeclareItem = logDeclareItemGroupView.SingleOrDefault(item => item.OrderItemID == wsOrderItem.OrderItemID);
                    if (logDeclareItem == null || logDeclareItem.Manufacturer != wsOrderItem.Manufacturer || logDeclareItem.PartNumber != wsOrderItem.PartNumber || logDeclareItem.Origin != wsOrderItem.Origin || logDeclareItem.Quantity != wsOrderItem.Quantity)
                    {
                        deliveryWithWsOrderAbnormalSB.AppendLine($"{tinyOrderID}订单号中");
                        deliveryWithWsOrderAbnormalSB.AppendLine($"订单中的型号:{wsOrderItem.PartNumber}的信息与到货不一致");
                        iswsOrderItemAbnormal = true;
                        break;
                    }
                }

                if (islogDeclareItemAbnormal || iswsOrderItemAbnormal)
                {
                    isAbnormal = true;
                    abnormalList.Add(tinyOrderID);
                    normalList.Remove(tinyOrderID);
                }
            }

            #region 正常封箱成功的TinyOrderID, 进行待报关申请页面的显示
            if (normalList.Count > 0)
            {
                var newArry = normalList.ToArray();
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
                {
                    Status = 1,
                }, item => newArry.Contains(item.TinyOrderID));
            }
            #endregion

            #region 封箱失败的TinyOrderID， 提示操作人
            if (isAbnormal)
            {
                //var abnormalArry = abnormalList.ToArray();
                //var noticeContent = string.Join(",", abnormalArry);
                //throw new Exception($"小订单{noticeContent}的待申报项与华芯通订单项视图检测中，型号，品牌，产地，数量有不一致的内容，需要等跟单先处理到货异常");
                throw new Exception($"检查到货与订单信息时发现:{deliveryWithWsOrderAbnormalSB.ToString()},请处理后再进行封箱操作!");
            }
            #endregion
        }

        /// <summary>
        /// 提货
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="adminID"></param>
        public void TakeGoods(string waybillID, string adminID)
        {
            var realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminID).RealName;

            using (var reponsitory = new PvCenterReponsitory())
            {
                CgLogs_Operator logs_operator = new CgLogs_Operator();
                logs_operator.Conduct = "分拣";
                logs_operator.CreatorID = adminID;
                logs_operator.Type = LogOperatorType.Insert;
                logs_operator.CreateDate = DateTime.Now;
                logs_operator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 分拣 {LogOperatorType.Insert.GetDescription()} 提货";
                logs_operator.MainID = waybillID;
                logs_operator.Enter();

                // 更新提货状态
                reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                {
                    ExcuteStatus = (int)CgLoadingExcuteStauts.Taking,
                }, item => item.ID == waybillID);

                // 更新Log_Waybills
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_Waybills>(new
                {
                    IsCurrent = false,
                }, item => item.Type == (int)WaybillStatusType.ExecutionStatus && item.MainID == waybillID);

                // 状态设置为等待分拣
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Waybills
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = waybillID,
                    Type = (int)WaybillStatusType.ExecutionStatus,//建议询问小辉
                    Status = (int)CgSortingExcuteStatus.Sorting,//建议询问小辉
                    CreateDate = DateTime.Now,
                    CreatorID = adminID,
                    IsCurrent = true,
                });
            }
        }

        #region 搜索方法

        /// <summary>
        /// 根据partNumber搜索
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public CgSortingsView SearchByPartNumber(string partNumber)
        {
            var productViews = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>();
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq_waybillIds = from product in productViews
                                  join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on product.ID equals notice.ProductID
                                  where product.PartNumber.Contains(partNumber) && notice.WareHouseID.StartsWith(wareHouseID)
                                  orderby notice.WaybillID descending
                                  select notice.WaybillID;

            var linq_ids = linq_waybillIds.Distinct().Take(500);

            var linq = from waybill in waybillView
                       join id in linq_ids on waybill.ID equals id
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据Supplier搜索
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public CgSortingsView SearchBySupplier(string supplier)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.Supplier.Contains(supplier)
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据开始时间搜索
        /// </summary>
        /// <param name="startdate"></param>
        /// <returns></returns>
        public CgSortingsView SearchByStartDate(DateTime startdate)
        {
            var linq = from waybill in this.IQueryable.Cast<MyWaybill>()
                       where waybill.CreateDate >= startdate
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据截止时间搜索
        /// </summary>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public CgSortingsView SearchByEndDate(DateTime enddate)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var linq = from waybill in waybillView
                       where waybill.CreateDate < enddate.AddDays(1)
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        public CgSortingsView SearchByDate(DateTime? startdate, DateTime? enddate)
        {

            Expression<Func<MyWaybill, bool>> predicate = waybill => (startdate == null ? true : waybill.CreateDate >= startdate)
                && (enddate == null ? true : waybill.CreateDate < enddate.Value.AddDays(1));

            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var linq = waybillView.Where(predicate);

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }


        /// <summary>
        /// 根据运单ID搜索
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public CgSortingsView SearchByWaybillID(string waybillID)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.ID.Contains(waybillID)
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据运单号，入仓号，订单号搜索
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CgSortingsView SearchByID(string id)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq_waybillIDs = from waybill in waybillView
                                  where waybill.ID.Contains(id) || waybill.Code.Contains(id) || waybill.EnterCode.Contains(id) || waybill.OrderID.Contains(id)
                                  orderby waybill.ID descending
                                  select waybill.ID;

            var linq_orderIDs = from input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                on input.ID equals notice.InputID
                                where notice.WareHouseID.StartsWith(this.wareHouseID)
                                  && input.OrderID.Contains(id)
                                  && notice.Status != (int)NoticesStatus.Closed
                                orderby notice.WaybillID descending
                                select notice.WaybillID;

            var linq_IDs = linq_waybillIDs.Distinct().Take(500).Concat(linq_orderIDs.Distinct().Take(500)).Distinct();

            var linq = from waybill in waybillView
                       join key in linq_IDs on waybill.ID equals key
                       orderby waybill.ID descending
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据业务来源搜索
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CgSortingsView SearchBySource(int sources)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            // var includes = sources.Select(item => (int)item);
            var linq = from waybill in waybillView
                       where (int)waybill.Source == sources
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据运单到货方式搜索
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CgSortingsView SearchByWaybillType(int type)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where (int)waybill.Type == type
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 根据运单执行状态搜索
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public CgSortingsView SearchByStatus(params CgSortingExcuteStatus[] status)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            //var includes = status.Select(item => (int)item);
            var linq = from waybill in waybillView
                       where status.Contains(waybill.ExcuteStatus)
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
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
        public CgSortingsView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;

            var waybillView = this.IQueryable.Cast<MyWaybill>();

            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgSortingsView(this.Reponsitory, waybillView);
            }

            var linq_waybillIDs = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  where notice.WareHouseID.StartsWith(this.wareHouseID)
                                  orderby notice.WaybillID descending
                                  select notice.WaybillID;

            var linq_ids = linq_waybillIDs.Distinct(); //.OrderByDescending(item => item).Take(500)

            var linq = from waybill in waybillView
                       join id in linq_ids on waybill.ID equals id
                       orderby waybill.ID descending
                       select waybill;

            var view = new CgSortingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        #endregion

        /// <summary>
        /// 返回SortingID
        /// </summary>
        /// <returns></returns>
        public string GetSortingID()
        {
            return PKeySigner.Pick(PkeyType.Sortings);
        }

        /// <summary>
        /// 根据订单ID获取 订单ID对应的报关申报项日志TinyOrderID, OrderItemID,
        /// 只针对代报关入库
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public object[] GetDeclareItemInfo(string orderID)
        {
            using (var reponsitory = new PvWmsRepository())
            {
                #region _bak

                //var linq1 = from waybill in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                //            join declare in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>()
                //            on waybill.wbID equals declare.WaybillID
                //            join declareitem in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>()
                //            on declare.TinyOrderID equals declareitem.TinyOrderID
                //            where waybill.OrderID == orderID
                //            select new
                //            {
                //                declareitem.TinyOrderID,
                //                declareitem.OrderItemID,
                //            };

                #endregion

                var linq = from storage in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                           join declareitem in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>() on storage.ID equals declareitem.StorageID
                           join input in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                            on storage.InputID equals input.ID
                           where input.OrderID == orderID
                           select new
                           {
                               declareitem.TinyOrderID,
                               declareitem.OrderItemID,
                           };

                return linq.ToArray();
            }
        }

        /// <summary>
        /// 根据StorageID去删除已经入库的分拣及库存
        /// </summary>
        /// <param name="storageID">storageID 库存ID</param>
        public void DeleteSorting(string storageID, string adminID)
        {
            var linq_declare = from declare in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>()
                               join declareitem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>() on declare.TinyOrderID equals declareitem.TinyOrderID
                               select new
                               {
                                   declare.TinyOrderID,
                                   declare.Status,
                                   declareitem.OrderItemID,
                                   declareitem.StorageID,
                                   declareitem.Quantity,
                                   declareitem.BoxCode,
                               };

            if (linq_declare.Any(item => item.StorageID == storageID && item.Status >= (int)TinyOrderDeclareStatus.Declaring))
            {
                throw new Exception($"库存，{storageID} 不能删除，该分拣入库的库存已经申报!");
            }

            List<ItemChangeNotice> itemChangeNoticeList = new List<ItemChangeNotice>();

            var realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminID).RealName;
            var storage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Single(item => item.ID == storageID);
            var product = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>().Single(item => item.ID == storage.ProductID);
            var sorting = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Single(item => item.ID == storage.SortingID);
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => item.ID == storageID);
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Sortings>(item => item.ID == storage.SortingID);
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Storage>(new
            {
                IsCurrent = false,
            }, item => item.IsCurrent == true && item.StorageID == storageID);

            //ok ,sl

            CgLogs_Operator logs_operator = new CgLogs_Operator();
            logs_operator.Conduct = "分拣";
            logs_operator.CreatorID = adminID;
            logs_operator.Type = LogOperatorType.Delete;
            logs_operator.CreateDate = DateTime.Now;
            logs_operator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 分拣 {LogOperatorType.Delete.GetDescription()} 型号: {product.PartNumber} 品牌: {product.Manufacturer} 产地: {storage.Origin} 数量: {storage.Quantity}";

            //this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(item => item.StorageID == storageID);

            // 获取StorageID对应的 TinyOrderID
            var inputChange = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Single(item => item.ID == storage.InputID);
            var tinyOrderID = inputChange.TinyOrderID;
            var itemID = inputChange.ItemID;
            var notice = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().SingleOrDefault(item => item.ID == sorting.NoticeID);

            if (notice != null)
            {
                var noticeProduct = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>().SingleOrDefault(item => item.ID == notice.ProductID);

                if (product.Manufacturer != noticeProduct.Manufacturer)
                {
                    itemChangeNoticeList.Add(new ItemChangeNotice
                    {
                        AdminID = adminID,
                        CreateDate = DateTime.Now,
                        ItemID = itemID,
                        TinyOrderID = tinyOrderID,
                        OldValue = product.Manufacturer,
                        NewValue = noticeProduct.Manufacturer,
                        Type = OrderItemChangeType.BrandChange,
                    });
                }

                if (storage.Origin != notice.Origin)
                {
                    itemChangeNoticeList.Add(new ItemChangeNotice
                    {
                        AdminID = adminID,
                        CreateDate = DateTime.Now,
                        ItemID = itemID,
                        TinyOrderID = tinyOrderID,
                        OldValue = storage.Origin,
                        NewValue = notice.Origin,
                        Type = OrderItemChangeType.OriginChange,
                    });
                }

                /* 删除分拣时先不要发产品变更通知
                #region 产品变更通知
                //#if DEBUG
                if (itemChangeNoticeList.Count() > 0)
                {
                    string url = Wms.Services.FromType.ItemChangeNotice.GetDescription();
                    var result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { Notices = itemChangeNoticeList });
                    var message = result.JsonTo<JMessage>();
                    if (message.code != 200)
                    {
                        throw new Exception("通知华芯通产品变更通知失败:" + message.data);
                    }
                }
                //#endif
                #endregion
                */
            }


            // 把TinyOrderID对应的的申报日志，申报项日志全部删除.
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(item => item.TinyOrderID == tinyOrderID && item.StorageID == storageID);
            //this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_Declare>(item => item.TinyOrderID == tinyOrderID);

            //删除箱号
            CgBoxManage.Current.Delete(sorting.BoxCode);

            //if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Any(item => item.ID == sorting.BoxCode || item.Series == sorting.BoxCode))
            //{
            //    if (!this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.BoxCode == sorting.BoxCode) &&
            //        !this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.BoxCode == sorting.BoxCode))
            //    {
            //        this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Boxes>(item => item.ID == sorting.BoxCode || item.Series == sorting.BoxCode);
            //    }
            //}

            var waybill = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                .Select(item => new
                {
                    item.wbID,
                    item.wbFatherID
                })
                .Single(item => item.wbID == sorting.WaybillID);
            string fatherID = string.IsNullOrEmpty(waybill.wbFatherID) ? waybill.wbID : waybill.wbFatherID;

            logs_operator.MainID = fatherID;
            logs_operator.Enter();

            // 删除入库分拣后，后续的对订单，运单状态的处理
            CgInNoticesView.UpdateNoticeHandle(fatherID);
        }


        /// <summary>
        /// 修改到货历史及封箱页面中的箱号
        /// </summary>
        /// <param name="storageID"></param>
        /// <param name="boxcode"></param>
        public void ModifyBoxCode(string storageID, string boxcode, string adminID)
        {
            // sorting picking  declareitem
            var storage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Single(item => item.ID == storageID);
            var sorting = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Single(item => item.ID == storage.SortingID);

            // 获取到货历史中的视图记录            
            var sortingView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgInputReportTopView>().Single(item => item.SortingID == sorting.ID);

            if (sortingView.DelcareStatus.HasValue && sortingView.DelcareStatus.Value >= 30)
            {
                throw new Exception($"库存{storageID}的箱号不能修改, 该分拣入库的库存已经申报!");
            }

            // 更新分拣中的箱号
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
            {
                BoxCode = boxcode,
            }, item => item.ID == sorting.ID);

            var storagelogs = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Storage>().SingleOrDefault(item => item.StorageID == storageID && item.IsCurrent == true);

            if (storagelogs != null)
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Storage>(new
                {
                    IsCurrent = false,
                }, item => item.StorageID == storageID && item.IsCurrent == true);

                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Storage
                {
                    ID = Guid.NewGuid().ToString(),
                    AdminID = adminID,
                    BoxCode = boxcode,
                    CreateDate = DateTime.Now,
                    IsCurrent = true,
                    StorageID = storageID,
                    Summary = null,
                    Weight = storagelogs.Weight,
                });
            }

            // 如果申报项日志中有, 则更新BoxCode
            if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>().Any(item => item.StorageID == storage.ID))
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
                {
                    BoxCode = boxcode,
                }, item => item.StorageID == storage.ID);
            }
        }

        /// <summary>
        /// 修改封箱页面中的到货数量修改，
        /// 该功能暂时取消，不再需要
        /// </summary>
        /// <param name="storageID"></param>
        /// <param name="boxcode"></param>
        /// <param name="quantity"></param>
        public void ModifyQuantity(string storageID, string adminID, decimal quantity)
        {
            var storagelogs = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Storage>().SingleOrDefault(item => item.ID == storageID && item.IsCurrent == true);
            var sortingID = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Single(item => item.ID == storageID).SortingID;

            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
            {
                Quantity = quantity,
            }, item => item.ID == sortingID);

            if (storagelogs != null)
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                {
                    Quantity = quantity,
                }, item => item.ID == storageID);

                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Storage>(new
                {
                    IsCurrent = false,
                }, item => item.StorageID == storageID && item.IsCurrent == true);

                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Storage
                {
                    ID = Guid.NewGuid().ToString(),
                    AdminID = adminID,
                    BoxCode = storagelogs.BoxCode,
                    CreateDate = DateTime.Now,
                    IsCurrent = true,
                    StorageID = storageID,
                    Summary = null,
                    Weight = storagelogs.Weight,
                });
            }
        }

        //根据林团裕和荣检要求:即使装箱或者封箱后仍然可以继续填写或者修改运单号和承运商
        public void ModifyWbCodeAndCarrierID(string waybillID, string wbCode, string carrierID, int type)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    CarrierID = carrierID,
                    Code = wbCode,
                    Type = type,
                }, item => item.ID == waybillID);
            }
        }

        #region Helper Class
        /// <summary>
        /// 符合Sorting视图头部定义的内部类
        /// </summary>
        private class MyWaybill
        {
            public string ID { get; set; }
            public DateTime CreateDate { get; set; }
            public DateTime? ModifyDate { get; set; }
            public string EnterCode { get; set; }
            public string ClientName { get; set; }
            public int? ChargeWH { get; set; }
            public string Supplier { get; set; }
            public string SupplierName { get; set; }
            public int? SupplierGrade { get; set; }
            public Yahv.Underly.CgSortingExcuteStatus ExcuteStatus { get; set; }
            public Yahv.Underly.WaybillType Type { get; set; }
            public string Code { get; set; }

            public string CarrierID { get; set; }
            public string CarrierName { get; set; }
            public string ConsignorID { get; set; }
            public string ConsignorPlace { get; set; }
            public WayCondition Condition { get; set; }
            public Origin Place { get; set; }
            public string OrderID { get; set; }
            public CgNoticeSource Source { get; set; }
            public CgNoticeType NoticeType { get; set; }
            public DateTime? AppointTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 转运使用
            /// </remarks>
            public string TransferID { get; set; }
            public string Driver { get; set; }
            public string CarNumber1 { get; set; }

            public CgLoadingExcuteStauts? LoadingExcuteStatus { get; set; }

            /// <summary>
            /// 自提时间
            /// </summary>
            public DateTime? TakingDate { get; set; }

            /// <summary>
            /// 客服人员名称
            /// </summary>
            public string Merchandiser { get; set; }

            /// <summary>
            /// 自提地址
            /// </summary>
            public string TakingAddress { get; set; }

            /// <summary>
            /// 自提联系人
            /// </summary>
            public string TakingContact { get; set; }

            /// <summary>
            /// 自提联系电话
            /// </summary>
            public string TakingPhone { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 经过商议暂时增加，等与库房前端对通后，我们可以没有这个传递与处理了。
            /// </remarks>
            public string[] Files { get; set; }

            /// <summary>
            /// 是否可操作
            /// </summary>
            public bool? Operated { get; set; }
        }

        private class MyInput
        {
            public string ID { get; set; }
            public string Code { get; set; }
            public string OriginID { get; set; }
            public string OrderID { get; set; }
            public string TinyOrderID { get; set; }
            public string ItemID { get; set; }
            public string ProductID { get; set; }
            public string ClientID { get; set; }
            public string PayeeID { get; set; }
            public string ThirdID { get; set; }
            public string TrackerID { get; set; }
            public string SalerID { get; set; }
            public string PurchaserID { get; set; }
            public int? Currency { get; set; }
            public decimal? UnitPrice { get; set; }
            public DateTime CreateDate { get; set; }
        }

        private class ItemChangeNotice
        {
            /// <summary>
            /// 订单项ID
            /// </summary>
            public string ItemID { get; set; }

            /// <summary>
            /// 小订单ID
            /// </summary>
            public string TinyOrderID { get; set; }

            /// <summary>
            /// 库房分拣员(变更产地/品牌)
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 旧值
            /// </summary>
            public string OldValue { get; set; }

            /// <summary>
            /// 新值
            /// </summary>
            public string NewValue { get; set; }

            public OrderItemChangeType Type { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateDate { get; set; }
        }

        /// <summary>
        /// SplitOrderItemNotice参数
        /// </summary>
        private class SplitOrderItemNotice
        {
            /// <summary>
            /// 进项InputID
            /// </summary>
            public string InputID { get; set; }
            /// <summary>
            /// 辅助 SortingID
            /// </summary>
            public string SortingID { get; set; }
            /// <summary>
            /// 订单项ID (OrderItemID)
            /// </summary>
            public string ItemID { get; set; }

            /// <summary>
            /// 小订单ID
            /// </summary>
            public string TinyOrderID { get; set; }

            /// <summary>
            /// 库房分拣元
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 新拆出的品牌
            /// </summary>
            public string Brand { get; set; }

            /// <summary>
            /// 新拆出的产地
            /// </summary>
            public string Origin { get; set; }

            /// <summary>
            /// 新拆出的数量
            /// </summary>
            public decimal Qty { get; set; }
        }

        /// <summary>
        /// SplitChangeRequest参数
        /// </summary>
        private class SplitChangeRequest
        {
            /// <summary>
            /// 所有的拆项信息
            /// </summary>
            public List<SplitOrderItemNotice> Notices { get; set; }
        }

        public enum OrderItemChangeType
        {
            [Description("")]
            None = 0,

            [Description("产地变更")]
            OriginChange = 1,

            [Description("品牌变更")]
            BrandChange = 2,

            [Description("型号变更")]
            ProductModelChange = 3,

            [Description("批次变更")]
            BatchChange = 4,

            [Description("海关编码变更")]
            HSCodeChange = 5,

            [Description("报关品名变更")]
            TariffNameChange = 6,

            [Description("封装变更")]
            PackageCaseChange = 7,
        }

        private class MySorting
        {
            public string SortingID { get; set; }
            public string NoticeID { get; set; }
            public string InputID { get; set; }
            public int Quantity { get; set; }
            public string Origin { get; set; }

            public MyProduct Product { get; set; }

            public string Summary { get; set; }
        }

        private class MyProduct
        {
            public string PartNumber { get; set; }

            public string Manufacturer { get; set; }

            public string PackageCase { get; set; }

            public string Packaging { get; set; }
        }
        #endregion
    }
}