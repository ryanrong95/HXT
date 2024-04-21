using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class File : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.FileType = ExtendsEnum.ToArray<FileType>().Select(item => new
            {
                Value = (int)item,
                Text = item.GetDescription()
            }).Where(item => item.Value >= 2000 && item.Value < 3000);
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        protected void uploadFile()
        {
            try
            {
                string StaffID = Request.Form["StaffID"];
                string fileType = Request.Form["fileType"];

                IList<HttpPostedFile> files = HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            FileDirectory dic = new FileDirectory(file.FileName, (FileType)Enum.Parse(typeof(FileType), fileType));
                            dic.StaffID = StaffID;
                            dic.AdminID = Erp.Current.ID;
                            dic.Save(file);
                        }
                    }
                }
                Response.Write((new { success = true, data = "" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        protected object data()
        {
            string StaffID = Request.QueryString["ID"];
            var files = new Services.Views.StaffFileAlls(StaffID).ToArray();
            var admins = Alls.Current.Admins;
            var linq = from file in files
                       join admin in admins on file.AdminID equals admin.ID
                       select new
                       {
                           ID = file.ID,
                           CustomName = file.CustomName,
                           FileType = ((FileType)Enum.Parse(typeof(FileType), file.Type.ToString())).GetDescription(),
                           CreateDate = file.CreateDate?.ToString("yyyy-MM-dd"),
                           Creater = admin.RealName,
                           Url = FileDirectory.ServiceRoot + file.Url,
                       };
            return linq;
        }

        protected void Delete()
        {
            string[] ids = new string[] { Request.Form["ID"] };
            new Yahv.Services.Views.CenterFilesTopView().Delete(ids);
        }
    }
}