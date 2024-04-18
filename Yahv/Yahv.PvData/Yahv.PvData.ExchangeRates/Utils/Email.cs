using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.ExchangeRates.Utils
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    public class SmtpContext
    {
        static object locker = new object();
        static SmtpContext current;

        /// <summary>
        /// 邮件服务器
        /// </summary>
        private string MailServer;

        /// <summary>
        /// 发送邮箱地址
        /// </summary>
        private string MailUserName;

        /// <summary>
        /// 发送邮箱密码
        /// </summary>
        private string MailPassword;

        /// <summary>
        /// 收件人
        /// </summary>
        private string Receivers;

        private SmtpContext()
        {
            this.MailServer = ConfigurationManager.AppSettings["MailServer"].ToString();
            this.MailUserName = ConfigurationManager.AppSettings["MailUserName"].ToString();
            this.MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
            this.Receivers = ConfigurationManager.AppSettings["Receivers"].ToString();
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
        /// <param name="subject">邮件标题</param>
        /// <param name="message">邮件内容</param>
        public void Send(string subject, string message)
        {
            try
            {
                SmtpClient client = new SmtpClient(this.MailServer);//邮件服务器
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(this.MailUserName, this.MailPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(this.MailUserName, "中心数据抓取服务");//发件人
                mail.Subject = subject;
                mail.BodyEncoding = System.Text.Encoding.Default;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.To.Add(this.Receivers);//收件人

                client.Send(mail);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
