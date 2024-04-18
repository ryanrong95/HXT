using Needs.Utils.Converters;
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
    public partial class DecEdocEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }

        protected void Load_Data()
        {
            string id = Request.QueryString["ID"];
            this.Model.ID = id;
            this.Model.EdocCode = Needs.Wl.Admin.Plat.AdminPlat.BaseEdocCode.OrderBy(item => item.Code).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).Json();

            var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[id];
            this.Model.CustomMaster = head.CustomMaster;
            this.Model.ConsigneeName = head.ConsigneeName;
            this.Model.ConsigneeCusCode = head.ConsigneeCusCode;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SaveEdocFiles()
        {
            var file = Request.Files["EdocFile"];
            var DecHeadID = Request.Form["ID"];
            var CustomMaster = Request.Form["CustomMaster"];
            var EdocCode = Request.Form["EdocCode"];
            var EdocCopId = Request.Form["EdocCopId"];
            var EdocSize = Request.Form["EdocSize"];
            var EdocOwnerName = Request.Form["EdocOwnerName"];
            var EdocOwnerCode = Request.Form["EdocOwnerCode"];
            var Summary = Request.Form["Summary"];

            //处理附件
            if (file.ContentLength != 0)
            {
                string fileName = file.FileName.ReName();
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.DeclareDirectory);
                httpFile.CreateDataDirectory();
                string[] result = httpFile.SaveAs(file);

                //TODO:不同类型的单据 如何生成id？？？
                var serino = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.EdocInvoice);
                var edoc = new Needs.Ccs.Services.Models.EdocRealation
                {
                    DeclarationID = DecHeadID,
                    EdocID = string.Concat(CustomMaster, serino),
                    EdocCode = EdocCode,
                    EdocFomatType = "US",
                    EdocCopId = EdocCopId,
                    SignTime = DateTime.Now,
                    EdocOwnerCode = EdocOwnerCode,
                    EdocOwnerName = EdocOwnerName,
                    EdocSize = EdocSize,
                    FileUrl = result[1],
                    OpNote = Summary
                };

                edoc.EnterSuccess += EdocFile_EnterSuccess;
                edoc.EnterError += EdocFile_EnterError;
                edoc.Enter();
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