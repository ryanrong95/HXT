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
    public class ACK2Service
    {
        //public string EncryptedMsg { get; set; }
        public ACK2Response Response { get; set; }

        public ACK2Service(ACK2Response response)
        {
            this.Response = response;
        }

        //public void DecryptMsg()
        //{
        //    try
        //    {
        //        KeyConfig Config = ApiService.Current.KeyConfigs;
        //        DecryptMessage decryptMessage = new DecryptMessage(Config, this.EncryptedMsg);
        //        string decryptedMsg = decryptMessage.Decrypt();

        //        ACK2Response response = JsonConvert.DeserializeObject<ACK2Response>(decryptedMsg);
        //        this.Response = response;
        //        Handle();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public void Handle()
        {
            try
            {
                string TransactionName = "";
                using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
                {
                    var RequestMsg = reponsitory.ReadTable<Layer.Data.Sqls.foricDBS.TSTRequest>().Where(t => t.msgId == this.Response.header.msgId).FirstOrDefault();
                    if (RequestMsg != null)
                    {
                        TransactionName = RequestMsg.TransactionName;
                    }
                }

                if (TransactionName != "")
                {
                    DurApiLogs durApiLogs = new DurApiLogs();
                    durApiLogs.TransactionName = TransactionName;
                    durApiLogs.msgId = this.Response.header.msgId;
                    durApiLogs.ResponseContent = this.Response.Json();
                    durApiLogs.Enter();

                    DurTSTResponse durTSTResponse = new DurTSTResponse();
                    durTSTResponse.TransactionName = TransactionName;
                    durTSTResponse.msgId = this.Response.header.msgId;
                    durTSTResponse.orgId = this.Response.header.orgId;
                    durTSTResponse.noOfTxs = this.Response.header.noOfTxs;
                    durTSTResponse.totalTxnAmount = this.Response.header.totalTxnAmount;
                    durTSTResponse.ctry = this.Response.header.ctry;

                    ACK2ResponseTxnResponses txnResponse = this.Response.txnResponses.FirstOrDefault();
                    durTSTResponse.responseType = txnResponse.responseType;
                    durTSTResponse.customerReference = txnResponse.customerReference;
                    durTSTResponse.msgRefId = txnResponse.msgRefId;
                    durTSTResponse.txnRefId = txnResponse.txnRefId;
                    durTSTResponse.txnType = txnResponse.txnType;
                    durTSTResponse.txnStatus = txnResponse.txnStatus;
                    durTSTResponse.txnRejectCode = txnResponse.txnRejectCode;
                    durTSTResponse.txnStatusDescription = txnResponse.txnStatusDescription;

                    durTSTResponse.Enter();

                    if (txnResponse.txnStatus == DBSConstConfig.DBSConstError.DBSRJCT)
                    {
                        string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                        string sendContent = "代码:" + txnResponse.txnRejectCode + " 描述:" + txnResponse.txnStatusDescription;
                        SmtpContext.Current.Send(receivers, "星展银行返回失败：" + this.Response.header.msgId, sendContent);
                    }

                }
                else
                {
                    DurApiLogs durApiLogs = new DurApiLogs();
                    durApiLogs.TransactionName = DBSConstConfig.DBSConstTransName.UnknownTransName;
                    durApiLogs.msgId = this.Response.header.msgId;
                    durApiLogs.RequestContent = this.Response.Json();
                    durApiLogs.Enter();

                    string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    string sendContent = "未知消息ID";
                    SmtpContext.Current.Send(receivers, "星展银行返回未知消息ID" + this.Response.header.msgId, sendContent);
                }
            }
            catch (Exception ex)
            {
                string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                SmtpContext.Current.Send(receivers, "星展银行返回失败：" + this.Response.header.msgId, ex.ToString());
            }
        }
    }
}