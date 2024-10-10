using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Serializers;
using Needs.Wl;
using NPOI.SS.Formula.Functions;
using Currency = Needs.Ccs.Services.Enums.Currency;
using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Descriptions;

namespace WebApp.Finance.Receipt
{
    /// <summary>
    /// 财务新增收款编辑界面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.Admin = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
        }

        protected void LoadComboBoxData()
        {
            this.Model.CurrData = Needs.Wl.Admin.Plat.AdminPlat.Currencies
                .Select(item => new { Value = item.Code, Text = item.Code + " " + item.Name }).Json();

            var accountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts;

            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();

            this.Model.FinanceReceiptData = "".Json();
            this.Model.AccountData = "".Json();

            this.Model.CenterPaymentType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>().Select(item => new { Value = item.Key, Text = item.Value }).Json() ;

            this.Model.AccountProperty = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.AccountProperty>().Select(item => new { Value = item.Key, Text = item.Value }).Json(); ;

            this.Model.ClientIno = Needs.Wl.Admin.Plat.AdminPlat.Clients.Select(item => new { Value = item.Company.Name, Text = item.Company.Name }).Json();

            this.Model.CenterDepositReceived = FeeTypeTransfer.Current.L2CInTransfer(Needs.Ccs.Services.Enums.FinanceFeeType.DepositReceived);
            //编辑收款
            string ID = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var finance = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceReceipts.FirstOrDefault(item => item.ID == ID);
            string ceterFeetype = FeeTypeTransfer.Current.L2CInTransfer(finance.FeeType);
            if (finance != null)
            {
                this.Model.FinanceReceiptData = new
                {
                    ID = ID,
                    SeqNo = finance.SeqNo,
                    Payer = finance.Payer,
                    FeeType = ceterFeetype,
                    ReceiptType = finance.ReceiptType,
                    ReceiptDate = finance.ReceiptDate.ToShortDateString(),
                    Amount = finance.Amount,
                    Currency = finance.Currency,
                    Rate = finance.Rate,
                    Vault = finance.Vault.ID,
                    Account = finance.Account.ID,
                    BankAccount = finance.Account.BankAccount,
                    Admin = finance.Admin.RealName,
                    Summary = finance.Summary,
                    AccountProperty = finance.AccountProperty,

                }.Json();

                this.Model.FinanceVaultData = accountData.Where(a => a.Currency == finance.Currency)
                    .Select(item => new {Value = item.FinanceVaultID, Text = item.FinanceVaultName}).Distinct().Json();

                this.Model.AccountData =
                    accountData.Where(a => a.Currency == finance.Currency && a.FinanceVaultID == finance.Vault.ID)
                        .Select(item => new
                        {
                            item.ID,
                            item.AccountName,
                            item.BankAccount
                        }).Distinct().Json();
            }
        }

        /// <summary>
        /// 根据金库显示账户
        /// </summary>
        /// <returns></returns>
        protected object GetAccountByVault()
        {
            var currency = Request.Form["Currency"];
            var vault = Request.Form["FinanceVault"];
            return new
            {
                data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(v => v.FinanceVaultID == vault && v.Currency == currency).Select(item => new
                {
                    item.ID,
                    item.AccountName,
                    item.BankAccount
                }).Distinct()
            };
        }

        /// <summary>
        /// 获取实时汇率
        /// </summary>
        /// <returns></returns>
        protected decimal? GetExchangeRate()
        {
            decimal? exchangeRate = null;
            var currency = Request.Form["Currency"];
            if (currency == MultiEnumUtils.ToCode<Currency>(Currency.CNY))
            {
                exchangeRate = 1;
            }
            else
            {
                var realExchange = Needs.Wl.Admin.Plat.AdminPlat.RealTimeRates.FirstOrDefault(rate => rate.Code == currency);
                exchangeRate = realExchange?.Rate;
            }

            return exchangeRate;
        }

        /// <summary>
        /// check clientName 是否存在
        /// </summary>
        protected void CheckClient()
        {
            var client = Request.Form["Payer"].Replace("&amp;", "&");
            var count = Needs.Wl.Admin.Plat.AdminPlat.Clients.Count(t => t.Company.Name == client);
            Response.Write(count > 0
                ? (new { success = true, message = "付款人存在" }).Json()
                : (new { success = false, message = "付款人不存在" }).Json());
        }

        /// <summary>
        /// 判断输入的流水号是否重复
        /// </summary>
        protected void CheckSeqNo() 
        {
            var SeqNo = Request.Form["SeqNo"];
            var receipt = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceReceipts
                .Where(item => item.SeqNo == SeqNo).OrderByDescending(t=>t.CreateDate).FirstOrDefault();

            string SeqNoPostfix = "";
            bool isSuccess = true;
            if (receipt != null) 
            {
                isSuccess = false;
                string oldSeq = receipt.SeqNo;
                string[] oldSeqs = oldSeq.Split('-');
                if (oldSeqs.Length > 1) 
                {
                    SeqNoPostfix = "流水号已重复，现有最后一个后缀是 "+ oldSeqs[1];
                }
                else 
                {
                    SeqNoPostfix = "流水号已重复，该流水号没使用后缀";
                }
            }
           

            Response.Write((new { success = isSuccess, message = SeqNoPostfix }).Json());
        }

        /// <summary>
        /// 金库只能是同类型币种。
        /// </summary>
        protected object GetVaultByCurrency()
        {
            var currency = Request.Form["Currency"];
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(s => s.Currency == currency)
                .AsQueryable().Select(v => new
                {
                    Value = v.FinanceVaultID,
                    Text = v.FinanceVaultName
                }).Distinct();
            return data;
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            try
            {
                string oldSeqNo = "";
                string Model = Request.Form["Model"].Replace("&quot;", "\'");
                dynamic model = Model.JsonTo<dynamic>();
                //ID 可为null
                string ID = model.ID ?? string.Empty;
                decimal Amount = model.Amount ?? 0.0M;

                string feetype = (string)model.FeeType;
                FinanceFeeType feeType = FeeTypeTransfer.Current.C2LInTransfer(feetype);

                var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var ErmAdminID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == currentAdmin.ID)?.ErmAdminID;
                currentAdmin.ErmAdminID = ErmAdminID;

                var financeReceipt = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceReceipts[ID] ??
                                     new Needs.Ccs.Services.Models.FinanceReceipt();
                financeReceipt.Payer = model.Payer;
                oldSeqNo = financeReceipt.SeqNo;
                financeReceipt.SeqNo = model.SeqNo;
                financeReceipt.FeeType = feeType;
                financeReceipt.ReceiptType = (PaymentType)Enum.Parse(typeof(PaymentType), (string)model.PaymentType);
                financeReceipt.ReceiptDate = Convert.ToDateTime(model.Date);
                financeReceipt.Currency = model.CurrencyID;
                financeReceipt.Rate = model.Rate;
                financeReceipt.Vault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault[(string)model.FinanceVaultID];
                financeReceipt.Difference = Amount - financeReceipt.Amount;
                financeReceipt.Amount = ((decimal)model.Amount).ToRound(2);
                financeReceipt.Account = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[(string)model.AccountID];
                financeReceipt.Admin = currentAdmin;
                financeReceipt.Summary = model.Summary;
                financeReceipt.AccountProperty = (AccountProperty)Enum.Parse(typeof(AccountProperty), (string)model.AccountProperty);

                financeReceipt.EnterSuccess += FinanceReceipt_EnterSuccess;
                financeReceipt.Updated += FinanceReceipt_UpdateSuccess;
                financeReceipt.Enter();
                financeReceipt.Post2Center(oldSeqNo);
            }
            catch (Exception e)
            {
                Response.Write((new { success = false, message = "收款记录保存失败: " + e.Message }).Json());
            }
        }

        /// <summary>
        /// 收款记录保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceReceipt_EnterSuccess(object sender, SuccessEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                var financeReceipt = (Needs.Ccs.Services.Models.FinanceReceipt)e.Object;
                if (financeReceipt.FeeType == FinanceFeeType.DepositReceived)
                {
                    var client = Needs.Wl.Admin.Plat.AdminPlat.Clients.Where(t => t.Company.Name == financeReceipt.Payer).FirstOrDefault();
                    NoticeLog noticeLog = new NoticeLog();
                    noticeLog.MainID = financeReceipt.ID;
                    noticeLog.AdminIDs.Add(client.Merchandiser.ID);
                    noticeLog.NoticeType = SendNoticeType.ReceivingPending;
                    noticeLog.SendNotice();
                }
            });

            Response.Write((new { success = true, message = "收款记录保存成功！" }).Json());
        }
        /// <summary>
        /// 收款记录保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceReceipt_UpdateSuccess(object sender, FinanceReceiptUpdatedEventArgs e)
        {
            Response.Write((new { success = true, message = "收款记录更新成功！" }).Json());
        }

        protected object AccountCatalogsTree()
        {
            
            var treeStr = AccountCatalogsAlls.Current.JsonIn(AccountCatalogType.Input.GetDescription());  //new AccountCatalogsTree().Json();
            
            treeStr = treeStr.Replace("\"name\":", "\"text\":");
            return treeStr;
        }
    }
}