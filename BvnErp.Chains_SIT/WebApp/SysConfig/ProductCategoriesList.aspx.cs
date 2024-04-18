using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig
{
    public partial class ProductCategoriesList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            //品名 型号 管控类型进行筛选
            string model = Request.QueryString["Model"];
            string name = Request.QueryString["Name"];
            string hscode = Request.QueryString["HSCode"];
            var ProductCategories = Needs.Wl.Admin.Plat.AdminPlat.ProductCategories.AsQueryable();
            if (!string.IsNullOrEmpty(model))
            {
                ProductCategories = ProductCategories.Where(item => item.Model.Contains(model));
            }
            if (!string.IsNullOrEmpty(name))
            {
                ProductCategories = ProductCategories.Where(item => item.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(hscode))
            {
                ProductCategories = ProductCategories.Where(item => item.HSCode.Contains(hscode));
            }
            Func<Needs.Ccs.Services.Models.ProductCategory, object> linq = item => new
            {
                item.ID,
                item.Name,
                item.Model,
                item.HSCode,
                item.Elements,
                item.Qty,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
               
            this.Paging(ProductCategories,linq);
        }
    }
}