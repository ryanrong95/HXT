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
    public class AREService
    {
        public ARERequest Request { get; set; }

        public AREService(ARERequest request)
        {
            this.Request = request;
        }

        public ReturnType SearchFlow()
        {
            ReturnType returnType = new ReturnType();

            try
            {
                var DateToday = DateTime.Now.Date;
                using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
                {
                    var postRecord = reponsitory.ReadTable<Layer.Data.Sqls.foricDBS.ApiLogs>().
                                     Where(t => t.TransactionName == DBSConstConfig.DBSConstTransName.ARE && t.CreateDate > DateToday).OrderByDescending(t => t.CreateDate).ToList();

                    foreach(var reponseFlow in postRecord)
                    {
                        AREResponse reponse = JsonConvert.DeserializeObject<AREResponse>(reponseFlow.ResponseContent);                       
                        var IsRecored = reponsitory.ReadTable<Layer.Data.Sqls.foricDBS.AccountFlow>().Where(t => t.msgId == reponse.header.msgId).FirstOrDefault();                        
                        if (IsRecored != null && reponse.txnEnqResponse.acctInfo.FirstOrDefault().accountNo == this.Request.accInfo.accountNo)
                        {
                            returnType.IsSuccess = true;
                            returnType.Msg = "今日已成功查询";
                            return returnType;
                        }   
                    }

                    var postReturn = PostMsg();                   
                    returnType.IsSuccess = true;
                    returnType.Msg = "查询成功";
                    if (!postReturn.IsSuccess)
                    {
                        returnType.IsSuccess = false;
                        returnType.Msg = postReturn.Msg;
                    }
                    return returnType;
                }
            }
            catch (Exception ex)
            {
                returnType.IsSuccess = false;
                returnType.Msg = ex.ToString();
                return returnType;
            }
            
        }

        public ReturnType PostMsg()
        {
            DurApiLogs durApiLogs = new DurApiLogs();
            durApiLogs.TransactionName = DBSConstConfig.DBSConstTransName.ARE;

            ReturnType returnType = new ReturnType();

            try
            {
                KeyConfig Config = ApiService.Current.KeyConfigs;
                UrlConfig urlConfig = ApiService.Current.UrlConfigs;
                string msg = this.Request.Json();

                EncryptMessage encryptMessage = new EncryptMessage(Config, msg);
                string encryptedMessage = encryptMessage.Encrypt();

                string requestURL = urlConfig.ApiServerUrl + urlConfig.AREUrl;
                PostMessage postMessage = new PostMessage(requestURL, encryptedMessage, Config);
                string reponseMsg = postMessage.PushMsg();

                DecryptMessage decryptMessage = new DecryptMessage(Config, reponseMsg);
                string decryptedMsg = decryptMessage.Decrypt();

                durApiLogs.msgId = this.Request.header.msgId;
                durApiLogs.RequestContent = msg;                
                durApiLogs.Url = requestURL;
                durApiLogs.ResponseContent = decryptedMsg;
                durApiLogs.Enter();

                GatewayResponse response = JsonConvert.DeserializeObject<GatewayResponse>(decryptedMsg);
                if (response.error != null && response.error.status.Equals(DBSConstConfig.DBSConstError.DBSRJCT))
                {
                    returnType.IsSuccess = false;
                    returnType.Code = response.error.code;
                    returnType.Msg = response.error.description;

                    return returnType;
                }

                AREResponse aBEFailResponse = JsonConvert.DeserializeObject<AREResponse>(decryptedMsg);
                if (aBEFailResponse.txnEnqResponse.enqStatus.Equals(DBSConstConfig.DBSConstError.DBSRJCT))
                {
                    returnType.IsSuccess = false;
                    returnType.Code = aBEFailResponse.txnEnqResponse.enqRejectCode;
                    returnType.Msg = aBEFailResponse.txnEnqResponse.enqStatusDescription;

                    return returnType;
                }

                string msgId = aBEFailResponse.header.msgId;
                var AccountInfo = aBEFailResponse.txnEnqResponse.acctInfo.FirstOrDefault();
                var IniParty = AccountInfo.initiatingParty.FirstOrDefault();
                foreach(var item in IniParty.txnInfo)
                {
                    DurAccountFlow flow = new DurAccountFlow();
                    flow.msgId = msgId;
                    flow.accountNo = AccountInfo.accountNo;
                    flow.accountCcy = AccountInfo.accountCcy;
                    flow.availableBal = AccountInfo.availableBal;
                    flow.initiatingPartyName = IniParty.name;
                    flow.drCrInd = item.drCrInd;
                    flow.txnCode = item.txnCode;
                    flow.txnDesc = item.txnDesc;
                    flow.txnDate = Convert.ToDateTime(item.txnDate);
                    flow.valueDate = Convert.ToDateTime(item.valueDate);
                    flow.txnCcy = item.txnCcy;
                    flow.txnAmount = item.txnAmount;
                    flow.Enter();
                }

                AREResponse aBESuccResponse = JsonConvert.DeserializeObject<AREResponse>(decryptedMsg);
                returnType.IsSuccess = true;
                returnType.Data = aBESuccResponse;

                return returnType;
            }
            catch (Exception ex)
            {
                returnType.IsSuccess = false;
                returnType.Msg = ex.ToString();
                return returnType;
            }
        }
    }
}