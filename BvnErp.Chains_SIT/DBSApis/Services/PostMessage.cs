using DBSApis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DBSApis.Services
{
    public class PostMessage
    {
        public KeyConfig Config { get; set; }
        public string Url { get; set; }
        public string EncryptedMessage { get; set; }

        public PostMessage(string url,string encryptedMessage, KeyConfig keyConfig)
        {
            this.Url = url;
            this.EncryptedMessage = encryptedMessage;
            this.Config = keyConfig;
        }
 
        public string PushMsg()
        {
            try
            {
                Models.HttpRequest request = new Models.HttpRequest();
                request.Timeout = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.TimeOut;
                request.ContentType = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.ContentType;

                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("x-api-key", Config.KeyId);
                headers.Add("X-DBS-ORG_ID", Config.OrgId);
                request.Headers = headers;

                return request.Post(this.Url, this.EncryptedMessage);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
                //return ConstConfig.ConstError.Error003;
            }
            
        }
    }
}