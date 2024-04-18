using Needs.Utils.Descriptions;
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
    /// <summary>
    /// 销售机会编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
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

        #region 加载显示数据
        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var clientid = Request.QueryString["ClientID"];
            this.Model.Client = new NtErp.Crm.Services.Views.ClientAlls()[clientid].Json();
            this.Model.ClientData = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Select(item => new { item.ID, item.Name }).Json();
            this.Model.CompanyData = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item => item.Type == CompanyType.plot).
                Select(item => new { item.ID, item.Name }).OrderBy(item => item.Name).Json();
            this.Model.Currency = EnumUtils.ToDictionary<CurrencyType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Industries = new NtErp.Crm.Services.Views.IndustryAlls().Where(item => item.FatherID == null).
                Select(item => new { item.ID, item.Name }).Json();
            this.Model.Type = EnumUtils.ToDictionary<ProjectType>().Select(item => new { value = item.Key, text = item.Value }).Json();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ProjectID"];
            var project = new NtErp.Crm.Services.Views.ProjectAlls().SearchByID(id);
            if (project != null)
            {
                this.Model.Project = new
                {
                    project.ID,
                    project.ClientID,
                    project.CompanyID,
                    project.Currency,
                    project.Name,
                    project.ProductName,
                    project.Type,
                    ModelDate = project.ModelDate?.ToString(),
                    ProductDate = project.ProductDate?.ToString(),
                    project.MonthYield,
                    StartDate = project.StartDate?.ToString(),
                    EndDate = project.EndDate?.ToString(),
                    project.Contactor,
                    project.Phone,
                    project.Address,
                    Industry = project.Industry?.ID,
                    project.Summary,
                }.Json();
            }
            else
            {
                this.Model.Project = string.Empty.Json();
            }

        }

        /// <summary>
        /// 查询列表数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ProjectID"];
            var products = new NtErp.Crm.Services.Views.ProductItemAlls().SearchByProjectID(id).OrderByDescending(item => item.UpdateDate);

            Func<NtErp.Crm.Services.Models.ProductItem, object> convert = item => new
            {
                item.ID,
                item.standardProduct.Name,
                item.standardProduct.Origin,
                VendorName = item.standardProduct.Manufacturer.Name,
                item.RefUnitQuantity,
                item.RefQuantity,
                item.RefUnitPrice,
                item.Status,
                StatusName = item.Status.GetDescription(),
                item.ExpectRate,
                ExpectDate = item.ExpectDate?.ToString("yyyy-MM-dd"),
                item.ExpectQuantity,
                item.ExpectTotal,
                CompeteManu = item.CompeteProduct?.ManufacturerID,
                CompeteModel = item.CompeteProduct?.Name,
                CompetePrice = item.CompeteProduct?.UnitPrice,
                item.Summary,
                item.Files,
                FileName = item.Files?.SingleOrDefault(f => f.Type == NtErp.Crm.Services.Models.FileType.Item)?.Name,
                FileUrl = item.Files?.SingleOrDefault(f => f.Type == NtErp.Crm.Services.Models.FileType.Item)?.Url,

                item.FAEAdminName,
                item.PMAdminName,
                item.SaleAdminName,
                item.AssistantAdiminName,
                item.PurchaseAdminName,
                item.IsApr,

                IsSample = item.Sample?.ID != null ? "是" : "否",
                SampleType = item.Sample?.Type.GetDescription(),
                SampleDate = item.Sample?.Date.ToString("yyyy-MM-dd"),
                SampleQuantity = item.Sample?.Quantity,
                SamplePrice = item.Sample?.UnitPrice,
                SampleTotalPrice = item.Sample?.TotalPrice,
                SampleContactor = item.Sample?.Contactor,
                SamplePhone = item.Sample?.Phone,
                SampleAddress = item.Sample?.Address,

                item.Enquiry?.MOQ,
                item.Enquiry?.MPQ,
                ReportDate = item.Enquiry?.ReportDate.ToString("yyyy-MM-dd"),
                EnquiryValidity = item.Enquiry?.Validity.ToString("yyyy-MM-dd"),
                EnquiryValidityCount = item.Enquiry?.ValidityCount,
                EnquirySalePrice = item.Enquiry?.SalePrice,
                EnquirySummary = item.Enquiry?.Summary,
            };
            this.Paging(products, convert);
        }
        #endregion


        #region 数据保存
        /// <summary>
        /// 销售机会数据保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ProjectID"];
            var project = new NtErp.Crm.Services.Views.ProjectAlls()[id] ?? new NtErp.Crm.Services.Models.Project();
            project.Name = Request.Form["Name"].Trim();
            project.CompanyID = Request.Form["CompanyID"];
            project.Currency = (CurrencyType)int.Parse(Request.Form["Currency"]);
            project.ProductName = Request.Form["ProductName"].Trim();
            project.ClientID = Request["ClientID"];

            if (string.IsNullOrEmpty(id))
            {
                var projectAlls = new NtErp.Crm.Services.Views.ProjectAlls().SearchByClientID(Request["ClientID"]).Where(item => item.Name == Request["Name"]);
                if (projectAlls.Count() > 0)
                {
                    this.Alert("您所要新增的项目名在当前的客户名称下已存在!", Request.UrlReferrer ?? Request.Url, true);
                }
            }
            project.Contactor = Request.Form["Contactor"];
            project.Phone = Request.Form["Phone"];
            project.Address = Request.Form["Address"];
            if (!string.IsNullOrWhiteSpace(Request.Form["ModelDate"]))
            {
                project.ModelDate = Convert.ToDateTime(Request.Form["ModelDate"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["ProductDate"]))
            {
                project.ProductDate = Convert.ToDateTime(Request.Form["ProductDate"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["MonthYield"]))
            {
                project.MonthYield = int.Parse(Request.Form["MonthYield"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["Type"]))
            {
                project.Type = (ProjectType)int.Parse(Request.Form["Type"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["StartDate"]))
            {
                project.StartDate = Convert.ToDateTime(Request.Form["StartDate"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["EndDate"]))
            {
                project.EndDate = Convert.ToDateTime(Request.Form["EndDate"]);
            }
            if (project.AdminID == null)
            {
                project.AdminID = Needs.Erp.ErpPlot.Current.ID;
            }
            project.Summary = Request.Form["Summary"];
            project.Status = ActionStatus.Complete;
            project.EnterSuccess += Project_EnterSuccess;
            project.Enter();
        }

        /// <summary>
        /// 保存成功触发时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Project_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.Form["Industry"]))
            {
                var industry = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Industry>.Create(Request.Form["Industry"]);
                Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.BindingIndustry(e.Object, industry);
            }
            var entity = (NtErp.Crm.Services.Models.Project)sender;
            var url = Request.UrlReferrer ?? Request.Url;
            var path = url.OriginalString;
            if (string.IsNullOrWhiteSpace(Request.QueryString["ProjectID"]))
            {
                if (string.IsNullOrWhiteSpace(Request.QueryString["ClientID"]))
                {
                    path = path + "?ClientID=" + entity.ClientID;
                }
                path = path + "&ProjectID=" + e.Object;
            }

            Alert("保存成功", path, false);
        }
        #endregion
    }
}