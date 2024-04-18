using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecEdocRealation : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.ID = Request.QueryString["ID"];
        }

        /// <summary>
        /// 电子单据列表
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];

            var edoc = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.EdocRealations.Where(t => t.DeclarationID == ID);

            //var netPath = System.Configuration.ConfigurationManager.AppSettings["netfilebaseurl"];
            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
            Func<Needs.Ccs.Services.Models.EdocRealation, object> convert = item => new
            {
                ID = item.ID,
                DeclarationID = item.DeclarationID,
                EdocID = item.EdocID,
                EdocCode = item.EdocCode,
                EdocName = item.Edoc.Name,
                EdocFomatType = item.EdocFomatType,
                OpNote = item.OpNote,
                EdocCopId = item.EdocCopId,
                SignTime = item.SignTime.ToShortDateString(),
                EdocSize = item.EdocSize,
                EdocOwnerCode = item.EdocOwnerCode,
                EdocOwnerName = item.EdocOwnerName,
                FileUrl = FileServerUrl + @"/" + item.FileUrl.ToUrl()
            };

            Response.Write(new
            {
                rows = edoc.Select(convert).ToArray(),
                total = edoc.Count()
            }.Json());
        }

        /// <summary>
        /// 重新生成合同、发票、装箱单附件
        /// </summary>
        protected void ReBuildPDF()
        {
            string ID = Request.Form["ID"];

            //var vendor = new Needs.Ccs.Services.VendorContext(Needs.Ccs.Services.VendorContextInitParam.DecHeadID, ID).Current1;
            var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[ID];

            var vendor = new Needs.Ccs.Services.VendorContext(Needs.Ccs.Services.VendorContextInitParam.Pointed, head.OwnerName).Current1;
            
            head.PaymentInstructionSaveAs();
            head.ContractSaveAs();
            head.PackingListSaveAs(vendor);
            Response.Write((new { success = true, message = "重新生成成功！" }).Json());
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void DeleteFile()
        {
            string ID = Request.Form["ID"];

            var file = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.EdocRealations[ID];
            file.AbandonSuccess += EdocFile_EnterSuccess;
            file.Abandon();

        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EdocFile_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "删除成功", ID = e.Object }).Json());
        }
    }
}