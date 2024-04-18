using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Utils.Serializers;

namespace Yahv.Services.Views
{
    //public class MyClass
    //{
    //    CenterProduct

    //}

    /// <summary>
    ///  出库报关的视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class PickingNoticesView<TReponsitory> : QueryView<PickingNotice, TReponsitory>
        where TReponsitory : class, IReponsitory, IDisposable, new()
    {

        public PickingNoticesView()
        {

        }

        public PickingNoticesView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PickingNotice> GetIQueryable()
        {
            return from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                   join output in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                   on notice.OutputID equals output.ID
                   join picking in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                    on notice.ID equals picking.NoticeID
                   join product in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
                   //group new { notice ,product,output,picking} by notice.BoxCode into notices
                   select new PickingNotice
                   {
                       ID = notice.ID,
                       Type = (Enums.CgNoticeType)notice.Type,
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
                       Status = (Enums.NoticesStatus)notice.Status,
                       Source = (Enums.CgNoticeSource)notice.Source,
                       Target = (Enums.NoticesTarget)notice.Target,
                       BoxCode = notice.BoxCode,
                       Weight = notice.Weight,
                       NetWeight = notice.NetWeight,
                       ExcuteStatus= (Underly.CgPickingExcuteStatus)notice.Status,
                       Product = new CenterProduct
                       {
                           ID = product.ID,
                           PartNumber = product.PartNumber,
                           Manufacturer = product.Manufacturer,
                           PackageCase = product.PackageCase,
                           Packaging = product.Packaging,
                           CreateDate = product.CreateDate
                       },
                       Output = new Output
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
                           Currency = (Underly.Currency)output.Currency,
                           Price = output.Price,
                           CreateDate = output.CreateDate,
                       },
                       Picking = new Picking
                       {
                           ID = picking.ID,
                           StorageID = picking.StorageID,
                           NoticeID = picking.NoticeID,
                           BoxCode = picking.BoxCode,
                           Quantity = picking.Quantity,
                           AdminID = picking.AdminID,
                           CreateDate = picking.CreateDate,
                           Weight = picking.Weight,
                           NetWeight = picking.NetWeight,
                           Volume = picking.Volume,
                       }
                   };
        }
    }
}
