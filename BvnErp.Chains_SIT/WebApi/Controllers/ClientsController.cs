using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using WebApi.Models;
using WebApp.App_Utils;


namespace WebApi.Controllers
{

    /// <summary>
    /// 华芯通客户信息相关接口
    /// </summary>
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ClientsController : ApiController
    {
        public readonly string UpLoadRoot = AppDomain.CurrentDomain.BaseDirectory + @"Files\UpLoad\";
        #region  客户信息
        /// <summary>
        /// 新增或修改客户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("api/clients")]
        [HttpPost]
        public HttpResponseMessage SaveClient([FromBody] ApiParamterModel.ClientModel param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();

            var ClientCode = param.EnterCode;
            var CompanyCode = param.Enterprise.Uscc;
            var CompanyName = param.Enterprise.Name;
            var CustomsCode = param.CustomsCode;
            var Corporate = param.Enterprise.Corporation;
            var Address = param.Enterprise.RegAddress;
            var ContactName = param.Contact.Name;
            var Mobile = param.Contact.Mobile;
            var Tel = param.Contact.Tel;
            var Email = param.Contact.Email;
            var Fax = param.Contact.Fax;
            var Rank = param.Rank;
            var Summary = param.Summary;

            Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
            string name = regex.Replace(CompanyName, " ").Trim();
            CompanyName = Needs.Utils.InputTextExtends.SBCToDBC(name);
            var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == CompanyName) as Needs.Ccs.Services.Models.Client ??
             new Needs.Ccs.Services.Models.Client();

            //查询业务员的信息
            var admin = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(x => x.RealName == param.Creator);

            if (client.Company == null)
            {
                client.Company = new Needs.Ccs.Services.Models.Company();
                client.Company.Contact = new Needs.Ccs.Services.Models.Contact();
                client.ClientStatus = Needs.Ccs.Services.Enums.ClientStatus.Auditing;
            }

            client.Company.Code = CompanyCode;
            client.Company.Name = CompanyName;
            client.Company.CustomsCode = CustomsCode;
            client.Company.Corporate = Corporate;
            client.Company.Address = Address;

            client.Company.Contact.Name = ContactName;
            client.Company.Contact.Mobile = Mobile;
            client.Company.Contact.Tel = Tel;
            client.Company.Contact.Email = Email;
            client.Company.Contact.Fax = Fax;

            client.Admin = admin;
            client.AdminID = admin.ID;
            client.ClientType = Needs.Ccs.Services.Enums.ClientType.External;
            client.ClientRank = (Needs.Ccs.Services.Enums.ClientRank)Rank;
            client.ClientCode = ClientCode;
            client.Summary = Summary;
            client.ClientNature = param.ClientNature;
            if (param.ServiceType != null)
            {
                if (param.ServiceType == (int)ServiceType.Warehouse)
                {
                    client.ClientStatus = ClientStatus.WaitingApproval;
                }

            }
            client.IsValid = param.IsDeclaretion;
            client.IsStorageValid = param.IsStorageService;
            client.ServiceType = param.ServiceType == null ? ServiceType.Unknown : (ServiceType)param.ServiceType;
            client.StorageType = param.StorageType == null ? StorageType.Unknown : (StorageType)param.StorageType;
            client.IsApi = true;
            client.Enter();
            if (param.BusinessLicense != null)
            {
                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.BusinessLicense;
                var dic = new { CustomName = param.BusinessLicense.Name, ClientID = client.ID, };
                string path = GetFileFromNetUrl(param.BusinessLicense.Url);
                var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(path, centerType, dic);
            }
            if (param.HKBusinessLicense != null)
            {
                var hkcenterType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.HKBusinessLicense;
                var hkdic = new { CustomName = param.HKBusinessLicense.Name, ClientID = client.ID, };
                string path = GetFileFromNetUrl(param.BusinessLicense.Url);
                var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(path, hkcenterType, hkdic);
            }
            return ApiResultModel.OutputResult(mo);
        }

        #endregion

