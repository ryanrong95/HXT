using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Notice
{
    public partial class Edit : Uc.PageBase
    {
        /// <summary>
        /// 公告新增页面
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.AdminName.Text = Needs.Erp.ErpPlot.Current.RealName;
                this.Model.Path = Request.ApplicationPath.Json();
            }
        }

        /// <summary>
        /// 数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {
            var notice = new NtErp.Crm.Services.Models.Notice();
            notice.Name = Request.Form["Name"];
            notice.Context = Request.Form["editorValue"];
            notice.CreateDate = DateTime.Parse(Request.Form["CreateDate"]);
            notice.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            notice.EnterSuccess += Notice_EnterSuccess;
            notice.Enter();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Notice_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            SaveFiles(sender as NtErp.Crm.Services.Models.Notice);
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="notice"></param>
        protected void SaveFiles(NtErp.Crm.Services.Models.Notice notice)
        {
            if (string.IsNullOrEmpty(Request.QueryString["fileNames"]))
            {
                return;
            }
            var filenames = Request.QueryString["fileNames"].Split(';');
            var filepath = Request.QueryString["filePaths"].Split(';');
            var Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            for (int i = 0; i < filenames.Count() - 1; i++)
            {
                var file = new NtErp.Crm.Services.Models.File();
                file.Notice = notice;
                file.Admin = Admin;
                file.Url = filepath[i];
                file.Name = filenames[i];
                file.Enter();
            }
        }

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