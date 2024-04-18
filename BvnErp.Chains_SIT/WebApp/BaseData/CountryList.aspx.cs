using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Linq;

namespace WebApp.BaseData
{
    public partial class CountryList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void data()
        {
            var country = Needs.Wl.Admin.Plat.AdminPlat.Countries.AsQueryable();

            string code = Request.QueryString["Code"];
            string name = Request.QueryString["Name"];
            string englishName = Request.QueryString["EnglishName"];

            if (!string.IsNullOrEmpty(code))
            {
                country = country.Where(item => item.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                country = country.Where(item => item.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(englishName))
            {
                country = country.Where(item => item.EnglishName.Contains(englishName));
            }

            this.Paging(country);
        }
    }
}