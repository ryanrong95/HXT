using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 预归类产品的视图
    /// </summary>
    public class DutiablePriceView<T> : Needs.Linq.Generic.Unique1Classics<DutiablePriceItem, ScCustomsReponsitory> where T : DutiablePriceItem, new()
    {
        public DutiablePriceView()
        {
        }

        public DutiablePriceView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DutiablePriceItem> GetIQueryable(Expression<Func<DutiablePriceItem, bool>> expression, params LambdaExpression[] expressions)
        {

            var linq = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                       orderby entity.GNo
                       select new DutiablePriceItem
                       {
                           DutiablePrice = entity.DeclTotal,
                           Model = entity.GoodsModel,
                           Qty = entity.GQty,
                           OrderItemID = entity.OrderItemID,
                           DecHeadID = entity.DeclarationID,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<DutiablePriceItem, bool>>);
            }

            var results = linq.Where(expression);

            return from result in results
                   join orderitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on result.OrderItemID equals orderitem.ID
                   join category in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on result.OrderItemID equals category.OrderItemID
                   select new DutiablePriceItem
                   {
                       ProductUniqueCode = orderitem.ProductUniqueCode,
                       DutiablePrice = result.DutiablePrice,
                       TaxName = category.TaxName,
                       TaxCode = category.TaxCode,
                       Model = result.Model,
                       Qty = result.Qty,
                   };
        }


        protected override IEnumerable<DutiablePriceItem> OnReadShips(DutiablePriceItem[] results)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Models.DutiablePriceItem> GetIQueryableResult(Expression<Func<DutiablePriceItem, bool>> expression)
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                       orderby entity.GNo
                       select new DutiablePriceItem
                       {
                           DecListID = entity.DecListID,
                           DutiablePrice = entity.DeclTotal,
                           Model = entity.GoodsModel,
                           Qty = entity.GQty,
                           OrderItemID = entity.OrderItemID,
                           DecHeadID = entity.DeclarationID,
                           Origin = entity.OriginCountry,
                       };

            var results = linq.Where(expression);

            var InsideResult = from result in results
                               join orderitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on result.OrderItemID equals orderitem.ID
                               join category in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on result.OrderItemID equals category.OrderItemID
                               //join product in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Products>() on orderitem.ProductID equals product.ID
                               join tariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on result.OrderItemID equals tariff.OrderItemID
                               where tariff.Type == (int)CustomsRateType.ImportTax
                               join addedValue in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on result.OrderItemID equals addedValue.OrderItemID
                               where addedValue.Type == (int)CustomsRateType.AddedValueTax
                               join consumeTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on result.OrderItemID equals consumeTax.OrderItemID
                               where consumeTax.Type == (int)CustomsRateType.ConsumeTax                              
                               select new DutiablePriceItem
                               {
                                   ProductUniqueCode = orderitem.ProductUniqueCode,
                                   DutiablePrice = result.DutiablePrice,
                                   TaxName = category.TaxName,
                                   TaxCode = category.TaxCode,
                                   Model = result.Model,
                                   Qty = result.Qty,
                                   TariffRate = tariff.Rate,
                                   AddedValueRate = addedValue.Rate,
                                   ConsumeTaxRate = consumeTax.Rate,
                                   TariffReceiptRate = tariff.ReceiptRate,
                                   AddedValueReceiptRate = addedValue.ReceiptRate,
                                   ConsumeTaxReceiptRate = consumeTax.ReceiptRate,
                                   Origin = result.Origin,
                                   Manfacture = orderitem.Manufacturer,
                                   ProductName = category.Name,
                                   HSCode = category.HSCode,
                                   OrderItemID = result.OrderItemID,
                                   DecListID = result.DecListID
                               };

            var IcgooResult = from inside in InsideResult
                              join pre in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>() on inside.ProductUniqueCode equals pre.ProductUnionCode into g
                              from pres in g.DefaultIfEmpty()
                              select new DutiablePriceItem
                              {
                                  ProductUniqueCode = inside.ProductUniqueCode,
                                  DutiablePrice = inside.DutiablePrice,
                                  DeclTotal = inside.DutiablePrice,
                                  TaxName = inside.TaxName,
                                  TaxCode = inside.TaxCode,
                                  Model = inside.Model,
                                  Qty = inside.Qty,
                                  TariffRate = inside.TariffRate,
                                  AddedValueRate = inside.AddedValueRate,
                                  ConsumeTaxRate = inside.ConsumeTaxRate,
                                  TariffReceiptRate = inside.TariffReceiptRate,
                                  AddedValueReceiptRate = inside.AddedValueReceiptRate,
                                  ConsumeTaxReceiptRate = inside.ConsumeTaxReceiptRate,
                                  Origin = inside.Origin,
                                  Manfacture = inside.Manfacture,
                                  ProductName = inside.ProductName,
                                  HSCode = inside.HSCode,
                                  Supplier = pres==null?"":pres.Supplier,
                                  OrderItemID = inside.OrderItemID,
                                  DecListID = inside.DecListID
                              };

            return IcgooResult;
        }
    }
}