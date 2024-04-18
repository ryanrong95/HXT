using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public class HttpRequest
    {
        /// <summary>
        /// Http获取Icgoo接口内容
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="requeststatus">是否成功</param>
        /// <returns></returns>
        public static string GetRequest(string url, ref bool requeststatus)
        {
            string result = "";
            try
            {
                //垃圾回收，防止系统中的http资源没有正确释放，导致后面GetResponse超时死掉
                System.GC.Collect();

                // 创建一个HTTP请求
                System.Net.HttpWebRequest request;
                request = (System.Net.HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";
                SetHeaderValue(request.Headers, "Authorization", Icgoo.IFAuthorization);

                //设置超时时间
                request.Timeout = Icgoo.RequestOvertime;

                //获取请求
                System.Net.HttpWebResponse response;
                response = (System.Net.HttpWebResponse)request.GetResponse();

                if (response != null)
                {
                    var httpStatusCode = (int)response.StatusCode;
                    if (httpStatusCode == (int)HttpStatusCode.OK)
                    {
                        requeststatus = true;
                        System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        string responseText = myreader.ReadToEnd();
                        myreader.Close();
                        result = responseText;
                    }
                    else
                    {
                        requeststatus = false;
                    }
                }
                else
                {
                    requeststatus = false;
                }

                //释放资源
                response.Close();
                request.Abort();
            }
            catch
            {
                requeststatus = false;
            }

            return result;
        }

        /// <summary>
        /// 设置Http请求头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
    }
}
