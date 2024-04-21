using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvRoute.Services.Models;
using Yahv.Utils.Serializers;

namespace Yahv.PvRoute.Services
{
    public class SFHelper
    {

        public void CreateFaceOrderLog(string faceOrderID)
        {
            string partnerID = "YDCXKg";//此处替换为您在丰桥平台获取的顾客编码          
            string checkword = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav";//此处替换为您在丰桥平台获取的校验码     
            string reqURL = "https://sfapi-sbox.sf-express.com/std/service";
            //string reqURL = "https://sfapi.sf-express.com/std/service"; //生产环境

            string serviceCode = "EXP_RECE_SEARCH_ROUTES";//路由查询-通过运单号 或 通过订单号，采用通过运单号搜索

            var sendJson = new
            {
                language = 0,//0：中文 1：英文 2：繁体
                trackingType=1,
                trackingNumber = new[]
                {
                   faceOrderID 
                },
                methodType=1
            }.Json();

            string msgData = JsonCompress(sendJson);
            string timestamp = GetTimeStamp(); //获取时间戳       

            string requestID = System.Guid.NewGuid().ToString(); //获取uuid

            string msgDigest = MD5ToBase64string(UrlEncode(msgData + timestamp + checkword));//获取数字签名

            string respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

            if (respJson != null)
            {
                try
                {
                    JObject rlt = respJson.JsonTo<JObject>();

                    var apiResultCode = rlt["apiResultCode"].Value<string>();
                    JToken apiResultData = rlt["apiResultData"];

                    apiResultData resultData = apiResultData.ToString().JsonTo<apiResultData>();

                    if (apiResultCode == "A1000" && resultData.success == true)
                    {
                        var message = resultData.msgData;
                        var routeResps = message.routeResps;
                        foreach (var resp in routeResps)
                        {
                            var mailNo = resp.mailNo;
                            if (mailNo != faceOrderID)
                            {
                                throw new Exception("不可思议的错误！！");
                            }
                            var routes = resp.routes;
                            foreach (var route in routes)
                            {
                                //请求Logs_Transport的Enter方法，注意要去重
                                new Logs_Transport()
                                {
                                    FaceOrderID = faceOrderID,
                                    Summary = route.remark,
                                    Json = msgData
                                }.Enter();

                            }

                        }


                        //    var message = rlt["msgData"];
                        //    foreach (var data in message)
                        //    {
                        //        var routeResps = data["routeResps"];

                        //        foreach (var rep in routeResps)
                        //        {
                        //            var mailNo = rep["mailNo"].Value<string>();
                        //            if (mailNo != faceOrderID)
                        //            {
                        //                throw new Exception("不可思议的错误！！");
                        //            }

                        //            var routes = rep["routes"];
                        //            foreach (var route in routes)
                        //            {
                        //                //请求Logs_Transport的Enter方法，注意要去重
                        //                new Logs_Transport()
                        //                {
                        //                    FaceOrderID = faceOrderID,
                        //                    Summary = route["remark"].Value<string>(),
                        //                    Json = msgData
                        //                }.Enter();

                        //            }

                        //    }

                        //}
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        #region 顺丰辅助方法
        private static string JsonCompress(string msgData)
        {
            StringBuilder sb = new StringBuilder();
            using (StringReader reader = new StringReader(msgData))
            {
                int ch = -1;
                int lastch = -1;
                bool isQuoteStart = false;
                while ((ch = reader.Read()) > -1)
                {
                    if ((char)lastch != '\\' && (char)ch == '\"')
                    {
                        if (!isQuoteStart)
                        {
                            isQuoteStart = true;
                        }
                        else
                        {
                            isQuoteStart = false;
                        }
                    }
                    if (!Char.IsWhiteSpace((char)ch) || isQuoteStart)
                    {
                        sb.Append((char)ch);
                    }
                    lastch = ch;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 请求顺丰接口
        /// </summary>
        /// <param name="reqURL">url地址</param>
        /// <param name="partnerID">顾客编码</param>
        /// <param name="requestID">uuid</param>
        /// <param name="serviceCode">确定哪个接口</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="msgDigest">数字签名</param>
        /// <param name="msgData">请求内容</param>
        /// <returns></returns>
        private static string callSfExpressServiceByCSIM(string reqURL, string partnerID, string requestID, string serviceCode, string timestamp, string msgDigest, string msgData)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqURL);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            Dictionary<string, string> content = new Dictionary<string, string>();
            content["partnerID"] = partnerID;
            content["requestID"] = requestID;
            content["serviceCode"] = serviceCode;
            content["timestamp"] = timestamp;
            content["msgData"] = msgData;
            content["msgDigest"] = msgDigest;

            if (!(content == null || content.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in content.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, content[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, content[key]);
                    }
                    i++;
                }

                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }

            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private static string UrlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (System.Web.HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(System.Web.HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        private static string MD5ToBase64string(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] MD5 = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));//MD5(注意UTF8编码)
            string result = Convert.ToBase64String(MD5);//Base64
            return result;
        }

        private static string Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);

            StringBuilder builder = new StringBuilder();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                builder.Append(line);
            }
            return builder.ToString();
        }

        private static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        #endregion

    }

    public class apiResultData
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 错误编号
        /// </summary>
        public string errorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errorMsg { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public msgData msgData { get; set; }
    }

    public class msgData
    {
        public List<routeResps> routeResps { get; set; }
    }

    public class routeResps
    {
        public string mailNo { get; set; }
        public List<routes> routes { get; set; }
    }

    public class routes
    {
        public DateTime acceptTime { get; set; }
        public string acceptAddress { get; set; }
        public string remark { get; set; }
        public string opCode { get; set; }
    }
}
