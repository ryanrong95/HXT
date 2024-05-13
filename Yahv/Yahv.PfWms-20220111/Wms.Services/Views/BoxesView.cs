using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Wms.Services.Views
{
    public class BoxesView : UniqueView<Models.Boxes, PvWmsRepository>
    {
        public BoxesView()
        {

        }

        protected override IQueryable<Boxes> GetIQueryable()
        {
            throw new Exception();
            //return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()                   
            //       select new Models.Boxes
            //       {
            //           ID = entity.ID,
            //           AdminID = entity.AdminID,
            //           CreateDate = entity.CreateDate,
            //           Code = entity.Code,
            //           Status = (BoxesStatus)entity.Status,
            //           Summary = entity.Summary,
            //           WarehouseID = entity.WarehouseID
            //       };
        }
    }


    public class CustomPickingNoticeView : QueryView<CustomPickingNotice, PvWmsRepository>
    {
        public CustomPickingNoticeView()
        {

        }
        protected override IQueryable<CustomPickingNotice> GetIQueryable()
        {
            return from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                   join output in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                   on notice.OutputID equals output.ID
                   join product in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
                   select new CustomPickingNotice
                   {
                       ID = notice.ID,
                       Type = (Yahv.Services.Enums.CgNoticeType)notice.Type,
                       WareHouseID = notice.WareHouseID,
                       WaybillID = notice.WaybillID,
                       InputID = notice.InputID,
                       OutputID = notice.OutputID,
                       ProductID = notice.ProductID,
                       Supplier = notice.Supplier,
                       DateCode = notice.DateCode,
                       Quantity = notice.Quantity,
                       Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
                       CreateDate = notice.CreateDate,
                       Status = (NoticesStatus)notice.Status,
                       Source = (CgNoticeSource)notice.Source,
                       Target = (NoticesTarget)notice.Target,
                       BoxCode = notice.BoxCode,
                       Weight = notice.Weight,
                       NetWeight = notice.NetWeight,
                       //ExcuteStatus = (PickingExcuteStatus)notice.Status,
                       Product = new CenterProduct
                       {
                           ID = product.ID,
                           PartNumber = product.PartNumber,
                           Manufacturer = product.Manufacturer,
                           PackageCase = product.PackageCase,
                           Packaging = product.Packaging,
                           CreateDate = product.CreateDate
                       },
                       Output = new Models.Output
                       {
                           ID = output.ID,
                           InputID = output.InputID,
                           OrderID = output.OrderID,
                           TinyOrderID = output.TinyOrderID,
                           ItemID = output.ItemID,
                           OwnerID = output.OwnerID,
                           SalerID = output.SalerID,
                           CustomerServiceID = output.CustomerServiceID,
                           PurchaserID = output.PurchaserID,
                           Currency = (Currency)output.Currency,
                           Price = output.Price,
                           CreateDate = output.CreateDate,
                       }
                   };
        }
    }
}
