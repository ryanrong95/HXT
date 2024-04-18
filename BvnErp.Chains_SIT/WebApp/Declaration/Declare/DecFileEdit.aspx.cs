using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Declaration.Declare
{
    public partial class DecFileEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }

        protected void Load_Data()
        {
            string id = Request.QueryString["ID"];
            this.Model.ID = id;
            var test = Needs.Ccs.Services.Enums.FileType.CCC.ToString();
            this.Model.DecFileType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FileType>()
                .Where(item => item.Key == Needs.Ccs.Services.Enums.FileType.CCC.GetHashCode().ToString() 
                || item.Key == Needs.Ccs.Services.Enums.FileType.AppraiseReuslt.GetHashCode().ToString()
                || item.Key == Needs.Ccs.Services.Enums.FileType.Unlist1.GetHashCode().ToString()
                || item.Key == Needs.Ccs.Services.Enums.FileType.Unlist2.GetHashCode().ToString()
                || item.Key == Needs.Ccs.Services.Enums.FileType.Unlist3.GetHashCode().ToString())
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SaveDecFile()
        {
            var file = Request.Files["DecFile"];
            var DecHeadID = Request.Form["ID"]; 
            var DecFileType = Request.Form["DecFileType"];
            var Summary = Request.Form["Summary"];

            //处理附件
            if (file.ContentLength != 0)
            {
                //string fileName = file.FileName.ReName();
                HttpFile httpFile = new HttpFile(file.FileName);
                httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.DecHeadCCC);
                httpFile.CreateDataDirectory();
                string[] result = httpFile.SaveAs(file);

                var decFile = new Needs.Ccs.Services.Models.DecFile
                {
                    DecHeadID = DecHeadID,
                    AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                    FileFormat = file.ContentType,
                    Name = file.FileName,
                    FileTypeInt = Convert.ToInt32(DecFileType),
                    Url = result[1],
                    Summary = Summary
                };

                decFile.EnterSuccess += EdocFile_EnterSuccess;
                decFile.EnterError += EdocFile_EnterError;
                decFile.Enter();
            }
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EdocFile_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EdocFile_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}