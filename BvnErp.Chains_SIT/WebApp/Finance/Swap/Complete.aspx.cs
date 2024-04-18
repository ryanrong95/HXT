using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap
{
    public partial class Complete : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            //金库
            this.Model.VaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Select(item => new { Value = item.ID, Text = item.Name }).Json();

            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[ID];

            //查询实时汇率 Begin
            string realTimeRate = "";

            var realTimeExchangeRatesView = new Needs.Wl.Models.Views.RealTimeExchangeRatesView();
            realTimeExchangeRatesView.AllowPaging = false;

            var predicate = PredicateBuilder.Create<Needs.Wl.Models.RealTimeExchangeRate>();
            predicate = predicate.And(t => t.Code == notice.Currency);
            realTimeExchangeRatesView.Predicate = predicate;
            var realTimeExchangeRateModel = realTimeExchangeRatesView.FirstOrDefault();
            if (realTimeExchangeRateModel != null)
            {
                realTimeRate = Convert.ToString(realTimeExchangeRateModel.Rate);
            }
            //查询实时汇率 End

            this.Model.AllData = new
            {
                ID = ID,
                TotalAmount = notice.TotalAmount,
                BankName = notice.BankName,
                RealTimeExchangeRate = realTimeRate,
            }.Json();
        }

        protected void AccountSelect()
        {
            //金库ID
            string id = Request.Form["ID"];
            //账户
            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(item => item.FinanceVaultID == id);
            Response.Write(financeAccounts.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
        }

        /// <summary>
        /// 人民币账户
        /// </summary>
        protected void RmbAccountSelect()
        {
            //金库ID
            string id = Request.Form["ID"];
            //账户
            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(item => item.FinanceVaultID == id && item.Currency == "CNY");
            Response.Write(financeAccounts.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
        }

        /// <summary>
        /// 外币账户
        /// </summary>
        protected void ForeignAccountSelect()
        {
            string ID = Request.Form["NoticeID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice.Where(item => item.ID == ID).FirstOrDefault();
            //金库ID
            string id = Request.Form["ID"];
            //账户
            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(item => item.FinanceVaultID == id && item.Currency == notice.Currency);
            Response.Write(financeAccounts.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
        }

        /// <summary>
        /// 供应商账户账户
        /// </summary>
        protected void ForeignAccountSelectIn()
        {
            string ID = Request.Form["NoticeID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice.Where(item => item.ID == ID).FirstOrDefault();
            string consignorCode = "";
            if (notice.ConsignorCode.Contains("香港畅运"))
            {
                consignorCode = "畅运";
            }
            else if (notice.ConsignorCode.Contains("香港万路通"))
            {
                consignorCode = "万路通";
            }
            //金库ID
            string id = Request.Form["ID"];
            //账户
            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(item => item.FinanceVaultID == id && item.Currency == notice.Currency && item.AccountName.Contains(consignorCode));
            Response.Write(financeAccounts.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
        }
        /// <summary>
        /// 加载换汇通知
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNoticeItem.Where(item => item.SwapNoticeID == ID);
            Func<SwapNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                ContrNo = item.SwapDecHead.ContrNo,
                OrderID = item.SwapDecHead.OrderID,
                Currency = item.SwapDecHead.Currency,
                SwapAmount = item.Amount,  //item.SwapDecHead.SwapAmount,
                DDate = item.SwapDecHead.DDate?.ToString("yyyy-MM-dd HH:mm:ss"),
            };
            Response.Write(new
            {
                rows = items.Select(convert).ToArray()
            }.Json());
        }
        //日志记录
        protected void LoadLogs()
        {
            string ID = Request.Form["ID"];
            var swapNoticelogs = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNoticelogsView.Where(item => item.SwapNoticeID == ID);
            Func<SwapNoticeLog, object> convert = item => new
            {
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
            swapNoticelogs = swapNoticelogs.OrderByDescending(t => t.CreateDate);
            Response.Write(new { rows = swapNoticelogs.Select(convert).ToArray(), }.Json());
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            try
            {
                var id = Request.Form["ID"];
                var BankName = Request.Form["BankName"];
                var ExchangeRate = Request.Form["ExchangeRate"];
                var TotalRmb = Request.Form["TotalRmb"];
                var VaultOut = Request.Form["VaultOut"];
                var VaultMid = Request.Form["VaultMid"];
                var VaultIn = Request.Form["VaultIn"];
                var AccountOut = Request.Form["AccountOut"];
                var AccountMid = Request.Form["AccountMid"]; 
                var AccountIn = Request.Form["AccountIn"];
                var Poundage = Request.Form["Poundage"];
                var SeqNoOut = Request.Form["SeqNoOut"];
                var SeqNoIn = Request.Form["SeqNoIn"];
                var SeqNoMidR = Request.Form["SeqNoMidR"];
                var SeqNoMidP = Request.Form["SeqNoMidP"];
                var SeqNoPoundage = Request.Form["SeqNoPoundage"];
                var ExchangeDate = Request.Form["ExchangeDate"];
                var RealTimeExchangeRate = Request.Form["RealTimeExchangeRate"];
                //查询账户
                var vaults = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault;
                var accounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts;
                //查询
                var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[id];
                notice.BankName = BankName;
                notice.ExchangeRate = decimal.Parse(ExchangeRate);
                notice.TotalAmountCNY = decimal.Parse(TotalRmb);
                notice.InVault = vaults[VaultIn];
                notice.MidVault = vaults[VaultMid];
                notice.OutVault = vaults[VaultOut];
                notice.InAccount = accounts[AccountIn];
                notice.MidAccount = accounts[AccountMid];
                notice.OutAccount = accounts[AccountOut];
                notice.SeqNoOut = SeqNoOut;
                notice.SeqNoIn = SeqNoIn;
                notice.SeqNoMidR = SeqNoMidR;
                notice.SeqNoMidP = SeqNoMidP;
                notice.SeqNoPoundage = SeqNoPoundage;
                notice.UpdateDate = DateTime.Parse(ExchangeDate);
                notice.RealTimeExchangeRate = decimal.Parse(RealTimeExchangeRate);
                if (Poundage == "" || Poundage == null)
                {
                    notice.Poundage = 0M;
                }
                else
                {
                    notice.Poundage = decimal.Parse(Poundage);
                }                
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                notice.SetOperator(admin);
                notice.Complete();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}