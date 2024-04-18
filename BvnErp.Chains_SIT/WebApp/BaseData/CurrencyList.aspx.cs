using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.BaseData
{
    public partial class CustomsRateList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //海关汇率、实时汇率
            //通过参数确认 rateType
        }

        protected void data()
        {
            var tariffs = Needs.Wl.Admin.Plat.AdminPlat.Currencies.AsQueryable();

            string code = Request.QueryString["Code"];
            string name = Request.QueryString["Name"];

            if (!string.IsNullOrEmpty(code))
            {
                tariffs = tariffs.Where(item => item.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                tariffs = tariffs.Where(item => item.Name.Contains(name));
            }

            base.Paging(tariffs);
        }
    }
}