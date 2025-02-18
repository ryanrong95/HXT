﻿using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class GoodsImportQuery : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();

            this.Model.FinanceAccountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.AccountSource == AccountSource.standard)
                .Select(item => new { Value = item.ID, Text = item.AccountName }).Json();
        }

        /// <summary>
        /// 根据金库显示账户
        /// </summary>
        /// <returns></returns>
        protected object GetAccountByVault()
        {
            var vault = Request.Form["FinanceVault"];
            return new
            {
                data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(v => v.Status == Needs.Ccs.Services.Enums.Status.Normal && v.FinanceVaultID == vault).Select(item => new
                {
                    Value = item.ID,
                    Text = item.AccountName
                }).Json()
            };
        }

        protected void data()
        {
            string Payer = Request.QueryString["Payer"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string FinanceVault = Request.QueryString["FinanceVault"];
            string Account = Request.QueryString["Account"];

            var receipts = new Needs.Ccs.Services.Views.GoodsImportView()
                .Where(item => item.Status == Status.Normal && item.FeeType == FinanceFeeType.DepositReceived && item.GoodsCreStatus == true)
                .OrderByDescending(item => item.ReceiptDate)
                .AsQueryable();


            if (!string.IsNullOrEmpty(Payer))
            {
                var payer = Payer.Trim();
                receipts = receipts.Where(t => t.Payer == payer);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                receipts = receipts.Where(t => t.ReceiptDate >= Convert.ToDateTime(StartDate));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var endDate = Convert.ToDateTime(EndDate).AddDays(1);
                receipts = receipts.Where(t => t.ReceiptDate < endDate);
            }
            if (!string.IsNullOrEmpty(FinanceVault))
            {
                var financeVault = FinanceVault.Trim();
                receipts = receipts.Where(t => t.Vault.ID == financeVault);
            }
            if (!string.IsNullOrEmpty(Account))
            {
                var account = Account.Trim();
                receipts = receipts.Where(t => t.Account.ID == account);
            }

            Func<Needs.Ccs.Services.Models.FinanceReceipt, object> convert = receipt => new
            {
                receipt.ID,
                SerialNumber = receipt.SeqNo,
                Vault = receipt.Vault.Name,
                Account = receipt.Account.AccountName,
                Payer = receipt.Payer,
                FeeType = receipt.FeeType.GetDescription(),
                ReceiptType = receipt.ReceiptType.GetDescription(),
                Amount = receipt.Amount,
                ExchangeRate = receipt.Rate,
                Currency = receipt.Currency,
                ReceiptDate = receipt.ReceiptDate.ToShortDateString(),
                BankName = receipt.Account.BankName,
                receipt.RequestID,
                receipt.GoodsCreWord,
                receipt.GoodsCreNo
            };

            this.Paging(receipts, convert);
        }

    }
}