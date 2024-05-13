using kyeSDK.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace kyeSDK
{
    public class DefaultKyeClient
    {
        private string appKey;
        private string appSecret;
        private string accessToken;
        private int timeout;
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="appKey">应用key</param>
        /// <param name="appSecret">秘钥</param>
        /// <param name="accessToken">授权令牌</param>
        /// <param name="timeout">timeout</param>
        public DefaultKyeClient(string appKey, string appSecret, int timeout = 15000)
        {
            this.appKey = appKey;
            this.appSecret = appSecret;
            this.timeout = timeout;
        }

        /// <summary>
        /// 执行方法 默认是生产环境，沙盒环境请传false
        /// </summary>
        /// <param name="kyeRequest">请求业务参数</param>
        /// <param name="isPro">是否生产环境  默认是生产环境</param>
        /// <returns></returns>
        public string Execute(KyeRequest kyeRequest,bool isPro = true)
        {
            if (string.IsNullOrEmpty(this.accessToken))
            {
                AccessTokenClient accessTokenClient = new AccessTokenClient();
                AccessTokenKyeResponse tokenKyeResponse = accessTokenClient.AccessToken(new kyeSDK.model.KyeAppInfo { appkey = this.appKey, appsecret = this.appSecret }, isPro);
                if (tokenKyeResponse.success)
                {
                    this.accessToken = tokenKyeResponse.data.token;
                }
                else
                {
                    return JsonConvert.SerializeObject(tokenKyeResponse);
                }
            }

            if (isPro)
            {
                return PostData(kyeRequest, "https://open.ky-express.com/router/rest");
            }
            else
            {
                return PostData(kyeRequest, "https://open.ky-express.com/sandbox/router/rest");
            }
            
        }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="kyeRequest">请求参数</param>
        /// <param name="postUrl">请求URL</param>
        /// <returns></returns>
        private string PostData(KyeRequest kyeRequest, string postUrl)
        {
            //获取TOKEN
            try
            {
                if (string.IsNullOrWhiteSpace(appKey))
                    return "{\"code\":37998,\"msg\":\"appKey is null\",\"traceId\":\"\"}";
                if (string.IsNullOrWhiteSpace(appSecret))
                    return "{\"code\":37998,\"msg\":\"appSecret is null\",\"traceId\":\"\"}";
                if (string.IsNullOrWhiteSpace(accessToken))
                    return "{\"code\":37998,\"msg\":\"accessToken is null\",\"traceId\":\"\"}";

                kyeRequest.paramInfo = kyeRequest.paramInfo.Replace("\n", "").Replace("\t", "").Replace("\r", "");
                //获取签名
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
                string sign = Md5(this.appSecret + t.ToString() + kyeRequest.paramInfo);

            //POST
            ReTryToken:
                string responseString = "";
                HttpWebRequest reqtoken = (HttpWebRequest)WebRequest.Create(postUrl);
                byte[] postBytes = Encoding.UTF8.GetBytes(kyeRequest.paramInfo);
                reqtoken.Timeout = this.timeout;
                reqtoken.Method = "POST";
                reqtoken.ContentType = kyeRequest.contentType;
                reqtoken.Headers.Add("X-from", "openapi_app");
                reqtoken.Headers.Add("token", this.accessToken);
                reqtoken.Headers.Add("sign", sign);
                reqtoken.Headers.Add("appkey", this.appKey);
                reqtoken.Headers.Add("method", kyeRequest.methodCode);
                reqtoken.Headers.Add("timestamp", t.ToString());
                reqtoken.Headers.Add("format", kyeRequest.responseFormat);

                using (Stream reqStream = reqtoken.GetRequestStream())
                {
                    reqStream.Write(postBytes, 0, postBytes.Length);
                }
                using (HttpWebResponse myResponse = (HttpWebResponse)reqtoken.GetResponse())
                {
                    responseString = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                }

                if (!string.IsNullOrWhiteSpace(responseString) && responseString.Contains("无效的token"))
                {
                    if (ReTryToken(postUrl))
                    {
                        goto ReTryToken;
                    }
                    else
                    {
                        return "{\"code\":10010,\"msg\":\"获取token失败\",\"traceId\":\"\"}";
                    }
                }
                return responseString;
            }
            catch (Exception)
            {
                return "{\"code\":10010,\"msg\":\"业务异常\",\"traceId\":\"\"}";
            }
        }


        /// <summary>
        /// MD5加密 大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string Md5(string str)
        {
            string req = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            for (int i = 0; i < s.Length; i++)
            {
                req = req + s[i].ToString("X2");
            }
            return req;
        }

        /// <summary>
        /// 重新获取Token
        /// </summary>
        /// <param name="url"></param>
        private bool ReTryToken(string url)
        {
            AccessTokenClient kyeAccessTokenClient = new AccessTokenClient();
            AccessTokenKyeResponse accessTokenKyeResponse = null;
            if (url.Contains("sandbox"))
            {
                accessTokenKyeResponse = kyeAccessTokenClient.AccessToken(new KyeAppInfo { appkey = this.appKey, appsecret = this.appSecret },false);
            }
            else
            {
                accessTokenKyeResponse = kyeAccessTokenClient.AccessToken(new KyeAppInfo { appkey = this.appKey, appsecret = this.appSecret });
            }
            if (accessTokenKyeResponse.success)
            {
                this.accessToken = accessTokenKyeResponse.data.token;
                return true;
            }
            return false;
        }

    }
}
