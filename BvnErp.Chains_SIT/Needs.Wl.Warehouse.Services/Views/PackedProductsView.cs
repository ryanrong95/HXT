using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Wl.Warehouse.Services.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Warehouse.Services.Views
{
    /// <summary>
    /// 香港库房，分拣页面已装箱产品
    /// </summary>
    public class PackedProductsView : View<PackedProductListModel, ScCustomsReponsitory>
    {
        private string OrderID;

        public PackedProductsView()
        {

        }

        public PackedProductsView(string orderID) : this()
        {
            this.OrderID = orderID;
        }

        protected override IQueryable<PackedProductListModel> GetIQueryable()
        {
            var sortings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();
            var packingItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>();
            var packings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>();
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var baseCountries = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();

            return from sorting in sortings

                   join orderItem in orderItems
                        on new
                        {
                            OrderItemID = sorting.OrderItemID,
                            OrderID = sorting.OrderID,
                            SortingsDataStatus = sorting.Status,
                            OrderItemsDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                        }
                        equals new
                        {
                            OrderItemID = orderItem.ID,
                            OrderID = this.OrderID,
                            SortingsDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                            OrderItemsDataStatus = orderItem.Status,
                        }

                   join orderItemCategory in orderItemCategories
                        on new { OrderItemID = orderItem.ID, OrderItemCategoryDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal, }
                        equals new { OrderItemID = orderItemCategory.OrderItemID, OrderItemCategoryDataStatus = orderItemCategory.Status, }
                        into orderItemCategories2
                   from orderItemCategory in orderItemCategories2.DefaultIfEmpty()

                   join packingItem in packingItems
                        on new { SortingID = sorting.ID, PackingItemDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal, }
                        equals new { SortingID = packingItem.SortingID, PackingItemDataStatus = packingItem.Status, }
                   join packing in packings
                        on new { PackingID = packingItem.PackingID, PackingDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal, }
                        equals new { PackingID = packing.ID, PackingDataStatus = packing.Status, }

                   join origin in baseCountries on orderItem.Origin equals origin.Code into baseCountries2
                   from origin in baseCountries2.DefaultIfEmpty()

                   orderby sorting.BoxIndex
                   select new PackedProductListModel
                   {
                       ID = sorting.ID,
                       PackingID = packing.ID,
                       BoxIndex = sorting.BoxIndex,
                       NetWeight = sorting.NetWeight,
                       GrossWeight = sorting.GrossWeight,
                       Model = orderItem.Model,
                       Name = orderItemCategory.Name,
                       Quantity = sorting.Quantity,
                       Origin = orderItem.Origin + " " + origin.Name,
                       Manufacturer = orderItem.Manufacturer,
                       PackingStatus = (Needs.Ccs.Services.Enums.PackingStatus)packing.PackingStatus,
                       SpecialType = (Needs.Ccs.Services.Enums.ItemCategoryType)orderItemCategory.Type,
                       OrderItemQty = orderItem.Quantity,
                   };
        }    
    }
}
