using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig
{
    public partial class ProductTaxCategoriesList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            //品名 型号 管控类型进行筛选
            string model = Request.QueryString["Model"];
            string name = Request.QueryString["Name"];
            string TaxCode = Request.QueryString["TaxCode"];
            string TaxName = Request.QueryString["TaxName"];
            var view = Needs.Wl.Admin.Plat.AdminPlat.ProductTaxCategories.AsQueryable();
            if (!string.IsNullOrEmpty(model))
            {
                view = view.Where(item => item.Model.Contains(model));
            }
            if (!string.IsNullOrEmpty(name))
            {
                view = view.Where(item => item.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(TaxCode))
            {
                view = view.Where(item => item.TaxCode.Contains(TaxCode));
            }
            if (!string.IsNullOrEmpty(TaxName))
            {
                view = view.Where(item => item.TaxName.Contains(TaxName));
            }
            Func<Needs.Ccs.Services.Models.ProductTaxCategory, object> convert = entity => new
            {
                ID = entity.ID,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                Name = entity.Name,
                Model = entity.Model,
                CreateDate = entity.CreateDate.ToShortDateString(),
            };
            this.Paging(view, convert);
        }
    }
}