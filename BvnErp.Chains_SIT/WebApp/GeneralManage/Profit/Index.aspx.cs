using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GeneralManage.Profit
{
    public partial class Index : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //var dminRole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.ToArray();
            //if (dminRole.Length != 0)
            //{
            //    this.Model.ServiceManager = dminRole.Where(manager => manager.Role.Name == "业务员" && manager.Admin.RealName != "张庆永").Select(item => new { Key = item.Admin.ID, Value = item.Admin.RealName }).ToArray().Json();

            //    this.Model.Merchandiser = dminRole.Where(manager => manager.Role.Name == "跟单员").Select(item => new { Key = item.Admin.ID, Value = item.Admin.RealName }).ToArray().Json();
            //}
            //else
            //{
            //    this.Model.ServiceManager = null;
            //    this.Model.Merchandiser = null;
            //}

        }
    }
}