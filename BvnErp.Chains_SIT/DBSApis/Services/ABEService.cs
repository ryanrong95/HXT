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
    public class ABEService
    {
        public ABERequest Request { get; set; }

        public ABEService(ABERequest request)
        {
            this.Request = request;
        }
        public ReturnType PostMsg()
        {
            ReturnType returnType = new ReturnType();

            try
            {
                KeyConfig Config = ApiService.Current.KeyConfigs;
                UrlConfig urlConfig = ApiService.Current.UrlConfigs;
                string msg = this.Request.Json();

                EncryptMessage encryptMessage = new EncryptMessage(Config, msg);
                string encryptedMessage = encryptMessage.Encrypt();

                PostMessage postMessage = new PostMessage(urlConfig.ApiServerUrl + urlConfig.ABEUrl, encryptedMessage, Config);
                string reponseMsg = postMessage.PushMsg();
            
               
                DecryptMessage decryptMessage = new DecryptMessage(Config, reponseMsg);
                string decryptedMsg = decryptMessage.Decrypt();
                   
                GatewayResponse response = JsonConvert.DeserializeObject<GatewayResponse>(decryptedMsg);
                if (response.error!=null&&response.error.status.Equals(DBSConstConfig.DBSConstError.DBSRJCT))
                {
                    returnType.IsSuccess = false;
                    returnType.Code = response.error.code;
                    returnType.Msg = response.error.description;

                    return returnType;
                }

                ABEFailResponse aBEFailResponse = JsonConvert.DeserializeObject<ABEFailResponse>(decryptedMsg);
                if (aBEFailResponse.accountBalResponse.enqStatus.Equals(DBSConstConfig.DBSConstError.DBSRJCT))
                {
                    returnType.IsSuccess = false;
                    returnType.Code = aBEFailResponse.accountBalResponse.enqRejectCode;
                    returnType.Msg = aBEFailResponse.accountBalResponse.enqStatusDescription;

                    return returnType;
                }

                ABESuccResponse aBESuccResponse = JsonConvert.DeserializeObject<ABESuccResponse>(decryptedMsg);
                returnType.IsSuccess = true;
                returnType.Data = aBESuccResponse;

                return returnType; 
            }
            catch(Exception ex)
            {
                returnType.IsSuccess = false;
                returnType.Msg = ex.ToString();
                return returnType;
            }
        }      
    }
}