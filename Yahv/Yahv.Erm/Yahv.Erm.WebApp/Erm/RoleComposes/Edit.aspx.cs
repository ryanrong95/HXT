using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.RoleComposes
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];

                this.Model = new
                {
                    Name = (string.IsNullOrWhiteSpace(id) || id == "undefined") ? null : Alls.Current.Roles[id].Name,
                    Roles = Alls.Current.Roles.GetChildRoles(Request.QueryString["id"]).Select(item => item.ID).ToArray()
                };
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var ids = this.hCheckId.Value.Split(',');
            string id = Request.QueryString["id"];

            if (ids.Length <= 0)
            {
                Easyui.Alert("操作提示", "角色不能为空!", Web.Controls.Easyui.Sign.Error);
                return;
            }

            var model = new Role()
            {
                Name = Request.Form["name"].Trim(),
                ID = Request.QueryString["id"],
                Type = RoleType.Compose,
            };

            model.EnterError += EnterError;
            model.EnterSuccess += EnterSuccess;
            model.Enter();

            id = model.ID;
        }

        private void EnterError(object sender, ErrorEventArgs e)
        {
            var entity = sender as Role;
            Easyui.Alert("操作提示", e.Message, Web.Controls.Easyui.Sign.Error);
        }

        private void EnterSuccess(object sender, SuccessEventArgs e)
        {
            var entity = sender as Role;
            if (entity == null || string.IsNullOrWhiteSpace(entity.ID))
            {
                Easyui.Reload("操作提示", "插入角色失败!!", Yahv.Web.Controls.Easyui.Sign.Error);
                return;
            }

            Alls.Current.Roles.Compose(entity.ID, this.hCheckId.Value.Split(','));
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var query = Alls.Current.Roles;

            return new
            {
                rows = query.Where(item => item.Type == RoleType.Customer && item.Status != RoleStatus.Super).OrderBy(t => t.ID).ToArray().Select(t => new
                {
                    t.ID,
                    StatusName = t.Status.GetDescription(),
                    Status = t.Status,
                    t.Name,
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                    IsSuper = t.Status == RoleStatus.Super,
                })
            };
        }
    }
}