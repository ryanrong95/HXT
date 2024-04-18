using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 归类后的订单产品明细
    /// 包含产品的归类信息、税费信息
    /// </summary>
    public class CategoriedOrderItemsView : View<Needs.Wl.Models.CategoriedOrderItem, ScCustomsReponsitory>
    {
        private string OrderID;

        public CategoriedOrderItemsView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Needs.Wl.Models.CategoriedOrderItem> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                   join itemCategorie in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on item.ID equals itemCategorie.OrderItemID into itemCategories
                   from itemCategorie in itemCategories.DefaultIfEmpty()
                   join itemTaxe in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemRateView>() on item.ID equals itemTaxe.OrderItemID into itemTaxes
                   from itemTaxe in itemTaxes.DefaultIfEmpty()
                   join inspFee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>().Where(s => s.Type == (int)Enums.OrderPremiumType.InspectionFee) on item.ID equals inspFee.OrderItemID into inspFees
                   from inspFee in inspFees.DefaultIfEmpty()
                   where item.Status == (int)Needs.Wl.Models.Enums.Status.Normal && item.OrderID == this.OrderID
                   select new Needs.Wl.Models.CategoriedOrderItem
                   {
                       ID = item.ID,
                       OrderID = item.OrderID,
                       ProductUniqueCode = item.ProductUniqueCode,
                       Name = item.Name,
                       Model = item.Model,
                       Manufacturer = item.Manufacturer,
                       Batch = item.Batch,
                       Origin = item.Origin,
                       Quantity = item.Quantity,
                       DeclaredQuantity = item.DeclaredQuantity,
                       Unit = item.Unit,
                       UnitPrice = item.UnitPrice,
                       TotalPrice = item.TotalPrice,
                       GrossWeight = item.GrossWeight,
                       IsSampllingCheck = item.IsSampllingCheck,
                       ClassifyStatus = (Enums.ClassifyStatus)item.ClassifyStatus,
                       InspectionFee = inspFee == null ? null : (decimal?)inspFee.UnitPrice * inspFee.Count * inspFee.Rate,

                       Type = itemCategorie == null ? Enums.ItemCategoryType.Normal : (Enums.ItemCategoryType)itemCategorie.Type,
                       TaxCode = itemCategorie == null ? "" : itemCategorie.TaxCode,
                       TaxName = itemCategorie == null ? "" : itemCategorie.TaxName,
                       HSCode = itemCategorie == null ? "" : itemCategorie.HSCode,
                       CategoriedName = itemCategorie == null ? "" : itemCategorie.Name,
                       Elements = itemCategorie == null ? "" : itemCategorie.Elements,
                       Unit1 = itemCategorie == null ? "" : itemCategorie.Unit1,
                       Unit2 = itemCategorie == null ? "" : itemCategorie.Unit2,
                       Qty1 = itemCategorie == null ? 0 : itemCategorie.Qty1,
                       Qty2 = itemCategorie == null ? 0 : itemCategorie.Qty2,
                       CIQCode = itemCategorie == null ? "" : itemCategorie.CIQCode,

                       ImportTaxRate = itemTaxe == null ? 0 : itemTaxe.ImportTaxRate.GetValueOrDefault(0),
                       ImportTaxValue = itemTaxe == null ? 0 : itemTaxe.ImportTaxValue.GetValueOrDefault(0),
                       AddedValueRate = itemTaxe == null ? 0 : itemTaxe.AddedValueTaxRate.GetValueOrDefault(0),
                       AddedValue = itemTaxe == null ? 0 : itemTaxe.AddedValue.GetValueOrDefault(0),
                       ConsumeTaxRate = itemTaxe == null ? 0 : itemTaxe.ConsumeTaxRate.GetValueOrDefault(0),
                       ConsumeTaxValue = itemTaxe == null ? 0 : itemTaxe.ConsumeTaxValue.GetValueOrDefault(0),

                       Status = (int)item.Status,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate,
                       Summary = item.Summary,
                   };
        }
    }
}