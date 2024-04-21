using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.Roles
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model = Alls.Current.Roles[Request.QueryString["id"]];
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];

            if (string.IsNullOrWhiteSpace(id) || id == "undefined")
            {
                var model = new Role()
                {
                    Name = Request.Form["name"].Trim(),
                    ID = Request.QueryString["id"],
                    Type = RoleType.Customer,
                };

                model.EnterError += EnterError;
                model.Enter();

                id = model.ID;
            }


            var arry = Request.Form["menustree"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Alls.Current.Roles[id].Map(arry);

            var keys = Request.Form.AllKeys.Where(item => item.StartsWith("particles_", StringComparison.OrdinalIgnoreCase));
            var role = Alls.Current.Roles[Request.QueryString["id"]];
            foreach (var key in keys)
            {
                string menuid = key.Split('_')[1];
                var menu = Alls.Current.Menus[menuid];
                role.Setting(menu.RightUrl, Server.HtmlDecode(Request.Form[key]));
            }

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "权限管理",
                   $"配置", arry.Json());

            //Easyui.Reload("操作提示", "操作成功!", Web.Controls.Easyui.Sign.None);
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCompose_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];

            if (string.IsNullOrWhiteSpace(id) || id == "undefined")
            {
                var model = new Role()
                {
                    Name = Request.Form["name"].Trim(),
                    ID = Request.QueryString["id"],
                    Type = RoleType.Customer,
                };

                model.EnterError += EnterError;
                model.Enter();

                id = model.ID;
            }


            var arry = Request.Form["menustree"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Alls.Current.Roles[id].Map(arry);

            var keys = Request.Form.AllKeys.Where(item => item.StartsWith("particles_", StringComparison.OrdinalIgnoreCase));
            foreach (var key in keys)
            {

                var role = Alls.Current.Roles[Request.QueryString["id"]];

                string menuid = key.Split('_')[1];
                var menu = Alls.Current.Menus[menuid];
                role.Setting(menu.RightUrl, Server.HtmlDecode(Request.Form[key]));
            }

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "权限管理",
                   $"配置", arry.Json());

            //Easyui.Reload("操作提示", "操作成功!", Web.Controls.Easyui.Sign.None);
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

        }

        protected string menustree()
        {
            return Alls.Current.Menus.Json(Request.QueryString["id"]);
        }

        protected object data()
        {
            string url = Request.QueryString["url"];

            var settings = Alls.Current.Roles[Request.QueryString["roleid"]]?.Settings[url];
            var sbu = Alls.Current.Particles.SingleByUrl(url);

            //选中菜单行的时候
            if (sbu == null)
            {
                return JArray.Parse("[]");
            }


            var selecteds = JArray.Parse(settings == null || string.IsNullOrEmpty(settings.Context) ? "[]" : settings.Context);
            var selectedId = selecteds.Select(item => item["ID"].Value<string>());

            var rtuns = new JArray();
            foreach (var item in JArray.Parse(sbu.Context))
            {
                var one = selecteds.SingleOrDefault(i => i["ID"].Value<string>() == item["ID"].Value<string>())
                    ?? item.DeepClone();

                //one["IsShow"] = one["IsShow"]?.Value<bool?>() == false ? false : true;
                one["IsShow"] = one["IsShow"] == null || one["IsShow"].Value<bool?>().GetValueOrDefault() == false ? false : true;
                //one["IsEdit"] = one["IsEdit"]?.Value<bool?>() == false ? false : true;
                one["IsEdit"] = one["IsEdit"] == null || one["IsEdit"]?.Value<bool?>().GetValueOrDefault() == false ? false : true;

                rtuns.Add(one);
            }
            return rtuns;

            return new
            {
                unSelect = selecteds,
                rows = JArray.Parse(sbu?.Context ?? "[]").Select(item =>
                {
                    return item;
                })
            };

        }

        //private void EnterSuccess(object sender, SuccessEventArgs e)
        //{
        //    var entity = sender as Role;
        //    Easyui.Reload("操作提示", "操作成功!", Yahv.Web.Controls.Easyui.Sign.None);
        //    this.Model = new AdminsRoll()[Request.QueryString["id"]] ?? new Admin();
        //}

        private void EnterError(object sender, ErrorEventArgs e)
        {
            var entity = sender as Role;
            Easyui.Alert("操作提示", e.Message, Web.Controls.Easyui.Sign.Error);
        }
    }
}