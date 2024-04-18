using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace Yahv.PvWsPortal2.MvcApp.App_Utils
{
    /// <summary>
    /// 手机短信内容模板
    /// </summary>
    public class SmsContents
    {
        /// <summary>
        /// 注册时发送的短信模板
        /// </summary>
        //public const string Register = "您好，您注册芯达通账号的校验码为{0},请不要把校验码泄露给其他人！该校验码3分钟内有效，非本人操作，请忽略本条消息。【芯达通】";
        public const string Register = "敬爱的会员，你的验证码是 {0}。我们不会以任何理由向你索要验证码。【芯达通】";
        /// <summary>
        /// 修改手机绑定的短信模板
        /// </summary>
        public const string ChangeMobile = "您好，您正在申请变更您的手机号码，请确保该操作是您本人进行，您的校验码是{0},该校验码3分钟内有效，非本人操作，请忽略本条消息。【芯达通】";

        /// <summary>
        /// 变更初始化密码
        /// </summary>
        public const string ChangePassword = "您好，您正在申请变更您的登录密码，请确保该操作是您本人进行，您的校验码是{0},该校验码3分钟内有效，非本人操作，请忽略本条消息。【芯达通】";
    }

    /// <summary>
    /// 手机短信验服务
    /// </summary>
    public class SmsService
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">接收方手机号码</param>
        /// <param name="message">短信内容</param>
        public static void Send(string mobile, string message)
        {
            string url = string.Format("http://cf.51welink.com/submitdata/Service.asmx/g_Submit?sname=dlydcx00&spwd=rYfl76qL&scorpid=&sprdid=1012818&sdst={0}&smsg={1}", mobile, message);
            string xml = HttpGet(url);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string state;
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
        }
        ///// <summary>
        ///// 发送短信
        ///// </summary>
        ///// <param name="mobile">接收方手机号码</param>
        ///// <param name="message">短信内容</param>
        //public static void Send(string mobile, string message)
        //{
        //    try
        //    {
        //        MvcApp.SmsService.Service1 sms = new MvcApp.SmsService.Service1();
        //        sms.g_Submit("dlydcx00", "rYfl76qL", "", "1012818", mobile, message);
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO:记录日志
        //        //ex.Message; 记录在系统日志中
        //    }
        //}


        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static string HttpGet(string url)
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
    }
}