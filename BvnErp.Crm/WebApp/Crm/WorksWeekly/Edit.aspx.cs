using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.WorksWeekly
{
    /// <summary>
    /// 工作周报编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WeekOfYear.Text = Get_WeekOfYear(DateTime.Now, new CultureInfo("zh-CN")).ToString();
                this.Model.Path = Request.ApplicationPath.Json();
                PageInit();
            }
        }

        //获取当前周
        private int Get_WeekOfYear(DateTime dt, CultureInfo ci)
        {
            return ci.Calendar.GetWeekOfYear(dt, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        void PageInit()
        {
            string id = Request.QueryString["ID"];
            this.Model.AllData = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWeekly[id].Json();
            var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.WorksWeeklyID == id && item.Status == Status.Normal);
            this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
        }

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        protected void Save()
        {
            string id = Request["ID"];
            var weekly = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWeekly[id] as
             NtErp.Crm.Services.Models.WorksWeekly ?? new NtErp.Crm.Services.Models.WorksWeekly();
            weekly.Context = Request["editorValue"];
            weekly.WeekOfYear = int.Parse(Request["WeekOfYear"]);
            weekly.Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            weekly.Status = Status.Normal;
            weekly.EnterSuccess += Contact_EnterSuccess;
            weekly.Enter();
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            SaveFiles(e.Object);  //保存文件
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        /// <summary>
        /// 文件数据保存
        /// </summary>
        /// <param name="id"></param>
        protected void SaveFiles(string id)
        {
            try
            {
                Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWeekly.DeleteFiles(id);
                if (string.IsNullOrWhiteSpace(Request.QueryString["fileNames"]))
                {
                    return;
                }
                string[] filenames = Request.QueryString["fileNames"].Split(';');
                string[] urls = Request.QueryString["filePaths"].Split(';');
                var WorksWeekly = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.WorksWeekly>.Create(id);
                for (var i = 0; i < urls.Length - 1; i++)
                {
                    var file = new NtErp.Crm.Services.Models.File();
                    file.WorksWeekly = WorksWeekly;
                    file.Admin = WorksWeekly.Admin;
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
        protected void SaveTemp()
        {
            var admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);

            string id = Request.Form["ID"];
            var weekly = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWeekly[id] ?? new NtErp.Crm.Services.Models.WorksWeekly();
            weekly.Context = Request.Form["Context"];
            weekly.WeekOfYear = int.Parse(Request.Form["WeekOfYear"]);
            weekly.Admin = admin;
            weekly.Status = Status.Temporary;
            weekly.EnterSuccess += Weekly_TempEnterSuccess;
            weekly.Enter();
        }

        /// <summary>
        /// 暂存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Weekly_TempEnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var weekly = sender as NtErp.Crm.Services.Models.WorksWeekly;
            Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWeekly.DeleteFiles(weekly.ID);
            if (!string.IsNullOrWhiteSpace(Request.Form["filename"]))
            {
                string[] filenames = Request.Form["filename"].Split(';');
                string[] urls = Request.Form["filepath"].Split(';');
                for (var i = 0; i < urls.Length - 1; i++)
                {
                    var file = new NtErp.Crm.Services.Models.File();
                    file.WorksWeekly = weekly;
                    file.Admin = weekly.Admin;
                    file.Url = urls[i];
                    file.Name = filenames[i];
                    file.Enter();
                }
            }

            Response.Write(e.Object);
        }
        #endregion

        /// <summary>
        /// 附件上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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