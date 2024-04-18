using Needs.Wl.Logs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Needs.Wl.Web.Mvc.Utils
{
    /// <summary>
    /// 发送邮件类相关
    /// </summary>
    public class SmtpContext
    {
        static object locker = new object();
        static SmtpContext current;

        /// <summary>
        /// 邮件服务器
        /// </summary>
        private static string MailServer;

        /// <summary>
        /// 发送邮箱地址
        /// </summary>
        private static string MailUserName;

        /// <summary>
        /// 发送邮箱密码
        /// </summary>
        private static string MailPassword;

        private SmtpContext()
        {
            SmtpContext.MailServer = System.Configuration.ConfigurationManager.AppSettings["MailServer"].ToString();
            SmtpContext.MailUserName = System.Configuration.ConfigurationManager.AppSettings["MailUserName"].ToString();
            SmtpContext.MailPassword = System.Configuration.ConfigurationManager.AppSettings["MailPassword"].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public static SmtpContext Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new SmtpContext();
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="message">邮件内容</param>
        public void Send(string to, string subject, string message)
        {
            try
            {
                SmtpClient client = new SmtpClient(SmtpContext.MailServer)
                {
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(SmtpContext.MailUserName, SmtpContext.MailPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(SmtpContext.MailUserName, "芯达通服务"),//发件人
                    Subject = subject,
                    BodyEncoding = System.Text.Encoding.Default,
                    Body = message,
                    IsBodyHtml = true
                };
                mail.To.Add(to);//收件人
                client.Send(mail);
            }
            catch(Exception ex)
            {
                ex.Log();
            }
        }
    }
}