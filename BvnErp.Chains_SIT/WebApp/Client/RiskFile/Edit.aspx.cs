using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using System.Net;
using WebApp.App_Utils;
using Needs.Utils;

namespace WebApp.Client.RiskFile
{
    public partial class Edit : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //参数
            string clientID = Request.QueryString["ID"];
            this.Model.ClientID = clientID;
            string fileID = Request.QueryString["FileID"];
            this.Model.FileID = fileID ?? "";

            this.Model.FileType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FileType>()
                .Where(item => item.Key == Needs.Ccs.Services.Enums.FileType.SignVideo.GetHashCode().ToString() ||
                item.Key == Needs.Ccs.Services.Enums.FileType.Photos.GetHashCode().ToString() ||
                item.Key == Needs.Ccs.Services.Enums.FileType.DeclareAgreement.GetHashCode().ToString() ||
                item.Key == Needs.Ccs.Services.Enums.FileType.SupplementAgreement.GetHashCode().ToString() ||
                item.Key == Needs.Ccs.Services.Enums.FileType.EvaluationInfo.GetHashCode().ToString()||
                item.Key == Needs.Ccs.Services.Enums.FileType.PECommitment.GetHashCode().ToString()||
                item.Key == Needs.Ccs.Services.Enums.FileType.SecurityDoc.GetHashCode().ToString() ||
                item.Key == Needs.Ccs.Services.Enums.FileType.Others.GetHashCode().ToString()
                ).Select(item => new { item.Key, item.Value }).Json();

            if (!string.IsNullOrEmpty(fileID))
            {
                var file = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(x => x.ID == fileID);
                // var file = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientFiles[fileID];
                this.Model.ClientFileData = file.Json();
            }
            else
            {
                this.Model.ClientFileData = null;
            }
        }

        /// <summary>
        /// 上传附件到中心
        /// </summary>
        protected void SaveClientFiles()
        {
            var file = Request.Files["ClientFile"];
            var ClientID = Request.Form["ClientID"];
            var FileType = Request.Form["FileType"];
            var Summary = Request.Form["Summary"];
            var FileID = Request.Form["FileID"];
            //处理附件
            if (file.ContentLength != 0)
            {
                string fileName = file.FileName.ReName();
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                httpFile.CreateDataDirectory();
                string[] result = httpFile.SaveAs(file);

                //文件修改为上传中心
                var centerType = (Needs.Ccs.Services.Models.ApiModels.Files.FileType)int.Parse(FileType);
                var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                var dic = new { CustomName = fileName, ClientID = ClientID, AdminID = ErmAdminID };
                var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + httpFile.VirtualPath, centerType, dic);

                Response.Write((new { success = true, message = "保存成功" }).Json());

                if (string.IsNullOrEmpty(URL))
                {
                    #region   调用前
                    //clientfile.Enter();
                    #endregion
                }
                else
                {
                    #region 调用后
                    try
                    {
                        string requestUrl = URL + "/CrmUnify/FileEnter";
                        HttpResponseMessage response = new HttpResponseMessage();
                        string requestClientUrl = requestUrl;//请求地址

                        var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[ClientID];
                        var entity = new ApiModel.ClientFile()
                        {
                            Enterprise = new EnterpriseObj
                            {
                                AdminCode = " ",
                                District = "",
                                Corporation = client.Company.Corporate,
                                Name = client.Company.Name,
                                RegAddress = client.Company.Address,
                                Uscc = client.Company.Code,
                                Status = 200
                            },
                            Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                            Name = fileName,
                            Type = int.Parse(FileType),
                            FileFormat = "",
                            Url = uploadFile[0].Url,
                            Summary = "",
                            Status = 200,
                        };
                        string apiclient = JsonConvert.SerializeObject(entity);
                        response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiclient);
                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                            return;
                        }

                        // clientfile.Enter();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    #endregion
                }
            }
        }
    }
}
