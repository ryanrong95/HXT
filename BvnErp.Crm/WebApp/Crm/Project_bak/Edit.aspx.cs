using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Project_bak
{
    /// <summary>
    /// 销售机会编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {

        private ApplyType ApplyType;

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //LoadComboBoxData();
                //LoadData();
            }
        }

        #region 加载显示数据
        ///// <summary>
        ///// 初始化下拉框数据
        ///// </summary>
        //protected void LoadComboBoxData()
        //{
        //    //var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(10000, item => item.Client.Status == ActionStatus.Complete
        //    //    || item.Client.Status == ActionStatus.Auditing).Select(item => new { item.Client.ID, item.Client.Name });
        //    var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientsBase.Where(item => item.Status == ActionStatus.Complete
        //        || item.Status == ActionStatus.Auditing).Select(item => new { item.ID, item.Name });
        //    this.Model.CompanyData = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item => item.Type == CompanyType.plot).
        //        Select(item => new { item.ID, item.Name }).OrderBy(item => item.Name).Json();
        //    this.Model.Vender = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures.Select(item => new { VendorID = item.ID, VendorName = item.Name }).
        //        OrderBy(item => item.VendorName).Json();
        //    this.Model.ClientData = client.Json();
        //    this.Model.Currency = EnumUtils.ToDictionary<CurrencyType>().Select(item => new { value = item.Key, text = item.Value }).Json();
        //    this.Model.Status = EnumUtils.ToDictionary<ProductStatus>().Select(item => new { Status = item.Key, StatusName = item.Value }).Json();
        //    this.Model.Type = EnumUtils.ToDictionary<ProjectType>().Select(item => new { value = item.Key, text = item.Value }).Json();
        //    this.Model.Industries = new NtErp.Crm.Services.Views.IndustryTree().tree;
        //    this.Model.Admins = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).Json();
        //}

        ///// <summary>
        ///// 加载数据
        ///// </summary>
        //protected void LoadData()
        //{
        //    string id = Request.QueryString["ID"];
        //    var projectdossier = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.GetTop(1, item => item.Project.ID == id).SingleOrDefault();
        //    if (projectdossier != null)
        //    {
        //        this.Model.Project = new
        //        {
        //            projectdossier.Project.ID,
        //            projectdossier.Project.Name,
        //            projectdossier.Project.Type,
        //            ClientID = projectdossier.Project.Client.ID,
        //            ClientName = projectdossier.Project.Client.Name,
        //            CompanyID = projectdossier.Project.Company.ID,
        //            Industry = projectdossier.Industries.SingleOrDefault()?.ID,
        //            projectdossier.Project.Currency,
        //            projectdossier.Project.Valuation,
        //            projectdossier.Project.StartDate,
        //            projectdossier.Project.EndDate,
        //            projectdossier.Project.Summary,
        //        }.Json();
        //    }
        //    else
        //    {
        //        this.Model.Project = projectdossier.Json();
        //    }

        //}

        ///// <summary>
        ///// 查询列表数据
        ///// </summary>
        //protected void data()
        //{
        //    string id = Request.QueryString["ID"];
        //    var products = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.MyProducts(id);

        //    Func<NtErp.Crm.Services.Models.ProductItem, object> convert = item => new
        //    {
        //        item.ID,
        //        item.standardProduct.Name,
        //        item.RefUnitQuantity,
        //        item.RefQuantity,
        //        item.RefUnitPrice,
        //        item.RefTotalPrice,
        //        item.ExpectRate,
        //        item.ExpectTotal,
        //        item.Quantity,
        //        item.UnitPrice,
        //        item.TotalPrice,
        //        item.Status,
        //        item.Count,
        //        CompeteManu=item.CompeteProduct?.ManufacturerID,
        //        CompeteModel=item.CompeteProduct?.Name,
        //        CompetePrice=item.CompeteProduct?.UnitPrice,
        //        item.OriginNumber,
        //        StatusName = item.Status.GetDescription(),
        //        VendorID = item.standardProduct.Manufacturer.ID,
        //        VendorName = item.standardProduct.Manufacturer.Name,
        //        item.FAEAdminID,
        //        item.FAEAdminName,
        //        item.PMAdminID,
        //        item.PMAdminName,
        //        item.ExpectDate,
        //        item.IsApr,
        //    };
        //    this.Paging(products, convert);
        //}
        #endregion


        #region 数据保存
        ///// <summary>
        ///// 销售机会数据保存
        ///// </summary>
        //protected void Save()
        //{
        //    string id = Request.QueryString["ID"];
        //    var project = new NtErp.Crm.Services.Views.ProjectAlls()[id] ?? new NtErp.Crm.Services.Models.Project();
        //    project.Name = Request.Form["Name"];
        //    project.Company = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create(Request.Form["CompanyID"]);
        //    project.Currency = (CurrencyType)int.Parse(Request.Form["Currency"]);
        //    if (project.Admin == null)
        //    {
        //        project.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
        //    }
        //    if (!string.IsNullOrWhiteSpace(Request.Form["Type"]))
        //    {
        //        project.Type = (ProjectType)int.Parse(Request.Form["Type"]);
        //    }
        //    if (!string.IsNullOrWhiteSpace(Request.Form["ClientID"]))
        //    {
        //        project.Client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(Request.Form["ClientID"]);
        //    }
        //    if (!string.IsNullOrWhiteSpace(Request.Form["StartDate"]))
        //    {
        //        project.StartDate = Convert.ToDateTime(Request.Form["StartDate"]);
        //    }
        //    if (!string.IsNullOrWhiteSpace(Request.Form["EndDate"]))
        //    {
        //        project.EndDate = Convert.ToDateTime(Request.Form["EndDate"]);
        //    }
        //    project.Summary = Request.Form["Summary"];
        //    project.Status = ActionStatus.Complete;
        //    project.EnterSuccess += Project_EnterSuccess;
        //    project.Enter();
        //}

        ///// <summary>
        ///// 产品列表数据保存
        ///// </summary>
        //protected void SaveList(string projectid)
        //{
        //    string data = Request.QueryString["data"].Replace("&quot;", "\"");
        //    string message = "保存成功";
        //    var products = data.JsonTo<List<NtErp.Crm.Services.Models.ProductExtends>>();
        //    foreach (var product in products)
        //    {
        //        try
        //        {
        //            var productItem = new NtErp.Crm.Services.Views.ProductItemAlls()[product.ID] as NtErp.Crm.Services.Models.ProductItem ??
        //                new NtErp.Crm.Services.Models.ProductItem();
        //            productItem.RefQuantity = product.RefQuantity;
        //            productItem.RefUnitQuantity = product.RefUnitQuantity;
        //            productItem.RefUnitPrice = product.RefUnitPrice;
        //            productItem.Quantity = product.Quantity;
        //            productItem.UnitPrice = product.UnitPrice;
        //            productItem.OriginNumber = product.OriginNumber;
        //            productItem.Count = product.Count;
        //            productItem.PMAdminID = product.PMAdminID;
        //            productItem.FAEAdminID = product.FAEAdminID;
        //            if((product.Status > ProductStatus.DO) && product.Status != productItem.Status)
        //            {
        //                ApplyType = (ApplyType)product.Status;
        //            }
        //            else
        //            {
        //                productItem.Status = product.Status;
        //                productItem.ExpectRate = product.ExpectRate;
        //            }
        //            productItem.ExpectDate = product.ExpectDate;
        //            if (productItem.standardProduct == null)
        //            {
        //                productItem.standardProduct = new NtErp.Crm.Services.Models.StandardProduct();
        //            }
        //            if (productItem.CompeteProduct == null)
        //            {
        //                productItem.CompeteProduct = new NtErp.Crm.Services.Models.CompeteProduct();
        //            }
        //            productItem.standardProduct.Name = product.Name;
        //            productItem.standardProduct.Manufacturer = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create(product.VendorID);
        //            productItem.CompeteProduct.ManufacturerID = product.CompeteManu;
        //            productItem.CompeteProduct.Name = product.CompeteModel;
        //            productItem.CompeteProduct.UnitPrice = product.CompetePrice;
        //            productItem.EnterSuccess += ProductItem_EnterSuccess;
        //            productItem.Enter();
        //            //绑定项目
        //            Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.BindingItem(projectid, productItem);
        //            this.ApplyType = 0;
        //        }
        //        catch (Exception e)
        //        {
        //            message += e.Message;
        //            continue;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 产品保存后触发事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ProductItem_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        //{
        //    if(this.ApplyType == ApplyType.DIApply || this.ApplyType == ApplyType.DWApply || this.ApplyType == ApplyType.MPApply)
        //    {
        //        var product = sender as NtErp.Crm.Services.Models.ProductItem;
        //        var apply = new NtErp.Crm.Services.Models.Apply();
        //        apply.MainID = product.ID;
        //        apply.Type = this.ApplyType;
        //        apply.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
        //        apply.Summary = "客户申请";
        //        apply.Status = ApplyStatus.Audting;
        //        apply.Enter();
        //    }
        //}

        //#region 按钮保存，暂时不用
        ///// <summary>
        ///// 保存
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ////protected void btnSave_Click(object sender, EventArgs e)
        ////{
        ////    string id = Request.QueryString["ID"];
        ////    var project = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects[id] as NtErp.Crm.Services.Models.Project ??
        ////        new NtErp.Crm.Services.Models.Project();
        ////    project.Name = Request.Form["Name"];
        ////    project.Client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(Request.Form["ClientID"]);
        ////    project.Company = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create(Request.Form["CompanyID"]);
        ////    if (!string.IsNullOrWhiteSpace(Request.Form["Valuation"]))
        ////    {
        ////        project.Valuation = decimal.Parse(Request.Form["Valuation"]);
        ////    }
        ////    project.Currency = (CurrencyType)int.Parse(Request.Form["Currency"]);
        ////    project.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
        ////    if (!string.IsNullOrWhiteSpace(Request.Form["StartDate"]))
        ////    {
        ////        project.StartDate = Convert.ToDateTime(Request.Form["StartDate"]);
        ////    }
        ////    if (!string.IsNullOrWhiteSpace(Request.Form["EndDate"]))
        ////    {
        ////        project.EndDate = Convert.ToDateTime(Request.Form["EndDate"]);
        ////    }
        ////    project.Summary = Request.Form["Summary"];
        ////    project.EnterSuccess += Project_EnterSuccess;
        ////    project.Enter();
        ////}
        //#endregion

        ///// <summary>
        ///// 保存成功触发时间
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Project_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(Request.Form["Industry"]))
        //    {
        //        var industry = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Industry>.Create(Request.Form["Industry"]);
        //        Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.BindingIndustry(e.Object, industry);
        //    }
        //    SaveList(e.Object);
        //}
        #endregion


        #region 自动审批
        ///// <summary>
        ///// 自动申请
        ///// </summary>
        //protected void AutoAppr(string id)
        //{
        //    var apply = new NtErp.Crm.Services.Models.Apply();
        //    apply.MainID = id;
        //    apply.Type = ApplyType.Project;
        //    apply.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
        //    apply.Summary = "销售机会申请";
        //    apply.Status = ApplyStatus.Approval;
        //    apply.EnterSuccess += Apply_EnterSuccess;
        //    apply.Enter();

        //}

        ///// <summary>
        ///// 审批操作
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Apply_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        //{
        //    var appr = new NtErp.Crm.Services.Models.ApplyStep();
        //    appr.ApplyID = e.Object;
        //    appr.AdminID = "Admin0000000038";
        //    appr.Step = (int)ApplyStep.Allow;
        //    appr.Status = ApplyStep.Allow;
        //    appr.Enter();
        //}
        #endregion
    }
}