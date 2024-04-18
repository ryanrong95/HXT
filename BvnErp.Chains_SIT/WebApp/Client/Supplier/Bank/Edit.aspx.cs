using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
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

namespace WebApp.Client.Supplier.Bank
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
            string clientSupplierID = Request.QueryString["SupplierID"];
            this.Model.ClientSupplierID = clientSupplierID;
            string accountID = Request.QueryString["SupplierBankID"];
            this.Model.AccountID = accountID ?? "";

            var clientsupplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSuppliers[clientSupplierID];
            this.Model.Supplier = clientsupplier.Json().Replace("'", "#39;");
            //国家、地区
            this.Model.Places = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(x => new { x.Code, x.Name }).Json();
            // 1.币种
            this.Model.Currency = EnumUtils.ToDictionary<Needs.Underly.CRMCurrency>().Select(item => new { item.Key, item.Value }).Json();
          
            //2.支付方式 
            this.Model.Methord = EnumUtils.ToDictionary<Methord>().Select(item => new
            {
                Key = item.Key,
                Value = item.Value
            }).Where(x => x.Key != "1" && x.Key != "4").Json();

            //银行名称，根据供应商英文名称查找所有 supplierBank,只需要名称即可
            var BankInfos = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierBanksByName.Where(t => t.SupplierName == clientsupplier.Name&&t.Status== Needs.Ccs.Services.Enums.Status.Normal).ToArray();
            this.Model.BankNames = BankInfos.Select(t => new { Code = t.BankName, Name = t.BankName }).Distinct().Json();

            //银行账户信息，用于填充页面控件
            this.Model.BankInfos = BankInfos.Json().Replace("'", "#39;"); 

            if (!string.IsNullOrEmpty(accountID))
            {
                var account = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierBanks[accountID];
                this.Model.SupplierBankData = new
                {
                    ID = account.ID,
                    BankAccount = account.BankAccount,
                    BankName = account.BankName,
                    BankAddress = account.BankAddress?.Replace("'", "#39;"),
                    SwiftCode = account.SwiftCode,
                    Summary = account.Summary,
                    account.Place,
                    account.Methord,
                    account.Currency
                }.Json();
            }
            else
            {
                this.Model.SupplierBankData = null;
            }
        }

        /// <summary>
        /// 保存供应商受益人信息
        /// </summary>
        protected void SaveClientSupplierAccount()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'").Replace("&amp;", "&");
            dynamic model = Model.JsonTo<dynamic>();
            string accountID = model.AccountID;
            string place = model.PlaceID;
          //  int currency = model.CurrencyID;
            int methord = model.MethordID;

            var supplieraccount = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierBanks[accountID] ?? new Needs.Ccs.Services.Models.ClientSupplierBank();

            //英文地址
            string bankaddress = model.BankAddress?.ToString().Replace("#39;", "\'");

            supplieraccount.ClientSupplierID = model.ClientSupplierID;
            // supplieraccount.Beneficiary = new Needs.Ccs.Services.Models.Beneficiary();
            //supplieraccount.Name = model.Name;
            supplieraccount.BankAccount = model.BankAccount;
            supplieraccount.BankName = model.BankName.ToString().Replace("#39;", "\'");
            supplieraccount.BankAddress = bankaddress.Replace("#39;", "'");
            supplieraccount.SwiftCode = model.SwiftCode;
            supplieraccount.Place = place;
            supplieraccount.Methord = (Methord)methord;
            supplieraccount.Currency = supplieraccount.Currency==null?Needs.Underly.CRMCurrency.Unknown:supplieraccount.Currency;
            supplieraccount.Summary = model.Summary.ToString().Replace("#39;", "\'");
            supplieraccount.EnterError += SupplierAccount_EnterError;
            supplieraccount.EnterSuccess += SupplierAccount_EnterSuccess;
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                supplieraccount.Enter();
                #endregion
            }
            else
            {
                #region 调用后
                try
                {
                    string requestUrl = URL + "/CrmUnify/BenefciaryEnter";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var supplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers[supplieraccount.ClientSupplierID];
                    var apiclient = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[supplier.ClientID];
                    var entity = new ApiBeneficiary()
                    {
                        Enterprise = new EnterpriseObj
                        {
                            AdminCode = " ",
                            District = " ",
                            Corporation = " ",
                            Name = supplier.Name,
                            RegAddress = " ",
                            Uscc = " ",
                            Status = 200
                        },
                        WsClient = new EnterpriseObj
                        {
                            AdminCode = " ",
                            District = "",
                            Corporation = apiclient.Company.Corporate,
                            Name = apiclient.Company.Name,
                            RegAddress = apiclient.Company.Address,
                            Uscc = apiclient.Company.Code,
                            Status = 200
                        },
                        InvoiceType = 1,
                        RealName = supplier.ChineseName,
                        Bank = supplieraccount.BankName,
                        BankAddress = supplieraccount.BankAddress,
                        Account = supplieraccount.BankAccount,
                        SwiftCode = supplieraccount.SwiftCode,
                        Methord = methord,
                        Currency =(int)supplieraccount.Currency,
                        District = 1,
                        Name = "",
                        Tel = "",
                        Mobile = "",
                        Email = "",
                        Status = 200,
                        Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                        CreateDate = DateTime.Now.ToString(),
                        UpdateDate = DateTime.Now.ToString(),
                        IsDefault = false,
                        Place=place
                       
                    };

                    string apiSupplier = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiSupplier);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }
                    supplieraccount.Enter();
                }
                catch (Exception)
                {

                    throw;
                }
                #endregion
            }

        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SupplierAccount_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SupplierAccount_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }

     
    }
}