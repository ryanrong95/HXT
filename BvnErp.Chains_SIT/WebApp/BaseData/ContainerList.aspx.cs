using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.BaseData
{
    public partial class ContainerList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化集装箱规格数据
        /// </summary>
        protected void data()
        {
            string code = Request.QueryString["Code"];
            string name = Request.QueryString["Name"];

            var containers = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.BaseContainers.AsQueryable();
            if (!string.IsNullOrEmpty(code))
            {
                containers = containers.Where(item => item.Code.Contains(code));
            }
            if (!string.IsNullOrEmpty(name))
            {
                containers = containers.Where(item => item.Name.Contains(name));
            }

            this.Paging(containers);
        }
    }
}