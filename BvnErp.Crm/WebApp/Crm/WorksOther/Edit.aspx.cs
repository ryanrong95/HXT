using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.WorksOther
{
    /// <summary>
    /// 工作计划编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Path = Request.ApplicationPath.Json();
                PageInit();
            }
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        private void PageInit()
        {
            string id = Request.QueryString["ID"];
            var work = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksOther[id];
            if (work == null)
            {
                AdminName.Text = Needs.Erp.ErpPlot.Current.RealName;
            }
            else
            {
                AdminName.Text = work.Admin.RealName;
            }
            this.Model.AllData = work.Json();
            var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.WorksOtherID == id && item.Status == Status.Normal);
            this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
        }


        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        protected void Save()
        {
            string id = Request["ID"];
            var worksOther = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksOther[id] as
             NtErp.Crm.Services.Models.WorksOther ?? new NtErp.Crm.Services.Models.WorksOther();
            worksOther.Context = Request["editorValue"];
            worksOther.Subject = Request["Subject"];
            worksOther.StartDate = DateTime.Parse(Request["StartDate"]);
            worksOther.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            worksOther.Status = Status.Normal;
            worksOther.EnterSuccess += Weekly_EnterSuccess;
            worksOther.Enter();
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Weekly_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            SaveFiles(e.Object);  //保存文件
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="id">workotherid</param>
        protected void SaveFiles(string id)
        {
            try
            {
                Needs.Erp.ErpPlot.Current.ClientSolutions.WorksOther.DeleteFiles(id);
                if (string.IsNullOrWhiteSpace(Request.QueryString["fileNames"]))
                {
                    return;
                }
                string[] filenames = Request.QueryString["fileNames"].Split(';');
                string[] urls = Request.QueryString["filePaths"].Split(';');
                var WorksOther = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.WorksOther>.Create(id);
                for (var i = 0; i < urls.Length - 1; i++)
                {
                    var file = new NtErp.Crm.Services.Models.File();
                    file.WorksOther = WorksOther;
                    file.Admin = WorksOther.Admin;
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
            var admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            string id = Request.Form["ID"];
            var worksOther = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksOther[id] ?? new NtErp.Crm.Services.Models.WorksOther();
            worksOther.Context = Request.Form["Context"];
            worksOther.Subject = Request.Form["Subject"];
            if (!string.IsNullOrWhiteSpace(Request.Form["StartDate"]))
                worksOther.StartDate = DateTime.Parse(Request.Form["StartDate"]);
            else
                worksOther.StartDate = null;
            worksOther.Admin = admin;
            worksOther.Status = Status.Temporary;
            worksOther.EnterSuccess += WorksOther_TempEnterSuccess;
            worksOther.Enter();
        }

        /// <summary>
        /// 暂存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorksOther_TempEnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var worksOther = sender as NtErp.Crm.Services.Models.WorksOther;
            Needs.Erp.ErpPlot.Current.ClientSolutions.WorksOther.DeleteFiles(worksOther.ID);
            //附件保存
            if (!string.IsNullOrWhiteSpace(Request.Form["filename"]))
            {
                string[] filenames = Request.Form["filename"].Split(';');
                string[] urls = Request.Form["filepath"].Split(';');
                for (var i = 0; i < urls.Length - 1; i++)
                {
                    var file = new NtErp.Crm.Services.Models.File();
                    file.WorksOther = worksOther;
                    file.Admin = worksOther.Admin;
                    file.Url = urls[i];
                    file.Name = filenames[i];
                    file.Enter();
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