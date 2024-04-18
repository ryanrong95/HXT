using DBSApis.Models;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DBSApis.Services
{
    public class FXBookingService
    {
        public FXBookingRequest Request { get; set; }

        public FXBookingService(FXBookingRequest request)
        {
            this.Request = request;
        }

        public ReturnType PostMsg()
        {
            DurApiLogs durApiLogs = new DurApiLogs();
            durApiLogs.TransactionName = DBSConstConfig.DBSConstTransName.FXBooking;

            ReturnType returnType = new ReturnType();
            try
            {
                DurFXRequest durFXRequest = new DurFXRequest();
                durFXRequest.TransactionName = DBSConstConfig.DBSConstTransName.FXBooking;
                durFXRequest.msgId = this.Request.header.msgId;
                durFXRequest.orgId = this.Request.header.orgId;
                durFXRequest.uid = this.Request.txnInfo.uid;
                durFXRequest.clientTxnsId = this.Request.txnInfo.clientTxnsId;
                durFXRequest.Enter();

                KeyConfig Config = ApiService.Current.KeyConfigs;
                UrlConfig urlConfig = ApiService.Current.UrlConfigs;
                string msg = this.Request.Json();

                durApiLogs.msgId = this.Request.header.msgId;
                durApiLogs.RequestContent = msg;
                string requestURL = urlConfig.ApiServerUrl + urlConfig.FXBookingUrl;
                durApiLogs.Url = requestURL;

                EncryptMessage encryptMessage = new EncryptMessage(Config, msg);
                string encryptedMessage = encryptMessage.Encrypt();

                PostMessage postMessage = new PostMessage(requestURL, encryptedMessage, Config);
                string reponseMsg = postMessage.PushMsg();


                DecryptMessage decryptMessage = new DecryptMessage(Config, reponseMsg);
                string decryptedMsg = decryptMessage.Decrypt();

                durApiLogs.ResponseContent = decryptedMsg;
                durApiLogs.Enter();

                GatewayResponse response = JsonConvert.DeserializeObject<GatewayResponse>(decryptedMsg);
                if (response.error != null && response.error.status.Equals(DBSConstConfig.DBSConstError.DBSRJCT))
                {
                    returnType.IsSuccess = false;
                    returnType.Code = response.error.code;
                    returnType.Msg = response.error.description;

                    DurGatewayResponse durGatewayResponse = new DurGatewayResponse();
                    durGatewayResponse.msgId = response.header.msgId;
                    durGatewayResponse.code = response.error.code;
                    durGatewayResponse.description = response.error.description;
                    durGatewayResponse.DBSstatus = response.error.status;
                    durGatewayResponse.Enter();

                    return returnType;
                }

                FXBookingResponse FXPricingResponse = JsonConvert.DeserializeObject<FXBookingResponse>(decryptedMsg);

                DurFXResponse durFXResponse = new DurFXResponse();
                durFXResponse.TransactionName = DBSConstConfig.DBSConstTransName.FXBooking;
                durFXResponse.msgId = FXPricingResponse.header.msgId;
                durFXResponse.orgId = FXPricingResponse.header.orgId;
                durFXResponse.txnRefId = FXPricingResponse.txnResponse.txnRefId;
                if (FXPricingResponse.txnResponse.txnDateTime != null)
                {
                    durFXResponse.txnDateTime = Convert.ToDateTime(FXPricingResponse.txnResponse.txnDateTime.Substring(0, 23));
                }
                durFXResponse.uid = FXPricingResponse.txnResponse.uid;
                durFXResponse.custProfile = FXPricingResponse.txnResponse.custProfile;
                durFXResponse.ccyPair = FXPricingResponse.txnResponse.ccyPair;
                durFXResponse.txnAmount = FXPricingResponse.txnResponse.txnAmount;
                durFXResponse.txnCcy = FXPricingResponse.txnResponse.txnCcy;
                durFXResponse.contraCcy = FXPricingResponse.txnResponse.contraCcy;
                durFXResponse.contraAmount = FXPricingResponse.txnResponse.contraAmount;
                durFXResponse.dealtSide = FXPricingResponse.txnResponse.dealtSide;
                durFXResponse.rate = FXPricingResponse.txnResponse.rate;
                durFXResponse.tenor = FXPricingResponse.txnResponse.tenor;
                durFXResponse.valueDate = FXPricingResponse.txnResponse.valueDate;
                durFXResponse.dealType = FXPricingResponse.txnResponse.dealType;
                durFXResponse.txnStatus = FXPricingResponse.txnResponse.txnStatus;
                durFXResponse.traceId = FXPricingResponse.txnResponse.traceId;
                durFXResponse.clientTxnsId = FXPricingResponse.txnResponse.clientTxnsId;
                durFXResponse.txnRejectCode = FXPricingResponse.txnResponse.txnRejectCode;
                durFXResponse.txnStatusDescription = FXPricingResponse.txnResponse.txnStatusDescription;
                durFXResponse.Enter();


                if (FXPricingResponse.txnResponse.txnStatus.Equals(DBSConstConfig.DBSConstError.FXError))
                {
                    returnType.IsSuccess = false;
                    returnType.Code = FXPricingResponse.txnResponse.txnRejectCode;
                    returnType.Msg = FXPricingResponse.txnResponse.txnStatusDescription;

                    return returnType;
                }
                else
                {
                    returnType.IsSuccess = true;
                    returnType.Data = FXPricingResponse;
                    return returnType;
                }
                
               
            }
            catch (Exception ex)
            {
                durApiLogs.ResponseContent = ex.ToString();
                durApiLogs.Enter();

                returnType.IsSuccess = false;
                returnType.Msg = ex.ToString();
                return returnType;
            }
        }
    }
}