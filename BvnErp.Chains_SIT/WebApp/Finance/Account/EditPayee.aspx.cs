using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Models.HttpUtility;
using Needs.Ccs.Services.Views;
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

namespace WebApp.Finance.Account.Payee
{
    public partial class EditPayee : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ID = Request.QueryString["ID"];
            string From = Request.QueryString["From"];
            this.Model.From = From;

            string PayeeName = string.Empty;
            string PayeeAccount = string.Empty;
            string PayeeBank = string.Empty;

            if (From == "Edit")
            {
                var payee = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[ID];
                PayeeName = payee.AccountName;
                PayeeAccount = payee.BankAccount;
                PayeeBank = payee.BankName;
            }

            this.Model.Payee = new
            {
                AccountID = ID,
                PayeeName = PayeeName,
                PayeeAccount = PayeeAccount,
                PayeeBank = PayeeBank,
            }.Json();
        }

        protected void Save()
        {
            try
            {
                string ID = Request.Form["ID"];
                string From = Request.Form["From"];                
                string PayeeName = Request.Form["PayeeName"];
                string PayeeAccount = Request.Form["PayeeAccount"];
                string PayeeBank = Request.Form["PayeeBank"];

                CenterAccount centerAccount = new CenterAccount();

                SendStrcut sendStrcut = new SendStrcut();
                sendStrcut.sender = "FSender001";
                sendStrcut.option = CenterConstant.Enter;

                var financeAccount = new Needs.Ccs.Services.Models.FinanceAccount();
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                if (From == "Edit")
                {
                    financeAccount = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[ID];
                    sendStrcut.option = CenterConstant.Update;
                }
                else
                {                    
                    financeAccount.Balance = 0;
                    financeAccount.AccountType = Needs.Ccs.Services.Enums.AccountType.basic;
                    financeAccount.Admin = admin;
                    financeAccount.AccountSource = Needs.Ccs.Services.Enums.AccountSource.easy;

                    centerAccount.Balance = 0;
                }

                financeAccount.AccountName = PayeeName;
                financeAccount.FinanceVaultID = null;
                if (!string.IsNullOrEmpty(PayeeAccount))
                {
                    financeAccount.BankAccount = PayeeAccount;
                }
                if (!string.IsNullOrEmpty(PayeeBank))
                {
                    financeAccount.BankName = PayeeBank;
                }

                centerAccount.AccountName = PayeeName;
                centerAccount.BankAccount = PayeeAccount;
                centerAccount.BankName = PayeeBank;
                centerAccount.AccountSource = (int)Needs.Ccs.Services.Enums.AccountSource.easy;
                var ErmAdminID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == admin.ID)?.ErmAdminID;
                centerAccount.CreatorID = ErmAdminID;
                centerAccount.Currency = "CNY";
                sendStrcut.model = centerAccount;

                //提交中心
                string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                string requestUrl = URL + FinanceApiSetting.AccountUrl;
                string apiclient = JsonConvert.SerializeObject(sendStrcut);

                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);


                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求中心接口失败：" }).Json());
                }
                else
                {
                    financeAccount.Enter();
                    Logs log = new Logs();
                    log.Name = "简易账户同步";
                    log.MainID = financeAccount.ID;
                    log.AdminID = "";
                    log.Json = apiclient;
                    log.Summary = "";
                    log.Enter();
                }

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
        }


    }
}