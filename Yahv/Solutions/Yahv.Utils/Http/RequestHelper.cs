using System;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Spire.Pdf.Exporting.XPS.Schema;
using System.Collections.Generic;

namespace Yahv.Utils.Http
{

    /// <summary>
    /// WEB请求帮助类
    /// </summary>
    static public class RequestHelper
    {

        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public string HttpGet(string url)
        {
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                //string err = ex.Message;
                //return string.Empty;
                throw ex;
            }
        }

        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        static public string HttpGet(string url, string ContentType)
        {
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "GET";
                request.ContentType = ContentType;
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                //string err = ex.Message;
                //return string.Empty;
                throw ex;
            }
        }

        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">Json格式的数据</param>
        /// <returns></returns>
        static public string HttpPost(string url, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                //string err = ex.Message;
                //return string.Empty;
                throw ex;
            }
        }

        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">Json格式的数据</param>
        /// <param name="ContentType">获取或设置 Content-type HTTP 标头的值，如"application/json"或 "application/x-www-form-urlencoded"</param>
        /// <returns></returns>
        static public string HttpPost(string url, string postData, string ContentType)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = ContentType;
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                //string err = ex.Message;
                //return string.Empty;
                throw ex;
            }
        }


        static public T PostUrl<T>(string url, string postData) where T : class
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ServicePoint.Expect100Continue = false;

            //req.TimeOut = "800";//设置请求超时时间，单位为毫秒

            req.ContentType = "application/json";
            byte[] data = Encoding.UTF8.GetBytes(postData);
            req.ContentLength = data.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();

            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
