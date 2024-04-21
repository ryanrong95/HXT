using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Wms.Services.Views
{
    public class SortingNoticesView : QueryView<Yahv.Services.Models.SortingNotice, PvWmsRepository>
    {
        public SortingNoticesView()
        {

        }
        public SortingNoticesView(PvWmsRepository repository) : base(repository)
        {

        }
        protected override IQueryable<Yahv.Services.Models.SortingNotice> GetIQueryable()
        {
            // 现有的Notice与Storages，通过NoticeID关联错误
            throw new Exception("现有的Notice与Storages，通过NoticeID关联错误");
            //return from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
            //       join input in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
            //       join sorting in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on notice.ID equals sorting.NoticeID
            //       join product in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
            //       join box in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Boxes>() on
            //       notice.BoxCode equals box.Code
            //       join storage in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on
            //       notice.ID equals storage.NoticeID
            //       select new SortingNotice
            //       {
            //           ID = notice.ID,
            //           Type = (CgNoticeType)notice.Type,
            //           WareHouseID = notice.WareHouseID,
            //           WaybillID = notice.WaybillID,
            //           InputID = notice.InputID,
            //           OutputID = notice.OutputID,
            //           ProductID = notice.ProductID,
            //           Supplier = notice.Supplier,
            //           DateCode = notice.DateCode,
            //           Quantity = notice.Quantity,
            //           Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
            //           CreateDate = notice.CreateDate,
            //           Status = (NoticesStatus)notice.Status,
            //           Source = (NoticeSource)notice.Source,
            //           Target = (NoticesTarget)notice.Target,
            //           BoxCode = notice.BoxCode,
            //           BoxDate = box.CreateDate,
            //           Weight = notice.Weight,
            //           NetWeight = notice.NetWeight,
            //           Origin = notice.Origin,
            //           Product = new CenterProduct
            //           {
            //               ID = product.ID,
            //               PartNumber = product.PartNumber,
            //               Manufacturer = product.Manufacturer,
            //               PackageCase = product.PackageCase,
            //               Packaging = product.Packaging,
            //               CreateDate = product.CreateDate
            //           },
            //           Input = new Input
            //           {
            //               ID = input.ID,
            //               Code = input.Code,
            //               OriginID = input.OriginID,
            //               OrderID = input.OrderID,
            //               TinyOrderID = input.TinyOrderID,
            //               ItemID = input.ItemID,
            //               ProductID = input.ProductID,
            //               ClientID = input.ClientID,
            //               PayeeID = input.PayeeID,
            //               ThirdID = input.ThirdID,
            //               TrackerID = input.TrackerID,
            //               SalerID = input.SalerID,
            //               PurchaserID = input.PurchaserID,
            //               Currency = (Currency)input.Currency,
            //               UnitPrice = input.UnitPrice,
            //           },
            //           Sorting = new Sorting
            //           {
            //               ID = sorting.ID,
            //               NoticeID = sorting.NoticeID,
            //               WaybillID = sorting.WaybillID,
            //               BoxCode = sorting.BoxCode,
            //               Quantity = sorting.Quantity,
            //               AdminID = sorting.AdminID,
            //               CreateDate = sorting.CreateDate,
            //               Weight = sorting.Weight,
            //               NetWeight = sorting.NetWeight,
            //               Volume = sorting.Volume,
            //           },
            //           Storage = new Storage
            //           {
            //               ID = storage.ID,
            //               Type = (StoragesType)storage.Type,
            //               WareHouseID = storage.WareHouseID,
            //               SortingID = storage.SortingID,
            //               InputID = storage.InputID,
            //               ProductID = storage.ProductID,
            //               Quantity = storage.Quantity,
            //               Origin = storage.Origin,
            //               IsLock = storage.IsLock,
            //               CreateDate = storage.CreateDate,
            //               Status = (GeneralStatus)storage.Status,
            //               ShelveID = storage.ShelveID,
            //               Supplier = storage.Supplier,
            //               DateCode = storage.DateCode,
            //           }

            //       };
        }
    }
}
