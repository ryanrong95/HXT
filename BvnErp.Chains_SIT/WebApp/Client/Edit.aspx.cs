using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;
using static Needs.Ccs.Services.Models.ApiModel;

namespace WebApp.Client
{
    public partial class Edit : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
        //ryan 20210519 张经理要求入仓号使用WL开头，系统自动默认，不允许修改
        private readonly string numberPre = "HXT";
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            //客户等级
            this.Model.ClientRanks = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ClientRank>().Select(item => new { item.Key, item.Value }).Json();
            //客户类型
            this.Model.ClientTypes = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ClientType>().Select(item => new { item.Key, item.Value }).Json();

            this.Model.ServiceType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ServiceType>().Select(item => new { item.Key, item.Value }).Where(x => x.Key != "0").Json();

            this.Model.StorageType = EnumUtils.ToDictionary<StorageType>().Select(item => new { item.Key, item.Value }).Where(x => x.Key != "0").Json();

            this.Model.ChargeWHType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ChargeWHType>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.ChargeType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ChargeType>().Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 初始化会员基本信息
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            this.Model.ID = id;
            var Files = new Needs.Ccs.Services.Models.CenterFilesTopView().Where(x => x.ClientID == id && x.Status != FileDescriptionStatus.Delete);
            var HKBusinessFile = Files.Where(x => x.Type == (int)Needs.Ccs.Services.Enums.FileType.HKBusinessLicense).OrderByDescending(x => x.CreateDate).Select(item => new { Name = item.CustomName, Url = FileServerUrl + @"/" + item.Url }).FirstOrDefault();
            var BusinessFile = Files.Where(x => x.Type == (int)Needs.Ccs.Services.Enums.FileType.BusinessLicense).OrderByDescending(x => x.CreateDate).Select(item => new { Name = item.CustomName, Url = FileServerUrl + @"/" + item.Url }).FirstOrDefault();
            if (!string.IsNullOrEmpty(id))
            {
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                this.Model.ClientInfoData = new
                {
                    ID = client.ID,
                    ClientCode = client.ClientCode ?? AutoClientNo(),
                    Rank = client.ClientRank,
                    CompanyID = client.Company.ID,
                    CompanyName = client.Company.Name,
                    CustomsCode = client.Company.CustomsCode,
                    CompanyCode = client.Company.Code,
                    Corporate = client.Company.Corporate,
                    Address = client.Company.Address,
                    ContactName = client.Company.Contact.Name,
                    Mobile = client.Company.Contact.Mobile,
                    Tel = client.Company.Contact.Tel,
                    Email = client.Company.Contact.Email,
                    Fax = client.Company.Contact.Fax,
                    Summary = client.Summary,
                    client.ClientNature,
                    ServiceType = client.ServiceType,
                    StorageType = client.StorageType,
                    ChargeWH = client.ChargeWH,
                    ChargeType = client.ChargeType,
                    client.AmountWH,
                    File = BusinessFile,
                    HKBusinessFile = HKBusinessFile,
                    client.ClientStatus
                    //识别 CRM 过来的文件和系统的文件
                }.Json();
            }
            else
            {
                this.Model.ClientInfoData = null;
            }
            this.Model.AutoclientCode = AutoClientNo();
        }

        /// <summary>
        /// 保存会员基本信息
        /// </summary>
        protected void SaveClientInfo()
        {
            var file = Request.Files["BusinessLicense"];
            var ClientID = Request.Form["ClientID"];
            var ClientCode = Request.Form["ClientCode"];
            var CompanyCode = Request.Form["CompanyCode"];
            var CompanyName = Request.Form["CompanyName"];
            var CustomsCode = Request.Form["CustomsCode"];
            var Corporate = Request.Form["Corporate"];
            var Address = Request.Form["Address"];
            var ContactName = Request.Form["ContactName"];
            var Mobile = Request.Form["Mobile"];
            var Tel = Request.Form["Tel"];
            var Email = Request.Form["Email"];
            var Fax = Request.Form["Fax"];
            var Rank = int.Parse(Request.Form["Rank"]);
            var Summary = Request.Form["Summary"];
            var serviceType = Request.Form["ServiceType"];
            var clientNature = Request.Form["ClientNature"];

            //  var hkContactName = Request.Form["Contactor"];
            var certificate = Request.Files["certificate"];
            var storageType = Request.Form["StorageType"];
            //var hkmobile = Request.Form["HKMobile"];
            var ch = Request.Form["ChargeWH"];
            var ChargeWH = int.Parse(Request.Form["ChargeWH"]); //是否收取入仓费

            var chargeType = Request.Form["ChargeType"];
            var AmountWH = Request.Form["AmountWH"];

            //客户名称输入处理
            Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
            string nametep = regex.Replace(CompanyName, " ").Trim();
            CompanyName = Needs.Utils.InputTextExtends.SBCToDBC(nametep);
            var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[ClientID] as Needs.Ccs.Services.Models.Client ??
            new Needs.Ccs.Services.Models.Client();

            if (client.Company == null)
            {
                client.Company = new Needs.Ccs.Services.Models.Company();
                client.Company.Contact = new Needs.Ccs.Services.Models.Contact();
                client.ClientStatus = Needs.Ccs.Services.Enums.ClientStatus.Auditing;
            }
            var oldName = client.Company?.Name;
            if (oldName != null && oldName.IndexOf("reg-") != -1)
            {
                string requestUrl = URL + "/Shenc/EnterpriseReName?OldName=" + oldName + "&NewName=" + CompanyName;
                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpClientHelp().HttpClient("POST", requestUrl);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求会员接口EnterpriseRename失败：" }).Json());
                    return;
                }
            }
            client.Company.Name = CompanyName;
            client.Company.Code = CompanyCode.Trim();
            client.Company.CustomsCode = CustomsCode;
            client.Company.Corporate = Corporate;
            client.Company.Address = Address;
            client.Company.Contact.Name = ContactName;
            client.Company.Contact.Mobile = Mobile;
            client.Company.Contact.Tel = Tel;
            client.Company.Contact.Fax = Fax;
            client.Company.Contact.Email = Email;

            client.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

            client.ClientType = Needs.Ccs.Services.Enums.ClientType.External;
            client.ClientRank = (Needs.Ccs.Services.Enums.ClientRank)Rank;
            client.ClientCode = ClientCode;
            client.Summary = Summary;
            client.ChargeWH = (ChargeWHType)ChargeWH;
            if (!string.IsNullOrEmpty(chargeType))
            {
                client.ChargeType = (ChargeType)int.Parse(chargeType);
            }
            if (!string.IsNullOrWhiteSpace(AmountWH))
            {
                client.AmountWH = Convert.ToDecimal(AmountWH);
            }

            //如果是已完善 将客户类型从代仓储修改为代报关，状态要退回到未完善,修改IsValid 和IsStorageValid 状态
            if (client.ClientStatus == ClientStatus.Confirmed && (ServiceType)int.Parse(serviceType) != client.ServiceType)
            {
                switch ((ServiceType)int.Parse(serviceType))
                {
                    case ServiceType.Customs:
                        if (client.ServiceType == ServiceType.Warehouse)
                        {
                            client.ClientStatus = ClientStatus.Auditing;
                            client.IsValid = false;
                            client.IsStorageValid = false;
                        }
                        else if (client.ServiceType == ServiceType.Both)
                        {
                            client.IsValid = true;
                            client.IsStorageValid = false;
                        }
                        break;

                    case ServiceType.Warehouse:
                        client.IsValid = false;
                        client.IsStorageValid = true;
                        break;

                    case ServiceType.Both:
                        if (client.ServiceType == ServiceType.Customs)
                        {
                            client.IsValid = true;
                            client.IsStorageValid = true;
                        }
                        else if (client.ServiceType == ServiceType.Warehouse)
                        {
                            client.ClientStatus = ClientStatus.Auditing;
                            client.IsValid = false;
                            client.IsStorageValid = true;
                        }
                        break;

                    default:
                        break;

                }

            }

            if (client.ClientStatus != ClientStatus.Confirmed && (ServiceType)int.Parse(serviceType) == ServiceType.Warehouse)
            {
                client.ClientStatus = ClientStatus.WaitingApproval;
            }


            client.ServiceType = (ServiceType)int.Parse(serviceType);

            if (!string.IsNullOrEmpty(storageType))
            {
                if ((StorageType)int.Parse(storageType) == StorageType.Person)
                {
                    client.Company.Contact.Name = CompanyName;
                }
                client.StorageType = (StorageType)int.Parse(storageType);

            }

            if (!string.IsNullOrWhiteSpace(clientNature))
            {
                client.ClientNature = int.Parse(clientNature);

            }
            else
            {
                client.ClientNature = (int)ClientNature.terminal;
            }
            client.EnterError += Client_EnterError;
            client.EnterSuccess += Client_EnterSuccess;
            if (string.IsNullOrEmpty(URL))
            {
                #region 没调接口的代码
                client.Enter();
                //处理附件
                if (file.ContentLength != 0)
                {
                    string fileName = file.FileName.ReName();
                    HttpFile httpFile = new HttpFile(fileName);
                    httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                    httpFile.CreateDataDirectory();
                    string[] result = httpFile.SaveAs(file);

                    var clientfile = new Needs.Ccs.Services.Models.ClientFile();
                    clientfile.ClientID = client.ID;
                    clientfile.Name = fileName;
                    clientfile.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    clientfile.FileType = Needs.Ccs.Services.Enums.FileType.BusinessLicense;
                    clientfile.FileFormat = file.ContentType;
                    clientfile.Url = result[1];
                    clientfile.Enter();
                }
                #endregion
            }
            else
            {
                #region 调用之后的代码

                try
                {
                    client.Enter();
                    var centerURL = string.Empty;
                    var hkURL = string.Empty;
                    var name = string.Empty;
                    var hkname = string.Empty;
                    //处理附件
                    if (file.ContentLength != 0)
                    {
                        string fileName = file.FileName.ReName();
                        HttpFile httpFile = new HttpFile(fileName);
                        httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                        httpFile.CreateDataDirectory();
                        string[] result = httpFile.SaveAs(file);


                        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.BusinessLicense;
                        var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                        var dic = new { CustomName = fileName, ClientID = client.ID, AdminID = ErmAdminID };
                        var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + httpFile.VirtualPath, centerType, dic);
                        centerURL = uploadFile[0].Url;
                        name = uploadFile[0].FileName;
                    }
                    //登记证附件处理
                    if (certificate.ContentLength != 0)
                    {
                        string fileName = certificate.FileName.ReName();
                        HttpFile httpFile = new HttpFile(fileName);
                        httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                        httpFile.CreateDataDirectory();
                        string[] result = httpFile.SaveAs(certificate);

                        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.HKBusinessLicense;
                        var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                        var dic = new { CustomName = fileName, ClientID = client.ID, AdminID = ErmAdminID };
                        var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + httpFile.VirtualPath, centerType, dic);
                        hkURL = uploadFile[0].Url;
                        hkname = uploadFile[0].FileName;
                    }
                    string requestUrl = URL + "/CrmUnify/WsClientEnter";
                    HttpResponseMessage response = new HttpResponseMessage();
                    var entity = new ClientModel()
                    {
                        Enterprise = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "",
                            Corporation = Corporate,
                            Name = CompanyName,
                            RegAddress = Address,
                            Uscc = CompanyCode,
                            Status = 200
                        },
                        Contact = new ApiContact
                        {
                            Type = 1,
                            Name = client.Company.Contact.Name,
                            Mobile = Mobile,
                            Tel = Tel,
                            Email = Email,
                            Fax = Fax,
                            CreateDate = DateTime.Now.ToString(),
                            UpdateDate = DateTime.Now.ToString()
                        },
                        BusinessLicense = new BusinessLicense
                        {
                            Url = centerURL,
                            Name = name,
                            FileFormat = "",
                            Type = (int)Needs.Ccs.Services.Models.ApiModels.Files.FileType.BusinessLicense,
                            Status = 200,
                            CreateDate = DateTime.Now.ToString(),
                            Summary = ""
                        },
                        HKBusinessLicense = new BusinessLicense
                        {

                            Url = hkURL,
                            Name = name,
                            FileFormat = "",
                            Type = (int)Needs.Ccs.Services.Models.ApiModels.Files.FileType.HKBusinessLicense,
                            Status = 200,
                            CreateDate = DateTime.Now.ToString(),
                            Summary = ""
                        },
                        Vip = false,
                        Creator = client.ServiceManager.RealName,//传业务员给CRM
                        CustomsCode = CustomsCode,
                        EnterCode = ClientCode,
                        Grade = Rank,
                        Summary = Summary,
                        Status = client.ClientStatus == Needs.Ccs.Services.Enums.ClientStatus.Confirmed ? 200 : 1,
                        CreateDate = DateTime.Now.ToString(),
                        UpdateDate = DateTime.Now.ToString(),
                        ClientNature = client.ClientNature,
                        ServiceType = (int)client.ServiceType,
                        StorageType = (int)client.StorageType,
                        IsDeclaretion = client.IsValid,
                        IsStorageService = client.IsStorageValid,
                        ChargeWH = ChargeWH,
                    };
                    string apiclient = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }

                    // Response.Write((new { success = true, message = "操作成功" }).Json());
                }
                catch (Exception ex)
                {
                    Response.Write(new { success = false, message = ex.Message });
                }
                #endregion
            }

        }

        /// <summary>
        /// 获取公司信息
        /// </summary>
        /// <returns></returns>
        protected string getCompanyInfo()
        {
            var result = "";
            string id = Request.Form["ID"];
            var Company = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Companies[id];
            if (Company != null)
            {
                result = Company.Json();
            }
            return result;
        }

        /// <summary>
        /// 验证公司编号
        /// </summary>
        /// <returns></returns>
        protected bool CheckClientNo()
        {
            var result = false;
            string clientno = Request.Form["ClientNo"];
            var id = Request.Form["ID"];
            result = Needs.Wl.Admin.Plat.AdminPlat.Clients.Where(t => t.ID != id && t.ClientCode == clientno).Count() < 1;
            return result;

        }

        /// <summary>
        /// 验证公司名称
        /// </summary>
        /// <returns></returns>
        protected void CheckCompanyName()
        {
            try
            {
                string companyName = Request.Form["CompanyName"];
                if (!string.IsNullOrEmpty(companyName))
                {
                    companyName = companyName.Trim();
                }
                var id = Request.Form["ID"];

                bool resultChongfu = Needs.Wl.Admin.Plat.AdminPlat.Clients.Where(t => t.ID != id && t.Company.Name == companyName).Count() < 1;
                if (resultChongfu == false)
                {
                    Response.Write(new { success = false, message = "公司名已存在" }.Json());
                    return;
                }

                // 调用工商信息接口 Begin

                //string adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                //string requestUrl = URL + "/Common/GetEnterpriseByName?" + string.Format("key={0}&siteuserID={1}", companyName, adminID);
                //HttpResponseMessage response = new HttpClientHelp().HttpClient("POST", requestUrl);
                //if (response == null || response.StatusCode != HttpStatusCode.OK)
                //{
                //    Response.Write(new { success = false, message = "查验工商信息失败1" }.Json());
                //    return;
                //}

                //string returnData = response.Content.ReadAsStringAsync().Result;
                //var rawResult = JsonConvert.DeserializeObject<SearchCompanyByNameModel>(returnData);

                //var resultByName = JsonConvert.DeserializeObject<SearchByNameModel>(rawResult.data);
                //if (resultByName.code != "10000" || resultByName.result == null || resultByName.result.error_code != 0)
                //{
                //    Response.Write(new { success = false, message = "查验工商信息失败2" }.Json());
                //    return;
                //}
                //if (resultByName.result.result == null || resultByName.result.result.baseInfo == null)
                //{
                //    Response.Write(new { success = false, message = "查验工商信息失败3" }.Json());
                //    return;
                //}

                // 调用工商信息接口 End


                //天眼查调用接口 start
                string TycUrl = ConfigurationManager.AppSettings["TycUrl"];
                string Tyctoken = ConfigurationManager.AppSettings["Tyctoken"];

                string url = $"{TycUrl}?keyword={companyName}";
                var response = HttpGet.CommonHttpRequest(url, "GET", authorization: Tyctoken);
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                #region 解析工商信息
                try
                {
                    TianyanchaBase baseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<TianyanchaBase>(response);
                    if (baseInfo.error_code == 0 && baseInfo.result != null)
                    {
                        var companyinfo = new
                        {
                            Uscc = baseInfo.result.creditCode, //统一社会信用代码
                            Corporation = baseInfo.result.legalPersonName, //公司法人
                            RegAddress = baseInfo.result.regLocation, //注册地址
                        };

                        Response.Write(new { success = true, message = "公司名称正确", companyinfo = companyinfo }.Json());
                    }
                    else
                    {
                        Response.Write(new { success = false, message = $"获取企业工商信息。结果解析异常：{companyName}" }.Json());
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(new { success = false, message = $"获取企业工商信息。结果解析异常：{companyName}" + ex.Message }.Json());
                }
                #endregion

                //天眼查调用接口 end
            }
            catch (Exception ex)
            {
                Response.Write(new { success = false, message = ex.Message }.Json());
            }
        }


        protected string AutoClientNo()
        {
            var result = 200;
            var codes = Needs.Wl.Admin.Plat.AdminPlat.Clients.Select(t => new { t.ClientCode }).Where(x => x.ClientCode != null && x.ClientCode.Contains(numberPre) && x.ClientCode.Length == 6).OrderBy(x => x.ClientCode).ToArray();
            foreach (var code in codes)
            {
                if (int.TryParse(code.ClientCode.Replace(numberPre, ""), out int number))
                {
                    if (number > 200) {
                        if (number != result + 1)
                        {
                            break;
                        }
                        result = number;
                    }
                }
            }

            return numberPre + (++result).ToString().PadLeft(3, '0');
        }





        //protected string GetClientNo()
        //{
        //    var result = 0;
        //    var codes = Needs.Wl.Admin.Plat.AdminPlat.Clients.Select(t => new { t.ClientCode }).OrderBy(x => x.ClientCode).ToArray();
        //    foreach (var code in codes)
        //    {
        //        if (int.Parse(code.ClientCode.Replace(numberPre, "")) != result + 1)
        //        {
        //            result++;
        //            break;
        //        }
        //    }

        //    return numberPre + result;
        //}

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var entity = sender as Needs.Ccs.Services.Models.Client;
            Response.Write((new { success = true, message = "保存成功", ID = e.Object, serviceType = entity.ServiceType.GetHashCode() }).Json());
        }
    }
}
