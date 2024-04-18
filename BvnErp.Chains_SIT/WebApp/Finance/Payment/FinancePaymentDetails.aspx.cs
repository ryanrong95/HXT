using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment
{
    public partial class FinancePaymentDetails : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadData();
            }
        }

        protected void LoadData()
        {
            this.Model.FinancePaymentData = "".Json();
            string ID = Request.QueryString["ID"];
            var finance = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinancePayment.Where(item => item.ID == ID).FirstOrDefault();

            if (finance != null)
            {
                string ceterFeetype = FeeTypeTransfer.Current.L2COutTransfer(finance.PayFeeType);
                this.Model.FinancePaymentData = new
                {
                    SeqNo = finance.SeqNo,
                    Payer = finance.Payer.ByName,
                    FinanceVault = finance.FinanceVault.Name,
                    FinanceAccount = finance.FinanceAccount.AccountName,
                    //PayFeeType = finance.FeeTypeInt > 10000 ? ((Needs.Ccs.Services.Enums.FeeTypeEnum)finance.FeeTypeInt).ToString()
                    //                                        : ((Needs.Ccs.Services.Enums.FinanceFeeType)finance.FeeTypeInt).GetDescription(),  //finance.PayFeeType.GetDescription(),
                    PayFeeType = ceterFeetype,
                    PayeeName = finance.PayeeName,
                    BankName = finance.BankName,
                    BankAccount = finance.BankAccount,
                    Amount = finance.Amount.ToRound(2),
                    Currency = finance.Currency,
                    ExchangeRate = finance.ExchangeRate,
                    PayDate = finance.PayDate.ToString("yyyy-MM-dd"),
                    PayType = finance.PayType.GetDescription(),
                }.Json();
            }
        }

        protected object AccountCatalogsTree()
        {

            var treeStr = AccountCatalogsAlls.Current.JsonOut(AccountCatalogType.Output.GetDescription());  //new AccountCatalogsTree().Json();

            treeStr = treeStr.Replace("\"name\":", "\"text\":");
            return treeStr;
        }

        /// <summary>
        /// 根据金库显示账户
        /// </summary>
        /// <returns></returns>
        protected object GetAccountByVault()
        {
            return new
            {
                data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(v => v.AccountSource == AccountSource.easy && v.BankAccount != null).Select(item => new
                {
                    item.ID,
                    item.AccountName,
                    item.BankAccount,
                    item.BankName,
                }).Distinct()
            };
        }
    }
}
