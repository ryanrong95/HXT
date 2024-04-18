using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CgSzStoragesTopView : UniqueView<CgSzStoragesTopModel, ScCustomsReponsitory>
    {
        protected override IQueryable<CgSzStoragesTopModel> GetIQueryable()
        {
            var linq = from sort in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgSzStoragesTopView>()
                       where sort.Type == 200
                       select new CgSzStoragesTopModel
                       {
                           ID = sort.ID,
                           OrderID = sort.OrderID,
                           TinyOrderID = sort.TinyOrderID,
                           ItemID = sort.ItemID,
                           WareHouseID = sort.WareHouseID,
                           InputID = sort.InputID,
                           Quantity = sort.Quantity,
                           Origin = sort.Origin,
                           BoxCode = sort.BoxCode,
                           DateBoxCode = sort.DateBoxCode,
                           DateCode = sort.DateCode,
                           Volume = sort.Volume,
                           UnitPrice = sort.UnitPrice,
                           PartNumber = sort.PartNumber,
                           Manufacturer = sort.Manufacturer,
                           ProductID = sort.ProductID,
                           Weight = sort.Weight,
                           NetWeight = sort.NetWeight,
                           StorageID = sort.StorageID,
                       };

            return linq;
        }
    }

    public class CgSzStoragesTopModel : IUnique
    {
        public string ID { get; set; }

        public string OrderID { get; set; }

        public string TinyOrderID { get; set; }

        public string PartNumber { get; set; }

        public string Manufacturer { get; set; }

        public string ItemID { get; set; }

        public string BoxCode { get; set; }

        public string DateBoxCode { get; set; }

        public decimal? Weight { get; set; }

        public decimal? NetWeight { get; set; }

        public decimal Quantity { get; set; }

        public string DateCode { get; set; }

        public string WareHouseID { get; set; }

        public string InputID { get; set; }

        public string Origin { get; set; }

        public string StorageID { get; set; }

        public decimal? UnitPrice { get; set; }

        public string ProductID { get; set; }

        public decimal? Volume { get; set; }
    }
}
