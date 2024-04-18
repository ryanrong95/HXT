using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Ccs.ServiceApplies.Apply
{
    public partial class UnDealList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadData()
        {
            var status = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.HandleStatus>().Select(item => new { item.Key, item.Value });
            this.Model.Status = status.Json();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string CompanyName = Request.QueryString["CompanyName"];
            string CreateDateFrom = Request.QueryString["CreateDateFrom"];
            string CreateDateTo = Request.QueryString["CreateDateTo"];

            var applies = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyServiceApplies.AsQueryable();
            applies = applies.Where(t => t.Status == Needs.Ccs.Services.Enums.HandleStatus.Pending);
            if (!string.IsNullOrEmpty(CompanyName))
            {
                applies = applies.Where(t => t.CompanyName.Contains(CompanyName));
            }
            if (!string.IsNullOrEmpty(CreateDateFrom))
            {
                var from = DateTime.Parse(CreateDateFrom);
                applies = applies.Where(t => t.CreateDate >= from);
            }
            if (!string.IsNullOrEmpty(CreateDateTo))
            {
                var to = DateTime.Parse(CreateDateTo).AddDays(1);
                applies = applies.Where(t => t.CreateDate <= to);
            }
            Func<Needs.Ccs.Services.Models.ServiceApplies, object> convert = apply => new
            {
                ID = apply.ID,
                Email = apply.Email,
                CompanyName = apply.CompanyName,
                Address = apply.Address,
                Contact = apply.Contact,
                Mobile = apply.Mobile,
                Tel = apply.Tel,
                Admin = apply.Admin?.RealName,
                Status = apply.Status.GetDescription(),
                CreateDate = apply.CreateDate.ToShortDateString(),
                UpdateDate = apply.UpdateDate,
                Summary = apply.Summary

            };

            this.Paging(applies, convert);
        }

        protected void SaveApplyHandle()
        {
            string ID = Request.Form["ID"];
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyServiceApplies[ID];
            apply.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            apply.Status = Needs.Ccs.Services.Enums.HandleStatus.Processed;
            apply.EnterSuccess += Apply_EnterSuccess;
            apply.EnterError += Apply_EnterError;
            apply.Enter();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}