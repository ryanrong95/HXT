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
    /// 销售机会跟踪记录编辑展示页面
    /// </summary>
    public partial class ReportList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                data();
            }
        }
        protected void data()
        {
            string ProjectID = Request.QueryString["ProjectID"];
            string ReportID = Request.QueryString["ReportID"];
            if (!string.IsNullOrWhiteSpace(ReportID)) //跟踪记录ID
            {
                var pro = new NtErp.Crm.Services.Views.ReportsAlls()[ReportID];
                if (pro != null)
                {
                    ProjectID = pro.Project.ID;
                }

            }
            this.Model.ProjectName = "";   //销售机会名称
            this.Model.ProjectOwner = "";  //销售机会所有人
            var project = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.GetTop(1, item => item.Project.ID == ProjectID).SingleOrDefault();
            if (project != null)
            {
                this.Model.ProjectName = project.Project.Name;
                this.Model.ProjectOwner = project.Project.AdminName;
            }
            var report = new NtErp.Crm.Services.Views.ReportsAlls().Where(c => c.Project.ID == ProjectID);
            var data = report.ToList();

            this.Model.AllData = data.Select(c => new
            {
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Date,
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).NextDate,
                OriginalStaffs = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).OriginalStaffs ?? string.Empty,
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Plan,
                AdminName = c.Admin.RealName,
                TypeName = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Type.GetDescription(),
                NextTypeName = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).NextType.GetDescription(),
                Context = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Content,
                c.ID,
                IsEdit = (DateTime.Now - c.CreateDate).Days <= 7 && c.Admin.ID == Needs.Erp.ErpPlot.Current.ID,
                Reply = new NtErp.Crm.Services.Views.ReplyAlls().Where(item => item.ReportID == c.ID).OrderBy(a => a.UpdateDate).Select(item => new
                {
                    item.Admin.RealName,
                    item.UpdateDate,
                    item.Context
                }).ToArray(),
                File = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ReportID == c.ID && item.Status == Status.Normal).
                Select(item => new
                {
                    URL = item.Url,
                    Name = item.Name
                }).ToArray(),
            }).Json();
        }
    }
}