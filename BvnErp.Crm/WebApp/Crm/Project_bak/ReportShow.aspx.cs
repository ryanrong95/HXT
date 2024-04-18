using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Project_bak
{
    /// <summary>
    /// 销售机会跟踪记录展示页面
    /// </summary>
    public partial class ReportShow : Uc.PageBase
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
            //string ProjectID = Request.QueryString["ProjectID"];
            //string ReportID = Request.QueryString["ReportID"];
            //if (!string.IsNullOrWhiteSpace(ReportID)) //跟踪记录ID
            //{
            //    var pro = new ReportsAlls()[ReportID];
            //    if (pro != null)
            //    {
            //        ProjectID = pro.Project.ID;
            //    }

            //}
            //this.Model.ProjectName = "";   //销售机会名称
            //this.Model.ProjectOwner = "";  //销售机会所有人
            //var project = new ProjectAlls()[ProjectID];
            //if (project != null)
            //{
            //    this.Model.ProjectName = project.Name;
            //    this.Model.ProjectOwner = project.Admin.RealName;
            //}
            //var report = new ReportsAlls().Where(c => c.Project.ID == ProjectID);
            //var data = report.ToList();

            //this.Model.AllData = data.Select(c => new
            //{
            //    JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Date,
            //    JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).NextDate,
            //    JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Plan,
            //    OriginalStaffs = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).OriginalStaffs ?? string.Empty,
            //    AdminName = c.Admin.RealName,
            //    TypeName = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Type.GetDescription(),
            //    NextTypeName = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).NextType.GetDescription(),
            //    Context = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(c.Context).Content,
            //    c.ID,
            //    IsOwner = c.Admin.ID == Needs.Erp.ErpPlot.Current.ID,
            //    Reply = new NtErp.Crm.Services.Views.ReplyAlls().Where(item => item.ReportID == c.ID).OrderBy(a => a.UpdateDate).Select(item => new
            //    {
            //        item.Admin.RealName,
            //        item.UpdateDate,
            //        item.Context
            //    }).ToArray(),
            //    File = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ReportID == c.ID && item.Status == Status.Normal).
            //    Select(item => new
            //    {
            //        URL = item.Url,
            //        Name = item.Name
            //    }).ToArray(),
            //    //isLeader= GetStaffs(Needs.Erp.ErpPlot.Current.ID).Contains(c.Admin.ID) //是否为上级人员
            //}).Json();
        }

        /// <summary>
        /// 获取员工集合
        /// </summary>
        /// <param name="AdminID">人员ID</param>
        /// <returns></returns>
        private IEnumerable<string> GetStaffs(string AdminID)
        {
            List<string> list = new List<string>() { AdminID };

            var admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(AdminID);
            if (admin.JobType == NtErp.Crm.Services.Enums.JobType.TPM)
            {
                return new string[0];
            }
            else
            {
                //获取所有员工
                var Mystaffids = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Select(item => item.ID).ToArray();
                return Mystaffids.Except(list);
            }
        }
    }
}