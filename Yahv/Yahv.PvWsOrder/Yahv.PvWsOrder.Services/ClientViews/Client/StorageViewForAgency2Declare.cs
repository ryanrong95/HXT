using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class StorageViewForAgency2Declare : UniqueView<StorageExtend, PvWsOrderReponsitory>
    {
        private string ClientID;

        public StorageViewForAgency2Declare()
        {

        }



        protected override IQueryable<StorageExtend> GetIQueryable()
        {
            var storages = new StoragesTopView<PvWsOrderReponsitory>(this.Reponsitory).Where(item => item.Quantity > 0)
                .Where(item => item.WareHouseID.StartsWith("HK"));
            return from storage in storages
                   orderby storage.EnterDate descending
                   select new StorageExtend
                   {
                       ID = storage.ID,
                       InputID = storage.InputID,
                       Input = new _Input
                       {
                           ID = storage.InputID,
                           OrderID = storage.OrderID,
                           ItemID = storage.ItemID,
                           ProductID = storage.ProductID,
                           Currency = (Currency?)storage.Currency,
                           UnitPrice = storage.UnitPrice,
                           DateCode = storage.DateCode,
                       },
                       ProductID = storage.ProductID,
                       Product = new CenterProduct
                       {
                           ID = storage.ProductID,
                           PartNumber = storage.PartNumber,
                           CustomsName = storage.CustomsName,
                           Manufacturer = storage.Manufacturer,
                           PackageCase = storage.PackageCase,
                           Packaging = storage.Packaging,
                       },
                       Quantity = storage.Quantity.GetValueOrDefault(),
                       Supplier = storage.Supplier,
                       DateCode = storage.DateCode,
                       WareHouseID = storage.WareHouseID,
                       Origin = storage.Origin,
                       CreateDate = storage.EnterDate,
                   };
        }
    }
}
