using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.CustomsQuarantine
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string Origin = Request.QueryString["Origin"];
            var quarantines = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsQuarantines.AsQueryable()
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (!string.IsNullOrEmpty(Origin))
            {
                Origin = Origin.Trim();
                quarantines = quarantines.Where(t => t.Origin==Origin);
            }
            Func<Needs.Ccs.Services.Models.CustomsQuarantine, object> convert = item => new
            {
                ID = item.ID,
                Origin = item.Origin,
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                EndDate = item.EndDate.ToString("yyyy-MM-dd"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                Summary = item.Summary
            };
            this.Paging(quarantines, convert);
        }
        /// <summary>
        /// 数据作废
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsQuarantines[id];
            if (del != null)
            {
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}