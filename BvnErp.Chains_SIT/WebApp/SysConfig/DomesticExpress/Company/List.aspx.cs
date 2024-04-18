using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.DomesticExpress.Company
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string Name = Request.QueryString["Name"];
            string Code = Request.QueryString["Code"];
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies.AsQueryable();
            if (!string.IsNullOrEmpty(Name))
            {
                Name = Name.Trim();
                list = list.Where(t => t.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Code))
            {
                Code = Code.Trim();
                list = list.Where(t => t.Code.Contains(Code));
            }
            Func<Needs.Ccs.Services.Models.ExpressCompany, object> convert = item => new
            {
                ID = item.ID,
                Name = item.Name,
                Code = item.Code,
                CustomerName=item.CustomerName,
                CustomerPwd=item.CustomerPwd,
                MonthCode=item.MonthCode,

            };
            this.Paging(list, convert);
        }
    }
}