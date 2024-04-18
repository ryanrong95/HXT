using Needs.Underly;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Project
{
    public partial class Show : Uc.PageBase
    {
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
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
            var projectdossier = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.GetTop(1000,item=>item.Project.ID==id).SingleOrDefault();
            this.Model.Project = new {
                projectdossier.Project.ID,
                projectdossier.Project.Name,
                ClientID = projectdossier. Project.Client.ID,
                CompanyID = projectdossier.Project.Company.ID,
                projectdossier.Project.Currency,
                projectdossier.Project.StartDate,
                projectdossier.Project.EndDate,
                projectdossier.Project.Summary
            }.Json();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var company = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item=>item.Type == CompanyType.plot);
            var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(1000,item => item.Client.Status == ActionStatus.Normal);
            var currency = Needs.Utils.Descriptions.EnumUtils.ToDictionary<CurrencyType>();
            this.Model.CompanyData = company.Select(item => new { item.ID, item.Name }).Json();
            this.Model.ClientData = client.Select(item => new { item.Client.ID, item.Client.Name }).Json();
            this.Model.Currency = currency.Select(item => new { value = item.Key, text = item.Value }).Json();
        }
    }
}