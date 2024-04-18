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

namespace WebApp.Crm.MyPlans
{
    public partial class Edit : Uc.PageBase
    {
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var MyPlan = Needs.Erp.ErpPlot.Current.ClientSolutions.MyPlans[id];
            if (MyPlan != null) {
                var data = (new
                {
                    Name = MyPlan.Name,
                    ClientID = MyPlan.client.ID,
                    CompanyID = MyPlan.Companys.ID,
                    Target = MyPlan.Target,
                    Methord = MyPlan.Methord,
                    PlanDate = MyPlan.PlanDate,
                    StartDate = MyPlan.StartDate,
                    EndDate = MyPlan.EndDate,
                    AdminID = MyPlan.Admin.ID,
                    Summary = MyPlan.Summary,
                    SaleID = MyPlan.SaleID,
                    SaleManagerID = MyPlan.SaleManagerID,
                }).Json();
                this.Model.Plan = data;
            }
            else
            {
                this.Model.Plan = "".Json();
            }
          
            
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            var plan = Needs.Erp.ErpPlot.Current.ClientSolutions.MyPlans[id] as NtErp.Crm.Services.Models.Plan ??
                new NtErp.Crm.Services.Models.Plan();
            string clientid = Request.Form["ClientID"];
            string companyid = Request.Form["CompanyID"];           
            plan.client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(clientid);
            plan.Companys = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create(companyid);

            #region 赋值
            plan.Name = Request.Form["Name"];
            //plan.ClientID = Request.Form["ClientID"];
            //plan.CompanyID = Request.Form["CompanyID"];
            plan.Target = (ActionTarget)int.Parse(Request.Form["Target"]);
            plan.Methord = (ActionMethord)int.Parse(Request.Form["Methord"]);
            if (!string.IsNullOrWhiteSpace(Request.Form["PlanDate"]))
            {
                plan.PlanDate = Convert.ToDateTime(Request.Form["PlanDate"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["StartDate"]))
            {
                plan.StartDate = Convert.ToDateTime(Request.Form["StartDate"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["EndDate"]))
            {
                plan.EndDate = Convert.ToDateTime(Request.Form["EndDate"]);
            }
            plan.AdminID = Request.Form["AdminID"];
            plan.Summary = Request.Form["Summary"];
            plan.SaleID = Request.Form["SaleID"];
            plan.SaleManagerID = Request.Form["SaleManagerID"];
            #endregion

            plan.EnterSuccess += Plan_EnterSuccess;
            plan.Enter();
        }

        /// <summary>
        /// 保存成功关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Plan_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }


        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var company = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item=>item.Type == CompanyType.plot);
            var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(10000,item => item.Client.Status == ActionStatus.Complete);
            var admin = new [] { new {
                ID = Needs.Erp.ErpPlot.Current.ID,
                Name = Needs.Erp.ErpPlot.Current.UserName
            } };
            this.Model.CompanyData = company.Json();
            this.Model.ClientData = client.Select(item=> new { text=item.Client.Name,value = item.Client.ID}).Json();
            this.Model.AdminData = admin.Json();
            this.Model.ActionMethord = EnumUtils.ToDictionary<ActionMethord>().Select(item => new { text = item.Value, value = item.Key }).Json();
            this.Model.ActionTarget = EnumUtils.ToDictionary<ActionTarget>().Select(item => new { text = item.Value, value = item.Key }).Json();
            var admins = new NtErp.Crm.Services.Views.AdminTopView();
        }

    }
}