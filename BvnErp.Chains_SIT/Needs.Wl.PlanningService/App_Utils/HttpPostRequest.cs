using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class HttpPostRequest
    {
        public int Timeout { get; set; }

        public string ContentType { get; set; }

        public int ResponseStatus { get; private set; }

        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Http Get 请求
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <returns></returns>
        public string Get(string url)
        {
            string result = "";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                                       | SecurityProtocolType.Tls
                                       | SecurityProtocolType.Tls11
                                       | SecurityProtocolType.Tls12;
                // 创建一个HTTP请求
                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "GET";

                //设置超时时间
                request.Timeout = this.Timeout;

                if (this.Headers != null)
                {
                    foreach (var item in this.Headers)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }

                //获取请求
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        result = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                    else
                    {
                        // requeststatus = false;
                    }
                }

                request.Abort();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }


        /// <summary>
        /// Http Post 请求
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="msg">推送报文</param>
        /// <returns></returns>
        public string Post(string url, string msg)
        {
            string result = "";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                                       | SecurityProtocolType.Tls
                                       | SecurityProtocolType.Tls11
                                       | SecurityProtocolType.Tls12;

                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "POST";

                request.Timeout = this.Timeout;
                request.ContentType = this.ContentType;

                if (this.Headers != null)
                {
                    foreach (var item in this.Headers)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }

                if (msg != null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    request.ContentLength = data.Length;
                    using (System.IO.Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
                    }
                }

                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        result = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                    else
                    {

                    }
                }

                request.Abort();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }
    }
}
