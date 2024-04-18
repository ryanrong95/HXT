using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Plat.Menus
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 获取菜单数据列表
        /// </summary>
        /// <returns></returns>
        protected JArray data()
        {
            var name = Request["name"];
            string id = null;
            if (!string.IsNullOrWhiteSpace(name))
            {
                id = new NtErp.Services.Views.MenusAlls().SingleOrDefault(t => t.ID == name || t.Name == name)?.ID;
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new JArray();
                }
            }
            var list = new NtErp.Services.Views.MenusAlls().Treegrid(id, true).AsQueryable();

            return JArray.FromObject(list);
        }


        protected bool change()
        {
            var id = Request["id"];
            var index = Request["index"];
            var menu = new NtErp.Services.Views.MenusAlls().SingleOrDefault(t => t.ID == id);
            if (menu != null)
            {
                menu.OrderIndex = Convert.ToInt32(index);
                menu.Enter();
            }
            return true;
        }


        protected bool del()
        {
            var id = Request["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                var menus = new NtErp.Services.Views.MenusAlls().Where(t => t.ID == id || t.FatherID == id);
                foreach (var item in menus)
                {
                    item.Abandon();
                }
               
            }
            return true;

        }

    }
}