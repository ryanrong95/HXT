using DBSApis.Models;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Services
{
    public class CNAPSService
    {
        public CNAPSRequest Request { get; set; }

        public CNAPSService(CNAPSRequest request)
        {
            this.Request = request;
        }

        public ReturnType PostMsg()
        {
            DurApiLogs durApiLogs = new DurApiLogs();
            durApiLogs.TransactionName = DBSConstConfig.DBSConstTransName.CNAPS;

            ReturnType returnType = new ReturnType();
            try
            {
                DurTSTRequest durTSTRequest = new DurTSTRequest();
                durTSTRequest.TransactionName = DBSConstConfig.DBSConstTransName.CNAPS;
                durTSTRequest.msgId = this.Request.header.msgId;
                durTSTRequest.orgId = this.Request.header.orgId;
                durTSTRequest.timeStamp = Convert.ToDateTime(this.Request.header.timeStamp);
                durTSTRequest.ctry = this.Request.header.ctry;
                durTSTRequest.noOfTxs = this.Request.header.noOfTxs;
                durTSTRequest.totalTxnAmount = this.Request.header.totalTxnAmount;
                CNAPSRequestTxnInfo singleTxnInfo = this.Request.txnInfoDetails.txnInfo.FirstOrDefault();
                durTSTRequest.customerReference = singleTxnInfo.customerReference;
                durTSTRequest.txnType = singleTxnInfo.txnType;
                durTSTRequest.txnDate = singleTxnInfo.txnDate;
                durTSTRequest.txnCcy = singleTxnInfo.txnCcy;
                durTSTRequest.txnAmount = singleTxnInfo.txnAmount;
                durTSTRequest.debitAccountCcy = singleTxnInfo.debitAccountCcy;                
                durTSTRequest.senderName = singleTxnInfo.senderParty.name;
                durTSTRequest.senderAccountNo = singleTxnInfo.senderParty.accountNo;
                durTSTRequest.senderBankCtryCode = singleTxnInfo.senderParty.bankCtryCode;
                durTSTRequest.senderSwiftBic = singleTxnInfo.senderParty.swiftBic;
                durTSTRequest.receivingName = singleTxnInfo.receivingParty.name;
                durTSTRequest.receivingAccountNo = singleTxnInfo.receivingParty.accountNo;
                durTSTRequest.receivingBankCtryCode = singleTxnInfo.receivingParty.bankCtryCode;
                durTSTRequest.receivingBankName = singleTxnInfo.receivingParty.bankName;
                durTSTRequest.Enter();

                KeyConfig Config = ApiService.Current.KeyConfigs;
                UrlConfig urlConfig = ApiService.Current.UrlConfigs;
                string msg = this.Request.Json();

                durApiLogs.msgId = this.Request.header.msgId;
                durApiLogs.RequestContent = msg;
                string requestURL = urlConfig.ApiServerUrl + urlConfig.CNAPSUrl;
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

                ACTACK1Response aCTACK1Response = JsonConvert.DeserializeObject<ACTACK1Response>(decryptedMsg);

                DurTSTResponse durTSTResponse = new DurTSTResponse();
                durTSTResponse.TransactionName = DBSConstConfig.DBSConstTransName.CNAPS;
                durTSTResponse.msgId = aCTACK1Response.header.msgId;
                durTSTResponse.orgId = aCTACK1Response.header.orgId;
                durTSTResponse.ctry = aCTACK1Response.header.ctry;

                ACTACK1ResponseTxnResponses ack1Response = aCTACK1Response.txnResponses.FirstOrDefault();
                durTSTResponse.responseType = ack1Response.responseType;
                durTSTResponse.msgRefId = ack1Response.msgRefId;
                durTSTResponse.txnStatus = ack1Response.txnStatus;
                durTSTResponse.txnRejectCode = ack1Response.txnRejectCode;
                durTSTResponse.txnStatusDescription = ack1Response.txnStatusDescription;
                durTSTResponse.Enter();

                if (ack1Response.txnRejectCode != null)
                {
                    returnType.IsSuccess = false;
                    returnType.Code = ack1Response.txnRejectCode;
                    returnType.Msg = ack1Response.txnStatusDescription;

                    return returnType;
                }
                else
                {
                    returnType.IsSuccess = true;
                    returnType.Data = aCTACK1Response;
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