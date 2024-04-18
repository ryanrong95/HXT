using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.ServiceAgreement
{
    public partial class AdvanceMoneyApplyList : Uc.PageBase
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

            this.Model.AdvanceMoneyApplyStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.AdvanceMoneyStatus>().Select(item => new { item.Key, item.Value }).Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string CreateDateFrom = Request.QueryString["CreateDateFrom"];
            string CreateDateTo = Request.QueryString["CreateDateTo"];
            string Status = Request.QueryString["AdvanceMoneyApplyStatus"];

            using (var query = new Needs.Ccs.Services.Views.AdvanceMoneyApplyListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(ClientCode))
                {
                    view = view.SearchByClientCode(ClientCode);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    view = view.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    int advanceMoneyApplyStatus = Convert.ToInt32(Status);
                    view = view.SearchByApplyStatus(advanceMoneyApplyStatus);
                }
                if (!string.IsNullOrEmpty(CreateDateFrom))
                {
                    var from = DateTime.Parse(CreateDateFrom);
                    view = view.SearchByCreateDateFrom(from);
                }
                if (!string.IsNullOrEmpty(CreateDateTo))
                {
                    var to = DateTime.Parse(CreateDateTo).AddDays(1);
                    view = view.SearchByCreateDateTo(to);
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void SetIsPaperUpload()
        {
            try
            {
                string AdvanceMoneyApplyID = Request.Form["AdvanceMoneyApplyID"];
                string ClientID = Request.Form["ClientID"];
                string TargetIsSAPaperUpload = Request.Form["TargetIsSAPaperUpload"];

                var advanceMoneyApplyListView = new Needs.Ccs.Services.Views.AdvanceMoneyApplyListView();


                if (TargetIsSAPaperUpload == "1")
                {
                    advanceMoneyApplyListView.SetSAUploaded(AdvanceMoneyApplyID);
                }
                else
                {
                    advanceMoneyApplyListView.SetSAUnUpload(AdvanceMoneyApplyID);
                }

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        protected void UploadFile()
        {
            try
            {
                var files = Request.Files.GetMultiple("uploadFile");
                var ClientID = Request.Form["ClientID"];
                var AdvanceMoneyApplyID = Request.Form["AdvanceMoneyApplyID"];
                var rowNum = int.Parse(Request.Form["RowNum"]);
                var file = Request.Files.GetMultiple("uploadFile")[rowNum];

                //文件保存
                string ext = System.IO.Path.GetExtension(file.FileName);
                string fileName = DateTime.Now.Ticks + ext;

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                #region 将中心原来文件置为失效
                var originalFiles = new CenterFilesTopView()
                            .Where(t => t.ApplicationID == AdvanceMoneyApplyID
                                     && t.ClientID == ClientID
                                     && t.Type == (int)Needs.Ccs.Services.Models.ApiModels.Files.FileType.AdvanceMoneyApplyAgreement)
                            .ToList();
                foreach (var origin in originalFiles)
                {
                    new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, origin.ID);
                }
                #endregion

                #region 本地文件同步中心文件库

                var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                var dic = new { CustomName = fileName, ClientID = ClientID, ApplicationID = AdvanceMoneyApplyID, AdminID = ErmAdminID };

                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.AdvanceMoneyApplyAgreement;
                //本地文件上传到服务器
                var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(fileDic.FilePath, centerType, dic);
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