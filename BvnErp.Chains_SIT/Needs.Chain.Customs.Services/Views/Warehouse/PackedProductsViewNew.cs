using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PackedProductsViewNew : QueryView<Models.PackedProductListModelNew, ScCustomsReponsitory>
    {
        private string OrderID;

        public PackedProductsViewNew()
        {

        }

        public PackedProductsViewNew(string orderID) : this()
        {
            this.OrderID = orderID;
        }

        protected override IQueryable<Models.PackedProductListModelNew> GetIQueryable()
        {
            var sortings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();
            var packingItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>();
            var packings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>();
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var baseCountries = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();
            var orderWaybills = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>();
            var orderWaybillItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>();
            var PvbCrmCarriersTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbCrmCarriersTopView>();

            var result = from sorting in sortings

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
                   select new Models.PackedProductListModelNew
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
                       SpecialType = orderItemCategory==null? Needs.Ccs.Services.Enums.ItemCategoryType.Normal:(Needs.Ccs.Services.Enums.ItemCategoryType)orderItemCategory.Type,
                       OrderItemQty = orderItem.Quantity,
                       OrderID = packing.OrderID,
                       Batch = orderItem.Batch,
                   };

            return result;

        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.PackedProductListModelNew> iquery = this.IQueryable.Cast<Models.PackedProductListModelNew>().OrderBy(item => item.BoxIndex);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_EntryNotices = iquery.ToArray();

            var orderids = ienum_EntryNotices.Select(t => t.OrderID).Distinct().ToList();

            var orderWaybill = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>()
                                join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>()
                                on c.ID equals d.OrderWaybillID
                                where orderids.Contains(c.OrderID)
                                select new
                                {
                                    OrderID = c.OrderID,
                                    SortingID = d.SortingID,
                                    waybillCode = c.WaybillCode,
                                    carrierid = c.CarrierID
                                }).ToArray();

            var carrierIDs = orderWaybill.Select(t => t.carrierid).Distinct();

            var carrierInfo = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbCrmCarriersTopView>()
                               where carrierIDs.Contains(c.ID)
                               select new
                               {
                                   ID = c.ID,
                                   Name = c.Name
                               }).ToArray();

            var finalRes = from ienum_PackedInfo in ienum_EntryNotices
                           join ienum_OrderWaybill in orderWaybill on ienum_PackedInfo.ID equals ienum_OrderWaybill.SortingID into g
                           from orderWaybillInfo in g.DefaultIfEmpty()
                           join ienum_CarrierInfo in carrierInfo on orderWaybillInfo?.carrierid equals ienum_CarrierInfo.ID into h
                           from carrier in h.DefaultIfEmpty()
                           select new Models.PackedProductListModelNew
                           {
                               ID = ienum_PackedInfo.ID,
                               PackingID = ienum_PackedInfo.PackingID,
                               BoxIndex = ienum_PackedInfo.BoxIndex,
                               NetWeight = ienum_PackedInfo.NetWeight,
                               GrossWeight = ienum_PackedInfo.GrossWeight,
                               Model = ienum_PackedInfo.Model,
                               Name = ienum_PackedInfo.Name,
                               Quantity = ienum_PackedInfo.Quantity,
                               Origin = ienum_PackedInfo.Origin,
                               Manufacturer = ienum_PackedInfo.Manufacturer,
                               PackingStatus = ienum_PackedInfo.PackingStatus,
                               SpecialType = ienum_PackedInfo.SpecialType,
                               OrderItemQty = ienum_PackedInfo.OrderItemQty,
                               OrderID = ienum_PackedInfo.OrderID,
                               CarrierName = carrier?.Name,
                               OrderWaybillCode = orderWaybillInfo?.waybillCode,
                               Batch = ienum_PackedInfo.Batch,
                           };

            var results = finalRes;

            var res = results.Select(
                        item => new
                        {
                            ID = item.ID,
                            PackingID = item.PackingID,
                            BoxIndex = item.BoxIndex,
                            NetWeight = item.NetWeight,
                            GrossWeight = item.GrossWeight,
                            Model = item.Model,
                            Name = item.Name,
                            Quantity = item.Quantity,
                            Origin = item.Origin,
                            Manufacturer = item.Manufacturer,
                            PackingStatus = item.PackingStatus,
                            SpecialType = item.SpecialType,
                            OrderItemQty = item.OrderItemQty,
                            OrderID = item.OrderID,
                            CarrierName = item.CarrierName,
                            OrderWaybillCode = item.OrderWaybillCode,
                            Batch = item.Batch
                        }
                     ).ToArray();


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = res.ToArray(),
            };
        }
    }
   
}
