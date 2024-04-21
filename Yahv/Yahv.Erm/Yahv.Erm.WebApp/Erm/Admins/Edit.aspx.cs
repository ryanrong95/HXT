using System;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Usually;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.Admins
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                var admin = Alls.Current.Admins[Request.QueryString["id"]];

                if (admin != null)
                {
                    this.Model = new
                    {
                        admin.UserName,
                        admin.RealName,
                        admin.SelCode,
                        admin.RoleID,
                        admin.Status,
                        //admin.DyjCode
                    };
                }
            }
        }

        protected object roles()
        {
            return Alls.Current.Roles.Select(item => new
            {
                id = item.ID,
                name = item.Name
            });
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            Admin model;

            if (!Alls.Current.Roles.Any(item => item.ID == Request.Form["RoleID"].Trim()))
            {
                Easyui.Alert("操作提示", "角色不能为空!", Sign.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Request.QueryString["id"]) || Request.QueryString["id"] == "undefined")
            {
                model = new Admin()
                {
                    UserName = Request.Form["UserName"].Trim(),
                    RealName = Request.Form["RealName"].Trim(),
                    Password = Request.Form["Password"].Trim(),
                    RoleID = Request.Form["RoleID"].Trim(),
                    //DyjCode = Request.Form["DyjCode"].Trim(),
                };

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "管理员管理", $"添加管理员：{model.RealName}", $"角色ID：{model.RoleID}");
            }
            else
            {
                model = Alls.Current.Admins[Request.QueryString["id"]];

                if (!string.IsNullOrWhiteSpace(Request.Form["Password"]))
                {
                    model.Password = Request.Form["Password"].Trim();
                }
                else
                {
                    model.Password = string.Empty;
                }

                model.RoleID = Request.Form["RoleID"].Trim();
                //model.DyjCode = Request.Form["DyjCode"].Trim();

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "管理员管理", $"修改管理员：{model.RealName}", $"角色ID：{model.RoleID}");
            }

            model.EnterSuccess += Admins_EnterSuccess;
            model.EnterError += Admins_EnterError;
            model.Enter();


        }

        private void Admins_EnterSuccess(object sender, SuccessEventArgs e)
        {
            //Easyui.Reload("操作提示", "操作成功!", Web.Controls.Easyui.Sign.None);
            //Easyui.AutoReload("操作成功!", Web.Controls.Easyui.AutoSign.Success);

            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

            //var entity = sender as Admin;
            //this.Model = new AdminsRoll()[Request.QueryString["id"]] ?? new Admin();
        }

        private void Admins_EnterError(object sender, ErrorEventArgs e)
        {
            var entity = sender as Admin;

            //Easyui.Alert("操作提示", e.Message, Web.Controls.Easyui.Sign.Error);
            Easyui.AutoAlert(e.Message, Web.Controls.Easyui.AutoSign.Error);
        }
    }
}