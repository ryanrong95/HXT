using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Project_bak
{
    /// <summary>
    /// 销售机会展示页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //var clients = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Where(item => item.Status == ActionStatus.Complete || item.Status == ActionStatus.Auditing);
                //var Manufacture = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures;
                //this.Model.Admin = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).Distinct().Json();
                //this.Model.Current = new NtErp.Crm.Services.Views.AdminTopView()[Needs.Erp.ErpPlot.Current.ID].Json();
                //this.Model.ClientData = clients.Select(item => new { item.ID, item.Name }).Json();
                //this.Model.Manufacture = Manufacture.Select(item => new { item.ID, item.Name }).OrderBy(item => item.Name).Json();
                //this.Model.ProjectType = EnumUtils.ToDictionary<ProjectType>().Select(item => new { value = item.Key, text = item.Value }).Json();
                //this.Model.Status = EnumUtils.ToDictionary<ProductStatus>().Select(item => new { value = item.Key, text = item.Value }).Json();
            }
        }


        ///// <summary>
        ///// 查询数据
        ///// </summary>
        //protected void data()
        //{
        //    string id = Request.QueryString["ID"];
        //    string projecttype = Request.QueryString["ProjectType"];
        //    string clientid = Request.QueryString["ClientID"];
        //    string AdminID = Request.QueryString["AdminID"];
        //    string ManufactureID = Request.QueryString["ManufactureID"];
        //    var status = Request.QueryString["Status"];
        //    var StartDate = Request.QueryString["StartDate"];
        //    var EndDate = Request.QueryString["EndDate"];
        //    var data = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects;

        //    List<LambdaExpression> lamdas = new List<LambdaExpression>();
        //    Expression<Func<ProjectDossier, bool>> expression = item => true;
        //    #region 页面查询条件
        //    if (!string.IsNullOrWhiteSpace(id))
        //    {
        //        Expression<Func<NtErp.Crm.Services.Models.Project, bool>> lambda1 = item => item.ID.Contains(id);
        //        lamdas.Add(lambda1);
        //    }
        //    if (!string.IsNullOrWhiteSpace(projecttype))
        //    {
        //        Expression<Func<NtErp.Crm.Services.Models.Project, bool>> lambda1 = item => item.Type == (ProjectType)int.Parse(projecttype);
        //        lamdas.Add(lambda1);
        //    }
        //    if (!string.IsNullOrWhiteSpace(clientid))
        //    {
        //        Expression<Func<NtErp.Crm.Services.Models.Project, bool>> lambda1 = item => item.Client.ID == clientid;
        //        lamdas.Add(lambda1);
        //    }
        //    if (!string.IsNullOrWhiteSpace(AdminID))
        //    {
        //        Expression<Func<NtErp.Crm.Services.Models.Project, bool>> lambda1 = item => item.Admin.ID == AdminID;
        //        lamdas.Add(lambda1);
        //    }
        //    if(!string.IsNullOrWhiteSpace(status))
        //    {
        //        var ids = data.GetProductIds((ProductStatus)int.Parse(status), StartDate, EndDate);
        //        Expression<Func<NtErp.Crm.Services.Models.Project, bool>> lambda1 = item => ids.Contains(item.ID);
        //        lamdas.Add(lambda1);
        //    }

        //    if (!string.IsNullOrWhiteSpace(ManufactureID))
        //    {
        //        var ids = data.MapCondition(item => item.ManufacturerID == ManufactureID);
        //        expression = item => ids.Contains(item.Project.ID);
        //    }
        //    #endregion

        //    #region 拼凑页面需要数据
        //    int page, rows;
        //    int.TryParse(Request.QueryString["page"], out page);
        //    int.TryParse(Request.QueryString["rows"], out rows);

        //    var projectdossiers = data.GetPageList(page, rows, expression, lamdas.ToArray());
            
        //    Response.Write(new
        //    {
        //        rows = projectdossiers.Select(
        //                item => new
        //                {
        //                    item.Project.ID,
        //                    item.Project.Name,
        //                    item.Project.Status,
        //                    item.Project.Admin.JobType,
        //                    item.Project.UpdateDate,
        //                    ClientName = item.Project.Client.Name,
        //                    CompanyName = item.Project.Company.Name,
        //                    TypeName = item.Project.Type.GetDescription(),
        //                    CurrencyName = item.Project.Currency.GetDescription(),
        //                    RefValuation = item.Products.Sum(items => items.RefTotalPrice),
        //                    ExpectValuation = item.Products.Sum(items => items.ExpectTotal),
        //                    StatusName = item.Project.Status.GetDescription(),
        //                    ClientAdminName = string.Join(",", item.ClientAdmins.Select(a => a.RealName).ToArray()),
        //                    AdminName = item.Project.Admin.RealName,
        //                }
        //             ).ToArray(),
        //        total = projectdossiers.Total,
        //    }.Json());
        //    #endregion
        //}

        ///// <summary>
        ///// 删除
        ///// </summary>
        //protected void Delete()
        //{
        //    string id = Request.Form["ID"];
        //    var projectdossier = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.GetTop(1, item => item.Project.ID == id).SingleOrDefault();
        //    if (projectdossier != null)
        //    {
        //        projectdossier.Project.AbandonSuccess += Del_AbandonSuccess;
        //        projectdossier.Project.Abandon();
        //    }
        //}

        ///// <summary>
        ///// 操作成功触发事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        //{
        //    Alert("操作成功!");
        //}

        ///// <summary>
        ///// 申请
        ///// </summary>
        //protected void Apply()
        //{
        //    string id = Request.Form["ID"];
        //    var apply = new NtErp.Crm.Services.Models.Apply();
        //    apply.MainID = id;
        //    apply.Type = ApplyType.Project;
        //    apply.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
        //    apply.Summary = "销售机会申请";
        //    apply.Enter();

        //    //更新plan状态
        //    var projectdossier = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.GetTop(1, item => item.Project.ID == id).SingleOrDefault();
        //    var project = projectdossier.Project as NtErp.Crm.Services.Models.Project ??
        //        new NtErp.Crm.Services.Models.Project();
        //    project.Status = ActionStatus.Auditing;
        //    project.Enter();
        //}
    }
}