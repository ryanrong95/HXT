﻿using Kdn.Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yahv.Utils.Serializers;

namespace Kdn.Library
{
    /**
    *
    * 快递鸟电子面单接口
    *
	* @技术QQ群: 340378554
    * @see: http://kdniao.com/api-eorder
    * @copyright: 深圳市快金数据技术服务有限公司
    * 
    * ID和Key请到官网申请：http://kdniao.com/reg
    */
    public class KdApiEOrder
    {
        public event RequestingEventHandler Requesting;
        public event ResponsedEventHandler Responsed;

        //电商ID
        private string EBusinessID = "1325757";
        //电商加密私钥，快递鸟提供，注意保管，不要泄漏
        private string AppKey = "96c5b231-7b0a-4839-885a-905cc82636a2";
        //请求url
        //正式环境地址
        private string ReqURL = "https://api.kdniao.com/api/Eorderservice";
        //测试环境地址
        //private string ReqURL = "http://testapi.kdniao.com:8081/api/EOrderService";

        public KdApiEOrder()
        {

        }


        /// <summary>
        /// Json方式  电子面单
        /// </summary>
        /// <returns></returns>
        public KdnResult orderTracesSubByJson(KdnRequest request)
        {
            //跨越的测试地址有问题,经过多方询问最后使用正式地址
          
            if (request.ShipperCode == ShipperCode.KYSY)
            {
                ReqURL = "https://api.kdniao.com/api/Eorderservice";
            }
            //this.ReqURL = request.ReqURL;

            if (this != null && this.Requesting != null)
            {
                this.Requesting(request, new RequestingEventArgs
                {
                    Request = request
                });
            }

            string json = request.Json();

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("RequestData", HttpUtility.UrlEncode((json), Encoding.UTF8));
            //param.Add("RequestData", HttpUtility.UrlEncode(Encoding.ASCII.GetBytes(json)));
            //param.Add("RequestData", HttpUtility.HtmlEncode(json));
            param.Add("EBusinessID", EBusinessID);
            param.Add("RequestType", "1007");
            string dataSign = encrypt(json, AppKey, "UTF-8");
            param.Add("DataSign", HttpUtility.UrlEncode(dataSign, Encoding.UTF8));
            param.Add("DataType", "2");

            var result = sendPost(ReqURL, param).JsonTo<KdnResult>();

            if (this != null && this.Responsed != null)
            {
                this.Responsed(request, new ResponsedEventArgs
                {
                    Request = request,
                    Result = result
                });
            }

            return result;
        }

        /// <summary>
        /// Post方式提交数据，返回网页的源代码
        /// </summary>
        /// <param name="url">发送请求的 URL</param>
        /// <param name="param">请求的参数集合</param>
        /// <returns>远程资源的响应结果</returns>
        private string sendPost(string url, Dictionary<string, string> param)
        {
            string result = "";
            StringBuilder postData = new StringBuilder();
            if (param != null && param.Count > 0)
            {
                foreach (var p in param)
                {
                    if (postData.Length > 0)
                    {
                        postData.Append("&");
                    }
                    postData.Append(p.Key);
                    postData.Append("=");
                    postData.Append(p.Value);
                }
            }
            byte[] byteData = Encoding.GetEncoding("UTF-8").GetBytes(postData.ToString());
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = url;
                request.Accept = "*/*";
                request.Timeout = 30 * 1000;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Method = "POST";
                request.ContentLength = byteData.Length;

                Stream stream = request.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream backStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(backStream, Encoding.GetEncoding("UTF-8"));
                result = sr.ReadToEnd();
                sr.Close();
                backStream.Close();
                response.Close();
                request.Abort();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        ///电商Sign签名
        ///</summary>
        ///<param name="content">内容</param>
        ///<param name="keyValue">Appkey</param>
        ///<param name="charset">URL编码 </param>
        ///<returns>DataSign签名</returns>
        private string encrypt(string content, string keyValue, string charset)
        {
            if (keyValue != null)
            {
                return base64(MD5(content + keyValue, charset), charset);
            }
            return base64(MD5(content, charset), charset);
        }

        ///<summary>
        /// 字符串MD5加密
        ///</summary>
        ///<param name="str">要加密的字符串</param>
        ///<param name="charset">编码方式</param>
        ///<returns>密文</returns>
        private string MD5(string str, string charset)
        {
            byte[] buffer = System.Text.Encoding.GetEncoding(charset).GetBytes(str);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider check;
                check = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] somme = check.ComputeHash(buffer);
                string ret = "";
                foreach (byte a in somme)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("X");
                    else
                        ret += a.ToString("X");
                }
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="charset">编码方式</param>
        /// <returns></returns>
        private string base64(string str, string charset)
        {
            return Convert.ToBase64String(System.Text.Encoding.GetEncoding(charset).GetBytes(str));
        }
    }
}
