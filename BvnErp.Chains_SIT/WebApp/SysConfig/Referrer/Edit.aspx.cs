using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.Referrer
{
    public partial class Edit : Uc.PageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void Save()
        {

            string name = Request.Form["Name"];
            var entity = new Needs.Ccs.Services.Models.Referrer();

            //   var name = Request.Form["Name"];
            var result = new Needs.Ccs.Services.Views.Origins.ReferrersOrigin();
            // var result = Needs.Wl.Admin.Plat.AdminPlat.Referrers.Any(x => x.Name == name && x.Status == Status.Normal);
            if (result.Any(x => x.Name == name && x.Status == Status.Normal))
            {
                Response.Write((new { success = false, message = "保存失败,引荐人名称已存在" }).Json());
                return;
            }
            var id = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
            entity.Name = name;
            entity.Summary = Request.Form["Summary"];
            entity.Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.ID ?? "Admin0000000282";
            entity.EnterError += Referrer_EnterError;
            entity.EnterSuccess += Referrer_EnterSuccess;
            entity.Enter();
        }


        /// <summary>
        ///判断名称是否重复
        /// </summary>
        /// <returns></returns>
        protected bool IsExitName()
        {
            var result = false;
            var name = Request.Form["Name"];
            result = Needs.Wl.Admin.Plat.AdminPlat.Referrers.Any(x => x.Name == name.Trim() && x.Status == Status.Normal);
            return result;
        }


        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Referrer_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Referrer_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}
