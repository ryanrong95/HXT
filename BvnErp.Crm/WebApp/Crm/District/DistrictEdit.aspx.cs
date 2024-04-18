using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.District
{
    /// <summary>
    /// 区域新增页面
    /// </summary>
    public partial class DistrictEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var Distincts = new NtErp.Crm.Services.Views.DistrictAlls().Where(item => item.Status == NtErp.Crm.Services.Enums.Status.Normal).
                    Select(item => new { item.ID, item.Name,item.Level});
                this.Model.Distinct = Distincts.Json();
            }

        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string Level = Request.Form["Level"];
            var distinct = new NtErp.Crm.Services.Models.District();
            distinct.Name = Request.Form["Name"];
            distinct.FatherID = Request.Form["Father"];
            distinct.Level = string.IsNullOrWhiteSpace(Level) ? 11 : int.Parse(Level) + 1;
            distinct.EnterSuccess += Distinct_EnterSuccess;
            distinct.Enter();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Distinct_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }
    }
}