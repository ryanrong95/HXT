using System;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Web;

namespace Yahv.Erm.Fingerprints.Services
{
    /// <summary>
    /// 浏览器 Agent
    /// </summary>
    public enum Agent
    {
        /// <summary>
        /// 库房
        /// </summary>
        Warehouse,
        /// <summary>
        /// 仓储服务
        /// </summary>
        WarehouseServicing,
        /// <summary>
        /// 其他
        /// </summary>
        [Obsolete("理论上禁用！")]
        Other
    }

    /// <summary>
    /// Api 请求帮助类
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

        ///// <summary>
        ///// Post 请求
        ///// </summary>
        ///// <param name="url">请求url</param>
        ///// <param name="data">请求参数</param>
        ///// <returns>返回string</returns>
        //public string Post(string url, object data = null)
        //{
        //    return Post(url, "POST", data);
        //}

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="url">要接收字符串的资源的 URI</param>
        /// <param name="data">要上载的字符串。</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        public string Post(string url, object data = null)
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
                    return client.UploadString(url, "POST", "");
                }

                var txts = data.GetType().GetProperties().Select(item => $"{item.Name}={item.GetValue(data)}");
                return client.UploadString(url, "POST", string.Join("&", txts));
            }
        }

        /// <summary>
        /// 请求Data
        /// </summary>
        /// <param name="address">要接收字符串的资源的 URI</param>
        /// <param name="method">用于将字符串发送到资源的 HTTP 方法。如果为 null，则对于 http 默认值为 POST</param>
        /// <param name="data">要上载的字符串。</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        [Obsolete("建议废弃， 使用：Post(string url, object data = null)")]
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

        /// <summary>
        /// 上传文件并请求返回值
        /// </summary>
        /// <param name="address"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [Obsolete("建议废弃")]
        public string PostFile(string address, string fileName, object data = null)
        {
            if (data != null)
            {
                string[] values = address.Split('?');

                var queries = HttpUtility.ParseQueryString(values.ElementAtOrDefault(1) ?? "", this.DefaultEncoding);

                foreach (var property in data.GetType().GetProperties())
                {
                    var value = property.GetValue(data);
                    queries[property.Name] = value == null ? null : value.ToString();
                }

                if (queries.Count > 0)
                {
                    address = $"{values[0]}?{queries}";
                }
            }

            using (WebClient client = new WebClient())
            {
                var bytes = client.UploadFile(address, "POST", fileName);
                string response = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                return response;
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

        /// <summary>
        /// 带有User-Agent索引器
        /// </summary>
        /// <param name="agent">User-Agent</param>
        /// <returns>帮助者</returns>
        public ApiHelper this[Agent agent]
        {
            get
            {
                //一定这样写，防止flag
                string name = Enum.GetName(typeof(Agent), agent);
                return concurrent.GetOrAdd(name, new ApiHelper(name));
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

        static ApiAsyncHelper asynchron;
        /// <summary>
        /// 异步规范
        /// </summary>
        static public ApiAsyncHelper Asynchron
        {
            get
            {
                if (asynchron == null)
                {
                    lock (locker)
                    {
                        if (asynchron == null)
                        {
                            asynchron = new ApiAsyncHelper();
                        }
                    }
                }
                return asynchron;
            }
        }
    }

    /// <summary>
    /// Api 请求异步帮助类
    /// </summary>
    public class ApiAsyncHelper
    {
        public Encoding DefaultEncoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        internal ApiAsyncHelper()
        {

        }

        class MyPost
        {
            /// <summary>
            /// 客户端
            /// </summary>
            public WebClient Client { get; set; }
            /// <summary>
            /// 自定义异步处理
            /// </summary>
            /// <remarks>
            /// 如果没有错误 Exception 参数为null
            /// </remarks>
            public Action<string, Exception> MyAction { get; set; }
        }

        class MyPostToekn
        {
            /// <summary>
            /// 客户端
            /// </summary>
            public WebClient Client { get; set; }

            public Type ChangeType { get; set; }

            /// <summary>
            /// 自定义异步处理
            /// </summary>
            /// <remarks>
            /// 如果没有错误 Exception 参数为null
            /// </remarks>
            public Delegate MyAction { get; set; }
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="action">回调委托,如果没有错误 Exception 参数为null</param>
        /// <returns>返回string</returns>
        public void Get(string url, Action<string, Exception> action)
        {
            this.Get(url, null, action);
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="data">请求参数</param>
        /// <param name="action">回调委托,如果没有错误 Exception 参数为null</param>
        /// <returns>返回string</returns>
        public void Get(string url, object data = null, Action<string, Exception> action = null)
        {
            if (data is JObject)
            {
                throw new Exception("不支持类型:" + typeof(JObject).FullName);
            }

            Uri uri = new Uri(url);

            if (data != null)
            {
                string[] values = url.Split('?');

                var queries = HttpUtility.ParseQueryString(uri.Query, this.DefaultEncoding);

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

            WebClient client = new WebClient { Encoding = this.DefaultEncoding };

            //uri.Query

            if (action != null)
            {
                client.DownloadStringCompleted += Get_DownloadStringCompleted; ;
            }

            client.DownloadStringAsync(uri, new MyPost
            {
                Client = client,
                MyAction = action
            });
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="action">回调委托,如果没有错误 Exception 参数为null</param>
        /// <returns>返回string</returns>
        public void Get<T>(string url, Action<T, Exception> action)
        {
            this.Get<T>(url, null, action);
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="data">请求参数</param>
        /// <param name="action">回调委托,如果没有错误 Exception 参数为null</param>
        /// <typeparam name="T">指定的Json类型</typeparam>
        /// <returns>返回string</returns>
        public void Get<T>(string url, object data = null, Action<T, Exception> action = null)
        {
            if (data is JObject)
            {
                throw new Exception("不支持类型:" + typeof(JObject).FullName);
            }

            Uri uri = new Uri(url);

            if (data != null)
            {
                string[] values = url.Split('?');

                var queries = HttpUtility.ParseQueryString(uri.Query, this.DefaultEncoding);

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

            WebClient client = new WebClient { Encoding = this.DefaultEncoding };

            //uri.Query

            if (action != null)
            {
                client.DownloadStringCompleted += Get_DownloadStringCompleted;
            }

            client.DownloadStringAsync(uri, new MyPostToekn
            {
                Client = client,
                ChangeType = typeof(T),
                MyAction = action
            });
        }

        void Get_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var mp = e.UserState as MyPost;
            if (mp != null)
            {
                using (mp.Client)
                {
                    mp.MyAction(e.Error == null ? e.Result : null, e.Error);
                }
            }

            var mpt = e.UserState as MyPostToekn;
            if (mpt != null)
            {
                using (mpt.Client)
                {
                    mpt.MyAction.DynamicInvoke(e.Error == null ? e.Result.JsonTo(mpt.ChangeType) : null, e.Error);
                }
            }
        }


        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="url">要接收字符串的资源的 URI</param>
        /// <param name="data">要上载的字符串。</param>
        /// <param name="action">回调委托,如果没有错误 Exception 参数为null</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        public void Post(string url, object data, Action<string, Exception> action = null)
        {
            if (data is JObject)
            {
                throw new Exception("不支持类型:" + typeof(JObject).FullName);
            }

            WebClient client = new WebClient { Encoding = this.DefaultEncoding };
            Uri uri = new Uri(url);
            if (action != null)
            {
                client.UploadStringCompleted += Post_UploadStringCompleted;
            }

            var content = data == null ? "" :
             string.Join("&", data.GetType().GetProperties().Select(item => $"{item.Name}={item.GetValue(data)}"));

            client.UploadStringAsync(uri, "POST", content, new MyPost
            {
                Client = client,
                MyAction = action
            });

        }


        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="url">要接收字符串的资源的 URI</param>
        /// <param name="data">要上载的字符串。</param>
        /// <param name="action">回调委托,如果没有错误 Exception 参数为null</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        public void Post<T>(string url, object data, Action<T, Exception> action = null)
        {
            if (data is JObject)
            {
                throw new Exception("不支持类型:" + typeof(JObject).FullName);
            }

            WebClient client = new WebClient { Encoding = this.DefaultEncoding };
            Uri uri = new Uri(url);
            if (action != null)
            {
                client.UploadStringCompleted += Post_UploadStringCompleted;
            }

            var content = data == null ? "" :
             string.Join("&", data.GetType().GetProperties().Select(item => $"{item.Name}={item.GetValue(data)}"));

            client.UploadStringAsync(uri, "POST", content, new MyPostToekn
            {
                Client = client,
                ChangeType = typeof(T),
                MyAction = action
            });

        }

        private void Post_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            var mp = e.UserState as MyPost;
            if (mp != null)
            {
                using (mp.Client)
                {
                    mp.MyAction(e.Error == null ? e.Result : null, e.Error);
                }
            }

            var mpt = e.UserState as MyPostToekn;
            if (mpt != null)
            {
                using (mpt.Client)
                {
                    mpt.MyAction.DynamicInvoke(e.Error == null ? e.Result.JsonTo(mpt.ChangeType) : null, e.Error);
                }
            }
        }


        /// <summary>
        /// json 请求
        /// </summary>
        /// <param name="url">要接收字符串的资源的 URI</param>
        /// <param name="data">要上载的字符串。</param>
        /// <param name="action">回调委托,如果没有错误 Exception 参数为null</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        public void JPost(string url, object data, Action<string, Exception> action = null)
        {
            if (data is JObject)
            {
                throw new Exception("不支持类型:" + typeof(JObject).FullName);
            }

            WebClient client = new WebClient { Encoding = this.DefaultEncoding };

            Uri uri = new Uri(url);
            if (action != null)
            {
                client.UploadStringCompleted += JPost_UploadStringCompleted; ;
            }

            client.Headers.Add("Content-Type", "application/json");
            client.Headers.Add("Accept", "application/json");

            client.UploadStringAsync(uri, "POST", data.Json(), new MyPost
            {
                Client = client,
                MyAction = action
            });
        }

        /// <summary>
        /// json 请求
        /// </summary>
        /// <param name="url">要接收字符串的资源的 URI</param>
        /// <param name="data">要上载的字符串。</param>
        /// <param name="action">回调委托,如果没有错误 Exception 参数为null</param>
        /// <returns>一个 System.String，包含服务器发送的响应。</returns>
        public void JPost<T>(string url, object data, Action<T, Exception> action = null)
        {
            if (data is JObject)
            {
                throw new Exception("不支持类型:" + typeof(JObject).FullName);
            }

            WebClient client = new WebClient { Encoding = this.DefaultEncoding };

            Uri uri = new Uri(url);
            if (action != null)
            {
                client.UploadStringCompleted += JPost_UploadStringCompleted; ;
            }

            client.Headers.Add("Content-Type", "application/json");
            client.Headers.Add("Accept", "application/json");

            client.UploadStringAsync(uri, "POST", data.Json(), new MyPostToekn
            {
                Client = client,
                ChangeType = typeof(T),
                MyAction = action
            });
        }

        private void JPost_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            var mp = e.UserState as MyPost;
            if (mp != null)
            {
                using (mp.Client)
                {
                    mp.MyAction(e.Error == null ? e.Result : null, e.Error);
                }
            }

            var mpt = e.UserState as MyPostToekn;
            if (mpt != null)
            {
                using (mpt.Client)
                {
                    mpt.MyAction.DynamicInvoke(e.Error == null ? e.Result.JsonTo(mpt.ChangeType) : null, e.Error);
                }
            }
        }

        //==========================================================↑异步开发  接受整数↓

        public void TestFun()
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            using (WebClient client = new WebClient())
            {

                //string address = "https://xxx.com";
                //client.Headers.Add(HttpRequestHeader.ContentType, "text/xml");
                //System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                //var response = client.UploadData(address, "POST", encoding.GetBytes(msg));
            }
        }
        private bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {   // 总是接受 认证平台 服务器的证书
            return true;
        }

    }
}

