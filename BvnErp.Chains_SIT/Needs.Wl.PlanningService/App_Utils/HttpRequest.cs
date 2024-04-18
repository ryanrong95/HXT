using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Needs.Wl.PlanningService
{
    public class HttpRequest
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

        public bool GetPDF(string url, string SavePath, string FileName)
        {
            var result = false;
            try
            {
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
                        //直到request.GetResponse()程序才开始向目标网页发送Post请求
                        System.IO.Stream responseStream = response.GetResponseStream();

                        //不存在就创建目录
                        if (!System.IO.Directory.Exists(SavePath))
                        {
                            System.IO.Directory.CreateDirectory(SavePath);
                        }

                        //创建本地文件写入流
                        System.IO.Stream stream = new System.IO.FileStream(SavePath + FileName, System.IO.FileMode.Create);

                        byte[] bArr = new byte[1024];
                        int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        while (size > 0)
                        {
                            stream.Write(bArr, 0, size);
                            size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        }
                        stream.Close();
                        responseStream.Close();

                        result = true;
                    }
                }
                request.Abort();
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

        /// <summary>
        /// GetMessage
        /// </summary>
        /// <returns></returns>
        public string GetMessage(string Url,string Param1)
        {
            string result;
            var sss = "{'requestitem': '报关单详情'," +
                                  "'data':" +
                                  "{" +
                                  "'CustomsID':'" + Param1 + "'}," +
                                  "'key':'78fc22d55739b169d06de663133bc467'}";

            byte[] byteData = Encoding.GetEncoding("UTF-8").GetBytes(sss);
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = Url;
                request.Accept = "*/*";
                request.Timeout = 30 * 1000;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Method = "POST";
                request.ContentLength = byteData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream backStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(backStream, Encoding.GetEncoding("UTF-8"));
                result = sr.ReadToEnd();
                sr.Close();
                backStream.Close();
                response.Close();
                request.Abort();

            }
            catch (Exception ex)
            {
                result = ex.Message;
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

                //设置超时时间
                request.Timeout = this.Timeout;
                //设置内容编码类型
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
    }
}