        #region  客户供应商
        /// <summary>
        ///  保存供应商
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/clients/suppliers")]
        public HttpResponseMessage SaveSupplier([FromBody] ApiParamterModel.Supplier param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
                //处理供应商英文名称所有空白符 包括换行
                Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                string name = regex.Replace(param.EnglishName, " ").Trim();
                name = Needs.Utils.InputTextExtends.SBCToDBC(name);
                var supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().FirstOrDefault(x => x.ClientID == client.ID && x.Name == param.EnglishName && x.Status == Needs.Ccs.Services.Enums.Status.Normal) ?? new Needs.Ccs.Services.Models.ClientSupplier();
                supplier.Name = name;
                supplier.ClientID = client.ID;
                supplier.ChineseName = param.ChineseName;
                supplier.Summary = param.Summary;
                supplier.Place = param.Place;
                supplier.SupplierGrade = (SupplierGrade)param.Grade;
                supplier.Enter();
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="supplierName">供应商</param>
        /// <returns></returns>

        [Route("api/clients/suppliers")]
        [HttpDelete]
        public HttpResponseMessage DeleteSupplier(string name, string supplierName)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            if (string.IsNullOrWhiteSpace(name))
            {
                mo.code = "-1";
                mo.desc = "请求参数name不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            if (string.IsNullOrWhiteSpace(supplierName))
            {
                mo.code = "-1";
                mo.desc = "请求参数supplierName不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                var supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().FirstOrDefault(x => x.ClientID == client.ID && x.Name == supplierName && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                if (supplier == null)
                {
                    mo.code = "-1";
                    mo.desc = "供应商不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                supplier.Abandon();
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }

            return ApiResultModel.OutputResult(mo);
        }

        #endregion

        #region  供应商的账户信息


        ///// <summary>
        ///// 获取供应商账户
        ///// </summary>
        ///// <param name="name">客户名称</param>
        ///// <param name="supplierName">客户供应商中文名称</param>
        ///// <returns></returns>
        //[Route("api/clients/{name}/suppliers/{supplierName}/banks")]
        //[HttpGet]
        //public HttpResponseMessage GetBanksList(string name, string supplierName)
        //{
        //    var mod = new ApiResultModel.CommonClassModel<object>();
        //    mod.data = string.Empty;

        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        mod.code = "-1";
        //        mod.desc = "请求参数name不允许为空";
        //        return ApiResultModel.OutputResult(mod);
        //    }
        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        mod.code = "-1";
        //        mod.desc = "请求参数supplierName不允许为空";
        //        return ApiResultModel.OutputResult(mod);
        //    }
        //    var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name);
        //    var supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().FirstOrDefault(x => x.ClientID == client.ID && x.ChineseName == supplierName);
        //    var id = supplier.ID;
        //    var accounts = new Needs.Ccs.Services.Views.ClientSupplierBanksView().AsQueryable();
        //    accounts = accounts.Where(t => t.ClientSupplierID == id && t.Status != Needs.Ccs.Services.Enums.Status.Delete);

        //    mod.data = (from account in accounts
        //                select new ApiBeneficiary
        //                {
        //                    Enterprise = new EnterpriseObj
        //                    {
        //                        Name = client.Company.Name,
        //                        AdminCode = "",
        //                        Status = 200,
        //                        Corporation = client.Company.Corporate,
        //                        RegAddress = client.Company.Address,
        //                        Uscc = client.Company.Code,
        //                    },
        //                    RealName = client.Company.Name,
        //                    RealID = (client.Company.Name).StrToMD5(),
        //                    Bank = account.BankName,
        //                    BankAddress = account.BankAddress,
        //                    Account = account.BankAccount,
        //                    SwiftCode = account.SwiftCode,
        //                    Methord = 1,
        //                    Currency = 0,
        //                    District = 0,
        //                    CreateDate = account.CreateDate.ToString(),
        //                    Status = account.Status.GetDescription(),
        //                    CreatorID = ""
        //                });
        //    mod.desc = "获取数据成功";
        //    return ApiResultModel.OutputResult(mod);
        //}
        /// <summary>
        /// 保存供应商账户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/suppliers/banks")]
        public HttpResponseMessage SaveBanksList([FromBody] ApiParamterModel.SupplierBank param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                if (string.IsNullOrWhiteSpace(param.Account))
                {
                    mo.code = "-1";
                    mo.desc = "请求参数Account不允许为空";
                    return ApiResultModel.OutputResult(mo);
                }

                if (string.IsNullOrWhiteSpace(param.Bank))
                {
                    mo.code = "-1";
                    mo.desc = "请求参数Bank不允许为空";
                    return ApiResultModel.OutputResult(mo);
                }
                //if (  string.IsNullOrWhiteSpace(param.BankAddress))
                //{
                //    mo.code = "-1";
                //    mo.desc = "请求参数BankAddress不允许为空";
                //    return ApiResultModel.OutputResult(mo);
                //}
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
                var supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().FirstOrDefault(x => x.ClientID == client.ID && x.Name == param.SupplierName && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                var id = supplier.ID;
                var supplieraccount = new Needs.Ccs.Services.Views.ClientSupplierBanksView().FirstOrDefault(x => x.ClientSupplierID == id && x.BankAccount == param.Account && x.Status == Needs.Ccs.Services.Enums.Status.Normal)
                    ?? new Needs.Ccs.Services.Models.ClientSupplierBank();
                //英文地址
                string bankaddress = param.BankAddress;
                supplieraccount.ClientSupplierID = id;
                supplieraccount.BankAccount = param.Account;
                supplieraccount.BankName = param.Bank.ToString().Replace("#39;", "\'");
                supplieraccount.BankAddress = bankaddress;
                supplieraccount.SwiftCode = param.SwiftCode;
                supplieraccount.Status = Needs.Ccs.Services.Enums.Status.Normal;
                supplieraccount.Place = param.Place;
                supplieraccount.Currency = (Needs.Underly.CRMCurrency)param.Currency;
                supplieraccount.Methord = (Methord)param.Methord;
                supplieraccount.Enter();
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);

        }


        /// <summary>
        /// 删除供应商账户
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="supplierName">供应商中文名称</param>
        /// <param name="account">供应商账户</param>
        /// <returns></returns>
        [Route("api/suppliers/banks")]
        [HttpDelete]
        public HttpResponseMessage DeleteSupplierBank(string name, string supplierName, string account = null)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();

            if (string.IsNullOrWhiteSpace(name))
            {
                mo.code = "-1";
                mo.desc = "请求参数name不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            if (string.IsNullOrWhiteSpace(supplierName))
            {
                mo.code = "-1";
                mo.desc = "请求参数supplierName不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            if (string.IsNullOrWhiteSpace(account))
            {
                mo.code = "-1";
                mo.desc = "请求参数account不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "客户名称不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().FirstOrDefault(x => x.ClientID == client.ID && x.Name == supplierName && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                if (supplier == null)
                {
                    mo.code = "-1";
                    mo.desc = "供应商不存在";
                    return ApiResultModel.OutputResult(mo);

                }
                var id = supplier.ID;
                var supplierBank = new Needs.Ccs.Services.Views.ClientSupplierBanksView().FirstOrDefault(x => x.ClientSupplierID == id && x.BankAccount == account && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                supplierBank.Abandon();
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }

            return ApiResultModel.OutputResult(mo);
        }

        #endregion

        #region 供应商提货地址信息

        /// <summary>
        /// 保存供应商提货地址
        /// </summary>
        /// <param name="param">入参</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/suppliers/address")]
        public HttpResponseMessage SaveSupplierAddress([FromBody] ApiParamterModel.SupplierAddress param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "客户名称不存在";
                    return ApiResultModel.OutputResult(mo);
                }

                var supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().FirstOrDefault(x => x.ClientID == client.ID && x.Name == param.SupplierName.Trim() && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                if (supplier == null)
                {
                    mo.code = "-1";
                    mo.desc = "供应商不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var id = supplier.ID;
                var supplieraddress = new Needs.Ccs.Services.Views.ClientSupplierAddressesView().FirstOrDefault(x => x.ClientSupplierID == id && x.Address == param.Address && x.Status == Needs.Ccs.Services.Enums.Status.Normal)
                   ?? new Needs.Ccs.Services.Models.ClientSupplierAddress();
                string address = param.Address;
                supplieraddress.Contact = new Needs.Ccs.Services.Views.ContactsView().FirstOrDefault(x => x.Name == param.Name && x.Mobile == param.Mobile && x.Status == Needs.Ccs.Services.Enums.Status.Normal) ??
                    new Needs.Ccs.Services.Models.Contact();
                supplieraddress.Contact.Name = param.Name;
                supplieraddress.Contact.Mobile = param.Mobile;
                supplieraddress.Contact.Tel = param.Mobile;
                supplieraddress.ClientSupplierID = id;
                supplieraddress.Address = address.ToString().Replace("#39;", "'");
                supplieraddress.IsDefault = param.IsDefault == null ? false : param.IsDefault;
                supplieraddress.Place = param.Place;
                supplieraddress.Status = Needs.Ccs.Services.Enums.Status.Normal;
                // supplieraddress.Summary = param.Summary.ToString().Replace("#39;", "\'");

                //默认地址
                if (supplieraddress.IsDefault)
                {
                    var clientaddress = new Needs.Ccs.Services.Views.ClientSupplierAddressesView().Where(t => t.ClientSupplierID == supplieraddress.ClientSupplierID);
                    foreach (var add in clientaddress)
                    {
                        add.IsDefault = false;
                        add.Enter();
                    }
                }
                supplieraddress.Enter();


            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;

            }

            return ApiResultModel.OutputResult(mo);

        }

        /// <summary>
        /// 删除供应商提货地址
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="supplierName">供应商英文名称</param>
        ///  <param name="address">提货地址</param>
        /// <returns></returns>
        [Route("api/suppliers/address")]
        [HttpDelete]
        public HttpResponseMessage DeleteSupplierAddress(string name, string supplierName, string address)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();

            if (string.IsNullOrWhiteSpace(supplierName))
            {
                mo.code = "-1";
                mo.desc = "请求参数supplierName不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                mo.code = "-1";
                mo.desc = "请求参数name不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "客户名称不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().FirstOrDefault(x => x.ClientID == client.ID && x.Name == supplierName && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                if (supplier == null)
                {
                    mo.code = "-1";
                    mo.desc = "供应商不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var id = supplier.ID;

                var supplierAddresse = new Needs.Ccs.Services.Views.ClientSupplierAddressesView().FirstOrDefault(x => x.ClientSupplierID == id && x.Address == address && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
                if (supplierAddresse == null)
                {
                    mo.code = "-1";
                    mo.desc = "供应商提货地址不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                supplierAddresse.Abandon();
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }

            return ApiResultModel.OutputResult(mo);
        }
        #endregion

        #region 客户的发票信息
        ///// <summary>
        ///// 获取发票信息
        ///// </summary>
        ///// <param name="name">客户名称</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/clients/invoice")]
        //public HttpResponseMessage GetClientInvoice(string name)
        //{
        //    var mod = new ApiResultModel.CommonClassModel<ResponseClientInvoice>();

        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        mod.code = "-1";
        //        mod.desc = "请求参数name不允许为空";
        //        return ApiResultModel.OutputResult(mod);
        //    }
        //    var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name);
        //    var invoice = new Needs.Ccs.Services.Views.ClientInvoicesView().FirstOrDefault(t => t.ClientID == client.ID && t.Status == Needs.Ccs.Services.Enums.Status.Normal);
        //    var invoiceConsignee = new Needs.Ccs.Services.Views.ClientInvoiceConsigneesView().FirstOrDefault(t => t.ClientID == name) ?? new Needs.Ccs.Services.Models.ClientInvoiceConsignee();
        //    try
        //    {
        //        mod.code = "0";
        //        mod.desc = "获取成功";
        //        mod.data = new ResponseClientInvoice()
        //        {
        //            ClientId = client.ID,
        //            CompanyName = client.Company.Name,
        //            CompanyCode = client.Company.Code,
        //            InvoiceId = invoice.ID,
        //            DeliveryType = (int)invoice.DeliveryType,
        //            TaxCode = invoice.TaxCode,
        //            BankName = invoice.BankName,
        //            BankAccount = invoice.BankAccount,
        //            Address = invoice.Address,
        //            Tel = invoice.Tel,
        //            Summary = invoice.Summary,
        //            InvoiceConsignee = new InvoiceConsignee
        //            {
        //                ConsigneeName = invoiceConsignee.Name,
        //                ConsigneeAddress = invoiceConsignee.Address,
        //                ConsigneeMobile = invoiceConsignee.Mobile,
        //                ConsigneeTel = invoiceConsignee.Tel,
        //                ConsigneeEmail = invoiceConsignee.Email,
        //            }
        //        };
        //    }
        //    catch (Exception ex)
        //    {

        //        mod.data = new ResponseClientInvoice();
        //        mod.code = "-1";
        //        mod.desc = "获取数据为空";
        //    }
        //    return ApiResultModel.OutputResult(mod);

        //}

        /// <summary>
        /// 新增或修改客户发票信息
        /// </summary>
        /// <returns></returns>
        [Route("api/clients/invoice")]
        [HttpPost]
        public HttpResponseMessage SaveClientInvoice([FromBody] ApiParamterModel.ClientInvoice param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
            if (client == null)
            {
                mo.code = "-1";
                mo.desc = "客户名称不存在";
                return ApiResultModel.OutputResult(mo);
            }
            var clientID = client.ID;
            var invoice = client.Invoice ?? new Needs.Ccs.Services.Models.ClientInvoice();
            try
            {
                var invoiceConsignee = client.InvoiceConsignee ?? new Needs.Ccs.Services.Models.ClientInvoiceConsignee();
                //发票信息
                invoice.ClientID = clientID;
                invoice.Title = param.Enterprise.Name;
                invoice.TaxCode = param.TaxperNumber;
                invoice.Address = param.InvoiceAddress ?? client.Company.Address;
                invoice.Tel = param.CompanyTel ?? "";
                invoice.BankName = param.Bank;
                invoice.BankAccount = param.Account;
                invoice.DeliveryType = (Needs.Ccs.Services.Enums.InvoiceDeliveryType)param.DeliveryType;
                invoice.Summary = param.Summary;

                //发票收件地址
                invoiceConsignee.ClientID = clientID;
                invoiceConsignee.Name = param.Name;
                invoiceConsignee.Mobile = param.Mobile ?? string.Empty;
                invoiceConsignee.Tel = param.Tel;
                invoiceConsignee.Email = param.Email;
                invoiceConsignee.Address = param.ConsigneeAddress;
                invoiceConsignee.Enter();
                invoice.Enter();

            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }



            return ApiResultModel.OutputResult(mo);
        }
        #endregion

        #region  客户的收件地址

        ///// <summary>
        ///// 获取客户的收件地址
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/clients/consignee")]
        //public HttpResponseMessage GetClientConsignees(string name)
        //{
        //    var mod = new ApiResultModel.CommonClassModel<object>();
        //    mod.data = string.Empty;

        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        mod.code = "-1";
        //        mod.desc = "请求参数name不允许为空";
        //        return ApiResultModel.OutputResult(mod);
        //    }
        //    try
        //    {
        //        var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name);
        //        var consignees = new Needs.Ccs.Services.Views.ClientConsigneesView().AsQueryable();
        //        consignees = consignees.Where(t => t.ClientID == client.ID && t.Status != Needs.Ccs.Services.Enums.Status.Delete);

        //        mod.data = (from consignee in consignees
        //                    select new
        //                    {
        //                        consignee.Name,
        //                        ContactName = consignee.Contact.Name,
        //                        consignee.Contact.Mobile,
        //                        consignee.Address,
        //                        IsDefault = consignee.IsDefault ? "是" : "否",
        //                        CreateDate = consignee.CreateDate.ToString(),
        //                        Status = consignee.Status.GetDescription(),
        //                    });
        //        mod.desc = "获取数据成功";
        //    }
        //    catch (Exception ex)
        //    {

        //        mod.code = "-1";
        //        mod.desc = ex.Message;
        //    }
        //    return ApiResultModel.OutputResult(mod);
        //}

        /// <summary>
        /// 保存客户收件地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/clients/consignee")]
        public HttpResponseMessage SaveConsignee([FromBody] ApiParamterModel.ClientConsigin param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "客户名称不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var clientID = client.ID;
                var consignee = new Needs.Ccs.Services.Views.ClientConsigneesView().FirstOrDefault(x => x.ClientID == clientID && x.Address == param.Address && x.Contact.Mobile == param.Mobile)
                    ?? new Needs.Ccs.Services.Models.ClientConsignee();

                consignee.Contact = new Needs.Ccs.Services.Models.Contact();
                consignee.Name = param.Receiver;
                consignee.Contact.Name = param.Name;
                consignee.Contact.Mobile = param.Mobile ?? "";
                consignee.Contact.Email = param.Email;
                consignee.Address = param.Address;
                consignee.ClientID = clientID;
                consignee.IsDefault = (bool)param.IsDefault;
                consignee.Summary = param.Summary;
                //默认地址
                if (consignee.IsDefault)
                {
                    var consignees = new Needs.Ccs.Services.Views.ClientConsigneesView().Where(t => t.ClientID == consignee.ClientID);
                    foreach (var con in consignees)
                    {
                        con.IsDefault = false;
                        con.Enter();
                    }
                }
                consignee.Enter();
                mo.code = "0";
                mo.desc = "保存成功";

            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;

            }
            return ApiResultModel.OutputResult(mo);

        }



        /// <summary>
        /// 删除收件地址
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="receiver">收件单位</param>
        /// <param name="address">收件地址</param>
        /// <param name="mobile">电话</param>
        /// <returns></returns>
        [Route("api/clients/consignee")]
        [HttpDelete]
        public HttpResponseMessage DeleteConsignee(string name, string receiver, string address, string mobile)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();

            if (string.IsNullOrWhiteSpace(name))
            {
                mo.code = "-1";
                mo.desc = "请求参数name不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            if (string.IsNullOrWhiteSpace(receiver))
            {
                mo.code = "-1";
                mo.desc = "请求参数receiver不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                mo.code = "-1";
                mo.desc = "请求参数address不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            //手机号客户端不为空
            //if (string.IsNullOrWhiteSpace(mobile))
            //{
            //    mo.code = "-1";
            //    mo.desc = "请求参数mobile不允许为空";
            //    return ApiResultModel.OutputResult(mo);
            //}
            try
            {

                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "客户名称不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var clientID = client.ID;

                var consignee = new Needs.Ccs.Services.Views.ClientConsigneesView().FirstOrDefault(x => x.ClientID == clientID && x.Address == address);
                if (consignee == null)
                {
                    mo.code = "-1";
                    mo.desc = "收件地址不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                consignee.Abandon();
                mo.code = "0";
                mo.desc = "删除成功";
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);
        }

        #endregion

        #region 会员账号
        ///// <summary>
        ///// 获取会员账号
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/clients/{id}/account")]
        //public HttpResponseMessage GetClientAccount(string id)
        //{
        //    var mod = new ApiResultModel.CommonClassModel<object>();
        //    mod.data = string.Empty;

        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        mod.code = "-1";
        //        mod.desc = "请求参数供应商ID不允许为空";
        //        return ApiResultModel.OutputResult(mod);
        //    }
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(id))
        //        {
        //            mod.code = "-1";
        //            mod.desc = "请求参数id不允许为空";
        //            return ApiResultModel.OutputResult(mod);
        //        }
        //        var useraccounts = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.ID == id);
        //        var accounts = useraccounts == null ? new Needs.Ccs.Services.Views.UsersView() : useraccounts.Users;

        //        mod.data = (from account in accounts
        //                    select new
        //                    {
        //                        account.ID,
        //                        Name = account.Name,
        //                        RealName = account.RealName,
        //                        Mobile = account.Mobile,
        //                        Email = account.Email,
        //                        Status = account.Status,
        //                        Summary = account.Summary,
        //                        IsMain = account.IsMain ? "是" : "否",
        //                        CreateDate = account.CreateDate.ToShortTimeString()
        //                    });
        //        mod.desc = "获取数据成功";
        //        return ApiResultModel.OutputResult(mod);
        //    }
        //    catch (Exception)
        //    {
        //        mod.code = "-1";
        //        mod.desc = "获取数据为空";
        //    }
        //    return ApiResultModel.OutputResult(mod);
        //}

        /// <summary>
        /// 保存会员账号
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/clients/account")]
        public HttpResponseMessage SaveClientAccount([FromBody] ApiParamterModel.ClientAccount param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "参数Name不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var clientID = client.ID;
                //查询业务员的信息
                var admin = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(x => x.RealName == param.Creator);

                var user = new Needs.Ccs.Services.Views.UsersView().FirstOrDefault(x => x.Name == param.UserName) as Needs.Ccs.Services.Models.User ??
                new Needs.Ccs.Services.Models.User();
                user.Name = param.UserName;
                user.Mobile = param.Mobile;
                user.Email = param.Email;
                user.Password = param.Password;
                user.RealName = param.RealName;
                user.ClientID = clientID;
                user.AdminID = admin?.ID ?? "";
                user.Summary = param.Summary;
                user.IsMain = param.IsMain;
                user.Enter();
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);

        }


        /// <summary>
        /// 删除会员账号
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="userName">登录账号</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/clients/account")]
        public HttpResponseMessage DeleteClientAccount(string name, string userName)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            if (string.IsNullOrWhiteSpace(userName))
            {
                mo.code = "-1";
                mo.desc = "请求参数userName不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "客户名称不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var clientID = client.ID;
                var account = new Needs.Ccs.Services.Views.UsersView().FirstOrDefault(x => x.ClientID == clientID && x.Name == userName);
                if (account == null)
                {
                    mo.code = "-1";
                    mo.desc = "账号不存在";
                    return ApiResultModel.OutputResult(mo);
                }

                account.Abandon();
                mo.code = "0";
                mo.desc = "删除成功";
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/account/reset")]
        public HttpResponseMessage ResetPassword(string name, string userName)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            if (string.IsNullOrWhiteSpace(name))
            {
                mo.code = "-1";
                mo.desc = "请求参数name不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            try
            {
                new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name).Users.FirstOrDefault(x => x.Name == userName).ResetPassword();
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = "重置失败";
            }
            return ApiResultModel.OutputResult(mo);
        }


        #endregion

        #region 客户附件信息
        ///// <summary>
        ///// 获取客户附件信息
        ///// </summary>
        ///// <param name="name">客户名称</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/clients/file")]
        //public HttpResponseMessage GetClientFiles(string name)
        //{
        //    var mod = new ApiResultModel.CommonClassModel<object>();
        //    mod.data = string.Empty;

        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        mod.code = "-1";
        //        mod.desc = "请求参数supplierId不允许为空";
        //        return ApiResultModel.OutputResult(mod);
        //    }
        //    var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name);
        //    var id = client.ID;
        //    var files = new Needs.Ccs.Services.Views.ClientFilesView().AsQueryable();
        //    var filesLst = files.Where(t => t.ClientID == id && t.Status != Needs.Ccs.Services.Enums.Status.Delete).ToList();

        //    try
        //    {
        //        mod.data = (from file in filesLst
        //                    select new
        //                    {
        //                        file.ID,
        //                        Name = file.Name,
        //                        FileType = file.FileType.GetDescription(),
        //                        FileFormat = file.FileFormat,
        //                        Url = Needs.Utils.FileDirectory.Current.FileServerUrl + "/" + file?.Url.ToUrl(),
        //                        CreateDate = file.CreateDate.ToString("yyyy-MM-dd"),
        //                        Status = file.Status.GetDescription(),
        //                        Summary = file.Summary

        //                    });
        //    }
        //    catch (Exception ex)
        //    {
        //        mod.code = "-1";
        //        mod.desc = ex.Message;

        //    }
        //    return ApiResultModel.OutputResult(mod);
        //}

        /// <summary>
        /// 保存客户文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/clients/file")]
        public HttpResponseMessage SaveClientFile([FromBody] ApiParamterModel.ClientFile param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "客户名称不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var id = client.ID;

                var clientfile = new Needs.Ccs.Services.Models.ClientFile();
                clientfile.ClientID = id;
                clientfile.Name = param.Name;
                clientfile.AdminID = client.AdminID;
                clientfile.FileType = (Needs.Ccs.Services.Enums.FileType)param.Type;
                clientfile.FileFormat = param.FileFormat;
                clientfile.Url = param.Url;
                clientfile.Summary = param.Summary;
                clientfile.Enter();
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);

        }

        /// <summary>
        /// 删除客户文件
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/clients/file")]
        public HttpResponseMessage DeleteClientFile(string name, string fileName)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            if (string.IsNullOrWhiteSpace(name))
            {
                mo.code = "-1";
                mo.desc = "请求参数name不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                mo.code = "-1";
                mo.desc = "请求参数fileName不允许为空";
                return ApiResultModel.OutputResult(mo);
            }
            try
            {
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == name);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "客户名称不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var id = client.ID;
                var file = new Needs.Ccs.Services.Views.ClientFilesView().FirstOrDefault(x => x.ClientID == id && x.Name == fileName);
                if (file == null)
                {
                    mo.code = "-1";
                    mo.desc = "上传附件不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                file.Abandon();
                mo.code = "0";
                mo.desc = "删除成功";
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);
        }


        #endregion

        #region  分配人员
        /// <summary>
        /// 客户分配人员
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/clients/Assign")]
        public HttpResponseMessage SaveClientAssign([FromBody] ApiParamterModel.ClientAssign param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                var entity = new Needs.Ccs.Services.Views.AdminsTopView().ToArray();
                var admin = entity.FirstOrDefault(x => x.RealName == param.ServiceManager);
                if (admin == null)
                {
                    mo.code = "-1";
                    mo.desc = "请求参数ServiceManager不存在";
                    return ApiResultModel.OutputResult(mo);

                }
                var merchandiser = entity.FirstOrDefault(x => x.RealName == param.Merchandiser);
                if (merchandiser == null)
                {
                    mo.code = "-1";
                    mo.desc = "请求参数Merchandiser不存在";
                    return ApiResultModel.OutputResult(mo);
                }

                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
                //if (param.Referrer != null)
                //{
                //    var referrer = entity.FirstOrDefault(x => x.RealName == param.Referrer);
                //    client.Referrer = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(referrer?.ID);
                //}
                client.ServiceManager = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)admin.ID);
                client.Merchandiser = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)merchandiser.ID);

                string info = client.ClientConfirm((string)param.Summary);
                if (!string.IsNullOrEmpty(info))
                {
                    mo.code = "-1";
                    mo.desc = info;
                    return ApiResultModel.OutputResult(mo);
                }

            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);
        }


