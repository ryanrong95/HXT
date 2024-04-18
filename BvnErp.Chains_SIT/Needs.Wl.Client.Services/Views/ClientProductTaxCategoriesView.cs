using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Client.Services.Views
{
    /// <summary>
    /// 客户自定义的产品税务归类
    /// </summary>
    public class ClientProductTaxCategoriesView : View<Needs.Wl.Models.ClientProductTaxCategory, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientProductTaxCategoriesView(string clientID)
        {
            this.ClientID = clientID;
        }

        protected override IQueryable<Needs.Wl.Models.ClientProductTaxCategory> GetIQueryable()
        {
            return from taxCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories>()
                   where taxCategory.Status == (int)Needs.Wl.Models.Enums.Status.Normal && taxCategory.ClientID == this.ClientID
                   select new Needs.Wl.Models.ClientProductTaxCategory
                   {
                       ID = taxCategory.ID,
                       ClientID = taxCategory.ClientID,
                       Name = taxCategory.Name,
                       Model = taxCategory.Model,
                       TaxCode = taxCategory.TaxCode,
                       TaxName = taxCategory.TaxName,
                       Status = taxCategory.Status,
                       TaxStatus = (Needs.Wl.Models.Enums.ProductTaxStatus)taxCategory.TaxStatus,
                       CreateDate = taxCategory.CreateDate,
                       UpdateDate = taxCategory.UpdateDate,
                       Summary = taxCategory.Summary
                   };
        }
    }
}
