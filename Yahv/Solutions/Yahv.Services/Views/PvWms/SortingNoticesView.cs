//using Layers.Data.Sqls;
//using Layers.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Linq;
//using Yahv.Services.Enums;
//using Yahv.Services.Models;
//using Yahv.Utils.Serializers;

//namespace Yahv.Services.Views
//{

//    public class SortingNoticesView<TReponsitory> : QueryView<SortingNotice, TReponsitory>
//            where TReponsitory : class, IReponsitory, IDisposable, new()
//    {

//        public SortingNoticesView()
//        {
             
//        }

//        public SortingNoticesView(TReponsitory reponsitory) : base(reponsitory)
//        {

//        }

//        protected override IQueryable<SortingNotice> GetIQueryable()
//        {
//            #region 方式一
//            return from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.SortingNoticesView>()
//                   select new SortingNotice
//                   {
//                       ID = notice.ntID,
//                       Type = (Enums.CgNoticeType)notice.ntType,
//                       WareHouseID = notice.ntWarehouseID,
//                       WaybillID = notice.ntWaybillID,
//                       InputID = notice.ntInputID,
//                       OutputID = notice.ntOutputID,
//                       ProductID = notice.ntProductID,
//                       Supplier = notice.ntSupplier,
//                       DateCode = notice.ntDateCode,
//                       Quantity = notice.ntQuantity,
//                       Conditions = notice.ntConditions.JsonTo<NoticeCondition>(),
//                       CreateDate = notice.ntCreateDate,
//                       Status = (Enums.NoticesStatus)notice.ntStatus,
//                       Source = (Enums.CgNoticeSource)notice.ntSource,
//                       Target = (Enums.NoticesTarget)notice.ntTarget,
//                       BoxCode = notice.BoxCode,
//                       Weight = notice.ntWeight,
//                       NetWeight = notice.ntNetWeight,
//                       Product = new CenterProduct
//                       {
//                           ID = notice.ptvID,
//                           PartNumber = notice.ptvPartNumber,
//                           Manufacturer = notice.ptvManufacturer,
//                           PackageCase = notice.ptvPackageCase,
//                           Packaging = notice.ptvPackaging,
//                           //CreateDate = notice.ptvCreateDate
//                       },
//                       Input = new Input
//                       {
//                           ID = notice.iptID,
//                           Code = notice.iptCode,
//                           OriginID = notice.iptOriginID,
//                           OrderID = notice.iptOrderID,
//                           TinyOrderID = notice.iptTinyOrderID,
//                           ItemID = notice.iptItemID,
//                           ProductID = notice.iptProductID,
//                           ClientID = notice.iptClientID,
//                           PayeeID = notice.iptPayeeID,
//                           ThirdID = notice.iptThirdID,
//                           TrackerID = notice.iptTrackerID,
//                           SalerID = notice.iptSalerID,
//                           PurchaserID = notice.iptPurchaserID,
//                           Currency = (Underly.Currency)notice.iptCurrency,
//                           UnitPrice = notice.iptUnitPrice,
//                           DateCode = notice.iptDateCode,
//                           Origin = notice.iptOrigin,
//                       },
//                       Sorting = new Sorting
//                       {
//                           ID = notice.sortID,
//                           NoticeID = notice.sortNoticeID,
//                           WaybillID = notice.sortWaybillID,
//                           BoxCode = notice.sortBoxCode,
//                           Quantity = notice.sortQuantity,
//                           AdminID = notice.sortAdminID,
//                           CreateDate = notice.sortCreateDate,
//                           Weight = notice.sortWeight,
//                           NetWeight = notice.sortNetWeight,
//                           Volume = notice.sortVolume,
//                       }
//                   };
//            #endregion

//            #region 方式二
//            //return from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
//            //       join input in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
//            //       join sorting in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on notice.ID equals sorting.NoticeID
//            //       join product in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
//            //       join box in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Boxes>() on
//            //       notice.BoxCode equals box.Code
//            //       join storage in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on
//            //       notice.ID equals storage.NoticeID
//            //       select new SortingNotice
//            //       {
//            //           ID = notice.ID,
//            //           Type = (Enums.NoticeType)notice.Type,
//            //           WareHouseID = notice.WareHouseID,
//            //           WaybillID = notice.WaybillID,
//            //           InputID = notice.InputID,
//            //           OutputID = notice.OutputID,
//            //           ProductID = notice.ProductID,
//            //           Supplier = notice.Supplier,
//            //           DateCode = notice.DateCode,
//            //           Quantity = notice.Quantity,
//            //           Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
//            //           CreateDate = notice.CreateDate,
//            //           Status = (Enums.NoticesStatus)notice.Status,
//            //           Source = (Enums.NoticeSource)notice.Source,
//            //           Target = (Enums.NoticesTarget)notice.Target,
//            //           BoxCode = notice.BoxCode,
//            //           BoxDate = box.CreateDate,
//            //           Weight = notice.Weight,
//            //           NetWeight = notice.NetWeight,
//            //           Product = new CenterProduct
//            //           {
//            //               ID = product.ID,
//            //               PartNumber = product.PartNumber,
//            //               Manufacturer = product.Manufacturer,
//            //               PackageCase = product.PackageCase,
//            //               Packaging = product.Packaging,
//            //               CreateDate = product.CreateDate
//            //           },
//            //           Input = new Input
//            //           {
//            //               ID = input.ID,
//            //               Code = input.Code,
//            //               OriginID = input.OriginID,
//            //               OrderID = input.OrderID,
//            //               TinyOrderID = input.TinyOrderID,
//            //               ItemID = input.ItemID,
//            //               ProductID = input.ProductID,
//            //               ClientID = input.ClientID,
//            //               PayeeID = input.PayeeID,
//            //               ThirdID = input.ThirdID,
//            //               TrackerID = input.TrackerID,
//            //               SalerID = input.SalerID,
//            //               PurchaserID = input.PurchaserID,
//            //               Currency = (Underly.Currency)input.Currency,
//            //               UnitPrice = input.UnitPrice,
//            //               DateCode = input.DateCode,
//            //               Origin = input.Origin,
//            //           },
//            //           Sorting = new Sorting
//            //           {
//            //               ID = sorting.ID,
//            //               NoticeID = sorting.NoticeID,
//            //               WaybillID = sorting.WaybillID,
//            //               BoxCode = sorting.BoxCode,
//            //               Quantity = sorting.Quantity,
//            //               AdminID = sorting.AdminID,
//            //               CreateDate = sorting.CreateDate,
//            //               Weight = sorting.Weight,
//            //               NetWeight = sorting.NetWeight,
//            //               Volume = sorting.Volume,
//            //           },
//            //           Storage = new Storage
//            //           {
//            //               ID = storage.ID,
//            //               Type = (Underly.StoragesType)storage.Type,
//            //               WareHouseID = storage.WareHouseID,
//            //               SortingID = storage.SortingID,
//            //               InputID = storage.InputID,
//            //               ProductID = storage.ProductID,
//            //               Quantity = storage.Quantity,
//            //               NoticeID = storage.NoticeID,
//            //               Origin = storage.Origin,
//            //               IsLock = storage.IsLock,
//            //               CreateDate = storage.CreateDate,
//            //               Status = (Underly.GeneralStatus)storage.Status,
//            //               ShelveID = storage.ShelveID,
//            //               Supplier = storage.Supplier,
//            //               DateCode = storage.DateCode,
//            //           }

//            //       };
//            #endregion
//        }
//    }
//}
