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
    /// 跟踪记录新增页面
    /// </summary>
    public partial class NAdd : Uc.PageBase
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
            }
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.Admins = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).Json();
            this.Model.Path = Request.ApplicationPath.Json();
            this.Model.TypeData = EnumUtils.ToDictionary<ActionMethord>().Select(item => new { text = item.Value, value = item.Key }).Json();
            //var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(10000, item => item.Client.Status == ActionStatus.Complete ||
            //    item.Client.Status == ActionStatus.Auditing).Select(item => new { item.Client.ID, item.Client.Name });
            var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientsBase.Where(item => item.Status == ActionStatus.Complete
                || item.Status == ActionStatus.Auditing).Select(item => new { item.ID, item.Name });
            this.Model.ClientData = client.Json();
        }


        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {
            string editorValue = Request.Form["editorValue"];
            var report = new NtErp.Crm.Services.Models.Report();
            report.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            report.Client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(Request.Form["ClientID"]);
            report.Content = editorValue;
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
            SaveList(e.Object); //附件保存
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
            if (string.IsNullOrWhiteSpace(Request.Form["Reader"]))
            {
                return;
            }
            string[] readers = Request.Form["Reader"]?.Split(',').ToArray();
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
        /// 附件保存
        /// </summary>
        /// <param name="reportid"></param>
        protected void SaveList(string reportid)
        {
            if (string.IsNullOrWhiteSpace(Request["fileNames"]))
            {
                return;
            }
            try
            {
                string[] filenames = Request["fileNames"].Split(';');
                string[] urls = Request["filePaths"].Split(';');
                var Report = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Report>.Create(reportid);
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