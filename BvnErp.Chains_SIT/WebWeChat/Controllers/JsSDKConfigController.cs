using Needs.Wl.Web.WeChat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebWeChat.Models;

namespace WebWeChat.Controllers
{
    [UserAuthorize(UserAuthorize = false)]
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class JsSDKConfigController : UserController
    {
        private string _accessTokenType = "accessToken";
        private string _jsapi_ticketType = "jsapi_ticket";

        public JsonResult GetWXConfig(GetWXConfigModel request)
        {
            string noncestr = Guid.NewGuid().ToString("N").Substring(0, 16);  //随机字符串
            //string timestamp = Convert.ToString(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());  //时间戳
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();

            string signature = GetSignature(noncestr, timestamp, request.CurrentUrl);

            var wxconfig = new
            {
                debug = ConfigurationManager.AppSettings["JSSDKDebug"] == "1",
                appId = ConfigurationManager.AppSettings["appID"],
                timestamp = timestamp,
                nonceStr = noncestr,
                signature = signature,
                jsApiList = new string[] { "scanQRCode" }
            };

            return Json(new { wxconfig = wxconfig, }, JsonRequestBehavior.AllowGet);
        }


        private string GetSignature(string noncestr, string timestamp, string currentUrl)
        {
            string jsapi_ticket = getDBTicket();
            if (jsapi_ticket == null)
            {
                string accessToken = getDBAccessToken();
                if (accessToken == null)
                {
                    accessToken = getNewAccessToken();
                }

                jsapi_ticket = getNewTicket(accessToken);
            }

            //string[] ArrTmp = {"jsapi_ticket","timestamp","nonce","url"}; 
            //Arrays.sort(ArrTmp); 
            //StringBuffer sf = new StringBuffer(); 
            //for(int i=0;i<ArrTmp.length;i++){ 
            //    sf.append(ArrTmp[i]); 
            //}

            // 将参数排序并拼接字符串  
            string str = "jsapi_ticket=" + jsapi_ticket + "&noncestr=" + noncestr + "&timestamp=" + timestamp + "&url=" + currentUrl;

            // 将字符串进行sha1加密
            string signature = SHA1(str);
            return signature;
        }

        /// <summary> 
        /// GET请求与获取结果 
        /// </summary> 
        private string HttpGet(string url, string postDataStr = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        private string getDBAccessToken()
        {
            var accessTokenModel = new Needs.Ccs.Services.Views.WxTokenView()
                .Where(t => t.Type == this._accessTokenType)
                .OrderByDescending(t => t.CreateTime)
                .FirstOrDefault();
            if (accessTokenModel == null)
            {
                return null;
            }
            if ((DateTime.Now - accessTokenModel.CreateTime).TotalSeconds > (7200 - 300))
            {
                return null;
            }
            return accessTokenModel.Value;
        }

        private string getDBTicket()
        {
            var accessTokenModel = new Needs.Ccs.Services.Views.WxTokenView()
                .Where(t => t.Type == this._jsapi_ticketType)
                .OrderByDescending(t => t.CreateTime)
                .FirstOrDefault();
            if (accessTokenModel == null)
            {
                return null;
            }
            if ((DateTime.Now - accessTokenModel.CreateTime).TotalSeconds > (7200 - 300))
            {
                return null;
            }
            return accessTokenModel.Value;
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        private string getNewAccessToken()
        {
            string grant_type = "client_credential";  //获取access_token填写client_credential
            string AppId = ConfigurationManager.AppSettings["appID"];  //第三方用户唯一凭证
            string secret = ConfigurationManager.AppSettings["appsecret"];  //第三方用户唯一凭证密钥，即appsecret
            //这个url链接地址和参数皆不能变  
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=" + grant_type + "&appid=" + AppId + "&secret=" + secret;

            string responseString = HttpGet(url);

            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);
            string access_token = responseJson.access_token;

            (new Needs.Ccs.Services.Models.WxToken
            {
                ID = Guid.NewGuid().ToString("N"),
                Type = this._accessTokenType,
                Value = access_token,
                CreateTime = DateTime.Now,
            }).Insert();

            return access_token;
        }

        /// <summary>
        /// 获取Ticket
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        private string getNewTicket(string access_token)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + access_token + "&type=jsapi";//这个url链接和参数不能变  

            string responseString = HttpGet(url);

            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);
            string ticket = responseJson.ticket;

            (new Needs.Ccs.Services.Models.WxToken
            {
                ID = Guid.NewGuid().ToString("N"),
                Type = this._jsapi_ticketType,
                Value = ticket,
                CreateTime = DateTime.Now,
            }).Insert();

            return ticket;
        }

        /// <summary>
        /// sha1加密
        /// </summary>
        /// <param name="decript"></param>
        /// <returns></returns>
        public string SHA1(string content)
        {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[]
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(content);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");
            return hash;
        }
    }
}