using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 出库通知Items视图
    /// </summary>
    public class ExitNoticeItemView : UniqueView<Models.ExitNoticeItem, ScCustomsReponsitory>
    {
        public ExitNoticeItemView()
        {
        }

        internal ExitNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExitNoticeItem> GetIQueryable()
        {
            //var decListsView = new DecListsView(this.Reponsitory);

            return from Item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                       //join decList in decListsView on Item.DecListID equals decList.ID
                   where Item.Status == (int)Enums.Status.Normal
                   select new Models.ExitNoticeItem
                   {
                       ID = Item.ID,
                       ExitNoticeID = Item.ExitNoticeID,
                       //DecList = decList,
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)Item.ExitNoticeStatus,
                       Status = (Enums.Status)Item.Status,
                       CreateDate = Item.CreateDate,
                       UpdateDate = Item.UpdateDate
                   };
        }
    }

    /// <summary>
    /// 香港出库通知Items视图
    /// </summary>
    public class HKExitNoticeItemView : UniqueView<Models.HKExitNoticeItem, ScCustomsReponsitory>
    {
        public HKExitNoticeItemView()
        {
        }

        internal HKExitNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.HKExitNoticeItem> GetIQueryable()
        {
            var decListsView = new DecListsView(this.Reponsitory);

            return from Item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                   join decList in decListsView on Item.DecListID equals decList.ID
                   where Item.Status == (int)Enums.Status.Normal
                   select new Models.HKExitNoticeItem
                   {
                       ID = Item.ID,
                       ExitNoticeID = Item.ExitNoticeID,
                       Quantity = Item.Quantity,
                       DecList = decList,
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)Item.ExitNoticeStatus,
                       Status = (Enums.Status)Item.Status,
                       CreateDate = Item.CreateDate,
                       UpdateDate = Item.UpdateDate
                   };
        }

        public IQueryable<HKExitProduct> GetExitProductData()
        {
            return from Item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                   join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on Item.DecListID equals decList.ID
                   join noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>() on decList.DeclarationNoticeItemID equals noticeItem.ID
                   join packingItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>() on noticeItem.SortingID equals packingItem.SortingID
                   join packing in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>() on packingItem.PackingID equals packing.ID
                   join storage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>() on packingItem.SortingID equals storage.SortingID
                   where packingItem.Status == (int)Enums.Status.Normal
                   where packing.Status == (int)Enums.Status.Normal
                   select new Models.HKExitProduct
                   {
                       ID = Item.ID,
                       ExitNoticeID = Item.ExitNoticeID,
                       Quantity = Item.Quantity,
                       DecList = new DecList
                       {
                           ID = decList.ID,
                           NetWt = decList.NetWt,
                           GrossWt = decList.GrossWt,
                           GName = decList.GName,
                           GoodsModel = decList.GoodsModel,
                           GoodsBrand = decList.GoodsBrand
                       },
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)Item.ExitNoticeStatus,
                       Status = (Enums.Status)Item.Status,
                       CreateDate = Item.CreateDate,
                       UpdateDate = Item.UpdateDate,
                       StoreStorage = new StoreStorage
                       {
                           ID = storage.ID,
                           StockCode = storage.StockCode,
                           BoxIndex = storage.BoxIndex
                       },

                       PackingDate = packing.PackingDate,
                   };
        }
    }

    /// <summary>
    /// 深圳出库通知Items视图
    /// </summary>
    public class SZExitNoticeItemView : UniqueView<Models.SZExitNoticeItem, ScCustomsReponsitory>
    {
        public SZExitNoticeItemView()
        {
        }

        internal SZExitNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZExitNoticeItem> GetIQueryable()
        {
            var sortingView = new SZSortingsView(this.Reponsitory);

            return from Item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                   join sorting in sortingView on Item.SortingID equals sorting.ID
                   join entryItems in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>()
                   on sorting.OrderItem.ID equals entryItems.OrderItemID
                   join storage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>() on sorting.ID equals storage.SortingID into g
                   from storage in g.DefaultIfEmpty()
                   //join hksorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on
                   //   new { orderitemid = sorting.OrderItem.ID, warehousetype = (int)Enums.WarehouseType.HongKong, dataStatus = (int)Enums.Status.Normal }
                   //   equals new { orderitemid = hksorting.OrderItemID, warehousetype = hksorting.WarehouseType, dataStatus = hksorting.Status }
                   where Item.Status == (int)Enums.Status.Normal
                   select new Models.SZExitNoticeItem
                   {
                       ID = Item.ID,
                       ExitNoticeID = Item.ExitNoticeID,
                       Quantity = Item.Quantity,
                       Sorting = sorting,
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)Item.ExitNoticeStatus,
                       Status = (Enums.Status)Item.Status,
                       CreateDate = Item.CreateDate,
                       UpdateDate = entryItems.UpdateDate,
                       //UpdateDate = hksorting.UpdateDate,
                       StoreStorage = new StoreStorage
                       {
                           ID = storage.ID,
                           StockCode = storage.StockCode==null?"": storage.StockCode,
                           BoxIndex = storage.BoxIndex== null ? "" : storage.BoxIndex
                       },                    
                   };
        }
    }

    public class CenterSZExitNoticeItemView : UniqueView<Models.CenterWayBill, ScCustomsReponsitory>
    {
        public CenterSZExitNoticeItemView()
        {
        }

        internal CenterSZExitNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.CenterWayBill> GetIQueryable()
        {
            var data = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgSZOutputDetail>()
                       join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                       on c.ItemID equals d.OrderItemID
                       select new CenterWayBill
                       {
                           ID = c.WaybillID,
                           WayBillType = (WaybillType)c.WaybillType,
                           InitExType ="",
                           InitExPayType = (PayType)c.ExPayType,
                           ConsigneeCompany = c.ConsigneeCompany,
                           ConsigneeAddress = c.ConsigneeAddress,
                           ConsigneeContact = c.ConsigneeContact,
                           ConsigneePhone = c.ConsigneePhone,
                           ConsignorContact = c.ConsigneeContact,
                           ConsignorPhone = c.ConsigneePhone,
                           Quantity = c.Quantity,
                           ShelveID = c.ShelveID,
                           BoxCode = c.BoxCode,
                           PartNumber = c.PartNumber,
                           Manufacturer = c.Manufacturer,
                           DeclareName = d.Name,
                           //DeclareName = c.ItemID,
                           OrderID = c.OrderID,
                           CarNumber = c.chcdCarNumber1,
                           //IsModify = c.IsModify,
                           CreateDate = c.wbCreateDate,
                           BoxingDate = c.AppointTime.Value
                       };

            return data;
        }
    }
}
