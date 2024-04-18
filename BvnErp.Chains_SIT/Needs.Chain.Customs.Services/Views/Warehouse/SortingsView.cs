
using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 分拣结果View
    /// </summary>
    public class SortingsView : UniqueView<Models.Sorting, ScCustomsReponsitory>
    {
        public SortingsView()
        {
        }

        internal SortingsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Sorting> GetIQueryable()
        {
            var itemView = new OrderItemsView(this.Reponsitory);
            //var StoreStorageView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>().Where(x => x.Status == (int)Enums.Status.Normal);
            var clientsView = new ClientsView(this.Reponsitory);
            return from sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>()
                   where sorting.Status == (int)Enums.Status.Normal
                   join item in itemView on sorting.OrderItemID equals item.ID
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on item.OrderID equals order.ID
                   join client in clientsView on order.ClientID equals client.ID
                   //join storage in StoreStorageView on sorting.ID equals storage.SortingID into storages
                   //from storage in storages.DefaultIfEmpty()
                   select new Models.Sorting
                   {
                       ID = sorting.ID,
                       AdminID = sorting.AdminID,
                       OrderID = sorting.OrderID,
                       OrderItem = item,
                       EntryNoticeItemID = sorting.EntryNoticeItemID,
                       WarehouseType = (Enums.WarehouseType)sorting.WarehouseType,
                       WrapType = sorting.WrapType,
                       //Product = item.Product,
                       Quantity = sorting.Quantity,
                       BoxIndex = sorting.BoxIndex,
                       NetWeight = sorting.NetWeight,
                       GrossWeight = sorting.GrossWeight,
                       DecStatus = (Enums.SortingDecStatus)sorting.DecStatus,
                       Status = (Enums.Status)sorting.Status,
                       CreateDate = sorting.CreateDate,
                       UpdateDate = sorting.UpdateDate,
                       Summary = sorting.Summary,
                       Order = new Order
                       {
                           ID = order.ID,
                           Currency = order.Currency,
                           Client = client,
                       },

                       SZPackingDate = sorting.SZPackingDate
                       //ClientCode = order.Client.ClientCode,
                       //Currency = order.Currency,
                       //StockCode = storage == null ? null : storage.StockCode,
                       //StorageBoxIndex = storage.BoxIndex
                   };
        }

        public IQueryable<SortingPacking> GetSortingPacking()
        {
            return from sorting in this.GetIQueryable()
                   join packingItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>() on sorting.ID equals packingItem.SortingID
                   join packing in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>() on packingItem.PackingID equals packing.ID
                   join orderItemCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on sorting.OrderItem.ID equals orderItemCategory.OrderItemID
                   where sorting.Status == Enums.Status.Normal
                   orderby sorting.BoxIndex
                   select new Models.SortingPacking
                   {
                       ID = sorting.ID,
                       Packing = new Packing
                       {
                           ID = packing.ID,
                           BoxIndex = packing.BoxIndex,
                           PackingDate = packing.PackingDate,
                           WrapType = packing.WrapType,
                           Weight = packing.Weight,
                           PackingStatus = (Enums.PackingStatus)packing.PackingStatus
                       },
                       AdminID = sorting.AdminID,
                       OrderID = sorting.OrderID,
                       OrderItem = sorting.OrderItem,
                       EntryNoticeItemID = sorting.EntryNoticeItemID,
                       //Product = sorting.Product,
                       Quantity = sorting.Quantity,
                       BoxIndex = sorting.BoxIndex,
                       NetWeight = sorting.NetWeight,
                       GrossWeight = sorting.GrossWeight,
                       DecStatus = (Enums.SortingDecStatus)sorting.DecStatus,
                       Status = (Enums.Status)sorting.Status,
                       CreateDate = sorting.CreateDate,
                       UpdateDate = sorting.UpdateDate,
                       Summary = sorting.Summary,

                   };
        }

        /// <summary>
        /// 从 DeliveriesTopView 视图获取分拣信息
        /// </summary>
        /// <param name="tinyOrderID"></param>
        /// <returns></returns>
        public List<SortingPackingFromDeliveriesTopViewModel> GetSortingPackingFromDeliveriesTopView(string tinyOrderID, string[] boxIndexs = null)
        {
            var deliveriesTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveriesTopView>();
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();

            var viewLinq = from delivery in deliveriesTopView

                           join orderItem in orderItems
                                 on new { OrderItemID = delivery.iptItemID, OrderItemDataStatus = (int)Enums.Status.Normal, }
                                 equals new { OrderItemID = orderItem.ID, OrderItemDataStatus = orderItem.Status, }
                                 into orderItems2
                           from orderItem in orderItems2.DefaultIfEmpty()

                           join orderItemCategory in orderItemCategories
                                 on new { OrderItemID = orderItem.ID, OrderItemCategoryDataStatus = (int)Enums.Status.Normal, }
                                 equals new { OrderItemID = orderItemCategory.OrderItemID, OrderItemCategoryDataStatus = orderItemCategory.Status, }
                                 into orderItemCategories2
                           from orderItemCategory in orderItemCategories2.DefaultIfEmpty()

                           where delivery.iptTinyOrderID == tinyOrderID
                           orderby delivery.sortBoxCode
                           select new SortingPackingFromDeliveriesTopViewModel
                           {
                               ID = delivery.stoSortingID,
                               SortingID = delivery.stoSortingID,
                               BoxIndex = delivery.sortBoxCode,
                               GrossWeight = delivery.sortWeight,
                               Model = delivery.ptvPartNumber,
                               CustomsName = orderItemCategory.Name,
                               Quantity = delivery.stoQuantity,
                               IptOrigin = delivery.iptOrigin,
                               Manufacturer = delivery.ptvManufacturer,
                               PackingDate = delivery.SortingDate,

                               OrderItemID = delivery.iptItemID,
                               InputID = delivery.stoInputID,
                               MainOrderID = delivery.iptOrderID,
                               TinyOrderID = delivery.iptTinyOrderID,
                           };

            if (boxIndexs != null)
            {
                viewLinq = viewLinq.Where(t => boxIndexs.Contains(t.BoxIndex));
            }

            var viewResults = viewLinq.ToList();

            //查询是否有报关通知
            var decNoticeDatas = (from decNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>()
                                  where decNotice.OrderID == tinyOrderID
                                  group decNotice by new { decNotice.OrderID } into g
                                  select new OrderPackingListModel
                                  {
                                      OrderID = g.Key.OrderID,
                                  }).ToList();

            //获得 OriginCode
            viewResults = viewResults.Select(t => new SortingPackingFromDeliveriesTopViewModel()
            {
                ID = t.ID,
                SortingID = t.SortingID,
                BoxIndex = t.BoxIndex,
                GrossWeight = t.GrossWeight,
                Model = t.Model,
                CustomsName = t.CustomsName,
                Quantity = t.Quantity,
                IptOrigin = t.IptOrigin,
                OriginCode = t.IptOrigin, //((Needs.Underly.Origin)Convert.ToInt32(t.IptOrigin)).GetOrigin().Code,
                Manufacturer = t.Manufacturer,
                DecStatus = (decNoticeDatas == null || decNoticeDatas.Count == 0) ? Enums.SortingDecStatus.No : Enums.SortingDecStatus.Yes,
                PackingStatus = (decNoticeDatas == null || decNoticeDatas.Count == 0) ? Enums.PackingStatus.UnSealed : Enums.PackingStatus.Sealed,
                PackingDate = t.PackingDate,

                OrderItemID = t.OrderItemID,
                InputID = t.InputID,
                MainOrderID = t.MainOrderID,
                TinyOrderID = t.TinyOrderID,
            }).ToList();

            return viewResults;
        }
    }

    /// <summary>
    /// 从 DeliveriesTopView 视图获取分拣信息 Model
    /// </summary>
    public class SortingPackingFromDeliveriesTopViewModel
    {
        /// <summary>
        /// SortingID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// SortingID
        /// </summary>
        public string SortingID { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 报关品名 ------------------- (另外查) -------------------
        /// </summary>
        public string CustomsName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string IptOrigin { get; set; }

        /// <summary>
        /// 原产地 Code ------------------- (另外查) -------------------
        /// </summary>
        public string OriginCode { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 香港仓库标记已报关 ------------------- (另外查) -------------------
        /// </summary>
        public Enums.SortingDecStatus DecStatus { get; set; }

        /// <summary>
        /// 装箱状态 ------------------- (另外查) -------------------
        /// </summary>
        public Enums.PackingStatus PackingStatus { get; set; }

        /// <summary>
        /// 装箱日期
        /// </summary>
        public DateTime? PackingDate { get; set; }



        /// <summary>
        /// OrderItemID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// InputID
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 大订单号
        /// </summary>
        public string MainOrderID { get; set; }

        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }
    }


    /// <summary>
    /// 香港分拣结果View
    /// </summary>
    public class HKSortingsView : UniqueView<Models.HKSorting, ScCustomsReponsitory>
    {
        public HKSortingsView()
        {
        }

        internal HKSortingsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.HKSorting> GetIQueryable()
        {
            var sortingView = new SortingsView(this.Reponsitory);

            var result = from sorting in sortingView
                         where sorting.WarehouseType == Enums.WarehouseType.HongKong
                         select new Models.HKSorting
                         {
                             ID = sorting.ID,
                             AdminID = sorting.AdminID,
                             OrderID = sorting.OrderID,
                             OrderItem = sorting.OrderItem,
                             EntryNoticeItemID = sorting.EntryNoticeItemID,
                             Quantity = sorting.Quantity,
                             BoxIndex = sorting.BoxIndex,
                             NetWeight = sorting.NetWeight,
                             GrossWeight = sorting.GrossWeight,
                             DecStatus = sorting.DecStatus,
                             Status = sorting.Status,
                             CreateDate = sorting.CreateDate,
                             UpdateDate = sorting.UpdateDate,
                             Summary = sorting.Summary,
                             Order = sorting.Order,
                             WrapType = sorting.WrapType,
                             //ClientCode = sorting.ClientCode,
                             //Currency = sorting.Currency,
                             //StockCode = sorting.StockCode,
                         };
            return result;
        }

    }

    /// <summary>
    /// 深圳分拣结果View
    /// </summary>
    public class SZSortingsView : UniqueView<Models.SZSorting, ScCustomsReponsitory>
    {
        public SZSortingsView()
        {
        }

        internal SZSortingsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZSorting> GetIQueryable()
        {
            var sortingView = new SortingsView(this.Reponsitory);
            var ItemView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>();
            var decListView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var result = from sorting in sortingView
                         join item in ItemView on sorting.EntryNoticeItemID equals item.ID
                         join declist in decListView on item.DecListID equals declist.ID
                         where sorting.WarehouseType == Enums.WarehouseType.ShenZhen
                         select new Models.SZSorting
                         {
                             ID = sorting.ID,
                             AdminID = sorting.AdminID,
                             OrderID = sorting.OrderID,
                             OrderItem = sorting.OrderItem,
                             EntryNoticeItemID = sorting.EntryNoticeItemID,
                             //Product = sorting.Product,
                             Quantity = sorting.Quantity,
                             BoxIndex = sorting.BoxIndex,
                             NetWeight = sorting.NetWeight,
                             GrossWeight = sorting.GrossWeight,
                             DecStatus = sorting.DecStatus,
                             Status = sorting.Status,
                             CreateDate = sorting.CreateDate,
                             UpdateDate = sorting.UpdateDate,
                             Summary = sorting.Summary,

                             Order = sorting.Order,
                             Origin = declist.OriginCountry,
                             UnitPrice = declist.DeclPrice,
                             //20190420
                             SZPackingDate = sorting.SZPackingDate
                         };
            return result;
        }

    }

    public class SortingPackingsView : UniqueView<Models.SortingPacking, ScCustomsReponsitory>
    {
        public SortingPackingsView()
        {
        }

        internal SortingPackingsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SortingPacking> GetIQueryable()
        {
            var sortingView = new Views.SortingsView(this.Reponsitory);
            //var itemView = new Views.OrderItemsView(this.Reponsitory);
            return from sorting in sortingView
                   join packingItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>() on sorting.ID equals packingItem.SortingID
                   join packing in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>() on packingItem.PackingID equals packing.ID
                   where sorting.Status == Enums.Status.Normal
                   orderby sorting.BoxIndex
                   select new Models.SortingPacking
                   {
                       ID = sorting.ID,
                       Packing = new Packing
                       {
                           ID = packing.ID,
                           BoxIndex = packing.BoxIndex,
                           PackingDate = packing.PackingDate,
                           WrapType = packing.WrapType,
                           Weight = packing.Weight,
                           PackingStatus = (Enums.PackingStatus)packing.PackingStatus
                       },
                       AdminID = sorting.AdminID,
                       OrderID = sorting.OrderID,
                       OrderItem = sorting.OrderItem,
                       EntryNoticeItemID = sorting.EntryNoticeItemID,
                       //Product = sorting.Product,
                       Quantity = sorting.Quantity,
                       BoxIndex = sorting.BoxIndex,
                       NetWeight = sorting.NetWeight,
                       GrossWeight = sorting.GrossWeight,
                       DecStatus = (Enums.SortingDecStatus)sorting.DecStatus,
                       Status = (Enums.Status)sorting.Status,
                       CreateDate = sorting.CreateDate,
                       UpdateDate = sorting.UpdateDate,
                       Summary = sorting.Summary,
                   };
        }
    }
}