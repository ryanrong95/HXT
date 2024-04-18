using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 客户自定义的产品税务归类
    /// </summary>
    public class ClientProductTaxCategoriesAllsView : UniqueView<Models.ClientProductTaxCategory, ScCustomsReponsitory>
    {
        public ClientProductTaxCategoriesAllsView()
        {
        }

        internal ClientProductTaxCategoriesAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientProductTaxCategory> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);

            return from taxCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories>()
                   join client in clientsView on taxCategory.ClientID equals client.ID
                   select new Models.ClientProductTaxCategory
                   {
                       ID = taxCategory.ID,
                       Client = client,
                       Name = taxCategory.Name,
                       Model = taxCategory.Model,
                       TaxCode = taxCategory.TaxCode,
                       TaxName = taxCategory.TaxName,
                       Status = (Enums.Status)taxCategory.Status,
                       TaxStatus = (Enums.ProductTaxStatus)taxCategory.TaxStatus,
                       CreateDate = taxCategory.CreateDate,
                       UpdateDate = taxCategory.UpdateDate,
                       Summary = taxCategory.Summary
                   };
        }
    }

    /// <summary>
    /// 客户自定义产品税务分类
    /// </summary>
    public sealed class ClientProductTaxCategoriesView : ClientProductTaxCategoriesAllsView
    {
        string ClientID = string.Empty;
        string Name = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientID">客户ID</param>
        /// <param name="name">报关品名</param>
        public ClientProductTaxCategoriesView(string clientID, string name)
        {
            this.ClientID = clientID;
            this.Name = name;
        }

        protected override IQueryable<Models.ClientProductTaxCategory> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Client.ID == this.ClientID && entity.Name.Contains(this.Name) &&
                   entity.TaxStatus == Enums.ProductTaxStatus.Audited && entity.Status == Enums.Status.Normal
                   select entity;
        }
    }
}
