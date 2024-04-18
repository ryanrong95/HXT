using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CnslAppTester
{
    class Program
    {

        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public string HttpGet(string url)
        {
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">接收方手机号码</param>
        /// <param name="message">短信内容</param>
        public static string Send(string mobile, string message)
        {
            string url = string.Format("http://cf.51welink.com/submitdata/Service.asmx/g_Submit?sname=dlydcx00&spwd=rYfl76qL&scorpid=&sprdid=1012818&sdst={0}&smsg={1}", mobile, message);
            string xml = HttpGet(url);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string state = null;
            XmlNodeList nodeList = doc.GetElementsByTagName("CSubmitState");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode item in nodeList[0].ChildNodes)
                {
                    if (item.Name == "State")
                    {
                        state = item.InnerText;
                        break;
                    }
                }
            }

            if (state == "")
            {

            }
            return $@"{mobile}
{xml}";
        }

        public const string Register = "敬爱的会员，你的验证码是 {0}。我们不会以任何理由向你索要验证码。【芯达通】";

        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("程序开始！");
                Random ran = new Random();
                string messageCode = ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
                string msg = string.Format(Register, messageCode);
                Console.WriteLine("开始发送");
                string xml1 = Send("13522675499", msg);
                string xml2 = Send("13051287915", msg);
                Console.WriteLine("发送完成，响应结果：");
                Console.WriteLine(xml1);
                Console.WriteLine(xml2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }


            Console.Read();
        }
    }
}
