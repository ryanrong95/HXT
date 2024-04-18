using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Plat.Roles
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request["id"];
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var model = new NtErp.Services.Views.RoleView().SingleOrDefault(item => item.ID == id);
                    if (model != null)
                    {
                        this._id.Value = model.ID;
                        this._name.Value = model.Name;
                        this._summary.Value = model.Summary;
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var name = this._name.Value;
            var summary = this._summary.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                Alert("必填项不能为空", Request.Url);
                return;
            }

            var entity = new NtErp.Services.Models.Role
            {
                ID = this._id.Value,
                Name = name,
                Summary = summary
            };
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }
        /// <summary>
        /// Enter成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("保存成功", Request.Url, true);
        }


    }
}