        #endregion

        #region  客户协议
        /// <summary>
        /// 客户协议
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/clients/agreement")]
        public HttpResponseMessage SaveClientAgreement([FromBody] ApiParamterModel.ClientAgreement param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                if (param == null)
                {
                    mo.code = "-1";
                    mo.desc = "请求参数为空";
                    return ApiResultModel.OutputResult(mo);
                }
                var clientAgreement = new Needs.Ccs.Services.Models.ClientAgreement();
                clientAgreement.ProductFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                clientAgreement.TaxFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                clientAgreement.AgencyFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                clientAgreement.IncidentalFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();

                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == param.Enterprise.Name);
                if (client == null)
                {
                    mo.code = "-1";
                    mo.desc = "请求参数企业Name不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                var admin = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(x => x.RealName == param.Creator);
                if (admin == null)
                {
                    mo.code = "-1";
                    mo.desc = "请求参数Creator不存在";
                    return ApiResultModel.OutputResult(mo);
                }
                clientAgreement.AdminID = admin.ID;
                clientAgreement.ClientID = client.ID;
                clientAgreement.StartDate = Convert.ToDateTime(param.StartDate);
                clientAgreement.EndDate = Convert.ToDateTime(param.EndDate);
                clientAgreement.AgencyRate = param.AgencyRate;
                clientAgreement.MinAgencyFee = param.MinAgencyFee;
                clientAgreement.IsPrePayExchange = param.IsPrePayExchange;
                clientAgreement.IsLimitNinetyDays = param.IsLimitNinetyDays;
                clientAgreement.InvoiceType = (Needs.Ccs.Services.Enums.InvoiceType)param.InvoiceType;
                clientAgreement.InvoiceTaxRate = clientAgreement.InvoiceType == Needs.Ccs.Services.Enums.InvoiceType.Full ? Needs.Ccs.Services.ConstConfig.ValueAddedTaxRate : param.InvoiceTaxRate;
                clientAgreement.Summary = param.Summary;
                //货款
                clientAgreement.ProductFeeClause.AgreementID = clientAgreement.ID;
                clientAgreement.ProductFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Product;
                clientAgreement.ProductFeeClause.PeriodType = Needs.Ccs.Services.Enums.PeriodType.PrePaid;
                clientAgreement.ProductFeeClause.ExchangeRateType = Needs.Ccs.Services.Enums.ExchangeRateType.RealTime;
                clientAgreement.ProductFeeClause.AdminID = clientAgreement.AdminID;

                //税款
                clientAgreement.TaxFeeClause.AgreementID = clientAgreement.ID;
                clientAgreement.TaxFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Tax;
                clientAgreement.TaxFeeClause.PeriodType = Needs.Ccs.Services.Enums.PeriodType.PrePaid;
                clientAgreement.TaxFeeClause.ExchangeRateType = Needs.Ccs.Services.Enums.ExchangeRateType.Custom;
                clientAgreement.TaxFeeClause.AdminID = clientAgreement.AdminID;

                //代理费
                clientAgreement.AgencyFeeClause.AgreementID = clientAgreement.ID;
                clientAgreement.AgencyFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.AgencyFee;
                clientAgreement.AgencyFeeClause.PeriodType = Needs.Ccs.Services.Enums.PeriodType.PrePaid;
                clientAgreement.AgencyFeeClause.ExchangeRateType = Needs.Ccs.Services.Enums.ExchangeRateType.RealTime;
                clientAgreement.AgencyFeeClause.AdminID = clientAgreement.AdminID;

                //杂费
                clientAgreement.IncidentalFeeClause.AgreementID = clientAgreement.ID;
                clientAgreement.IncidentalFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Incidental;
                clientAgreement.IncidentalFeeClause.PeriodType = Needs.Ccs.Services.Enums.PeriodType.PrePaid;
                clientAgreement.IncidentalFeeClause.ExchangeRateType = Needs.Ccs.Services.Enums.ExchangeRateType.None;
                clientAgreement.IncidentalFeeClause.AdminID = clientAgreement.AdminID;
                clientAgreement.Enter();


                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.ServiceAgreement;
                var dic = new { CustomName = param.ClientFile.Name, ClientID = client.ID, };
                string path = GetFileFromNetUrl(param.ClientFile.Url);

                var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(path, centerType, dic);


            }
            catch (Exception ex)
            {

                mo.code = "-1";
                mo.desc = "操作失败";
            }

