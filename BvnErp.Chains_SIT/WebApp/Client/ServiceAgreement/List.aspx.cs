using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.ServiceAgreement
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<object> listIsSAEleUploadOption = new List<object>();
            listIsSAEleUploadOption.Add(new { TypeValue = "0", TypeText = "未上传", });
            listIsSAEleUploadOption.Add(new { TypeValue = "1", TypeText = "已上传", });
            this.Model.IsSAEleUploadOption = listIsSAEleUploadOption.Json();

            List<object> listIsSAPaperUploadOption = new List<object>();
            listIsSAPaperUploadOption.Add(new { TypeValue = "0", TypeText = "未上交", });
            listIsSAPaperUploadOption.Add(new { TypeValue = "1", TypeText = "已上交", });
            this.Model.IsSAPaperUploadOption = listIsSAPaperUploadOption.Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientName = Request.QueryString["ClientName"];
            string IsSAEleUpload = Request.QueryString["IsSAEleUpload"];
            string IsSAPaperUpload = Request.QueryString["IsSAPaperUpload"];

            using (var query = new Needs.Ccs.Services.Views.ServiceAgreementListView())
            {
                var view = query;

                view = view.SearchByClientStatus(Needs.Ccs.Services.Enums.ClientStatus.Confirmed);

                if (!string.IsNullOrWhiteSpace(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByClientName(ClientName);
                }

                if (!string.IsNullOrEmpty(IsSAEleUpload))
                {
                    bool isSAEleUploadBoolean = (IsSAEleUpload == "1");
                    view = view.SearchByIsSAEleUpload(isSAEleUploadBoolean);
                }

                if (!string.IsNullOrEmpty(IsSAPaperUpload))
                {
                    bool isSAPaperUploadBoolean = (IsSAPaperUpload == "1");
                    view = view.SearchByIsSAPaperUpload(isSAPaperUploadBoolean);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 设置纸质协议上交状态
        /// </summary>
        protected void SetIsPaperUpload()
        {
            try
            {
                string ClientID = Request.Form["ClientID"];
                string TargetIsSAPaperUpload = Request.Form["TargetIsSAPaperUpload"];


                Needs.Ccs.Services.Models.Client client = new Needs.Ccs.Services.Models.Client();
                client.ID = ClientID;

                if (TargetIsSAPaperUpload == "1")
                {
                    client.SetSAUploaded();
                }
                else
                {
                    client.SetSAUnUpload();
                }

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传协议
        /// </summary>
        protected void UploadFile()
        {
            try
            {
               
                var ClientID = Request.Form["ClientID"];
                var rowNum = int.Parse(Request.Form["RowNum"]);
                
                var fileType = int.Parse(Request.Form["FileType"]);
                var FileType = (Needs.Ccs.Services.Models.ApiModels.Files.FileType)fileType;

                var file = FileType == Needs.Ccs.Services.Models.ApiModels.Files.FileType.ServiceAgreement ? 
                    Request.Files.GetMultiple("uploadFile")[rowNum] : Request.Files.GetMultiple("uploadStoFile")[rowNum];

                //文件保存
                string ext = System.IO.Path.GetExtension(file.FileName);
                string fileName = DateTime.Now.Ticks + ext;

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                

                #region 将中心原来文件置为失效
                var originalFiles = new CenterFilesTopView().Where(t => t.ClientID == ClientID && t.Type == fileType).ToList();
                foreach (var origin in originalFiles)
                {
                    new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, origin.ID);
                }
                #endregion

                #region 本地文件同步中心文件库

                var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                var dic = new { CustomName = fileName, ClientID = ClientID, AdminID = ErmAdminID };

                //本地文件上传到服务器
                var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(fileDic.FilePath, FileType, dic);
                string[] ID = { result[0].FileID };
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Normal }, ID);
                #endregion

                Response.Write((new { success = true, message = "上传成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
            

            
        }
    }
}