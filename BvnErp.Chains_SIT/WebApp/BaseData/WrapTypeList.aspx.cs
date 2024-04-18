using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.BaseData
{
    public partial class WrapTypeList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化包装种类数据
        /// </summary>
        protected void data()
        {
            string code = Request.QueryString["Code"];
            string name = Request.QueryString["Name"];

            var packingTypes = Needs.Wl.Admin.Plat.AdminPlat.BaseWrapTypesView.AsQueryable();
            if (!string.IsNullOrEmpty(code))
            {
                packingTypes = packingTypes.Where(item => item.Code.Contains(code));
            }
            if (!string.IsNullOrEmpty(name))
            {
                packingTypes = packingTypes.Where(item => item.Name.Contains(name));
            }

            this.Paging(packingTypes);
        }
    }
}