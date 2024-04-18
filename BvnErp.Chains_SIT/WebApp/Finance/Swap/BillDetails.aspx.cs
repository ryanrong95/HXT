using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap
{
    public partial class BillDetails : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //金库
            this.Model.VaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();
            //账户
            this.Model.AccountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Select(item => new { Value = item.ID, Text = item.AccountName }).Json();

            this.Model.SwapNoticeData = "".Json();
            string ID = Request.QueryString["ID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[ID];
            if (notice != null)
            {
                this.Model.SwapNoticeData = new
                {
                    TotalAmount = notice.TotalAmount,
                    ExchangeRate = notice.ExchangeRate,
                    TotalRmb = notice.TotalAmountCNY.ToRound(2),
                    BankName = notice.BankName,
                    AccountOut = notice.OutAccount?.ID,
                    AccountIn = notice.InAccount?.ID,
                    AccountMid = notice.MidAccount?.ID,
                    VaultOut = notice.OutAccount?.FinanceVaultID,
                    VaultIn = notice.InAccount?.FinanceVaultID,
                    VaultMid = notice.MidAccount?.FinanceVaultID,
                    Poundage = notice.Poundage,
                    ExchangeDate = notice.UpdateDate,
                }.Json();
            }   
        }

        /// <summary>
        /// 换汇明细项
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var items = new Needs.Ccs.Services.Views.SwapDetailView().Where(item => item.SwapNoticeID == ID);
            Func<SwapDetail, object> convert = item => new
            {
                ID = item.ID,
                ContrNo = item.ContrNo,
                OrderID = item.OrderID,
                ClientName = item.ClientName,
                Currency = item.Currency,
                RealExchangeRate = item.RealExchangeRate,               
                TransPremiumInsuranceAmount = Math.Round(item.TransPremiumInsuranceAmount.Value, 2, MidpointRounding.AwayFromZero),
                TransPremiumInsuranceAmountRMB = Math.Round(item.TransPremiumInsuranceAmountRMB.Value, 2, MidpointRounding.AwayFromZero),
                AcceptanceCustomer = item.AcceptanceCustomer == null ? 0 : Math.Round(item.AcceptanceCustomer.Value, 2, MidpointRounding.AwayFromZero),
                AcceptanceXDT = item.AcceptanceXDT == null ? 0 : Math.Round(item.AcceptanceXDT.Value, 2, MidpointRounding.AwayFromZero),
                DDate = item.DDate.Value.ToString("yyyy-MM-dd")
            };
            Response.Write(new
            {
                rows = items.Select(convert).ToArray()
            }.Json());
        }
    }
}