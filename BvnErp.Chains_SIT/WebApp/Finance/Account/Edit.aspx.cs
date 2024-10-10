using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.ApiSettings;
using Newtonsoft.Json;
using System.Net.Http;
using WebApp.App_Utils;
using System.Net;

namespace WebApp.Finance.Account
{
    /// <summary>
    /// 金库账户编辑界面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //金库
            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal).Select(item => new { Value = item.ID, Text = item.Name }).Json();
            //币种
            this.Model.CurrData = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Value = item.Code, Text = item.Code + " " + item.Name }).Json();
            //管理人
            var ServiceIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "华芯通财务" || manager.Role.Name == "出纳员").Select(item => item.Admin.ID).ToArray();
            this.Model.AllAdmin = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(item => ServiceIDs.Contains(item.ID)).Select(item => new { Value = item.ID, Text = item.ByName }).ToArray().Json();
            //账户基本类型
            this.Model.AccountType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.AccountType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
            //币种
            this.Model.Countries = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(item => new { Value = item.Code, Text = item.Code + " " + item.Name }).Json();
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        void PageInit()
        {
            this.Model.AllData = "".Json();
            string id = Request.QueryString["ID"];
            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[id];

            if (financeAccounts != null)
            {
                var types = new AccountTypeTransfer().Separate((Needs.Ccs.Services.Enums.AccountType)financeAccounts.AccountType);
                string accountType = "";
                if (types.Count() > 1)
                {
                    for (int i = 0; i < types.Count() - 1; i++)
                    {
                        accountType += types[i].ToString() + ",";
                    }
                    accountType += types[types.Count() - 1];
                }
                else
                {
                    accountType = types.FirstOrDefault().ToString();
                }

                this.Model.AllData = new
                {
                    ID = id,
                    FinanceVault = financeAccounts.FinanceVaultID,
                    AccountName = financeAccounts.AccountName,
                    BankName = financeAccounts.BankName,
                    BankAddress = financeAccounts.BankAddress,
                    BankAccount = financeAccounts.BankAccount,
                    SwiftCode = financeAccounts.SwiftCode,
                    Currency = financeAccounts.Currency,
                    Balance = financeAccounts.Balance,
                    Summary = financeAccounts.Summary,
                    CustomizedCode = financeAccounts.CustomizedCode,
                    AccountType = accountType,
                    CompanyName = financeAccounts.CompanyName,
                    AdminInchargeID = financeAccounts.AdminInchargeID,
                    Region = financeAccounts.Region
                }.Json();
            }
        }

        /// <summary>
        /// 校验账号是否可用，账号不允许重复
        /// 不管新增，修改都要校验
        /// </summary>
        protected void CheckBankAccountNo()
        {
            var id = Request.Form["ID"];
            var BankAccount = Request.Form["BankAccount"].Replace(",undefined", "");

            var OriginFinanceVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[id];
            if (OriginFinanceVault != null && OriginFinanceVault.BankAccount.Equals(BankAccount))
            {
                Response.Write((new { success = true, message = "" }).Json());
            }
            else
            {
                var financeVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(t => t.BankAccount == BankAccount).FirstOrDefault();

                if (financeVault != null)
                {
                    Response.Write((new { success = false, message = "银行账号不能重复" }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "" }).Json());
                }
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            var id = Request.Form["ID"];
            var FinanceVault = Request.Form["FinanceVault"];
            var AccountName = Request.Form["AccountName"];
            var BankName = Request.Form["BankName"];
            var BankAddress = Request.Form["BankAddress"];
            var BankAccount = Request.Form["BankAccount"].Replace(",undefined", "");
            var SwiftCode = Request.Form["SwiftCode"];
            var CustomizedCode = Request.Form["CustomizedCode"];
            var Currency = Request.Form["Currency"];
            var Balance = Request.Form["Balance"];
            var Summary = Request.Form["Summary"];
            var AccountType = Request.Form["AccountType"];
            var Owner = Request.Form["Owner"];
            var CompanyName = Request.Form["Company"];
            var Region = Request.Form["Region"];

            var ErmLeaderID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == Owner)?.ID;
            var ErmCreatorID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID)?.ID;
            var vault = new Needs.Ccs.Services.Views.FinanceVaultsView().Where(t => t.ID == FinanceVault).FirstOrDefault()?.Name;
            CenterAccount centerAccount = new CenterAccount();
            centerAccount.VaultName = vault;
            centerAccount.AccountName = AccountName;
            centerAccount.BankName = BankName;
            centerAccount.BankAddress = BankAddress;
            centerAccount.BankAccount = BankAccount;
            centerAccount.SwiftCode = SwiftCode;
            centerAccount.Currency = Currency;
            centerAccount.Summary = Summary;
            centerAccount.CreatorID = ErmCreatorID;
            centerAccount.AccountType = AccountType;
            centerAccount.Owner = ErmLeaderID;
            centerAccount.CompanyName = CompanyName;
            centerAccount.Region = Region;
            centerAccount.Balance = Convert.ToDecimal(Balance);
            centerAccount.AccountSource = (int)Needs.Ccs.Services.Enums.AccountSource.standard;

            SendStrcut sendStrcut = new SendStrcut();
            sendStrcut.sender = "FSender001";


            var financeVault = new Needs.Ccs.Services.Models.FinanceAccount();

            var oldfinanceVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[id];
            if (oldfinanceVault != null)
            {
                sendStrcut.option = CenterConstant.Update;
                financeVault = oldfinanceVault;
            }
            else
            {
                sendStrcut.option = CenterConstant.Enter;
            }

            sendStrcut.model = centerAccount;

            AccountTypeTransfer transfer = new AccountTypeTransfer();
            var accountType = transfer.Combine(centerAccount.AccountType);
            //var financeVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[id] as
            // Needs.Ccs.Services.Models.FinanceAccount ?? new Needs.Ccs.Services.Models.FinanceAccount();
            financeVault.FinanceVaultID = FinanceVault;
            financeVault.AccountName = AccountName;
            financeVault.BankName = BankName;
            financeVault.BankAddress = BankAddress;
            financeVault.BankAccount = BankAccount;
            financeVault.SwiftCode = SwiftCode;
            financeVault.CustomizedCode = CustomizedCode;
            financeVault.Currency = Currency;
            financeVault.Balance = decimal.Parse(Balance);
            financeVault.Summary = Summary;
            financeVault.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            financeVault.AccountType = accountType;
            financeVault.AdminInchargeID = Owner;
            financeVault.CompanyName = CompanyName;
            financeVault.Region = Region;
            financeVault.AccountSource = Needs.Ccs.Services.Enums.AccountSource.standard;
            financeVault.EnterSuccess += FinanceVault_EnterSuccess;
            financeVault.EnterError += FinanceVault_EnterError;


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
                financeVault.Enter();

                Logs log = new Logs();
                log.Name = "账户同步";
                log.MainID = financeVault.ID;
                log.AdminID = "";
                log.Json = apiclient;
                log.Summary = "";
                log.Enter();
            }
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceVault_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceVault_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}
