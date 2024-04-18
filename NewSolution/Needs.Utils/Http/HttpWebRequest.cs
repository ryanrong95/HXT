using System;
using System.Net;
using System.Text;

namespace Needs.Utils
{
    public partial class HttpRequest
    {
        /// <summary>
        /// 获取或设置请求超时时间
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 获取或设置 Content-type HTTP 标头的值
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 获取相应代码
        /// </summary>
        public HttpStatusCode ResponseStatusCode { get; private set; }

        /// <summary>
        /// 指定构成 HTTP 标头的名称/值对的集合
        /// </summary>
        public WebHeaderCollection Headers { get; set; }

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
                // 创建一个HTTP请求
                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "GET";
                request.Timeout = this.Timeout;
                request.Headers = this.Headers;

                //获取请求
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    this.ResponseStatusCode = response.StatusCode;

                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        result = streamReader.ReadToEnd();
                        streamReader.Close();
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
                // 创建一个HTTP请求
                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "POST";
                request.Timeout = this.Timeout;
                request.ContentType = this.ContentType;
                request.Headers = this.Headers;

                if (msg != null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    request.ContentLength = data.Length;
                    using (System.IO.Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
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
                }

                request.Abort();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public string Put(string url, string msg)
        {
            string result = "";
            try
            {
                // 创建一个HTTP请求
                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "PUT";
                request.Timeout = this.Timeout;
                request.ContentType = this.ContentType;
                request.Headers = this.Headers;

                if (msg != null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    request.ContentLength = data.Length;
                    using (System.IO.Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
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
                }

                request.Abort();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public string Delete(string url)
        {
            string result = "";
            try
            {
                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "DELETE";
                request.Timeout = this.Timeout;
                request.ContentType = this.ContentType;
                request.Headers = this.Headers;

                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        result = streamReader.ReadToEnd();
                        streamReader.Close();
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
