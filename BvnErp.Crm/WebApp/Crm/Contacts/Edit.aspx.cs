using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Contacts
{
    /// <summary>
    /// 联系人编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
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
                this.Model.Path = Request.ApplicationPath.Json();
                this.Model.Sex = EnumUtils.ToDictionary<Sex>().Select(item => new { value = item.Key, text = item.Value }).Json();
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var contact = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts[id];
            if (contact != null)
            {
                this.Model.Contact = contact.Detail;
                var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ContactID == contact.ID && item.Status == Status.Normal);
                this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
            }
            else
            {
                this.Model.Contact = string.Empty.Json();
                this.Model.Files = string.Empty.Json();
            }
            
        }


        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        protected void Save()
        {
            string id = Request.QueryString["ID"];
            string ClientID = Request.QueryString["ClientID"];
            var Contact = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts[id] as NtErp.Crm.Services.Models.Contact ?? new NtErp.Crm.Services.Models.Contact();

            Contact.Clients = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(ClientID);
            Contact.ClientID = Contact.Clients.ID;
            Contact.CompanyID = string.Empty;
            foreach (var key in Request.Form.AllKeys)
            {
                if (key != "__VIEWSTATE" && key != "__VIEWSTATEGENERATOR" && key != "__EVENTVALIDATION" && key != "btnSave")
                {
                    Contact[key] = Request.Form[key];
                }
            }
            Contact.Types = ConsigneeType.Goods;
            Contact.EnterSuccess += Contact_EnterSuccess;
            Contact.Enter();
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            SaveList(e.Object);
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        /// <summary>
        /// 附件数据保存到数据库中
        /// </summary>
        /// <param name="ContactID">联系人ID</param>
        protected void SaveList(string ContactID)
        {
            try
            {
                //删除原来的附件
                Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts.DeleteFiles(ContactID);
                if (string.IsNullOrWhiteSpace(Request.QueryString["fileNames"]))
                {
                    return;
                }
                string[] filenames = Request.QueryString["fileNames"].Split(';');
                string[] urls = Request.QueryString["filePaths"].Split(';');

                var Contact = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Contact>.Create(ContactID);
                var Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
                for (var i = 0; i < urls.Length - 1; i++)
                {
                    var file = new NtErp.Crm.Services.Models.File();
                    file.Contact = Contact;
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