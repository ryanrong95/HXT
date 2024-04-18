using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WebApp.App_Utils
{
    public class HttpGet
    {

        public HttpResponseMessage HttpClient(string url, Dictionary<string, string> Headers)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                responseMessage = httpClient.GetAsync(new Uri(url)).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                httpClient.Dispose();
            }
            return responseMessage;
        }



        /// <summary>
        /// 通用请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CommonHttpRequest(string url, string type, string data = "", string authorization = "")
        {
            HttpWebRequest myRequest = null;
            Stream outstream = null;
            HttpWebResponse myResponse = null;
            StreamReader reader = null;
            try
            {
                //构造http请求的对象
                myRequest = (HttpWebRequest)WebRequest.Create(url);


                //设置
                myRequest.ProtocolVersion = HttpVersion.Version10;
                myRequest.Method = type;

                if (!string.IsNullOrEmpty(authorization))
                {
                    myRequest.Headers["Authorization"] = authorization;
                }

                if (data.Trim() != "")
                {
                    myRequest.ContentType = "text/xml";
                    myRequest.ContentLength = data.Length;
                    myRequest.Headers.Add("data", data);

                    //转成网络流
                    byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);

                    outstream = myRequest.GetRequestStream();
                    outstream.Flush();
                    outstream.Write(buf, 0, buf.Length);
                    outstream.Flush();
                    outstream.Close();
                }
                // 获得接口返回值
                myResponse = (HttpWebResponse)myRequest.GetResponse();
                reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string ReturnXml = reader.ReadToEnd();
                reader.Close();
                myResponse.Close();
                myRequest.Abort();
                return ReturnXml;
            }
            catch (Exception)
            {
                // throw new Exception();
                if (outstream != null) outstream.Close();
                if (reader != null) reader.Close();
                if (myResponse != null) myResponse.Close();
                if (myRequest != null) myRequest.Abort();
                return "";
            }
        }
    }
}