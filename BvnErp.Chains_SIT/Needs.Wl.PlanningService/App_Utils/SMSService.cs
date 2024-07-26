using Needs.Ccs.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class SMSService
    {
        static object locker = new object();
        static SMSService current;

        public string MessageAddressUrl { get; set; }

        private SMSService()
        {
            this.MessageAddressUrl = Icgoo.MessageAddressUrl;
        }

        public static SMSService Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new SMSService();
                        }
                    }
                }
                return current;
            }
        }

        public void SendMessage(string Phone, string SendContent)
        {
            try
            {
                string MessageContent = "【华芯通】" + SendContent;
                string MessageAddressUrl = this.MessageAddressUrl;
                string MessageUrl = string.Format(MessageAddressUrl, Phone, MessageContent);
                string result = HttpGet(MessageUrl);               
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        private string HttpGet(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}
