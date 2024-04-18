using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.TaxCategory
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            //品名 型号 管控类型进行筛选
            string TaxCode = Request.QueryString["TaxCode"];
            string TaxName = Request.QueryString["TaxName"];
            string KeyWords = Request.QueryString["KeyWords"];
            var TaxCategories = Needs.Wl.Admin.Plat.AdminPlat.TaxCategories.AsQueryable();
            if (!string.IsNullOrEmpty(TaxCode))
            {
                TaxCategories = TaxCategories.Where(item => item.TaxCode.Contains(TaxCode));
            }
            if (!string.IsNullOrEmpty(TaxName))
            {
                TaxCategories = TaxCategories.Where(item => item.TaxName.Contains(TaxName));
            }
            if (!string.IsNullOrEmpty(KeyWords))
            {
                TaxCategories = TaxCategories.Where(item => item.KeyWords.Contains(KeyWords));
            }

            this.Paging(TaxCategories);
        }
    }
}