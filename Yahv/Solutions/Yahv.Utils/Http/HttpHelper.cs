using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Collections;
using Yahv.Utils.Serializers;

namespace Yahv.Utils.Http
{
    /// <summary>
    /// WebClient 请求帮助类
    /// </summary>
    public static class HttpHelper
    {
        #region
        /// <summary>
        /// Post请求题数据格式
        /// </summary>
        public struct ContentType
        {
            /// <summary>
            /// 文件上传
            /// </summary>
            public const string formData = "multipart/form-data";
            /// <summary>
            /// 键值对数据
            /// </summary>
            public const string wwwFormUrlencoded = "application/x-www-form-urlencoded";
            /// <summary>
            /// 一般字符串
            /// </summary>
            public const string textPlain = "text/plain";
            /// <summary>
            /// json格式的字符串
            /// </summary>
            public const string json = "application/json";
            /// <summary>
            /// javascript
            /// </summary>
            public const string javasCript = "application/javascript";
        }
        /// <summary>
        /// http请求指定返回数据格式
        /// </summary>
        public struct Accept
        {
            /// <summary>
            /// json格式
            /// </summary>
            public const string json = "application/json";
            /// <summary>
            /// xml格式
            /// </summary>
            public const string xml = "application/xml";
            /// <summary>
            /// html格式
            /// </summary>
            public const string html = "text/html";
            /// <summary>
            /// 普通字符串
            /// </summary>
            public const string plain = "text/plain";
        }

        public sealed class Response
        {
            internal Response(string data, CookieCollection cookie)
            {
                this.Data = data;

                //Http

                this.Cookie = cookie;
            }
            public string Data { get; }
            public CookieCollection Cookie { get; }
        }

        public sealed class Cookie
        {
            /// <param name="name">名称</param>
            /// <param name="value">值</param>
            public Cookie(string name, string value) { this.Name = name; this.Value = value; }
            /// <param name="name">名称</param>
            /// <param name="value">值</param>
            /// <param name="path">默认为/</param>
            public Cookie(string name, string value, string path) : this(name, value) { this.path = path; }

            /// <param name="name">名称</param>
            /// <param name="value">值</param>
            /// <param name="path">默认为/</param>
            /// <param name="domain">必须写对,级别等于或高于请求url的域名,如:请求的域abc.test.com,可以为abc.test.com或test.com</param>
            public Cookie(string name, string value, string path, string domain) : this(name, value, path) { this.Domain = domain; }
            public string Name { get; }
            public string Value { get; }

            string path = "/";
            internal string Path { get { return path; } }
            internal string Domain { get; }
        }
        public sealed class CookieCollection : IEnumerable<Cookie>
        {
            SortedList<string, Cookie> list = new SortedList<string, Cookie>();

            public CookieCollection()
            {

            }
            public CookieCollection(IEnumerable<Cookie> ienums)
            {
                foreach (var item in ienums)
                {
                    this.Add(item); ;
                }
            }
            public IEnumerator<Cookie> GetEnumerator()
            {
                return this.list.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            public void Add(Cookie item)
            {
                this.list[item.Name] = item;
            }
            public void Add(string name, string value)
            {
                this.list[name] = new Cookie(name, value);
            }
            public Cookie this[string key]
            {
                get { return this.FirstOrDefault(item => item.Name == key); }
            }
        }
        #endregion

