using Needs.Utils.Serializers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Concurrent;

namespace Needs.Utils.Http
{
    /// <summary>
    /// Api 
    /// json
    /// </summary>
    public class ApiHelper
    {
        public Encoding DefaultEncoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns>指定返回数据类型</returns>
        public T Get<T>(string url, object data = null) where T : class
        {
            string context = Get(url, data);

            if (string.IsNullOrWhiteSpace(context))
            {
                return null;
            }

            if (typeof(T) == typeof(JObject))
            {
                return JObject.Parse(context) as T;
            }

            return context.JsonTo<T>();
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="data">请求参数</param>
        /// <returns>返回string</returns>
        public string Get(string url, object data = null)
        {
            if (data is JObject)
            {
                return "不支持类型:" + typeof(JObject).FullName;
            }

            if (data != null)
            {
                string[] values = url.Split('?');

                var queries = HttpUtility.ParseQueryString(values.ElementAtOrDefault(1) ?? "", this.DefaultEncoding);

                foreach (var property in data.GetType().GetProperties())
                {
                    var value = property.GetValue(data);
                    queries[property.Name] = value == null ? null : value.ToString();
                }

                if (queries.Count > 0)
                {
                    url = $"{values[0]}?{queries}";
                }
            }

            using (WebClient client = new WebClient { Encoding = this.DefaultEncoding })
            {
                if (!string.IsNullOrWhiteSpace(userAgent))
                {
                    client.Headers.Add("User-Agent", userAgent);
                }
                return client.DownloadString(url);
            }
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns>指定返回数据类型</returns>
        public T Post<T>(string url, object data = null)
        {
            return Post(url, data).JsonTo<T>();
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="data">请求参数</param>
        /// <returns>返回string</returns>
        public string Post(string url, object data = null)
        {
            return Post(url, "POST", data);
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="address">要接收字符串的资源的 URI</param>
        /// <param name="method">用于将字符串发送到资源的 HTTP 方法。如果为 null，则对于 http 默认值为 POST</param>
        /// <param name="data">要上载的字符串。</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        string Post(string address, string method, object data = null)
        {
            if (data is JObject)
            {
                return "不支持类型:" + typeof(JObject).FullName;
            }

            using (WebClient client = new WebClient { Encoding = this.DefaultEncoding })
            {
                if (!string.IsNullOrWhiteSpace(userAgent))
                {
                    client.Headers.Add("User-Agent", userAgent);
                }

                if (data == null)
                {
                    return client.UploadString(address, method, "");
                }

                var txts = data.GetType().GetProperties().Select(item => $"{item.Name}={item.GetValue(data)}");
                return client.UploadString(address, method, string.Join("&", txts));
            }
        }

        /// <summary>
        /// 请求Data
        /// </summary>
        /// <param name="address">要接收字符串的资源的 URI</param>
        /// <param name="method">用于将字符串发送到资源的 HTTP 方法。如果为 null，则对于 http 默认值为 POST</param>
        /// <param name="data">要上载的字符串。</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        public string PostData(string address, object data = null)
        {
            if (data is JObject)
            {
                return "不支持类型:" + typeof(JObject).FullName;
            }

            using (WebClient client = new WebClient { Encoding = this.DefaultEncoding })
            {
                if (!string.IsNullOrWhiteSpace(userAgent))
                {
                    client.Headers.Add("User-Agent", userAgent);
                }

                if (data == null)
                {
                    return client.UploadString(address, "POST", "");
                }

                return client.UploadString(address, "POST", data.Json());
            }
        }

        /// <summary>
        /// json请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string JPost(string url, object data = null)
        {
            return JPost(url, "POST", data);
        }

        /// <summary>
        /// json 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T JPost<T>(string url, object data = null)
        {
            return JPost(url, data).JsonTo<T>(); ;
        }

        /// <summary>
        /// json 请求
        /// </summary>
        /// <param name="address">要接收字符串的资源的 URI</param>
        /// <param name="method">用于将字符串发送到资源的 HTTP 方法。如果为 null，则对于 http 默认值为 POST</param>
        /// <param name="data">要上载的字符串。</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        string JPost(string address, string method, object data = null)
        {
            using (WebClient client = new WebClient { Encoding = this.DefaultEncoding })
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Accept", "application/json");

                if (!string.IsNullOrWhiteSpace(userAgent))
                {
                    client.Headers.Add("User-Agent", userAgent);
                }

                return client.UploadString(address, method, data.Json());
            }
        }

        //这里不应该如此开发
        string userAgent;
        ConcurrentDictionary<string, ApiHelper> concurrent;
        ApiHelper()
        {
            concurrent = new ConcurrentDictionary<string, ApiHelper>();
        }

        ApiHelper(string userAgent)
        {
            this.userAgent = userAgent;
        }

        /// <summary>
        /// 带有User-Agent索引器
        /// </summary>
        /// <param name="agent">User-Agent</param>
        /// <returns>帮助者</returns>
        public ApiHelper this[string agent]
        {
            get
            {
                return concurrent.GetOrAdd(agent, new ApiHelper(agent));
            }
        }

        static ApiHelper current;
        static object locker = new object();
        /// <summary>
        /// Current规范
        /// </summary>
        static public ApiHelper Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ApiHelper();
                        }
                    }
                }
                return current;
            }
        }
    }
}
