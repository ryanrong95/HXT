using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance
{
    public partial class AccountSelect : Uc.PageBase
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
            string test = EnumUtils.ToDictionary<TTChargeBearer>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.ChargeBearer = EnumUtils.ToDictionary<TTChargeBearer>().Select(item => new { Text = item.Value, Value = item.Key }).Json();

            string NoticeID = Request.QueryString["NoticeID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice.Where(item => item.ID == NoticeID).FirstOrDefault();
            string consignorCode = "",partyName="";
            if (notice.ConsignorCode.Contains("香港畅运"))
            {
                consignorCode = "香港畅运";
                partyName = "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED";
            }
            else if (notice.ConsignorCode.Contains("香港万路通"))
            {
                consignorCode = "香港万路通";
                partyName = "HONG KONG WANLUTONG INTERNATIONAL LOGISTICS CO.,LIMITED";
            }

            this.Model.consignorCode = consignorCode;
            this.Model.partyName = partyName;
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

        protected void AccountInfo()
        {
            string accountID = Request.Form["AccountID"];
            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(t => t.ID == accountID).FirstOrDefault();

            string AccountNo = financeAccounts.BankAccount;
            string SwiftBic = financeAccounts.SwiftCode;
            string BankName = financeAccounts.BankName;

            Response.Write((new { AccountNo = AccountNo, SwiftBic = SwiftBic, BankName= BankName }).Json());
        }

        protected void TT()
        {
            string txnRefId = Request.Form["txnRefId"];
            decimal txnAmount = Convert.ToDecimal(Request.Form["txnAmount"].Replace("USD", ""));
            
            int iChargeBearer = Convert.ToInt16(Request.Form["ChargeBearer"]);
            string partyName = Request.Form["PartyName"];
            string receivingPartyAccountNo = Request.Form["AccountNo"];
            string receivingPartySwiftBic = Request.Form["SwiftCode"];
            string receivingBankName = Request.Form["BankName"];
            string senderPartyName = Request.Form["SenderPartyName"];           

            if (receivingBankName.Length > 35)
            {
                Response.Write((new { success = false, message = "付汇失败!收款方银行账户名称不能超过35个字符" }).Json());
            }
          
            string receivingPartyName = "";
            if (partyName.Contains("CHANGYUN"))
            {
                receivingPartyName = "HONG KONG CHANGYUN INTERNATIONAL";
            }
            else if (partyName.Contains("WANLUTONG"))
            {
                receivingPartyName = "HONG KONG WANLUTONG INTERNATIONAL";
            }

            TTChargeBearer chargeBearer = (TTChargeBearer)iChargeBearer;

            TTRequest tTRequest = GetTTJson(txnAmount, receivingPartyName, receivingPartyAccountNo, receivingPartySwiftBic,receivingBankName, chargeBearer, senderPartyName, txnRefId);
            string result2 = tTRequest.Json();
            var apisetting = new Needs.Ccs.Services.ApiSettings.DBSApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.TT;

            try
            {
                HttpPostRequest request = new HttpPostRequest();
                request.Timeout = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.TimeOut;
                request.ContentType = "application/json";
                var result = request.Post(apiurl, tTRequest.Json());

                var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);
                if (jResult.success)
                {
                    AfterTT afterBooking = new AfterTT(txnRefId);
                    afterBooking.Process();
                    Response.Write((new { success = true, message = "成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "付汇失败!请联系管理员" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "付汇失败：" + ex.Message }).Json());
            }
        }

        private TTRequest GetTTJson(decimal totalTxnAmount, string receivingPartyName, string receivingPartyAccountNo, string receivingPartySwiftBic,string receivingBankName, TTChargeBearer chargeBearer,string senderPartyName,string txnRefId)
        {
            TTRequest aCTRequest = new TTRequest();

            aCTRequest.header = new ACTRequestHeader();
            aCTRequest.header.msgId = ChainsGuid.NewGuidUp();
            aCTRequest.header.orgId = System.Configuration.ConfigurationManager.AppSettings["Api_OrgId"];
            aCTRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            aCTRequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequest.header.noOfTxs = 1;
            aCTRequest.header.totalTxnAmount = totalTxnAmount;

            aCTRequest.txnInfoDetails = new TTRequestTxnInfoDetails();
            aCTRequest.txnInfoDetails.txnInfo = new List<TTRequestTxnInfo>();

            TTRequestTxnInfo aCTRequestTxnInfo = new TTRequestTxnInfo();
            aCTRequestTxnInfo.customerReference = ChainsGuid.NewGuidUp().Substring(0, 15);
            aCTRequestTxnInfo.txnType = DBSConstConfig.DBSConstConfiguration.TTTxnType;
            aCTRequestTxnInfo.txnDate = DateTime.Now.ToString("yyyy-MM-dd");
            aCTRequestTxnInfo.txnCcy = DBSConstConfig.DBSConstConfiguration.USD;
            aCTRequestTxnInfo.txnAmount = totalTxnAmount;
            aCTRequestTxnInfo.debitAccountCcy = DBSConstConfig.DBSConstConfiguration.CNY;
            aCTRequestTxnInfo.fxContractRef1 = txnRefId;
            aCTRequestTxnInfo.fxAmountUtilized1 = totalTxnAmount;
            aCTRequestTxnInfo.chargeBearer = chargeBearer.ToString();


            aCTRequestTxnInfo.senderParty = new CNAPSSenderParty();
            aCTRequestTxnInfo.senderParty.name = senderPartyName;
            aCTRequestTxnInfo.senderParty.accountNo = DBSConstConfig.DBSConstConfiguration.CNYAccountNo;
            aCTRequestTxnInfo.senderParty.bankCtryCode = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequestTxnInfo.senderParty.swiftBic = System.Configuration.ConfigurationManager.AppSettings["Api_SwiftBic"];
           

            aCTRequestTxnInfo.receivingParty = new TTReceivingParty();
            aCTRequestTxnInfo.receivingParty.name = receivingPartyName;
            aCTRequestTxnInfo.receivingParty.accountNo = receivingPartyAccountNo;
            aCTRequestTxnInfo.receivingParty.bankCtryCode = DBSConstConfig.DBSConstConfiguration.HKCtry;           
            aCTRequestTxnInfo.receivingParty.swiftBic = receivingPartySwiftBic;
            aCTRequestTxnInfo.receivingParty.bankName = receivingBankName;
            aCTRequestTxnInfo.receivingParty.beneficiaryAddresses = new List<TTBeneficiaryAddresses>();
            TTBeneficiaryAddresses addresses = new TTBeneficiaryAddresses();
            addresses.address = "LOGISTICS CO., LIMITED";            
            aCTRequestTxnInfo.receivingParty.beneficiaryAddresses.Add(addresses);
          

            aCTRequestTxnInfo.bopInfo = new TTBopInfo();
            aCTRequestTxnInfo.bopInfo.specificPaymentPurpose = TTPaymentPurpose.PD.ToString();
            aCTRequestTxnInfo.bopInfo.taxFreeGoodsRelated = TTTaxFree.N.ToString();
            aCTRequestTxnInfo.bopInfo.paymentNature = TTPaymentNature.F.ToString();
            aCTRequestTxnInfo.bopInfo.bOPCode1PaymentCategory = TTPaymentCategory.TR.ToString();
            aCTRequestTxnInfo.bopInfo.bOPCode1SeriesCode = TTBOPCode.TT121010.ToString().Replace("TT", "");
            //aCTRequestTxnInfo.bopInfo.transactionRemarks1 = "一般贸易进口电子产品";
            aCTRequestTxnInfo.bopInfo.transactionRemarks1 = "General trading in import electronic goods";
            aCTRequestTxnInfo.bopInfo.counterPartyCtryCode = DBSConstConfig.DBSConstConfiguration.HKCtry;

            aCTRequest.txnInfoDetails.txnInfo.Add(aCTRequestTxnInfo);

            return aCTRequest;
        }
    }
}