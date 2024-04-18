using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services
{
    public class Commons
    {
        private static readonly HttpClient client = new HttpClient();

        readonly static public string UnifyApiUrl = ConfigurationManager.AppSettings["UnifyApiUrl"];
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

        public static string HttpPostRaw(string url, string data, string ContentType = "application/json")
        {
            string value = "";
            try
            {
                HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
                reqest.Method = "POST";
                reqest.ContentType = ContentType;
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

        public static string HttpGet(string url)
        {
            string value = "";
            try
            {
                HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
                reqest.Method = "GET";
                reqest.ContentType = "application/json";
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

        public static string ToUrlParams(Dictionary<string, object> map)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in map)
            {
                buff += pair.Key + "=" + pair.Value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }
    }
}
