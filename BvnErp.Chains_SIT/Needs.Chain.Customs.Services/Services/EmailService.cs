using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    /// <summary>
    /// 邮件服务
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
        /// 发送邮箱显示名
        /// </summary>
        private static string MailDisplayName;

        /// <summary>
        /// 发送邮箱密码
        /// </summary>
        private static string MailPassword;

        private SmtpContext()
        {
            SmtpContext.MailServer = System.Configuration.ConfigurationManager.AppSettings["MailServer"];
            SmtpContext.MailUserName = System.Configuration.ConfigurationManager.AppSettings["MailUserName"];
            SmtpContext.MailDisplayName = System.Configuration.ConfigurationManager.AppSettings["MailDisplayName"];
            SmtpContext.MailPassword = System.Configuration.ConfigurationManager.AppSettings["MailPassword"];
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
            //邮件信息
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(SmtpContext.MailUserName, SmtpContext.MailDisplayName);
            mail.Subject = subject;
            mail.BodyEncoding = System.Text.Encoding.Default;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.To.Add(to);

            //邮件服务器
            SmtpClient client = new SmtpClient(SmtpContext.MailServer);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(SmtpContext.MailUserName, SmtpContext.MailPassword);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(mail);
        }
    }
}
