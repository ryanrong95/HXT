using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class XDTEmailService
    {
        static object locker = new object();
        public static XDTEmailService current;

        public static string MailServer;
        public static string MailUserName;
        public static string MailPassword;

        private XDTEmailService()
        {
            XDTEmailService.MailServer = System.Configuration.ConfigurationManager.AppSettings["XDTMailServer"].ToString();
            XDTEmailService.MailUserName = System.Configuration.ConfigurationManager.AppSettings["XDTMailUserName"].ToString();
            XDTEmailService.MailPassword = System.Configuration.ConfigurationManager.AppSettings["XDTMailPassword"].ToString();
        }

        public static XDTEmailService Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new XDTEmailService();
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
                SmtpClient client = new SmtpClient(XDTEmailService.MailServer);//邮件服务器
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(XDTEmailService.MailUserName, XDTEmailService.MailPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(XDTEmailService.MailUserName, "华芯通服务");//发件人
                mail.Subject = subject;
                mail.BodyEncoding = System.Text.Encoding.Default;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.To.Add(to);//收件人
                client.Send(mail);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
