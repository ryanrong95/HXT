using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Trace
{
    /// <summary>
    /// 跟踪记录添加页面
    /// </summary>
    public partial class Add : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.Path = Request.ApplicationPath.Json();
            this.Model.TypeData = EnumUtils.ToDictionary<ActionMethord>().Select(item => new { text = item.Value, value = item.Key }).Json();
            this.Model.Admins = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).Json();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var report = new NtErp.Crm.Services.Views.ReportsAlls()[id];
            if (report != null)
            {
                this.Model.AllData = report.Context;
                var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ReportID == report.ID && item.Status == Status.Normal);
                this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
                this.Model.Readers = string.Join(",", report.Readers).Json();
            }
            else
            {
                this.Model.AllData = string.Empty.Json();
                this.Model.Files = string.Empty.Json();
                this.Model.Readers = string.Empty.Json();
            }
        }

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {
            string ClientID = Request.QueryString["ClientID"]; //客户ID
            string Id = Request.QueryString["ID"];
            var report = new NtErp.Crm.Services.Views.ReportsAlls()[Id] ?? new NtErp.Crm.Services.Models.Report();
            report.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            report.Client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(ClientID);
            report.Content = Request.Form["editorValue"];
            report.OriginalStaffs = Request.Form["OriginalStaffs"];
            report.Type = (ActionMethord)int.Parse(Request.Form["Type"]);
            report.Date = DateTime.Parse(Request.Form["Date"]);
            report.NextDate = DateTime.Parse(Request.Form["NextDate"]);
            report.Status = Status.Normal;
            report.EnterSuccess += Report_EnterSuccess;
            report.Enter();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Report_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            //指定阅读人处理
            Readers(e.Object);
            SaveList(e.Object);
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        /// <summary>
        /// 指定阅读人处理
        /// </summary>
        /// <param name="reportid"></param>
        private void Readers(string reportid)
        {
            var reportview = new NtErp.Crm.Services.Views.ReportsAlls();
            var report = reportview[reportid];
            reportview.DeleteBinding(report);
            if (string.IsNullOrWhiteSpace(Request.Form["Reader"]))
            {
                return;
            }
            string[] readers = Request.Form["Reader"].Split(',').ToArray();
            foreach (string reader in readers)
            {
                //新增绑定
                var admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(reader);
                reportview.BindingReader(report, admin);
                //提醒阅读人
                var warning = new NtErp.Crm.Services.Views.WorkWarningsAlls().Where(item => item.MainID == report.ID && item.Type ==
                   WarningType.ClientReportReadWarning && item.Admin.ID == admin.ID).SingleOrDefault() ?? new NtErp.Crm.Services.Models.WorkWarning();
                warning.MainID = report.ID;
                warning.Type = WarningType.ClientReportReadWarning;
                warning.Resource = Needs.Erp.ErpPlot.Current.RealName;
                warning.Admin = admin;
                warning.Summary = "客户的跟踪记录指定阅读人阅读";
                warning.Enter();
            }
        }

        /// <summary>
        /// 附件数据保存到数据库中
        /// </summary>
        /// <param name="reportid">跟踪记录ID</param>
        protected void SaveList(string reportid)
        {
            try
            {
                //删除原来的附件
                var reportall = new NtErp.Crm.Services.Views.ReportsAlls();
                reportall.DeleteFiles(reportid);
                if (string.IsNullOrWhiteSpace(Request.QueryString["fileNames"]))
                {
                    return;
                }
                string[] filenames = Request.QueryString["fileNames"].Split(';');
                string[] urls = Request.QueryString["filePaths"].Split(';');
                var Report = reportall[reportid];
                var Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
                for (var i = 0; i < urls.Length - 1; i++)
                {
                    var file = new NtErp.Crm.Services.Models.File();
                    file.Report = Report;
                    file.Admin = Admin;
                    file.Url = urls[i];
                    file.Name = filenames[i];
                    file.Enter();
                }
            }
            catch (Exception ex)
            {
                this.Alert("发生错误：" + ex.Message.ToString());
                return;
            }
        }
        #endregion

        #region 暂存
        /// <summary>
        /// 数据暂存
        /// </summary>
        protected void SaveTempData()
        {
            var admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            string Id = Request.Form["ID"];
            string ClientID = Request.Form["ClientID"];
            var report = new NtErp.Crm.Services.Views.ReportsAlls()[Id] ?? new NtErp.Crm.Services.Models.Report();
            report.Admin = admin;
            report.Client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(ClientID);
            report.Content = Request.Form["Content"];
            report.OriginalStaffs = Request.Form["OriginalStaffs"];
            report.Status = Status.Temporary;
            if (!string.IsNullOrWhiteSpace(Request.Form["Type"]))
                report.Type = (ActionMethord)int.Parse(Request.Form["Type"]);
            else
                report.Type = null;
            if (!string.IsNullOrWhiteSpace(Request.Form["Date"]))
                report.Date = DateTime.Parse(Request.Form["Date"]);
            else
                report.Date = null;
            if (!string.IsNullOrWhiteSpace(Request.Form["NextDate"]))
                report.NextDate = DateTime.Parse(Request.Form["NextDate"]);
            else
                report.NextDate = null;
            report.EnterSuccess += Report_TempEnterSuccess1;
            report.Enter();
        }

        /// <summary>
        /// 暂存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Report_TempEnterSuccess1(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var report = sender as NtErp.Crm.Services.Models.Report;
            //删除原来的附件
            var reportall = new NtErp.Crm.Services.Views.ReportsAlls();
            reportall.DeleteFiles(report.ID);
            //附件保存
            if (!string.IsNullOrWhiteSpace(Request.Form["fileName"]))
            {
                string[] filenames = Request.Form["fileName"].Split(';');
                string[] urls = Request.Form["filePath"].Split(';');
                for (var i = 0; i < urls.Length - 1; i++)
                {
                    var file = new NtErp.Crm.Services.Models.File();
                    file.Report = report;
                    file.Admin = report.Admin;
                    file.Url = urls[i];
                    file.Name = filenames[i];
                    file.Enter();
                }
            }

            reportall.DeleteBinding(report);
            if (!string.IsNullOrWhiteSpace(Request.Form["Reader"]))
            {
                //指定阅读人保存
                string[] readers = Request.Form["Reader"].Split(',').ToArray();
                foreach (string reader in readers)
                {
                    //新增绑定
                    var readadmin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(reader);
                    reportall.BindingReader(report, readadmin);
                }
            }

            Response.Write(e.Object);
        }
        #endregion

        /// <summary>
        /// 文件上传
        /// </summary>
        protected void Upload()
        {
            HttpFileCollection files = Request.Files;
            var fileNames = Request.QueryString["fileNames"].Split(';');
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFile item = files[i];
                var filename = fileNames[i];
                //文件上传到服务器
                item.SaveAs(Server.MapPath("~/UploadFiles/") + filename);
            }
        }
    }
}