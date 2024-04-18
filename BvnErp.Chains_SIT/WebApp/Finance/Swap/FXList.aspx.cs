using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Underly;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using NPOI.Util;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance
{
    public partial class FXList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {

        }


        protected void data()
        {
            var data = new FXPricingView().OrderByDescending(t => t.CreateDate).AsQueryable();
            

            Response.Write(new
            {
                rows = data.Select(
                        order => new
                        {
                            txnAmount = order.txnAmount + order.txnCcy,
                            rate = order.rate,
                            contraAmount = order.contraAmount + order.contraCcy,
                            validTill = order.validTill,
                            uid = order.uid,
                            RemainingSec = CalcSec(order.validTill.Value)
                        }
                     ).ToArray(),
                total = data.Count(),
            }.Json());
        }

       private long CalcSec(DateTime ValidTill)
        {
            long iSec = 0;
            DateTime dtNow = DateTime.Now;
            TimeSpan timeSpan1 = ValidTill- dtNow ;
            iSec = Convert.ToInt64(timeSpan1.TotalSeconds);
            return iSec;
        }

        protected void bookingData()
        {
            string uid = Request.QueryString["FXuid"];
            var data = new FXBookingView().Where(t => t.uid == uid).AsQueryable();

            Response.Write(new
            {
                rows = data.Select(
                        order => new
                        {
                            txnAmount = order.txnAmount + order.txnCcy,
                            rate = order.rate,
                            contraAmount = order.contraAmount + order.contraCcy,
                            valueDate = order.valueDate,
                            txnRefId = order.txnRefId,
                            isACT = order.IsACT,
                            isTT = order.IsTT,
                            uid = order.uid
                        }
                     ).ToArray(),
                total = data.Count(),
            }.Json());
        }

        /// <summary>
        /// 查询人民币余额
        /// </summary>
        protected void CNYRefresh()
        {
            ABERequest aBERequest = getABERequest();
            aBERequest.accountBalInfo = new ABEAccountBalInfo();
            aBERequest.accountBalInfo.accountNo = DBSConstConfig.DBSConstConfiguration.CNYAccountNo;

            var apisetting = new Needs.Ccs.Services.ApiSettings.DBSApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ABE;

            try
            {
                HttpPostRequest request = new HttpPostRequest();
                request.Timeout = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.TimeOut;
                request.ContentType = "application/json";
                var result = request.Post(apiurl, aBERequest.Json());

                var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);
                if (jResult.success)
                {
                    AccountBalResponse reponse = JsonConvert.DeserializeObject<AccountBalResponse>(jResult.data);
                    Response.Write((new { success = true, data = reponse.clsAvailableBal }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "查询人民币余额失败!请联系管理员" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "查询人民币余额失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 查询美金余额
        /// </summary>
        protected void USDRefresh()
        {
            ABERequest aBERequest = getABERequest();
            aBERequest.accountBalInfo = new ABEAccountBalInfo();
            aBERequest.accountBalInfo.accountNo = DBSConstConfig.DBSConstConfiguration.USDAccountNo;

            var apisetting = new Needs.Ccs.Services.ApiSettings.DBSApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ABE;

            try
            {
                HttpPostRequest request = new HttpPostRequest();
                request.Timeout = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.TimeOut;
                request.ContentType = "application/json";
                var result = request.Post(apiurl, aBERequest.Json());

                var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);
                if (jResult.success)
                {
                    AccountBalResponse reponse = JsonConvert.DeserializeObject<AccountBalResponse>(jResult.data);
                    Response.Write((new { success = true, data = reponse.clsAvailableBal }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "查询美金余额失败!请联系管理员" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "查询美金余额失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 生成余额查询Json
        /// </summary>
        /// <returns></returns>
        private ABERequest getABERequest()
        {
            ABERequest aBERequest = new ABERequest();

            aBERequest.header = new ABEHeader();
            aBERequest.header.msgId = ChainsGuid.NewGuidUp();
            aBERequest.header.orgId = System.Configuration.ConfigurationManager.AppSettings["Api_OrgId"];
            aBERequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            aBERequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;

            aBERequest.txnInfo = new ABETxnInfo();
            aBERequest.txnInfo.txnType = DBSConstConfig.DBSConstConfiguration.ABETxtType;

            return aBERequest;
        }

        /// <summary>
        /// 发送报价请求
        /// </summary>
        protected void FXPricing()
        {
            decimal txnAmount = Convert.ToDecimal(Request.Form["txnAmount"]);
            FXQuoteRequest fXQuoteRequest = FXPricingJosn(txnAmount);

            var apisetting = new Needs.Ccs.Services.ApiSettings.DBSApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.FXPricing;

            try
            {
                HttpPostRequest request = new HttpPostRequest();
                request.Timeout = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.TimeOut;
                request.ContentType = "application/json";
                var result = request.Post(apiurl, fXQuoteRequest.Json());

                var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);
                if (jResult.success)
                {
                    Response.Write((new { success = true, message = "成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "查询汇率失败!请联系管理员" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "查询汇率失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 获取报价Json
        /// </summary>
        /// <param name="txnAmount"></param>
        /// <returns></returns>
        private FXQuoteRequest FXPricingJosn(decimal txnAmount)
        {
            FXQuoteRequest fXQuoteRequest = new FXQuoteRequest();

            fXQuoteRequest.header = new FXQuoteHeader();
            fXQuoteRequest.header.msgId = ChainsGuid.NewGuidUp();
            fXQuoteRequest.header.orgId = System.Configuration.ConfigurationManager.AppSettings["Api_OrgId"];
            fXQuoteRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            fXQuoteRequest.txnInfo = new FXQuoteTxnInfo();
            fXQuoteRequest.txnInfo.ccyPair = "USDCNY";
            fXQuoteRequest.txnInfo.dealtSide = "BUY";
            fXQuoteRequest.txnInfo.txnAmount = txnAmount;
            fXQuoteRequest.txnInfo.txnCcy = "USD";
            fXQuoteRequest.txnInfo.tenor = "TODAY";
            fXQuoteRequest.txnInfo.clientTxnsId = ChainsGuid.NewGuidUp();

            return fXQuoteRequest;
        }        

        protected void FXBookingCheck()
        {
            bool success = false;
            string msg = "";
            string swapNoticeID = Request.Form["swapNoticeID"];
            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice.Where(t => t.ID == swapNoticeID).FirstOrDefault();
            if (notices != null)
            {
                if (string.IsNullOrEmpty(notices.uid))
                {
                    success = true;
                    msg = "成功";
                }
                else
                {
                    msg = "该条记录已经锁定一个汇率，不能重复锁定";
                }
            }
            Response.Write((new { success = success, message = msg }).Json());

        }
        protected void FXBooking()
        {
            string swapNoticeID = Request.Form["swapNoticeID"];
            string uid = Request.Form["uid"];
            FXBookingRequest fXBookingRequest = GetFXBookingJson(uid);

            var apisetting = new Needs.Ccs.Services.ApiSettings.DBSApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.FXBooking;

            try
            {
                HttpPostRequest request = new HttpPostRequest();
                request.Timeout = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.TimeOut;
                request.ContentType = "application/json";
                var result = request.Post(apiurl, fXBookingRequest.Json());

                var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);
                if (jResult.success)
                {
                    AfterBooking afterBooking = new AfterBooking(uid, swapNoticeID);
                    afterBooking.Process();
                    Response.Write((new { success = true, message = "成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "查询汇率失败!请联系管理员" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "查询汇率失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 获取BookingJson
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        private FXBookingRequest GetFXBookingJson(string uid)
        {
            FXBookingRequest fXBookingRequest = new FXBookingRequest();

            fXBookingRequest.header = new FXQuoteHeader();
            fXBookingRequest.header.msgId = ChainsGuid.NewGuidUp();
            fXBookingRequest.header.orgId = System.Configuration.ConfigurationManager.AppSettings["Api_OrgId"];
            fXBookingRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            fXBookingRequest.txnInfo = new FXBookingTxnInfo();
            fXBookingRequest.txnInfo.uid = uid;
            fXBookingRequest.txnInfo.clientTxnsId = ChainsGuid.NewGuidUp();

            return fXBookingRequest;
        }

        protected void ACT()
        {
            try
            {
                string txnRefId = Request.Form["txnRefId"];
                decimal txnAmount = Convert.ToDecimal(Request.Form["txnAmount"].Replace("USD", ""));              
                ACTRequest aCTRequest = GetACTJson(txnAmount, txnRefId);

                var apisetting = new Needs.Ccs.Services.ApiSettings.DBSApiSetting();
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ACT;


                HttpPostRequest request = new HttpPostRequest();
                request.Timeout = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.TimeOut;
                request.ContentType = "application/json";
                var result = request.Post(apiurl, aCTRequest.Json());

                var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);
                if (jResult.success)
                {
                    AfterACT afterBooking = new AfterACT(txnRefId);
                    afterBooking.Process();
                    Response.Write((new { success = true, message = "成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "行内转账失败!请联系管理员" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "行内转账失败：" + ex.Message }).Json());
            }
        }

        private ACTRequest GetACTJson(decimal totalTxnAmount, string fxContractRef1)
        {
            ACTRequest aCTRequest = new ACTRequest();

            aCTRequest.header = new ACTRequestHeader();
            aCTRequest.header.msgId = ChainsGuid.NewGuidUp();
            aCTRequest.header.orgId = System.Configuration.ConfigurationManager.AppSettings["Api_OrgId"];
            aCTRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            aCTRequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequest.header.noOfTxs = 1;
            aCTRequest.header.totalTxnAmount = totalTxnAmount;

            aCTRequest.txnInfoDetails = new ACTRequestTxnInfoDetails();
            aCTRequest.txnInfoDetails.txnInfo = new List<ACTRequestTxnInfo>();

            ACTRequestTxnInfo aCTRequestTxnInfo = new ACTRequestTxnInfo();
            aCTRequestTxnInfo.customerReference = ChainsGuid.NewGuidUp().Substring(0, 15);
            aCTRequestTxnInfo.txnType = DBSConstConfig.DBSConstConfiguration.ACTTxnType;
            aCTRequestTxnInfo.txnDate = DateTime.Now.ToString("yyyy-MM-dd");
            aCTRequestTxnInfo.txnCcy = DBSConstConfig.DBSConstConfiguration.USD;
            aCTRequestTxnInfo.txnAmount = totalTxnAmount;
            aCTRequestTxnInfo.debitAccountCcy = DBSConstConfig.DBSConstConfiguration.CNY;
            aCTRequestTxnInfo.fxContractRef1 = fxContractRef1;
            aCTRequestTxnInfo.fxAmountUtilized1 = totalTxnAmount;

            aCTRequestTxnInfo.senderParty = new ACTSenderParty();
            aCTRequestTxnInfo.senderParty.name = "XINDATONGSUPPLYCHAIN";
            aCTRequestTxnInfo.senderParty.accountNo = DBSConstConfig.DBSConstConfiguration.CNYAccountNo;
            aCTRequestTxnInfo.senderParty.bankCtryCode = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequestTxnInfo.senderParty.swiftBic = System.Configuration.ConfigurationManager.AppSettings["Api_SwiftBic"];

            aCTRequestTxnInfo.receivingParty = new ACTReceivingParty();
            aCTRequestTxnInfo.receivingParty.name = "XINDATONGSUPPLYCHAIN";
            aCTRequestTxnInfo.receivingParty.accountNo = DBSConstConfig.DBSConstConfiguration.USDAccountNo;

            aCTRequest.txnInfoDetails.txnInfo.Add(aCTRequestTxnInfo);

            return aCTRequest;
        }

        
    }
}