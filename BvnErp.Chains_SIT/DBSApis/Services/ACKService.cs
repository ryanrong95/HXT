using DBSApis.Models;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Services
{
    public class ACKService
    {
        public string EncryptedMsg { get; set; }

        public ACKService(string encryptedMsg)
        {
            this.EncryptedMsg = encryptedMsg;
        }

        public void DecryptMsg()
        {
            try
            {
                KeyConfig Config = ApiService.Current.KeyConfigs;
                DecryptMessage decryptMessage = new DecryptMessage(Config, this.EncryptedMsg);
                string decryptedMsg = decryptMessage.Decrypt();

                ACK2Response response = JsonConvert.DeserializeObject<ACK2Response>(decryptedMsg);
                if (response.txnResponses.FirstOrDefault().responseType == DBSConstConfig.DBSConstConfiguration.ResponseTypeACK2)
                {
                    ACK2Service aCK2Service = new ACK2Service(response);
                    aCK2Service.Handle();
                }
                else
                {
                    ACK3Response responseAck3 = JsonConvert.DeserializeObject<ACK3Response>(decryptedMsg);
                    ACK3Service aCK3Service = new ACK3Service(responseAck3);
                    aCK3Service.Handle();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}