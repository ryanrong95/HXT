using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class HttpRequest
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
        /// Http Post 请求
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="msg">推送报文</param>
        /// <returns></returns>
        public static string Post(string url, string msg)
        {
            string result = "";
            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                //                       | SecurityProtocolType.Tls
                //                       | SecurityProtocolType.Tls11
                //                       | SecurityProtocolType.Tls12;

                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "POST";

              
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