        #region 基本http请求
        public static void DownloadFile(string url, string fullPath)
        {
            using (var wc = new WebClient { })
            {
                wc.DownloadFile(url, fullPath);
            }
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="encoding">请求和相应的数据编码格式</param>
        /// <param name="accept">指定的返回数据格式</param>
        /// <returns></returns>
        public static string Get(string url, Encoding encoding, string accept = Accept.plain)
        {
            try
            {
                using (var wc = new WebClient { Encoding = encoding })
                {

                    //string html = wc.DownloadString(url);

                    wc.Headers.Add("Accept", accept);
                    using (var readStream = wc.OpenRead(url))
                    using (var sr = new StreamReader(readStream, encoding))
                    {
                        var result = sr.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (System.Web.HttpException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 键值对方式发送Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">name value集合的数据</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="accept">指定的返回数据格式</param>
        /// <returns></returns>
        public static string Post(string url, IEnumerable<KeyValuePair<string, string>> data, Encoding encoding, string accept = Accept.plain)
        {
            using (var client = new WebClient() { Encoding = encoding })
            {
                client.Headers.Add("Accept", accept);
                NameValueCollection nvs = new NameValueCollection();
                foreach (var prop in data)
                {
                    nvs.Add(prop.Key, prop.Value);
                }
                return Encoding.UTF8.GetString(client.UploadValues(url, "POST", nvs));
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="paramData">在请求体中要发送的字符</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="contentType">请求体数据</param>
        /// <param name="accept">接收数据格式</param>
        /// <returns></returns>
        public static string Post(string url, string paramData, Encoding encoding, string contentType = ContentType.wwwFormUrlencoded, string accept = Accept.plain)
        {
            try
            {
                if (url.ToLower().IndexOf("https", System.StringComparison.Ordinal) > -1)
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                                   new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; });
                }

                var wc = new WebClient() { Encoding = encoding };
                wc.Headers.Add("Content-Type", contentType);
                wc.Headers.Add("Accept", accept);
                return wc.UploadString(url, "POST", paramData);
            }
            catch (System.Web.HttpException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// UTF8编码的Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="accept">指定的返回数据格式</param>
        /// <returns></returns>
        public static string Get(string url, string accept = Accept.plain)
        {
            return Get(url, Encoding.UTF8, accept);
        }
        /// <summary>
        /// 键值对方式发送Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data"></param>
        /// <param name="accept">指定的返回数据格式</param>
        /// <returns></returns>
        public static string Post(string url, IEnumerable<KeyValuePair<string, string>> data, string accept = Accept.plain)
        {
            return Post(url, data, Encoding.UTF8, accept);
        }
        /// <summary>
        /// UTF8编码的Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramData"></param>
        /// <param name="contentType"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public static string Post(string url, string paramData, string contentType = ContentType.wwwFormUrlencoded, string accept = Accept.plain)
        {
            return Post(url, paramData, Encoding.UTF8, contentType, accept);
        }
        #endregion

        #region 带cookie的http请求
        static void SetCookie(this HttpWebRequest request, CookieCollection cookies)
        {
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                foreach (var item in cookies)
                {
                    request.CookieContainer.Add(new System.Net.Cookie(item.Name, item.Value, item.Path, item.Domain ?? request.RequestUri.Host));
                }
            }
        }
        static Response GetResponse(this HttpWebRequest request, Encoding encoding)
        {
            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                using (var instream = response.GetResponseStream())
                {
                    byte[] buffer;
                    StringBuilder txt = new StringBuilder();
                    int index = 0;
                    do
                    {
                        buffer = new byte[64];
                        index = instream.Read(buffer, 0, buffer.Length);
                        txt.Append(encoding.GetString(buffer).TrimEnd('\0'));
                    } while (index > 0);
                    return new Response(txt.ToString(), new CookieCollection(response.Cookies.Cast<System.Net.Cookie>().Select(item => new Cookie(item.Name, item.Value))));
                }
            }
            catch (System.Net.HttpListenerException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Response Get(string url, CookieCollection cookies, Encoding encoding, string accept = Accept.plain)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.AllowAutoRedirect = true;
            request.Accept = accept;
            request.SetCookie(cookies);
            return request.GetResponse(encoding);
        }
        public static Response Post(string url, string data, CookieCollection cookies, Encoding encoding, string contentTepe = ContentType.wwwFormUrlencoded, string accept = Accept.plain)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.AllowAutoRedirect = true;
            request.ContentType = contentTepe;
            request.Accept = accept;
            request.SetCookie(cookies);
            if (!string.IsNullOrWhiteSpace(data))
            {
                var bytes = encoding.GetBytes(data);
                request.ContentLength = bytes.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                request.ContentLength = 0;
            }
            return request.GetResponse(encoding);
        }

        public static Response Get(string url, CookieCollection cookie, string accept = Accept.plain)
        {
            return Get(url, cookie, Encoding.UTF8, accept);
        }
        public static Response Post(string url, string data, CookieCollection cookie, string contentTepe = ContentType.wwwFormUrlencoded, string accept = Accept.plain)
        {
            return Post(url, data, cookie, Encoding.UTF8, ContentType.wwwFormUrlencoded, Accept.plain);
        }
        #endregion

        #region 发送请求并将返回数据反序列化为指定类型
        static T DeSerializer<T>(this string str, string accept) where T : class
        {
            try
            {
                T t;
                if (accept == Accept.xml)
                {
                    t = str.XmlTo<T>();
                }
                else
                {
                    t = str.JsonTo<T>();
                }
                return t;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static T Get<T>(string url, Encoding encoding, string accept = Accept.json) where T : class
        {

            var luc = Get(url, encoding, accept);
            //var type = typeof(T);
            //Console.WriteLine(type.FullName);
            return luc.DeSerializer<T>(accept);
        }

        public static T Post<T>(string url, string paramData, Encoding encoding, string contentType = ContentType.json, string accept = Accept.json) where T : class
        {
            return Post(url, paramData, encoding, contentType, accept).DeSerializer<T>(accept);
        }

        public static T Get<T>(string url, string accept = Accept.json) where T : class
        {
            return Get<T>(url, Encoding.UTF8, accept);
        }

        public static T Post<T>(string url, string paramData, string contentType = ContentType.json, string accept = Accept.json) where T : class
        {
            return Post<T>(url, paramData, Encoding.UTF8, contentType, accept);
        }
        #endregion


      

    }
}
