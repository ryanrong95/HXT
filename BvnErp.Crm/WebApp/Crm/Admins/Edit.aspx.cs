using Needs.Utils.Serializers;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Enums;
using System;
using NtErp.Crm.Services.Extends;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NtErp.Crm.Services.Views;

namespace WebApp.Crm.Admins
{
    /// <summary>
    /// 管理员编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var company = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item => item.Type == CompanyType.plot).OrderBy(item => item.Name);
                this.Model.Company = company.Json();
                this.Model.JobData = EnumUtils.ToDictionary<JobType>().Select(item => new { value = item.Key, text = item.Value }).Json();
                this.Model.ScoreTypeData = EnumUtils.ToDictionary<ScoreType>().Select(item => new { value = item.Key, text = item.Value }).Json();
                LoadData();
            }
        }


        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var admins = new NtErp.Crm.Services.Views.AdminTopView()[id];
            LoadMyManufacture(id, (JobType)admins.JobType);
            this.Model.Admin = admins.Json();
        }

        /// <summary>
        /// 加载我的品牌
        /// </summary>
        /// <param name="JobType"></param>
        private void LoadMyManufacture(string id, JobType JobType)
        {
            var Manufacture = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures;

            var admin = new AdminProjectViewBase(JobType).GetTop(1, item => item.Admin.ID == id).SingleOrDefault();
            string[] vendors = admin?.Manufactures.Select(item => item.ID).ToArray();

            //获取我的品牌
            if (vendors != null)
            {
                this.Model.Manufacture = Manufacture.Select(item => new ShowModel
                {
                    ID = item.ID,
                    Name = item.Name,
                    Checked = vendors.Contains(item.ID) ? "checked" : "unchecked",
                }).OrderBy(item => item.Name);
            }
            else
            {
                this.Model.Manufacture = Manufacture.Select(item => new ShowModel
                {
                    ID = item.ID,
                    Name = item.Name,
                    Checked = "unchecked",
                }).OrderBy(item => item.Name);
            }
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            var adminproject = new NtErp.Crm.Services.Models.AdminProject()
            {
                AdminID = id,
                JobType = (JobType)int.Parse(Request.Form["JobType"]),
                Company = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create(Request.Form["CompanyID"]),
                ScoreType = (ScoreType)int.Parse(Request.Form["ScoreType"]),
                SalaryBase = decimal.Parse(Request.Form["SalaryBase"]),
                DyjID = Request.Form["DyjID"],
                Summary = Request.Form["Summary"],
            };
            adminproject.EnterSuccess += adminproject_EnterSuccess;
            adminproject.EnterError += Adminproject_EnterError;
            adminproject.Enter();
        }

        /// <summary>
        /// 保存失败事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adminproject_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert(e.Message, Request.Url, true);
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void adminproject_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            string[] manus = Request.Form["SelectedManu"].Split(',').ToArray();
            Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Binding(e.Object, manus);

            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }


        protected class ShowModel
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Checked { get; set; }
        }
    }
}