using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Plat.Roles
{
    public partial class Setting : Needs.Web.Sso.Forms.ErpPage
    {
        /// <summary>
        /// 当前会话角色
        /// </summary>
        protected NtErp.Services.Models.Role CurrentRole
        {
            get
            {
                var id = Request["id"];
                if (string.IsNullOrWhiteSpace(id))
                {
                    return null;
                }
                return new NtErp.Services.Views.RoleView().SingleOrDefault(t => t.ID == id);
            }
        }
        /// <summary>
        /// 当前会话角色菜单
        /// </summary>
        protected JArray RoleMenus
        {
            get
            {
                JArray arry = JArray.FromObject(this.CurrentRole.Menus.Where(t => t.FatherID != null).Select(t => t.ID));
                return arry;
            }
        }
        /// <summary>
        /// 当前会话角色颗粒化
        /// </summary>
        protected JArray RoleUnites
        {
            get
            {
                JArray arry = JArray.FromObject(this.CurrentRole.Unites.Select(t => t.ID));
                return arry;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["RoleID"] = Request["id"];
                this.Model =new NtErp.Services.Views.MenusAlls().Tree();
            }
        }

        /// <summary>
        /// 角色和菜单关系保存
        /// </summary>
        protected void menuSave()
        {
            var ids = Request["ids"];
            if (this.CurrentRole != null && !string.IsNullOrWhiteSpace(ids))
            {
                var arry = ids.Split(',');
                if (arry != null && arry.Length > 0)
                {
                    List<NtErp.Services.Models.Menu> menus = new List<NtErp.Services.Models.Menu>();
                    foreach (var id in arry)
                    {
                        this.CurrentRole.Menus.Bind(id);
                    }
                }
            }

        }

        /// <summary>
        /// 角色和菜单关系删除
        /// </summary>
        protected void menuRemove()
        {
            var ids = Request["ids"];
            if (this.CurrentRole != null && !string.IsNullOrWhiteSpace(ids))
            {
                var arry = ids.Split(',');
                if (arry != null && arry.Length > 0)
                {
                    foreach (var id in arry)
                    {
                        this.CurrentRole.Menus.UnBind(id);
                    }
                }
            }
        }

        /// <summary>
        /// 角色的菜单页的颗粒化数据
        /// </summary>
        protected JArray roleUnites()
        {
            var menu = Request["menu"];
            var list = new NtErp.Services.Views.UnitesAllsView().Where(t => t.Menu == menu);

            return JArray.FromObject(list);
        }


        /// <summary>
        /// 角色颗粒化保存
        /// </summary>
        protected void save()
        {
            var ids = Request["ids"];
            if (this.CurrentRole != null && !string.IsNullOrWhiteSpace(ids))
            {
                var arry = ids.Split(',');
                if (arry != null && arry.Length > 0)
                {
                    List<NtErp.Services.Models.RoleUnite> unites = new List<NtErp.Services.Models.RoleUnite>();
                    foreach (var id in arry)
                    {
                        this.CurrentRole.Unites.Bind(id);
                       
                    }
                }
            }

        }
        /// <summary>
        /// 角色颗粒化删除
        /// </summary>
        protected void remove()
        {
            var ids = Request["ids"];
            if (this.CurrentRole != null && !string.IsNullOrWhiteSpace(ids))
            {
                var arry = ids.Split(',');
                if (arry != null && arry.Length > 0)
                {
                    foreach (var id in arry)
                    {
                        this.CurrentRole.Unites.UnBind(id);
                    }
                    
                }
            }
        }


    }
}