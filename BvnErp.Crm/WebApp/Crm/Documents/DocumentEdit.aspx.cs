using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Documents
{
    public partial class DocumentEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadData();
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var document = new NtErp.Crm.Services.Views.DocumentAlls()[id];
            this.Model.Data = document.Json();
        }

        /// <summary>
        /// 数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            var file = Request.Files[0];

            var document = new NtErp.Crm.Services.Views.DocumentAlls()[id] ?? new NtErp.Crm.Services.Models.Document();
            //校验是否有文件上传
            if(fileUpload.HasFile)
            {
                #region 保存文件
                string path = System.Configuration.ConfigurationManager.AppSettings["UploadDocuments"].ToString();
                string filepath = Server.MapPath("~" + path);
                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }
                string filename = Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(file.FileName).ToLower();
                file.SaveAs(filepath + filename);
                #endregion
                document.Url = Request.ApplicationPath + path + filename;
                document.Name = file.FileName;
                document.Size = file.ContentLength / 1024;
            }

            document.DirectoryID = Request.QueryString["DirectoryID"];
            document.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            document.Title = Request.Form["Title"];
            document.Summary = Request.Form["Summary"];
            document.EnterSuccess += Document_EnterSuccess;
            document.Enter();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Document_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Uri url = Request.UrlReferrer ?? Request.Url;
            Alert("保存成功", url, true);
        }
    }
}