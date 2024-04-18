using Layers.Data.Sqls;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyStorageView : UniqueView<StorageExtend, PvWsOrderReponsitory>
    {
        private string ClientID;

        public MyStorageView()
        {

        }

        public MyStorageView(string EnterpriseID)
        {
            this.ClientID = EnterpriseID;
        }


        protected override IQueryable<StorageExtend> GetIQueryable()
        {
            var storages = new StoragesTopView<PvWsOrderReponsitory>(this.Reponsitory).Where(item => item.Quantity > 0 && item.ClientID == this.ClientID)
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

        public List<StorageListModel> GetStorageList(string clientName)
        {
            var storages = new StoragesTopView<PvWsOrderReponsitory>(this.Reponsitory)
                .Where(item => item.WareHouseID.StartsWith("HK") && item.ClientName == clientName);

            var result = storages.Select(item => new StorageListModel
            {
                PartNumber = item.PartNumber,
                Manufacturer = item.Manufacturer,
                Quantity = item.Quantity.GetValueOrDefault(),
                DateCode = item.DateCode ?? "",
                PackageCase = item.PackageCase ?? "",
            }).ToList();

            return result;
        }
    }
}
