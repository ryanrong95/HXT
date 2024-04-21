using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yahv.Csrm.WebApi.Models;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;
using nPayee = YaHv.Csrm.Services.Models.Origins.nPayee;

namespace Yahv.Csrm.WebApi.Controllers
{



    /// <summary>
    /// Crm贯通
    /// </summary>
    public class CrmUnifyController : ClientController
    {
        public Enterprise Company = new CompaniesRoll()["DBAEAB43B47EB4299DD1D62F764E6B6A"].Enterprise;
        // GET: Test
        /// <summary>
        /// 客户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return null;
        }

        #region 代仓储客户添加，修改
        /// <summary>
        /// 代仓储客户添加，修改
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Code:100 参数不全，200 成功,300 内部错误</returns>
        [HttpPost]
        public ActionResult WsClientEnter([System.Web.Http.FromBody]Models.Client client)
        {
            try
            {
                var creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == client.Creator);
                if (client.Enterprise == null || string.IsNullOrWhiteSpace(client.Enterprise.Name) || string.IsNullOrWhiteSpace(client.Contact.Name))
                {
                    return eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空,联系人不能为空" });
                }
                else if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }
                else if (creator == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "Creator不存在" });
                }
                else
                {
                    var Creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == client.Creator);
                    Enterprise enterprise = new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll().FirstOrDefault(item => item.Name == client.Enterprise.Name);
                    var user = new SiteUserXdt();
                    if (enterprise != null)
                    {
                        user.EnterpriseID = enterprise.ID;
                        client.Enterprise.AdminCode = enterprise.AdminCode;
                    }
                    client.Enterprise.AdminCode = string.IsNullOrWhiteSpace(client.Enterprise.AdminCode) ? "" : client.Enterprise.AdminCode;
                    var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == client.Enterprise.Name) ?? new WsClient();
                    bool oldIsStorageService = wsclient.IsStorageService;
                    bool oldIsDeclaretion = wsclient.IsDeclaretion;

                    wsclient.EnterCode = client.EnterCode;
                    wsclient.CustomsCode = client.CustomsCode;
                    wsclient.Grade = client.Grade;
                    wsclient.Enterprise = client.Enterprise;
                    wsclient.Vip = client.Vip;
                    wsclient.Summary = client.Summary;
                    wsclient.WsClientStatus = client.Status;
                    wsclient.CreatorID = Creator.ID;
                    wsclient.Company = Company;//芯达通内部公司
                    wsclient.Nature = client.ClientNature;
                    wsclient.ServiceType = client.ServiceType;
                    wsclient.IsDeclaretion = client.IsDeclaretion;
                    wsclient.IsStorageService = client.IsStorageService;
                    wsclient.StorageType = client.StorageType;
                    wsclient.ChargeWHType = client.ChargeWH;//是否收取入仓费
                    wsclient.Enter();

                    wsclient.Assin(Creator.ID, MapsType.ServiceManager);

                    if (wsclient.SiteUsers.Any())
                    {
                        if (oldIsStorageService != client.IsStorageService || oldIsDeclaretion != client.IsDeclaretion)
                        {
                            foreach (var item in wsclient.SiteUsers)
                            {
                                item.IsDeclaretion = client.IsDeclaretion;
                                item.IsStorageService = client.IsStorageService;
                                item.Enter();
                            }
                        }
                    }
                    else
                    {
                        user.EnterpriseID = wsclient.ID;
                        user.UserName = wsclient.Enterprise.Name;
                        user.Password = "XDT123".StrToMD5();
                        user.IsStorageService = client.IsStorageService;
                        user.IsDeclaretion = client.IsDeclaretion;
                        user.IsMain = true;
                        user.UserNameRepeat += SiteUser_UserNameRepeat;
                        user.EnterSuccess += SiteUser_EnterSuccess;
                        user.Enter();
                    }

                    //添加联系人
                    if (client.Contact != null)
                    {
                        new WsContact
                        {
                            Type = client.Contact.Type,
                            Name = client.Contact.Name,
                            Tel = client.Contact.Tel,
                            Mobile = client.Contact.Mobile,
                            Fax = client.Contact.Fax,
                            Enterprise = client.Enterprise,
                            Email = client.Contact.Email,
                            Status = YaHv.Csrm.Services.Status.Normal,
                            CreateDate = client.Contact.CreateDate,
                            UpdateDate = client.Contact.CreateDate,
                            CreatorID = creator.ID,
                            EnterpriseID = client.Enterprise.ID,
                            IsDefault = true
                        }.Enter();
                    }

                    //营业执照上传
                    if (client.BusinessLicense != null && !string.IsNullOrWhiteSpace(client.BusinessLicense?.Url))
                    {
                        new YaHv.Csrm.Services.Models.Origins.FileDescription
                        {
                            Enterprise = client.Enterprise,
                            Name = client.BusinessLicense.Name,
                            Type = client.BusinessLicense.Type,
                            Url = client.BusinessLicense.Url,
                            EnterpriseID = client.Enterprise.ID,
                            FileFormat = client.BusinessLicense.FileFormat,
                            Summary = client.BusinessLicense.Summary,
                            CreatorID = creator.ID
                        }.Enter();
                    }
                    if (client.HKBusinessLicense != null && !string.IsNullOrWhiteSpace(client.HKBusinessLicense?.Url))
                    {
                        new YaHv.Csrm.Services.Models.Origins.FileDescription
                        {
                            Enterprise = client.Enterprise,
                            Name = client.HKBusinessLicense.Name,
                            Type = client.HKBusinessLicense.Type,
                            Url = client.HKBusinessLicense.Url,
                            EnterpriseID = client.Enterprise.ID,
                            FileFormat = client.HKBusinessLicense.FileFormat,
                            Summary = client.HKBusinessLicense.Summary,
                            CreatorID = creator.ID
                        }.Enter();
                    }
                    eJson(new JMessage { code = 200, success = true, data = null });
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });

            }
            return eJson();
        }
        #endregion

        #region SetGrade
        [HttpPost]
        public ActionResult SetGrade()
        {
            try
            {
                string name = Request["EnterpriseName"];
                int grade = int.Parse(Request["Grade"]);
                if (grade < 1 || grade > 9)
                {
                    return eJson(new JMessage { code = 400, success = false, data = "" });//等级超出范围
                }
                var client = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == name);
                if (client == null)
                {
                    return eJson(new JMessage { code = 300, success = false, data = "" });//客户不存在
                }
                client.Grade = (Yahv.Underly.ClientGrade)grade;
                client.Enter();
                //client.EnterSuccess += Client_EnterSuccess;
                return eJson(new JMessage { code = 200, success = true, data = "保存成功" });
            }
            catch (Exception ex)
            {
                return eJson(new JMessage { code = 100, success = false, data = "" });
            }
        }
        #endregion
        #region 删除代仓储客户
        /// <summary>
        /// 删除代仓储客户
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelWsClient([System.Web.Http.FromBody]Models.Client client)
        {
            try
            {
                var clientid = client.Enterprise.Name.MD5();
                var entity = new WsClientsRoll()[clientid];
                if (entity == null)
                {
                    eJson(new JMessage { code = 101, success = false, data = null });
                }
                else
                {
                    WsClient wsclient = new WsClient
                    {
                        Enterprise = client.Enterprise,
                        Company = Company
                    };
                    wsclient.AbandonSuccess += Client_AbandonSuccess;
                    wsclient.ID = clientid;
                    wsclient.Company = Company;
                    wsclient.Abandon();
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }
            return eJson();
        }

        private void Client_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }
        #endregion

        #region 客户到货地址
        /// <summary>
        /// 添加或修改到货地址
        /// </summary>
        /// <param name="Consignee"></param>
        /// <returns>Code:100 参数不全，200 成功,300 内部错误</returns>
        [HttpPost]
        public ActionResult ConsigneeEnter([System.Web.Http.FromBody]Models.Consignee Consignee)
        {
            try
            {
                var creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == Consignee.Creator);

                if (Consignee.Enterprise == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空" });
                }
                else if (Consignee.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }
                else if (creator == null)
                {
                    return eJson(new JMessage { code = 400, success = false, data = "Creator不存在" });
                }
                else
                {
                    var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == Consignee.Enterprise.Name);
                    //string address1 = Consignee.Address.Split(' ')[0] == "中国" ? Consignee.Address : string.Join(" ", "中国", Consignee.Address);
                    Consignee.Enterprise = wsclient.Enterprise;
                    var entity = new WsConsignee
                    {
                        Title = Consignee.Title,
                        Place = Consignee.Place == null ? Origin.CHN.GetOrigin().Code : Consignee.Place,
                        EnterpriseID = Consignee.Enterprise.ID,
                        DyjCode = string.IsNullOrWhiteSpace(Consignee.DyjCode) ? "" : Consignee.DyjCode,
                        District = Consignee.District,
                        Address = Consignee.Address,
                        Postzip = string.IsNullOrWhiteSpace(Consignee.Postzip) ? "" : Consignee.Postzip,
                        Name = Consignee.Name,
                        Tel = string.IsNullOrWhiteSpace(Consignee.Tel) ? "" : Consignee.Tel,
                        Mobile = Consignee.Mobile,
                        Email = Consignee.Email,
                        Status = Consignee.Status,
                        CreatorID = creator.ID,
                        Province = Consignee.Province,
                        City = Consignee.City,
                        Land = Consignee.Land,
                        Enterprise = Consignee.Enterprise,
                        IsDefault = Consignee.IsDefault
                    };
                    var consignee = new WsClientsRoll()[Consignee.Enterprise.ID].Consignees[entity.ID];
                    if (!string.IsNullOrWhiteSpace(consignee?.Tel))
                    {
                        entity.Tel = consignee.Tel;
                    }
                    entity.Enter();
                    eJson(new JMessage { code = 200, success = true, data = null });
                }
            }
            catch (Exception ex)
            {
                return eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }
        #endregion

        #region 删除到货地址
        /// <summary>
        /// 删除到货地址
        /// </summary>
        /// <param name="consignee"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelConsignee([System.Web.Http.FromBody]WsConsignee consignee)
        {
            try
            {
                if (consignee.Enterprise == null || consignee.Enterprise.ID == null)
                {
                    eJson(new JMessage { code = 100, success = false, data = null });
                }
                else
                {
                    //string address1 = consignee.Address.Split(' ')[0] == "中国" ? consignee.Address : string.Join(" ", "中国", consignee.Address);
                    consignee.Address = consignee.Address;
                    consignee.EnterpriseID = consignee.Enterprise.ID;
                    consignee.AbandonSuccess += Consignee_AbandonSuccess;
                    consignee.Abandon();
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }

        private void Consignee_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }
        #endregion

        #region 添加或修改发票
        /// <summary>
        /// 添加或修改发票
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns> 
        [HttpPost]
        public ActionResult InvoiceEnter([System.Web.Http.FromBody]Models.Invoice invoice)
        {
            try
            {
                var creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == invoice.Creator);
                if (invoice.Enterprise == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空" });
                }
                else if (invoice.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }
                else if (creator == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "Creator不存在" });
                }
                else
                {
                    var client = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == invoice.Enterprise.Name);
                    var _invoice = client.Invoice;
                    //发票中的地址如果有变化修改企业信息中的RegAddress
                    //if (invoice.Enterprise.RegAddress != client.Enterprise.RegAddress)
                    //{
                    //    client.Enterprise.RegAddress = invoice.Enterprise.RegAddress;
                    //    client.Enterprise.Enter();
                    //}
                    Underly.InvoiceType type = Underly.InvoiceType.Unkonwn;
                    if (_invoice?.Type != null)
                    {
                        type = _invoice.Type;
                    }
                    var entity = new WsInvoice
                    {
                        EnterpriseID = client.Enterprise.ID,
                        Enterprise = client.Enterprise,
                        CompanyTel = invoice.CompanyTel,
                        Bank = invoice.Bank,
                        BankAddress = _invoice?.BankAddress,
                        Type = type,
                        Account = invoice.Account,
                        TaxperNumber = invoice.TaxperNumber,
                        District = invoice.District,
                        Address = invoice.Address,
                        Postzip = _invoice == null ? invoice.Postzip : _invoice.Postzip,
                        Name = invoice.Name,
                        Tel = invoice.Tel,
                        Mobile = invoice.Mobile,
                        Email = invoice.Email,
                        Status = invoice.Status,
                        CreatorID = creator.ID,
                        Province = invoice.Province,
                        City = invoice.City,
                        Land = invoice.Land,
                        DeliveryType = (InvoiceDeliveryType)invoice.DeliveryType,
                        IsDefault = true,
                        InvoiceAddress = invoice.InvoiceAddrss
                    };
                    entity.Enter();
                    eJson(new JMessage { code = 200, success = true, data = null });
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }
            return eJson();
        }

        #endregion

        #region 删除发票
        /// <summary>
        /// 删除发票
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelInvoice([System.Web.Http.FromBody]WsInvoice invoice)
        {
            try
            {
                if (invoice.Enterprise == null)
                {
                    eJson(new JMessage { code = 100, success = false, data = null });
                }
                else
                {
                    var enterprise = new EnterprisesRoll().FirstOrDefault(item => item.Name == invoice.Enterprise.Name);
                    invoice.EnterpriseID = enterprise.ID;
                    invoice.AbandonSuccess += Invoice_AbandonSuccess;
                    invoice.Abandon();
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }

        private void Invoice_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }
        #endregion

        #region 供应商
        /// <summary>
        /// 添加供应商(关系)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WsSupplierEnter([System.Web.Http.FromBody]Models.ClientSuppler data)
        {
            try
            {
                var creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == data.Creator);
                if (data.Client == null || data.Supplier.Enterprise == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空" });
                }
                else if (data.Client.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }

                else if (data.Creator == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "Creator不存在" });
                }
                else
                {
                    var admincode = string.IsNullOrWhiteSpace(data.Supplier.Enterprise.AdminCode) ? "" : data.Supplier.Enterprise.AdminCode;
                    data.Supplier.Enterprise.AdminCode = admincode;
                    var client = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == data.Client.Name);
                    var nsupplier = client.nSuppliers.FirstOrDefault(item => item.RealEnterprise.Name == data.Supplier.Enterprise.Name)
                    ?? new nSupplier();
                    Enterprise enterprise = new EnterprisesRoll().FirstOrDefault(item => item.Name == data.Supplier.Enterprise.Name);
                    if (enterprise != null)
                    {
                        nsupplier.RealEnterprise = new Enterprise();
                        nsupplier.RealEnterprise.AdminCode = enterprise.AdminCode;
                    }
                    nsupplier.RealEnterprise = data.Supplier.Enterprise;
                    nsupplier.ChineseName = data.Supplier.ChineseName;
                    nsupplier.EnglishName = data.Supplier.EnglishName;
                    nsupplier.Grade = data.Supplier.Grade;
                    nsupplier.Creator = creator.ID;
                    nsupplier.Summary = data.Supplier.Summary;
                    nsupplier.Status = data.Supplier.Status;
                    nsupplier.EnterpriseID = client.ID;
                    //更新时间-20210324 byjss
                    nsupplier.UpdateDate = DateTime.Now;

                    nsupplier.Enter();
                    //wssupplier.MapsSupplier(wssupplier.Enterprise.ID, wssupplier.Creator.ID);
                    eJson(new JMessage { code = 200, success = true, data = null });
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }

        #endregion

        #region 删除客户与供应商的关系
        /// <summary>
        /// 删除客户与供应商的关系
        /// </summary>
        /// <param name="client"></param>
        /// <param name=supplier"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult DelWsSuppliers([System.Web.Http.FromBody]Models.ClientSuppler data)
        {
            try
            {
                if (data.Client == null || data.Supplier.Enterprise == null)
                {
                    eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空" });
                }
                else
                {
                    var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == data.Client.Name);
                    var supplier = wsclient.nSuppliers.SingleOrDefault(item => item.RealEnterprise.Name == data.Supplier.Enterprise.Name);
                    supplier.Abandon();
                    eJson(new JMessage { code = 200, success = true, data = null });
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }
        #endregion

        #region 业务员
        /// <summary>
        /// 分配业务员，跟单员
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Assign([System.Web.Http.FromBody]Models.AssinAdmin data)
        {
            try
            {
                var serviceManager = new AdminsAllRoll().FirstOrDefault(item => item.RealName == data.ServiceManager);
                var merchandiser = new AdminsAllRoll().FirstOrDefault(item => item.RealName == data.Merchandiser);
                var refferer = new AdminsAllRoll().FirstOrDefault(item => item.RealName == data.Referrer);
                if (serviceManager == null)
                {
                    eJson(new JMessage { code = 300, success = false, data = "业务员不存在" });
                }
                else
                {
                    var client = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == data.Client.Name);
                    if (client == null)
                    {
                        eJson(new JMessage { code = 300, success = false, data = "客户不存在" });
                    }
                    else
                    {
                        client.Company = Company;
                        if (client.IsDeclaretion != data.IsDeclaretion || client.IsStorageService != data.IsStorageService)
                        {
                            client.IsDeclaretion = data.IsDeclaretion;
                            client.IsStorageService = data.IsStorageService;
                            client.Enter();
                            foreach (var item in client.SiteUsers)
                            {
                                item.IsDeclaretion = data.IsDeclaretion;
                                item.IsStorageService = data.IsStorageService;
                                item.Enter();
                            }
                        }
                        client.Assin(serviceManager.ID, MapsType.ServiceManager);
                        if (refferer != null)
                        {
                            client.Assin(refferer.ID, MapsType.Referrer);
                        }
                        if (merchandiser != null)
                        {
                            client.Assin(merchandiser.ID, MapsType.Merchandiser);
                            if (client.WsClientStatus != ApprovalStatus.Normal)
                            {
                                client.Complete();
                            }
                        }
                        eJson(new JMessage { code = 200, success = true, data = null });
                    }
                }

            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 100, success = false, data = ex.Message });
            }
            return eJson();
        }

        #endregion

        #region 添加或修改受益人
        /// <summary>
        /// 
        /// </summary>
        /// <param name="beneficiary">受益人</param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult BenefciaryEnter([System.Web.Http.FromBody]Models.nPayee beneficiary)
        {
            try
            {
                var creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == beneficiary.Creator);
                if (beneficiary.WsClient == null || beneficiary.Enterprise == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "客户企业信息与供应商企业信息不能为空" });
                }
                else if (beneficiary.WsClient.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }
                else if (creator == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "Creator不存在" });
                }
                else
                {
                    var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == beneficiary.WsClient.Name);
                    var nsupplier = wsclient.nSuppliers.SingleOrDefault(item => item.RealEnterprise.Name == beneficiary.Enterprise.Name);
                    var entity = new nPayee
                    {
                        EnterpriseID = wsclient.Enterprise.ID,
                        RealEnterprise = nsupplier.RealEnterprise,
                        RealID = nsupplier.RealEnterprise.ID,
                        nSupplierID = nsupplier.ID,
                        Bank = beneficiary.Bank,
                        BankAddress = beneficiary.BankAddress,
                        Account = beneficiary.Account,
                        SwiftCode = beneficiary.SwiftCode,
                        Methord = beneficiary.Methord,
                        Currency = beneficiary.Currency,
                        Contact = beneficiary.Name,
                        Tel = beneficiary.Tel,
                        Mobile = beneficiary.Mobile,
                        Email = beneficiary.Email,
                        Status = beneficiary.Status,
                        Creator = creator.ID,
                        IsDefault = beneficiary.IsDefault,
                        Place = beneficiary.Place
                    };
                    var npayee = nsupplier.nPayees.FirstOrDefault(item =>
                            item.EnterpriseID == wsclient.ID
                        && item.RealID == nsupplier.RealEnterprise.ID
                        && item.nSupplierID == nsupplier.ID
                        && item.Account == entity.Account
                        && item.Methord == entity.Methord
                        && item.Currency == entity.Currency);
                    if (npayee != null)
                    {
                        entity.ID = npayee.ID;
                        entity.Contact = npayee.Contact;
                        entity.Tel = npayee.Tel;
                        entity.Email = npayee.Email;
                        entity.ID = npayee.ID;

                    }
                    entity.Enter();
                    eJson(new JMessage { code = 200, success = true, data = null });
                }

            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 100, success = false, data = ex.Message });
            }
            return eJson();
        }

        #endregion

        #region 删除受益人
        /// <summary>
        /// 删除受益人
        /// </summary>
        /// <param name="beneficiary"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult DelBeneficiary([System.Web.Http.FromBody]Models.nPayee beneficiary)
        {
            try
            {
                if (beneficiary.WsClient == null || beneficiary.Enterprise == null)
                {
                    eJson(new JMessage { code = 100, success = false, data = "客户企业信息与供应商企业信息不能为空" });
                }
                else
                {
                    var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == beneficiary.WsClient.Name);
                    var nsupplier = wsclient.nSuppliers.SingleOrDefault(item => item.RealEnterprise.Name == beneficiary.Enterprise.Name);

                    if (nsupplier == null)
                    {
                        eJson(new JMessage { code = 100, success = false, data = "供应商企业不存在" });
                    }
                    else
                    {
                        var entity = new nPayee
                        {
                            EnterpriseID = nsupplier.Enterprise.ID,
                            RealEnterprise = nsupplier.Enterprise,
                            RealID = nsupplier.Enterprise.ID,
                            nSupplierID = nsupplier.ID,
                            Bank = beneficiary.Bank,
                            BankAddress = beneficiary.BankAddress,
                            Account = beneficiary.Account,
                            SwiftCode = beneficiary.SwiftCode,
                            Methord = beneficiary.Methord,
                            Currency = beneficiary.Currency,
                            Contact = beneficiary.Name,
                            Tel = beneficiary.Tel,
                            Mobile = beneficiary.Mobile,
                            Email = beneficiary.Email,
                            Status = beneficiary.Status,
                            IsDefault = beneficiary.IsDefault,
                            Place = beneficiary.Place
                        };

                        var npayee = nsupplier.nPayees.FirstOrDefault(item =>
                            item.EnterpriseID == wsclient.ID
                        && item.RealID == nsupplier.RealEnterprise.ID
                        && item.nSupplierID == nsupplier.ID
                        && item.Account == entity.Account
                        && item.Methord == entity.Methord
                        && item.Currency == entity.Currency);
                        if (npayee == null)
                        {
                            return eJson(new JMessage { code = 100, success = false, data = "收款账号不存在" });

                        }
                        else
                        {
                            entity.ID = npayee.ID;
                        }
                        entity.AbandonSuccess += Beneficiary_AbandonSuccess;
                        entity.Abandon();
                    }

                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 100, success = false, data = ex.Message });
            }
            return eJson();
        }

        private void Beneficiary_AbandonSuccess(object sender, Usually.AbandonedEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }

        #endregion

        #region 营业执照上传
        /// <summary>
        /// 营业执照上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ActionResult FileEnter([System.Web.Http.FromBody]Models.FileDesc file)
        {
            try
            {
                var creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == file.Creator);
                var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == file.Enterprise.Name);
                if (file.Enterprise == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "客户企业信息不能为空" });
                }
                else if (file.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }
                else if (wsclient == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "客户不存在" });
                }
                else if (creator == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "Creator不能为空" });
                }
                else
                {

                    YaHv.Csrm.Services.Models.Origins.FileDescription entity = new YaHv.Csrm.Services.Models.Origins.FileDescription
                    {
                        EnterpriseID = wsclient.Enterprise.ID,
                        Enterprise = wsclient.Enterprise,
                        Name = file.Name,
                        Type = file.Type,
                        Url = file.Url,
                        FileFormat = file.FileFormat,
                        Status = file.Status,
                        CreatorID = creator.ID,
                        Summary = file.Summary,
                        CreateDate = file.CreateDate
                    };
                    entity.EnterSuccess += File_EnterSuccess;
                    entity.Enter();
                }

            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 100, success = false, data = ex.Message });
            }
            return eJson();
        }

        private void File_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }
        #endregion

        #region 供应商的提货地址
        /// <summary>
        /// 新增或修改供应商的提货地址
        /// </summary>
        /// <param name="consignor"></param>
        /// <returns></returns>
        public ActionResult ConsignorEnter([System.Web.Http.FromBody]Models.Consignor consignor)
        {
            try
            {
                var creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == consignor.Creator);
                if (consignor.Enterprise == null || consignor.WsClient == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "客户企业信息与供应商企业信息不能为空" });
                }
                else if (consignor.WsClient.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }
                else if (creator == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "Creator不存在" });
                }
                else
                {
                    var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == consignor.WsClient.Name);
                    if (wsclient == null)
                    {
                        return eJson(new JMessage { code = 100, success = false, data = "客户不存在" });
                    }
                    else if (!wsclient.nSuppliers.Any(item => item.RealEnterprise.Name == consignor.Enterprise.Name))
                    {
                        return eJson(new JMessage { code = 100, success = false, data = "供应商不存在" });
                    }
                    else
                    {
                        var nsupplier = wsclient.nSuppliers.SingleOrDefault(item => item.RealEnterprise.Name == consignor.Enterprise.Name);
                        string address1 = consignor.Address.Split(' ')[0] == "中国" ? consignor.Address : string.Join(" ", "中国", consignor.Address);
                        nConsignor entity = new nConsignor
                        {
                            Title = consignor.Title,
                            EnterpriseID = nsupplier.Enterprise.ID,
                            RealID = nsupplier.RealEnterprise.ID,
                            nSupplierID = nsupplier.ID,
                            Province = consignor.Province,
                            City = consignor.City,
                            Land = consignor.Land,
                            Address = address1,
                            Postzip = consignor.Postzip == null ? "" : consignor.Postzip,
                            Contact = consignor.Name,
                            Tel = consignor.Tel,
                            Mobile = consignor.Mobile,
                            Email = consignor.Email,
                            Status = consignor.Status,
                            Creator = creator.ID,
                            IsDefault = consignor.IsDefault,
                            Place = consignor.Place == null ? Origin.HKG.GetOrigin().Code : consignor.Place

                        };
                        var nconsignor = nsupplier.nConsignors.ToArray().SingleOrDefault(item => item.UniqueSign == entity.UniqueSign);
                        if (nconsignor != null)
                        {
                            entity.ID = nconsignor.ID;
                        }
                        entity.EnterSuccess += Consignor_EnterSuccess;
                        entity.Enter();
                    }
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage
                {
                    code = 300,
                    success = false,
                    data = ex.Message
                });
            }

            return eJson();
        }

        private void Consignor_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }
        #endregion

        #region 删除供应商的提货地址
        /// <summary>
        /// 删除供应商的提货地址
        /// </summary>
        /// <param name="consignor"></param>
        /// <returns></returns>
        public ActionResult DelConsignor([System.Web.Http.FromBody]Models.Consignor consignor)
        {
            try
            {
                if (consignor.Enterprise == null || consignor.WsClient == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "客户企业信息与供应商企业信息不能为空" });
                }
                else
                {
                    var client = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == consignor.WsClient.Name);
                    var supplier = client.nSuppliers.SingleOrDefault(item => item.RealEnterprise.Name == consignor.Enterprise.Name);
                    string address1 = consignor.Address.Split(' ')[0] == "中国" ? consignor.Address : string.Join(" ", "中国", consignor.Address);
                    nConsignor entity = new nConsignor
                    {
                        Title = consignor.Title,
                        EnterpriseID = supplier.Enterprise.ID,
                        RealID = supplier.RealEnterprise.ID,
                        nSupplierID = supplier.ID,
                        //DyjCode = consignor.DyjCode == null ? "" : consignor.DyjCode,
                        Province = consignor.Province,
                        City = consignor.City,
                        Land = consignor.Land,
                        Address = address1,
                        Postzip = consignor.Postzip == null ? "" : consignor.Postzip,
                        Contact = consignor.Name,
                        Tel = consignor.Tel,
                        Mobile = consignor.Mobile,
                        Email = consignor.Email,
                        Status = consignor.Status,
                        IsDefault = consignor.IsDefault,
                        Place = consignor.Place == null ? Origin.HKG.GetOrigin().Code : consignor.Place
                    };
                    var nconsignor = supplier.nConsignors.ToArray().SingleOrDefault(item => item.UniqueSign == entity.UniqueSign);

                    nconsignor.AbandonSuccess += Consignor_AbandonSuccess; ;
                    nconsignor.Abandon();
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }

        private void Consignor_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }
        #endregion

        #region 网站用户
        /// <summary>
        /// 新增网站用户，修改，重置密码
        /// </summary>
        /// <param name="siteUser"></param>
        /// <returns></returns>
        public ActionResult SiteUser([System.Web.Http.FromBody]SiteUserXdt siteUser)

        {
            try
            {
                //SiteUserXdt siteUser = HttpUtility.HtmlDecode(data).JsonTo<SiteUserXdt>();
                if (siteUser.Enterprise == null)
                {
                    eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空" });
                }
                else if (siteUser.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }
                else
                {
                    var enterprise = new EnterprisesRoll().FirstOrDefault(item => item.Name == siteUser.Enterprise.Name);
                    var user = new SiteUsersXdtRoll(enterprise).FirstOrDefault(item => item.UserName == siteUser.UserName);
                    siteUser.ID = user?.ID;
                    siteUser.EnterpriseID = enterprise.ID;
                    siteUser.Password = siteUser.Password;
                    siteUser.UserNameRepeat += SiteUser_UserNameRepeat;
                    siteUser.EnterSuccess += SiteUser_EnterSuccess;

                    siteUser.QQ = user?.QQ;
                    siteUser.Wx = user?.Wx;

                    siteUser.Enter();
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }

        private void SiteUser_UserNameRepeat(object sender, Usually.ErrorEventArgs e)
        {
            eJson(new JMessage { code = 400, success = false, data = "用户名已存在" });
        }

        private void SiteUser_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }
        #endregion


        #region 网站用户删除
        /// <summary>
        /// DelSiteUser
        /// </summary>
        /// <param name="siteUser"></param>
        /// <returns></returns>
        public ActionResult DelSiteUser([System.Web.Http.FromBody]SiteUserXdt siteUser)

        {
            try
            {
                //SiteUserXdt siteUser = HttpUtility.HtmlDecode(data).JsonTo<SiteUserXdt>();

                if (siteUser.Enterprise == null)
                {
                    eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空" });
                }

                else
                {
                    var xdt = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == siteUser.Enterprise.Name).SiteUsers.SingleOrDefault(item => item.UserName == siteUser.UserName);
                    if (xdt == null)
                    {
                        eJson(new JMessage { code = 100, success = false, data = "用户不存在" });
                    }
                    else
                    {
                        siteUser.ID = xdt.ID;
                        siteUser.EnterpriseID = xdt.Enterprise.ID;
                        siteUser.AbandonSuccess += SiteUser_AbandonSuccess; ; ;
                        siteUser.Abandon();
                    }
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }

        private void SiteUser_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = null });
        }
        #endregion

        #region 网站用户重置密码
        /// <summary>
        /// 新增网站用户，修改，重置密码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult ResetPwd([System.Web.Http.FromBody]SiteUserXdt siteUser)

        {
            try
            {
                // SiteUserXdt siteUser = HttpUtility.HtmlDecode(data).JsonTo<SiteUserXdt>();
                if (siteUser.Enterprise == null)
                {
                    eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空" });
                }
                else
                {
                    var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == siteUser.Enterprise.Name);
                    siteUser.EnterpriseID = wsclient.Enterprise.ID;
                    siteUser.ResetPwd();
                    eJson(new JMessage { code = 200, success = true, data = null });
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }
        #endregion


        #region 合同
        public ActionResult Contract([System.Web.Http.FromBody]Models.ApiContract apicontract)
        {
            try
            {
                var creator = new AdminsAllRoll().FirstOrDefault(item => item.RealName == apicontract.Creator);
                if (apicontract.Enterprise == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "企业信息不能为空" });
                }
                else if (apicontract.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户名称不规范" });
                }
                else if (creator == null)
                {
                    return eJson(new JMessage { code = 100, success = false, data = "Creator不存在" });
                }
                else
                {
                    var wsclient = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == apicontract.Enterprise.Name);
                    apicontract.Enterprise.ID = wsclient.Enterprise.ID;
                    if (apicontract.Agreement != null)
                    {
                        var contract = new Contract
                        {
                            Enterprise = apicontract.Enterprise,
                            StartDate = apicontract.Agreement.StartDate,
                            EndDate = apicontract.Agreement.EndDate,
                            AgencyRate = apicontract.Agreement.AgencyRate,
                            MinAgencyFee = apicontract.Agreement.MinAgencyFee,
                            InvoiceTaxRate = apicontract.Agreement.InvoiceTaxRate,
                            InvoiceType = apicontract.Agreement.InvoiceType,
                            ExchangeMode = apicontract.Agreement.ExchangeMode,
                            CreatorID = creator.ID,
                            Summary = apicontract.Agreement.Summary,
                            CreateDate = apicontract.Agreement.CreateDate,
                            UpdateDate = apicontract.Agreement.UpdateDate,
                            Company = Company
                        };
                        contract.Enter();
                        //同步信用和账期
                        string conduct = "供应链";
                        var payment = PaymentManager.Erp(creator.ID)[apicontract.Enterprise.ID, Company.ID][conduct];
                        SyncCredit(apicontract.Agreement.AgencyFeeClause, CatalogConsts.代理费, payment);    //代理费
                        SyncCredit(apicontract.Agreement.IncidentalFeeClause, CatalogConsts.杂费, payment);    //杂费
                        SyncCredit(apicontract.Agreement.ProductFeeClause, CatalogConsts.货款, payment);    //货款
                        SyncCredit(apicontract.Agreement.TaxFeeClause, CatalogConsts.税款, payment);    //税款

                        eJson(new JMessage { code = 200, success = true, data = null });
                    }
                    if (apicontract.ServiceAgreement != null && !string.IsNullOrWhiteSpace(apicontract.ServiceAgreement.Url))
                    {
                        var serviceAgreement = new YaHv.Csrm.Services.Models.Origins.FileDescription
                        {
                            Enterprise = apicontract.Enterprise,
                            CompanyID = Company.ID,
                            EnterpriseID = apicontract.Enterprise.ID,
                            Name = apicontract.ServiceAgreement.Name,
                            Type = apicontract.ServiceAgreement.Type,
                            Url = apicontract.ServiceAgreement.Url,
                            FileFormat = apicontract.ServiceAgreement.FileFormat,
                            Status = apicontract.ServiceAgreement.Status,
                            CreatorID = creator.ID,
                            Summary = apicontract.ServiceAgreement.Summary,
                            CreateDate = apicontract.ServiceAgreement.CreateDate
                        };
                        serviceAgreement.Enter();
                        eJson(new JMessage { code = 200, success = true, data = null });
                    }
                }
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }

            return eJson();
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 芯达通转换账期
        /// </summary>
        /// <param name="xdt"></param>
        /// <returns></returns>
        private DebtTerm ConvertDebtTerm(ClientFeeSettlement xdt, DebtTerm model)
        {
            DebtTerm debtTerm = model;


            //约定期限
            if (xdt.PeriodType == PeriodType.AgreedPeriod)
            {
                debtTerm.SettlementType = SettlementType.DueTime;

                debtTerm.Months = 0;
                debtTerm.Days = xdt.DaysLimit ?? 0;
            }

            //月结
            if (xdt.PeriodType == PeriodType.Monthly)
            {
                debtTerm.SettlementType = SettlementType.Month;

                debtTerm.Months = 1;
                debtTerm.Days = xdt.MonthlyDay ?? 0;
            }

            //汇率
            switch (xdt.ExchangeRateType)
            {
                case ExchangeRateType.Custom:
                    debtTerm.ExchangeType = ExchangeType.Customs;
                    break;
                case ExchangeRateType.RealTime:
                    debtTerm.ExchangeType = ExchangeType.Floating;
                    break;
                default:
                    debtTerm.ExchangeType = ExchangeType.TenAmChineseBank;
                    break;
            }

            return debtTerm;
        }

        /// <summary>
        /// 同步信用
        /// </summary>
        /// <param name="item">费用类</param>
        /// <param name="catalog">分类</param>
        /// <param name="payment">财务</param>
        private void SyncCredit(ClientFeeSettlement item, string catalog, Accountors payment)
        {
            try
            {
                if (item != null && item.PeriodType != PeriodType.PrePaid)
                {
                    decimal creditLine = (item?.UpperLimit - payment.Credit[catalog][Currency.CNY].Total) ?? 0;
                    payment.Credit[catalog].Credit(Currency.CNY, creditLine);       //同步信用

                    DebtTerm model = payment.DebtTerm[catalog] ?? new DebtTerm();
                    model = ConvertDebtTerm(item, model);
                    model.Enter();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 仓储协议
        [HttpPost]
        public ActionResult StorageAgreement(string ClientName, string FileID)
        {
            try
            {
                var client = new WsClientsRoll().FirstOrDefault(item => item.Enterprise.Name == ClientName);
                if (client == null)
                {
                    return eJson(new JMessage { code = 600, success = false, data = "客户不存在" });
                }
                var file = new CenterFiles(FileType.StorageAgreement)[FileID];
                // response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/loginClient", entity);
                YaHv.Csrm.Services.StorageAgreement.Add(new YaHv.Csrm.Services.Models.FileMessage
                {
                    CustomName = file.CustomName,
                    AdminID = file.AdminID,
                    ClientID = client.ID,
                    Url = file.Url,
                    Type = file.Type
                });
                eJson(new JMessage { code = 200, success = true, data = null });
            }
            catch (Exception ex)
            {
                eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }
            return eJson();

        }
        #endregion
    }
}