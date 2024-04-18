using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Client.Services.Views
{
    /// <summary>
    /// 客户的预归类产品
    /// 包含归类信息
    /// </summary>
    public class ClientClassifiedPreProductsView : View<ClassifiedPreProduct, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientClassifiedPreProductsView(string clientID)
        {
            this.ClientID = clientID;
        }

        protected override IQueryable<ClassifiedPreProduct> GetIQueryable()
        {
            return from product in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>()
                   join classify in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>() on product.ID equals classify.PreProductID
                   into classifProducts
                   from classifProduct in classifProducts.DefaultIfEmpty()
                   where product.ClientID == this.ClientID && product.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   orderby product.CreateDate descending
                   select new ClassifiedPreProduct
                   {
                       ID = product.ID,
                       ClientID = product.ClientID,
                       ProductUnionCode = product.ProductUnionCode,
                       Model = product.Model,
                       Manufacturer = product.Manufacturer,
                       BatchNo = product.BatchNo,
                       Price = product.Price,
                       Currency = product.Currency,
                       Supplier = product.Supplier,

                       ProductName = classifProduct == null ? "" : classifProduct.ProductName,
                       HSCode = classifProduct == null ? "" : classifProduct.HSCode,
                       TariffRate = classifProduct == null ? null : classifProduct.TariffRate,
                       AddedValueRate = classifProduct == null ? null : classifProduct.AddedValueRate,
                       ExciseTaxRate = classifProduct == null ? null : classifProduct.ExciseTaxRate,
                       TaxCode = classifProduct == null ? "" : classifProduct.TaxCode,
                       TaxName = classifProduct == null ? "" : classifProduct.TaxName,
                       Type = classifProduct == null ? Wl.Models.Enums.ItemCategoryType.Normal : (Wl.Models.Enums.ItemCategoryType?)classifProduct.Type,
                       ClassifyStatus = classifProduct == null ? Wl.Models.Enums.ClassifyStatus.Unclassified : (Wl.Models.Enums.ClassifyStatus?)classifProduct.ClassifyStatus,
                       InspectionFee = classifProduct == null ? null : classifProduct.InspectionFee,
                       Unit1 = classifProduct == null ? "" : classifProduct.Unit1,
                       Unit2 = classifProduct == null ? "" : classifProduct.Unit2,
                       CIQCode = classifProduct == null ? "" : classifProduct.CIQCode,
                       Elements = classifProduct == null ? "" : classifProduct.Elements,
                       Summary = classifProduct == null ? "" : classifProduct.Summary,

                       Status = (int)product.Status,
                       CreateDate = product.CreateDate,
                       UpdateDate = product.UpdateDate,

                       ClassifyFirstOperator = classifProduct.ClassifyFirstOperator,
                       ClassifySecondOperator = classifProduct.ClassifySecondOperator
                   };
        }
    }
}
