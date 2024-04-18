using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client
{
    public partial class WsAgreement : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            string id = Request.QueryString["ID"];
            var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
            this.Model.Client = client.Json().Replace("'", "#39;"); ;
            this.Model.ID = id;
            if (!string.IsNullOrEmpty(id))
            {
                if (client.ServiceType == Needs.Ccs.Services.Enums.ServiceType.Warehouse || client.ServiceType == Needs.Ccs.Services.Enums.ServiceType.Both)
                {

                    switch (client.StorageType)
                    {
                        case Needs.Ccs.Services.Enums.StorageType.Domestic:
                            this.Model.WsAgreement = new { PartA = client.Company.Name, PartB = "深圳市芯达通供应链管理有限公司" }.Json();
                            break;
                        case Needs.Ccs.Services.Enums.StorageType.HKCompany:
                            this.Model.WsAgreement = new { PartA = client.Company.Name, PartB = "香港畅运国际物流有限公司" }.Json();
                            break;
                        case Needs.Ccs.Services.Enums.StorageType.Person:
                            this.Model.WsAgreement = new { PartA = client.Company.Name, PartB = "深圳市芯达通供应链管理有限公司" }.Json();
                            break;
                        default:
                            this.Model.WsAgreement = null;
                            break;
                    }
                  
                }
                else
                {
                    this.Model.WsAgreement = null;
                }


                var serviceFile = new CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.StorageAgreement && x.Status != FileDescriptionStatus.Delete);
                if (serviceFile != null)
                {
                    string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
                    this.Model.ServiceFile = new { Name = serviceFile.CustomName, Url = FileServerUrl + @"/" + serviceFile.Url.ToUrl() }.Json();
                }
                else
                {
                    this.Model.ServiceFile = null;
                }

            }
            else
            {

                this.Model.WsAgreement = null;
                this.Model.ServiceFile = null;
            }

        }

        /// <summary>
        /// 保存仓储协议
        /// </summary>
        protected void Save()
        {
            try
            {
                var id = Request.Form["ID"];
                var file = Request.Files["ServiceAgreement"];
                //处理附件
                if (file.ContentLength != 0)
                {
                    string fileName = file.FileName.ReName();
                    HttpFile httpFile = new HttpFile(fileName);
                    httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                    httpFile.CreateDataDirectory();
                    string[] result = httpFile.SaveAs(file);
                    //上传中心
                    var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.StorageAgreement;
                    var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                    var dic = new { CustomName = fileName, ClientID = id, AdminID = ErmAdminID };
                    var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + httpFile.VirtualPath, centerType, dic);
                    var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                    string requestUrl = URL + "/CrmUnify/StorageAgreement";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var entity = new { ClientName = client.Company.Name, FileID = uploadFile[0].FileID };
                    string apiclient = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiclient);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }
                }
                Response.Write((new { success = true, message = "保存成功", ID = id }).Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { success = false, message = ex.Message });

            }


        }


       

        protected void ExportAgreement()
        {
            try
            {
                string id = Request.Form["ID"];
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "香港本地交货协议.docx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                client.SaveStorageAgreement(file.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败" + ex.Message }).Json());
            }
        }

    }
}