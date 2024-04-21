//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Models;
//using Yahv.Linq;
//using Yahv.Underly;

//namespace Wms.Services.Views
//{
//    public class StoragesView : UniqueView<Models.Storage, PvWmsRepository>
//    {
//        public StoragesView()
//        {

//        }
//        protected override IQueryable<Storage> GetIQueryable()
//        {
//           // var productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

//            return from storage in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.StoragesTopView>()                 
                  
//                   select new Models.Storage
//                   {
//                       ID = storage.ID,
//                       Type = (StoragesType)storage.Type,
//                       WareHouseID = storage.WareHouseID,
//                       SortingID = storage.SortingID,
//                       InputID = storage.InputID,
//                       ProductID = storage.ProductID,
//                       Quantity = storage.Quantity ?? 0,
//                       //NoticeID = storage.NoticeID,
//                       Place = storage.Origin/*(Origin)(int.Parse(storage.Origin ?? null))*/,//原产地
//                       IsLock = storage.IsLock,
//                       CreateDate = storage.CreateDate,
//                       Status = (Enums.StoragesStatus)storage.Status,
//                       ShelveID = storage.ShelveID,
//                       Supplier = storage.Supplier,
//                       DateCode = storage.DateCode,
//                       Summary = storage.Summary,
//                       Product = new Yahv.Services.Models.CenterProduct {
//                           ID=storage.ProductID,
//                           PartNumber=storage.PartNumber,
//                           PackageCase=storage.PackageCase,
//                           Manufacturer=storage.Manufacturer,
//                           Packaging=storage.Packaging
//                       },
//                       Input = new Yahv.Services.Models.Input
//                       {
//                           ID = storage.InputID,
//                           Code = storage.Code,
//                           OriginID = storage.OriginID,
//                           OrderID = storage.OrderID,
//                           TinyOrderID= storage.TinyOrderID,
//                           ItemID = storage.ItemID,
//                           ProductID = storage.ProductID,
//                           ClientID = storage.ClientID,
//                           PayeeID = storage.PayeeID,
//                           ThirdID = storage.ThirdID,
//                           TrackerID = storage.TrackerID,
//                           SalerID = storage.SalerID,
//                           PurchaserID = storage.PurchaserID,
//                           Currency = (Currency)storage.Currency,
//                           UnitPrice = storage.UnitPrice,
//                           DateCode = storage.DateCode,
//                           Origin = storage.Origin,
//                           CreateDate = storage.CreateDate,
//                       }

//                   };
//        }
//    }
//}
