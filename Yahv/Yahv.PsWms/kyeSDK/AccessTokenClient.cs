using kyeSDK.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace kyeSDK
{
    public class AccessTokenClient
    {

        /// <summary>
        /// 获取TOKEN  默认是生产环境，沙盒环境请传false
        /// </summary>
        /// <param name="kyeAppInfo">请求参数</param>
        /// <param name="isPro">是否生产环境  默认是生产环境</param>
        /// <returns></returns>
        public AccessTokenKyeResponse AccessToken(KyeAppInfo kyeAppInfo, bool isPro = true)
        {
            if (isPro)
            {
                return execute("https://open.ky-express.com/security/token", kyeAppInfo);
            }
            else
            {
                return execute("https://open.ky-express.com/security/sandbox/accessToken", kyeAppInfo);
            }
            
        }


        private AccessTokenKyeResponse execute(string url, KyeAppInfo kyeAppInfo)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(kyeAppInfo.appkey))
                    return new AccessTokenKyeResponse { code = 37998, msg = "appKey is null", data = null, success = false };
                if (string.IsNullOrWhiteSpace(kyeAppInfo.appsecret))
                    return new AccessTokenKyeResponse { code = 37998, msg = "appSecret is null", data = null, success = false };

                string responseString = "";
                Dictionary<String, Object> requestParameters = new Dictionary<String, Object>();
                requestParameters.Add("appkey", kyeAppInfo.appkey);
                requestParameters.Add("appsecret", kyeAppInfo.appsecret);
                //取token
                HttpWebRequest reqtoken = (HttpWebRequest)WebRequest.Create(url);
                byte[] postBytestoken = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestParameters));
                reqtoken.Method = "POST";
                reqtoken.ContentType = "application/json";
                reqtoken.Headers.Add("X-from", "openapi_app");
                using (Stream reqStream = reqtoken.GetRequestStream())
                {
                    reqStream.Write(postBytestoken, 0, postBytestoken.Length);
                }
                using (HttpWebResponse myResponse = (HttpWebResponse)reqtoken.GetResponse())
                {
                    responseString = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                }

                return JsonConvert.DeserializeObject<AccessTokenKyeResponse>(responseString);

            }
            catch (Exception)
            {
                return new AccessTokenKyeResponse { code = 10010, msg = "业务异常", data = null, success = false };
            }
        }


    }
}
