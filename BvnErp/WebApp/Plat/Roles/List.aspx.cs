using Needs.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Plat.Roles
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var admin = Needs.Erp.ErpPlot.Current;
            }
        }

        /// <summary>
        /// 获取角色数据列表
        /// </summary>
        /// <returns></returns>
        protected JArray data()
        {
            var rolename = Request["rolename"];

            var list = new NtErp.Services.Views.RoleView().AsQueryable();
            if (!string.IsNullOrWhiteSpace(rolename))
            {
                list = list.Where(t => t.ID.Contains(rolename) || t.Name.Contains(rolename));
            }
            var arry = new JArray();
            if (list != null && list.Count() > 0)
            {
                arry = JArray.FromObject(list.ToArray());
            }
            return arry;
        }

        protected bool del()
        {
            var ids = Request["ids"];

            try
            {
                var arry = ids.Split(',');
                if (arry != null)
                {
                    foreach (var id in arry)
                    {
                        var entity = new NtErp.Services.Models.Role
                        {
                            ID = id,
                        };
                        entity.AbandonSuccess += Entity_AbandonSuccess;
                        entity.Abandon();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        /// <summary>
        /// 删除成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {

        }
    }
}