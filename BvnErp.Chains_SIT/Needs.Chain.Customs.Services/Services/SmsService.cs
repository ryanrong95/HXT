using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    /// <summary>
    /// 短信服务
    /// </summary>
    public class SmsService
    {
        static object locker = new object();
        static SmsService current;

        private SmsService()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public static SmsService Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new SmsService();
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">接收方手机号码</param>
        /// <param name="message">短信内容</param>
        /// <returns>返回结果</returns>
        public string Send(string mobile, string message)
        {
            string msgAddress = SmsConfig.MessageAddress;
            string msgUrl = string.Format(msgAddress, mobile, message);

            //发送请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(msgUrl);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            //获取响应
            string result;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                {
                    result = streamReader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
