using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.ProductTaxCategory
{
    public partial class AuditedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化产品税务归类数据
        /// </summary>
        protected void data()
        {
            string clientCode = Request.QueryString["ClientCode"];
            string name = Request.QueryString["Name"];

            var taxCategories = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClientProductTaxCategories.Where(tc => tc.TaxStatus != ProductTaxStatus.Auditing)
                                                                                            .OrderBy(tc => tc.Client.ClientCode).AsQueryable();
            if (!string.IsNullOrEmpty(clientCode))
            {
                taxCategories = taxCategories.Where(t => t.Client.ClientCode.Contains(clientCode.Trim()));
            }
            if (!string.IsNullOrEmpty(name))
            {
                taxCategories = taxCategories.Where(t => t.Name.Contains(name.Trim()));
            }

            Func<Needs.Ccs.Services.Models.ClientProductTaxCategory, object> convert = taxCategory => new
            {
                taxCategory.ID,
                taxCategory.Client.ClientCode,
                ClientName = taxCategory.Client.Company.Name,
                taxCategory.Name,
                taxCategory.Model,
                taxCategory.TaxCode,
                taxCategory.TaxName,
                TaxStatusValue = taxCategory.TaxStatus,
                TaxStatus = taxCategory.TaxStatus.GetDescription(),
            };
            this.Paging(taxCategories, convert);
        }
    }
}