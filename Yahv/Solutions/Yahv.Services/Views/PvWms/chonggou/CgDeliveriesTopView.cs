using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class CgDeliveriesTopView<TReponsitory> : UniqueView<_Storage, TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CgDeliveriesTopView()
        {
        }

        public CgDeliveriesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<_Storage> GetIQueryable()
        {
            //throw new NotImplementedException();
            return from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgDeliveriesTopView>()
                   select new _Storage
                   {
                       ID = storage.ID,
                       Type = (StoragesType)storage.Type,
                       InputID = storage.InputID,
                       Origin = storage.Origin,
                       DateCode = storage.DateCode,
                       Supplier = storage.Supplier,
                       ProductID = storage.ProductID,
                       Quantity = storage.Quantity,
                       Total = storage.Total,
                       Summary = storage.Summary,
                       ShelveID = storage.ShelveID,
                       SortingID = storage.SortingID,

                       Input = new _Input
                       {
                           ID = storage.InputID,
                           OrderID = storage.OrderID,
                           TinyOrderID = storage.TinyOrderID,
                           ItemID = storage.ItemID,
                           ProductID = storage.ProductID,
                           UnitPrice = storage.UnitPrice,
                           Currency = storage.Currency==null?Currency.Unknown: (Currency)storage.Currency
                       },
                       Product = new CenterProduct
                       {
                           ID = storage.ProductID,
                           PartNumber = storage.PartNumber,
                           Manufacturer = storage.Manufacturer,
                           PackageCase = storage.PackageCase,
                           Packaging = storage.Packaging,
                       },
                   };
        }
    }
}
