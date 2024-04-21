using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.IO;
using System.Net.Http;
using Yahv.Utils.Http;

namespace Yahv.Csrm.WebApp
{
    public static class HttpClientHelp
    {

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="requestType">请求方式：GET/POST</param>
        /// <param name="url">请求地址</param>
        /// <param name="body">请求体，默认为空值</param>
        /// <returns></returns>
        //public HttpResponseMessage HttpClient(string requestType,string url, string body="")
        //{
        //    HttpClient httpClient = new HttpClient();
        //    HttpResponseMessage responseMessage = new HttpResponseMessage();

        //    try
        //    {

        //        switch (requestType.ToUpper())
        //        {
        //            case "POST":
        //                HttpContent httpContent = new StringContent(body);
        //                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //                responseMessage = httpClient.PostAsync(url, httpContent).Result;
        //                break;
        //            case "GET":
        //                httpClient.DefaultRequestHeaders.Accept.Clear();
        //                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
        //                responseMessage = httpClient.GetAsync(new Uri(url)).Result;
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        httpClient.Dispose();
        //    }
        //    return responseMessage;
        //}
        ///
        public static string HttpPostRaw(string url, string data)
        {
            string value = "";
            try
            {
                HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
                reqest.Method = "POST";
                reqest.ContentType = "application/json";
                Stream stream = reqest.GetRequestStream();
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(data);
                stream.Write(bs, 0, bs.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)reqest.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                value = sr.ReadToEnd();
                response.Close();
                return value;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        /// <summary>
        /// 通用请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CommonHttpRequest(string url, string type, string data = "")
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
