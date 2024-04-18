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

namespace WebApp.Crm.Project_bak
{
    /// <summary>
    /// 编辑页面
    /// </summary>
    public partial class ReportEdit : Uc.PageBase
    {
        #region 页面初始化
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
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.Path = Request.ApplicationPath.Json();
            this.Model.TypeData = EnumUtils.ToDictionary<ActionMethord>().Select(item => new { text = item.Value, value = item.Key }).Json();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var report = Needs.Erp.ErpPlot.Current.ClientSolutions.MyReports[id];
            if (report != null)
            {
                this.Model.AllData = report.Context;
            }
            else
            {
                this.Model.AllData = string.Empty.Json();
            }
            var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ReportID == id && item.Status == Status.Normal);
            this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
        }
        #endregion


        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {
            string Id = Request.QueryString["ID"];
            var report = new NtErp.Crm.Services.Views.ReportsAlls()[Id] ?? new NtErp.Crm.Services.Models.Report();
            report.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            report.Project = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Project>.Create(Request.QueryString["ProjectID"]);
            report.Content = Request.Form["editorValue1"];
            report.Plan = Request.Form["editorValue2"];
            report.OriginalStaffs = Request.Form["OriginalStaffs"];
            report.Type = (ActionMethord)int.Parse(Request.Form["Type"]);
            report.Date = DateTime.Parse(Request.Form["Date"]);
            report.NextDate = DateTime.Parse(Request.Form["NextDate"]);
            report.NextType = (ActionMethord)int.Parse(Request.Form["NextType"]);
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
            Uri url = Request.UrlReferrer ?? Request.Url;
            SaveList(e.Object);
            Alert("保存成功", url, true);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="reportid"></param>
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
                this.Alert("发生错误：" + ex.Message.ToString(), Request.UrlReferrer ?? Request.Url, true);
                return;
            }
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