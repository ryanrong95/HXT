using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client.Approval
{
    public partial class Edit : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        public string CenterServiceUrl { get; set; }
        public string CenterbusiniseUrl { get; set; }
        string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
            GetServiceFileHttpUrl();
            GetBusinessFileHttpUrl();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                this.Model.ClientInfoData = new
                {
                    ID = client.ID,
                    ClientStatus = client.ClientStatus,
                    CompanyName = client.Company.Name,
                    Rank = client.ClientRank.GetDescription(),
                    ClientNature = client.ClientNature == null ? "" : ((ClientNature)client.ClientNature.Value).GetDescription(),
                    ClientAgreementData = client.Agreement,
                    InvoiceType = client.Agreement?.InvoiceType.GetDescription(),
                    StartDate = client.Agreement?.StartDate.ToShortDateString(),
                    EndDate = client.Agreement?.EndDate.ToShortDateString(),
                    GoodsPeriodTypeDec = client.Agreement?.ProductFeeClause.PeriodType.GetDescription(),
                    GoodsExchangeRateTypeDec = client.Agreement?.ProductFeeClause.ExchangeRateType.GetDescription(),
                    TaxExchangeRateTypeDec = client.Agreement?.TaxFeeClause.ExchangeRateType.GetDescription(),
                    TaxPeriodTypeDec = client.Agreement?.TaxFeeClause.PeriodType.GetDescription(),
                    AgencyExchangeRateTypeDec = client.Agreement?.AgencyFeeClause.ExchangeRateType.GetDescription(),
                    AgencyPeriodType = client.Agreement?.AgencyFeeClause.PeriodType.GetDescription(),
                    IncidentalExchangeRateTypeDec = client.Agreement?.IncidentalFeeClause.ExchangeRateType.GetDescription(),
                    IncidentalPeriodTypeDec = client.Agreement?.IncidentalFeeClause?.PeriodType.GetDescription(),
                    ServiceType = client.ServiceType,
                    ServiceTypeDes = client.ServiceType.GetDescription(),
                    StorageType = ((StorageType)client.StorageType).GetDescription(),
                    client.IsStorageValid,
                    client.IsValid
                }.Json();

                this.Model.ID = id;
                var ServiceIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员").Select(item => item.Admin.ID).ToArray();
                this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(item => ServiceIDs.Contains(item.ID)).Select(item => new { Key = item.ID, Value = item.RealName }).ToArray().Json();
                var handerIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "跟单员").Select(item => item.Admin.ID).ToArray();
                this.Model.Merchandiser = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(item => handerIDs.Contains(item.ID)).Select(item => new { Key = item.ID, Value = item.RealName }).ToArray().Json();
                var admins = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAdmins.Where(t => t.ClientID == id && t.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();
                this.Model.Referrers = new Needs.Ccs.Services.Views.Origins.ReferrersOrigin().Where(item => item.Status == Status.Normal).Select(item => new { Key = item.Name, Value = item.Name }).ToArray().Json();
                this.Model.ClientAssignData = admins.Json();

            }


            var serviceFile = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(t => t.ClientID == id && t.Type == (int)Needs.Ccs.Services.Enums.FileType.ServiceAgreement);
            var businessLicenceFile = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.BusinessLicense);
            var hkbusinessLicenceFile = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.HKBusinessLicense);
            var storageAgreementFile = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.StorageAgreement);

            if (serviceFile != null)
            {
                this.CenterServiceUrl = FileDirectory.Current.PvDataFileUrl + "/" + serviceFile?.Url.ToUrl();
                this.Model.serviceFile = new
                {
                    ID = serviceFile?.ID,
                    FileName = serviceFile.CustomName,
                    Url = serviceFile.Url.Replace(@"\", @"\\"),
                    WebUrl = this.CenterServiceUrl,
                    FileType = FileType.ServiceAgreement
                }.Json();
            }
            else
            {
                this.Model.serviceFile = "".Json();

            }
            if (businessLicenceFile != null)
            {
                this.CenterbusiniseUrl = FileDirectory.Current.PvDataFileUrl + "/" + businessLicenceFile?.Url.ToUrl();
                this.Model.businessLicenceFile = new
                {
                    ID = businessLicenceFile?.ID,
                    FileName = businessLicenceFile?.CustomName,
                    // FileFormat = serviceFile.FileFormat,
                    Url = businessLicenceFile.Url.Replace(@"\", @"\\"),
                    WebUrl = this.CenterbusiniseUrl,
                    FileType = FileType.BusinessLicense
                }.Json();

            }
            else
            {

                this.Model.businessLicenceFile = "".Json();
            }

            if (hkbusinessLicenceFile != null)
            {
                this.CenterbusiniseUrl = FileDirectory.Current.PvDataFileUrl + "/" + hkbusinessLicenceFile?.Url.ToUrl();
                this.Model.HKbusinessLicenceFile = new
                {
                    ID = hkbusinessLicenceFile?.ID,
                    FileName = hkbusinessLicenceFile?.CustomName,
                    // FileFormat = serviceFile.FileFormat,
                    Url = hkbusinessLicenceFile.Url.Replace(@"\", @"\\"),
                    WebUrl = this.CenterbusiniseUrl,
                    FileType = FileType.HKBusinessLicense
                }.Json();

            }
            else
            {

                this.Model.HKbusinessLicenceFile = "".Json();
            }

            if (storageAgreementFile != null)
            {
                this.CenterbusiniseUrl = FileDirectory.Current.PvDataFileUrl + "/" + storageAgreementFile?.Url.ToUrl();
                this.Model.StorageAgreementFile = new
                {
                    ID = storageAgreementFile?.ID,
                    FileName = storageAgreementFile?.CustomName,
                    // FileFormat = serviceFile.FileFormat,
                    Url = storageAgreementFile.Url.Replace(@"\", @"\\"),
                    WebUrl = this.CenterbusiniseUrl,
                    FileType = FileType.StorageAgreement
                }.Json();

            }
            else
            {
                this.Model.StorageAgreementFile = "".Json();
            }
        }



        protected void LoadLogs()
        {
            string ID = Request.Form["ID"];
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientLogs.Where(item => item.ClientID == ID).OrderByDescending(item => item.CreateDate).AsQueryable();
            Func<Needs.Ccs.Services.Models.ClientLog, object> convert = item => new
            {
                ID = item.ID,
                Client = item.ClientID,
                Admin = item.Admin.RealName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = item.Summary
            };
            Response.Write(new { rows = list.Select(convert).ToArray(), }.Json());
        }

        /// <summary>
        /// 总经理审批
        /// </summary>
        protected void Approve()
        {
            try
            {
                string id = Request.Form["ID"];
                string MerchandiserID = Request.Form["MerchandiserID"];
                string referrer = Request.Form["Referrer"];
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView[id];
                client.Merchandiser = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)MerchandiserID);

                //领导的奇怪要求 显示张庆永操作
                //var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                //if (Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张令金")
                //{
                //    adminid = "Admin0000000282";//张庆永的ID
                //}
                //client.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)adminid);

                //20230301 AdminID记录真实操作人， log显示张庆永
                client.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                client.Referrer = referrer;
                if (client != null)
                {

                    client.EnterSuccess += ClientStatus_EnterError_EnterSuccess;
                    client.EnterError += ClientStatus_EnterError;
                    client.Approve(referrer);
                }

                try
                {
                    string requestUrl = URL + "/CrmUnify/Assign";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var entity = new ApiModel.ClientAssign()
                    {
                        Client = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "",
                            Corporation = client.Company.Corporate,
                            Name = client.Company.Name,
                            RegAddress = client.Company.Address,
                            Uscc = client.Company.Code,
                            Status = 200,
                        },
                        ServiceManager = client.ServiceManager.RealName,
                        Merchandiser = client.Merchandiser.RealName,
                        Referrer = string.Empty,
                        IsDeclaretion = client.IsValid.HasValue ? client.IsValid.Value : false,
                        IsStorageService = client.IsStorageValid.HasValue ? client.IsStorageValid.Value : false,
                    };
                    string apiSupplier = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiSupplier);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                // Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                // Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }


        }

        protected void ApproveRefuse()
        {
            try
            {
                string id = Request.Form["ID"];
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Clients[id];
                //领导的奇怪要求 显示张庆永操作
                //var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                //if (Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张令金")
                //{
                //    adminid = "Admin0000000282";//张庆永的ID
                //}
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                string summary = Request.Form["Summary"];
                if (entity != null)
                {
                    entity.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)adminid);
                    entity.EnterSuccess += ClientStatus_EnterError_EnterSuccess;
                    entity.EnterError += ClientStatus_EnterError;
                    entity.ApprovalRefused(summary);
                }

            }
            catch (Exception ex)
            {

            }


        }


        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStatus_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStatus_EnterError_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "提交成功", ID = e.Object }).Json());
        }

        protected void GetServiceFileHttpUrl()
        {
            var url = this.CenterServiceUrl;
            if (!string.IsNullOrEmpty(url))
            {
                string fileName = Path.GetFileName(url);
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                //httpFile.CreateDataDirectory();
                string filePath = Path.Combine(FileDirectory.Current.FilePath, httpFile.VirtualPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            //创建本地文件写入流
                            byte[] bArr = new byte[1024];
                            int iTotalSize = 0;
                            int size = stream.Read(bArr, 0, (int)bArr.Length);
                            while (size > 0)
                            {
                                iTotalSize += size;
                                fs.Write(bArr, 0, size);
                                size = stream.Read(bArr, 0, (int)bArr.Length);
                            }
                        }
                        stream.Close();
                    }

                    this.FileDoc1.HRef = Path.Combine(FileServerUrl, httpFile.VirtualPath, fileName);
                }
                catch (Exception)
                {
                }

            }

        }

        protected void GetBusinessFileHttpUrl()
        {
            var url = this.CenterbusiniseUrl;
            if (!string.IsNullOrEmpty(url))
            {
                string fileName = Path.GetFileName(url);
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                //httpFile.CreateDataDirectory();
                string filePath = Path.Combine(FileDirectory.Current.FilePath, httpFile.VirtualPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            //创建本地文件写入流
                            byte[] bArr = new byte[1024];
                            int iTotalSize = 0;
                            int size = stream.Read(bArr, 0, (int)bArr.Length);
                            while (size > 0)
                            {
                                iTotalSize += size;
                                fs.Write(bArr, 0, size);
                                size = stream.Read(bArr, 0, (int)bArr.Length);
                            }
                        }
                        stream.Close();
                    }

                    this.FileDoc.HRef = Path.Combine(FileServerUrl, httpFile.VirtualPath, fileName);
                }
                catch (Exception)
                {
                }

            }

        }
    }
}
