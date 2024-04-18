using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.File
{
    public partial class List : Uc.PageBase
    {
        string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            this.Model.ID = id;
            this.Model.FileType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FileType>().Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            string Name = Request.QueryString["Name"];
            string FileType = Request.QueryString["FileType"];
            var files = new Needs.Ccs.Services.Models.CenterFilesTopView().Where(x => x.ClientID == id && 
                                                                                      x.Status != FileDescriptionStatus.Delete && 
                                                                                      (x.Type == (int)Needs.Ccs.Services.Enums.FileType.BusinessLicense ||
                                                                                      x.Type == (int)Needs.Ccs.Services.Enums.FileType.PowerOfAttorney ||
                                                                                      x.Type == (int)Needs.Ccs.Services.Enums.FileType.ServiceAgreement ));
            // var filesss = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientFiles.AsQueryable();
            if (!string.IsNullOrEmpty(Name))
            {
                files = files.Where(t => t.CustomName.Contains(Name));
            }
            if (!string.IsNullOrEmpty(FileType))
            {
                var filetype = (Needs.Ccs.Services.Enums.FileType)int.Parse(FileType);
                files = files.Where(t => t.Type == (int)filetype);
            }

            Func<Needs.Ccs.Services.Models.CenterFileDescription, object> convert = file => new
            {
                file.ID,
                Name = file.CustomName,
                FileType = ((Needs.Ccs.Services.Models.ApiModels.Files.FileType)file.Type).GetDescription(),
                Url = FileServerUrl + "/" + file?.Url.ToUrl(),
                CreateDate = file.CreateDate.Value.ToString("yyyy-MM-dd"),
                Status =((Status)file.Status).GetDescription(),
                Summary = ""
            };
            this.Paging(files, convert);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void DeleteFile()
        {
            string ids = Request.Form["ID"];
            ids.Split(',').ToList().ForEach(t =>
            {
                //var file = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientFiles[t];
                //file.AbandonSuccess += ClientFile_EnterSuccess;
                //file.Abandon();
                new CenterFilesTopView().AbandonFile(t);
            });
                Response.Write((new { success = true, message = "删除成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientFile_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "删除成功", ID = e.Object }).Json());
        }
    }

}