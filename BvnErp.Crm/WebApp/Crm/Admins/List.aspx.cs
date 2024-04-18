using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Admins
{
    /// <summary>
    /// 管理员列表界面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.JobData = EnumUtils.ToDictionary<JobType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            }
        }

        protected void data()
        {
            string UserName = Request.QueryString["UserName"];
            string RealName = Request.QueryString["RealName"];
            string JobType = Request.QueryString["JobType"];
            var admins = new NtErp.Crm.Services.Views.AdminTopView().AsQueryable();

            //筛选页面需要的数据
            Func<NtErp.Crm.Services.Models.AdminTop, object> convert = item => new
            {
                item.ID,
                item.UserName,
                item.RealName,
                item.JobType,
                item.IsAgree,
                item.WXID,
                JobTypeName = item.JobType == 0 ? string.Empty : item.JobType.GetDescription(),
                item.CreateDate,
            };

            if (!string.IsNullOrWhiteSpace(UserName))
            {
                admins = admins.Where(item => item.UserName.Contains(UserName));
            }
            if (!string.IsNullOrWhiteSpace(RealName))
            {
                admins = admins.Where(item => item.RealName.Contains(RealName));
            }
            if (!string.IsNullOrWhiteSpace(JobType))
            {
                admins = admins.Where(item => item.JobType == (JobType)int.Parse(JobType));
            }

            this.Paging(admins, convert);
        }

        /// <summary>
        /// 授权微信
        /// </summary>
        protected void Auth()
        {
            string id = Request.Form["ID"];
            bool isagree = Boolean.Parse(Request.Form["IsAgree"]);
            var adminproject = new NtErp.Crm.Services.Models.AdminProject
            {
                AdminID = id,
                IsAgree = isagree,
            };
            adminproject.IsAgreeUpdate();
        }
    }
}