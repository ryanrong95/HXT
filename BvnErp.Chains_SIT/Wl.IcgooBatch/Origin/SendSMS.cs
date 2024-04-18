using Needs.Ccs.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wl.IcgooBatch
{
    public static class SendSMS
    {
        public static string SendMessage(string Phone, string SendContent)
        {
            try
            {
                string MessageContent = "【创新恒远】" + SendContent;
                string MessageAddressUrl = Icgoo.MessageAddressUrl;
                string MessageUrl = string.Format(MessageAddressUrl, Phone, MessageContent);
                string result = HttpGet(MessageUrl);
                return "";
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpGet(string Url)
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