            return ApiResultModel.OutputResult(mo);
        }

        #endregion


        /// <summary>
        ///根据远程URL　保存附件到本地后 ，重新上传中心
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string GetFileFromNetUrl(string url)
        {
            try
            {
                CreateDirectory();

                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.Method = "GET";
                //获得用户名密码的Base64编码  添加Authorization到HTTP头 不需要的账号密码的可以注释下面两行代码
                string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "File", "pdejhff1")));
                req.Headers.Add("Authorization", "Basic " + code);
                byte[] fileBytes;
                using (WebResponse webRes = req.GetResponse())
                {
                    int length = (int)webRes.ContentLength;
                    HttpWebResponse response = webRes as HttpWebResponse;
                    Stream stream = response.GetResponseStream();

                    //读取到内存
                    MemoryStream stmMemory = new MemoryStream();
                    byte[] buffer = new byte[length];
                    int i;
                    //将字节逐个放入到Byte中
                    while ((i = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stmMemory.Write(buffer, 0, i);
                    }
                    fileBytes = stmMemory.ToArray();//文件流Byte，需要文件流可直接return，不需要下面的保存代码
                    stmMemory.Close();

                    MemoryStream m = new MemoryStream(fileBytes);
                    string file = string.Format(UpLoadRoot + System.IO.Path.GetFileName(url));//可根据文件类型自定义后缀
                    FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
                    m.WriteTo(fs);
                    m.Close();
                    fs.Close();
                    return file;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        public void CreateDirectory()
        {
            //上传目录
            FileInfo uploadDirectory = new FileInfo(this.UpLoadRoot);
            if (!uploadDirectory.Directory.Exists)
            {
                uploadDirectory.Directory.Create();
            }
            //下载目录
            FileInfo downDirectory = new FileInfo(this.UpLoadRoot);
            if (!downDirectory.Directory.Exists)
            {
                downDirectory.Directory.Create();
            }
        }
        #region  官网非会员注册
        /// <summary>
        /// 非会员注册
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("api/loginClient")]
        [HttpPost]
        public HttpResponseMessage LoginClient([FromBody] ApiParamterModel.Member param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                DeliveryNoticeApiLog paramLog = new DeliveryNoticeApiLog();
                paramLog.ID = Guid.NewGuid().ToString("N");
                paramLog.ResponseContent = "api/loginClient中请求参数param内容：" + JsonConvert.SerializeObject(param);
                paramLog.Status = Status.Normal;
                paramLog.CreateDate = DateTime.Now;
                paramLog.UpdateDate = DateTime.Now;
                paramLog.Enter();

                var CompanyName = param.Company;
                var UserName = param.UserName;
                var ContactName = param.Contacts;
                var Mobile = param.Mobile;
                var Password = param.Password;
                var CustomsCode = param.CustomsCode;
                var Address = param.Address;
                var Email = param.Email;
                var Tel = param.Tel;
                Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                string name = regex.Replace(CompanyName, " ").Trim();
                CompanyName = Needs.Utils.InputTextExtends.SBCToDBC(name);
                var client = new Needs.Ccs.Services.Views.ClientsView().FirstOrDefault(x => x.Company.Name == CompanyName) as Needs.Ccs.Services.Models.Client ??
                 new Needs.Ccs.Services.Models.Client();
                //用户注册的话判断名称是否存在
                if (!string.IsNullOrEmpty(param.UserName) && !string.IsNullOrEmpty(param.Password))
                {

                    var count = new CompaniesView().Where(x => x.Name == CompanyName).Count();
                    if (count > 0)
                    {

                        mo.code = "-1";
                        mo.desc = "公司名称已存在";
                        return ApiResultModel.OutputResult(mo);
                    }

                    var users = new Needs.Ccs.Services.Views.UsersView().FirstOrDefault(x => x.Name == param.UserName);
                    if (users != null)
                    {
                        mo.code = "-1";
                        mo.desc = "用户名已存在";
                        return ApiResultModel.OutputResult(mo);

                    }
                    else
                    {
                        client.Company = new Needs.Ccs.Services.Models.Company();
                        //  client.CompanyID = ChainsGuid.NewGuidUp();

                        client.Company.Name = CompanyName;
                        client.Company.Contact = new Needs.Ccs.Services.Models.Contact();
                        client.Company.Contact.Name = ContactName;
                        client.Company.Contact.Mobile = Mobile;
                        client.Company.Contact.Email = Email;
                        client.ClientStatus = Needs.Ccs.Services.Enums.ClientStatus.Auditing;
                        client.ClientType = Needs.Ccs.Services.Enums.ClientType.External;
                        client.ClientRank = ClientRank.ClassNine;
                        client.ChargeWH = ChargeWHType.Charge;//是否收入入仓费赋默认值
                        client.Company.Code = param.Uscc;
                        client.IsApi = true;

                        DeliveryNoticeApiLog clientLog1 = new DeliveryNoticeApiLog();
                        clientLog1.ID = Guid.NewGuid().ToString("N");
                        clientLog1.ResponseContent = "api/loginClient中client内容1：" + JsonConvert.SerializeObject(client);
                        clientLog1.Status = Status.Normal;
                        clientLog1.CreateDate = DateTime.Now;
                        clientLog1.UpdateDate = DateTime.Now;
                        clientLog1.Enter();

                        client.Enter();

                        var user = new Needs.Ccs.Services.Models.User();
                        user.ClientID = client.ID;
                        user.Name = param.UserName;
                        user.Mobile = param.Mobile;
                        user.Password = param.Password;
                        user.RealName = param.Contacts ?? param.UserName;//用户联系人
                        user.AdminID = "";
                        user.IsMain = true;
                        user.Enter();

                    }

                }
                else
                {
                    client.Company.ID = client.Company.ID;
                    client.Company.Name = CompanyName;
                    client.Company.Contact.Name = ContactName;
                    client.Company.Contact.Mobile = Mobile;
                    client.Company.CustomsCode = CustomsCode;
                    client.Company.Corporate = param.Corporate;
                    client.Company.Address = Address;
                    client.Company.Contact.Tel = Tel;
                    // client.Company.Contact.Fax=param.fa
                    client.Company.Contact.Email = Email;
                    client.Company.Code = param.Uscc;
                    client.ClientType = Needs.Ccs.Services.Enums.ClientType.External;
                    client.ClientRank = client.ClientRank == ClientRank.ClassNine ? ClientRank.ClassNine : client.ClientRank;
                    client.IsApi = true;

                    DeliveryNoticeApiLog clientLog2 = new DeliveryNoticeApiLog();
                    clientLog2.ID = Guid.NewGuid().ToString("N");
                    clientLog2.ResponseContent = "api/loginClient中client内容2：" + JsonConvert.SerializeObject(client);
                    clientLog2.Status = Status.Normal;
                    clientLog2.CreateDate = DateTime.Now;
                    clientLog2.UpdateDate = DateTime.Now;
                    clientLog2.Enter();

                    client.Enter();


                }


            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
                ex.CcsLog("WebApi接口api/loginClient");
            }

            return ApiResultModel.OutputResult(mo);

        }


        #endregion


        #region  修改reg企业名称
        /// <summary>
        /// 修改reg企业名称
        /// </summary>
        /// <param name="OldName"></param>
        /// <param name="NewName"></param>
        /// <returns></returns>
        public HttpResponseMessage ModifyCompanyName(string OldName, string NewName)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                if (OldName.IndexOf("reg-") != -1)
                {
                    var client = new Needs.Ccs.Services.Views.ClientsView().Where(x => x.ClientStatus != ClientStatus.Confirmed).FirstOrDefault(x => x.Company.Name == OldName);
                    if (client == null)
                    {
                        mo.code = "-1";
                        mo.desc = "参数Name不存在";
                        return ApiResultModel.OutputResult(mo);
                    }

                    Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                    string name = regex.Replace(NewName, " ").Trim();
                    NewName = Needs.Utils.InputTextExtends.SBCToDBC(name);
                    client.Company.Name = NewName;
                    client.Enter();

                }
            }
            catch (Exception ex)
            {

                mo.code = "-1";
                mo.desc = ex.Message;
            }

            return ApiResultModel.OutputResult(mo);

        }

        #endregion

    }
}
