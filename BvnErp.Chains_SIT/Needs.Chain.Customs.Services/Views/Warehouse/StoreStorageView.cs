using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 库存库
    /// </summary>
    public class StoreStorageView : UniqueView<Models.StoreStorage, ScCustomsReponsitory>
    {
        public StoreStorageView()
        {
        }

        internal StoreStorageView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.StoreStorage> GetIQueryable()
        {
            var sortingView = new SortingsView(this.Reponsitory);
            var clientsView = new ClientsView(this.Reponsitory);

            return from storage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>()
                   join sorting in sortingView on storage.SortingID equals sorting.ID
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on sorting.OrderID equals order.ID //20180408
                   join client in clientsView on order.ClientID equals client.ID
                   where storage.Status == (int)Enums.Status.Normal
                   select new Models.StoreStorage
                   {
                       ID = storage.ID,
                       OrderItemID = storage.OrderItemID,
                       Sorting = sorting,
                       //Product = sorting.Product,
                       Quantity = sorting.Quantity,
                       Purpose = (Enums.StockPurpose)storage.Purpose,
                       StockCode = storage.StockCode,
                       BoxIndex = storage.BoxIndex,
                       CreateDate = storage.CreateDate,
                       UpdateDate = storage.UpdateDate,
                       Status = (Enums.Status)storage.Status,
                       Summary = storage.Summary,
                       OrderItem=sorting.OrderItem,
                       Order= new Models.Order {
                           ID=order.ID,
                           Client = client,
                           Currency=order.Currency
                       }
                   };
        }
    }

    /// <summary>
    /// 香港库存库
    /// </summary>
    public class HKStoreStorageView : UniqueView<Models.HKStoreStorage, ScCustomsReponsitory>
    {
        public HKStoreStorageView()
        {
        }

        internal HKStoreStorageView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.HKStoreStorage> GetIQueryable()
        {
            var storeStorage = new StoreStorageView(this.Reponsitory)
                .Where(item => item.Sorting.WarehouseType == Enums.WarehouseType.HongKong);
            return from storage in storeStorage
                   select new Models.HKStoreStorage
                   {
                       ID = storage.ID,
                       OrderItemID = storage.OrderItemID,
                       Sorting = storage.Sorting,
                       OrderItem=storage.OrderItem,
                       //Product = storage.Product,
                       Quantity = storage.Quantity,
                       Purpose = storage.Purpose,
                       StockCode = storage.StockCode,
                       BoxIndex = storage.BoxIndex,
                       CreateDate = storage.CreateDate,
                       UpdateDate = storage.UpdateDate,
                       Status = storage.Status,
                       Summary = storage.Summary,
                       Order=storage.Order
                   };
        }
    }

    /// <summary>
    /// 深圳库存库
    /// </summary>
    public class SZStoreStorageView : UniqueView<Models.SZStoreStorage, ScCustomsReponsitory>
    {
        public SZStoreStorageView()
        {
        }

        internal SZStoreStorageView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZStoreStorage> GetIQueryable()
        {
            var storeStorage = new StoreStorageView(this.Reponsitory);
            var sortingView = new SZSortingsView(this.Reponsitory);

            return from storage in storeStorage
                   join sorting in sortingView on storage.Sorting.ID equals sorting.ID
                   select new Models.SZStoreStorage
                   {
                       ID = storage.ID,
                       OrderItemID = storage.OrderItemID,
                       Sorting = storage.Sorting,
                       //Product = storage.Product,
                       Quantity = storage.Quantity,
                       Purpose = storage.Purpose,
                       StockCode = storage.StockCode,
                       BoxIndex = storage.BoxIndex,
                       CreateDate = storage.CreateDate,
                       UpdateDate = storage.UpdateDate,
                       Status = storage.Status,
                       Summary = storage.Summary,
                       SZSorting = sorting,
                       Order =storage.Order
                   };
        }
    }
}
