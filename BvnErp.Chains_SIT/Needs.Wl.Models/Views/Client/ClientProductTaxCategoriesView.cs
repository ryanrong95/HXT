using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 客户自定义的产品税务归类
    /// </summary>
    public class ClientProductTaxCategoriesView : View<Models.ClientProductTaxCategory, ScCustomsReponsitory>
    {
        public ClientProductTaxCategoriesView()
        {
        }

        internal ClientProductTaxCategoriesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientProductTaxCategory> GetIQueryable()
        {
            return from taxCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories>()
                   where taxCategory.Status == (int)Enums.Status.Normal
                   select new Models.ClientProductTaxCategory
                   {
                       ID = taxCategory.ID,
                       ClientID = taxCategory.ClientID,
                       Name = taxCategory.Name,
                       Model = taxCategory.Model,
                       TaxCode = taxCategory.TaxCode,
                       TaxName = taxCategory.TaxName,
                       Status = taxCategory.Status,
                       TaxStatus = (Enums.ProductTaxStatus)taxCategory.TaxStatus,
                       CreateDate = taxCategory.CreateDate,
                       UpdateDate = taxCategory.UpdateDate,
                       Summary = taxCategory.Summary
                   };
        }
    }
}
