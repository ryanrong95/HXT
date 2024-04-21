using System;
using System.Data;
using System.IO;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
//using PKeyType = Yahv.Erm.Services.PKeyType;
using Yahv.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Mail;

namespace Yahv.Erm.AttendService
{
    /// <summary>
    /// 通知提醒任务
    /// </summary>
    public class WarningTask
    {
        private List<WarningContext> SendContext { get; set; }

        public WarningTask()
        {
        }

        public void Init()
        {
            var staffs = new Erm.Services.Views.StaffAlls()
                .Where(item => item.Labour.EnterpriseID == Services.Common.ErmConfig.LabourEnterpriseID || item.Labour.EnterpriseID == Services.Common.ErmConfig.LabourEnterpriseID2)
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).ToArray();

            var fileView = new Yahv.Services.Views.CenterFilesTopView()
                .Where(item => item.Type == (int)FileType.BackgroundInvestigation && item.Status == Yahv.Services.Models.FileDescriptionStatus.Normal).ToArray();

            var datas = from staff in staffs
                        join file in fileView on staff.ID equals file.StaffID into files
                        select new Staff
                        {
                            ID = staff.ID,
                            Code = staff.Code,
                            Name = staff.Name,
                            LastBackgroundCheck = files.Max(item => item.CreateDate),
                        };
            //提示需要做背景调查的人
            var Names = datas.Where(item => item.LastBackgroundCheck == null || ((DateTime)item.LastBackgroundCheck).AddMonths(11) < DateTime.Now)
                .Select(item => item.Name);
            if (Names.Count() == 0)
            {
                return;
            }

            this.SendContext = new List<WarningContext>();
            this.SendContext.Add(new WarningContext()
            {
                Title = "芯达通背景调查提醒",
                CreateDate = DateTime.Now,
                WarningMethod = WarningMethod.Mail,
                Context = "芯达通背景调查提醒名单：" + string.Join(";", Names),
                Email = System.Configuration.ConfigurationManager.AppSettings["NoticeMail"].ToString(),
            });
            this.SendContext.Add(new WarningContext()
            {
                CreateDate = DateTime.Now,
                WarningMethod = WarningMethod.Msg,
                Context = "芯达通背景调查提醒名单：" + string.Join(";", Names),
                Moblie = System.Configuration.ConfigurationManager.AppSettings["NoticeMobile"].ToString(),
            });
        }

        public void PushWarning()
        {
            foreach (var item in this.SendContext)
            {
                if (item.WarningMethod == WarningMethod.Mail)
                {
                    SendMail(item);
                }
                if (item.WarningMethod == WarningMethod.Msg)
                {
                    SendMsg(item);
                }
            }
        }

        private void SendMail(WarningContext item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.Email))
                {
                    SmtpService.Current.Send(item.Email, item.Title, item.Context);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SendMsg(WarningContext item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.Moblie))
                {
                    SMSService.Current.SendMessage(item.Moblie, item.Context);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 获取配置文件时间
        /// </summary>
        /// <returns></returns>
        public ExecDto GetConfig()
        {
            using (StreamReader reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json")))
            {
                string json = reader.ReadToEnd();
                return JsonSerializerExtend.JsonTo<ExecDto>(json);
            }
        }

        /// <summary>
        /// 修改配置文件执行时间
        /// </summary>
        public void SetConfig(ExecDto dto)
        {
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json"), dto.Json());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    /// <summary>
    /// 发送短信
    /// </summary>
    public class SMSService
    {
        static object locker = new object();
        static SMSService current;

        //private string MessageAddressUrl = "http://cf.51welink.com/submitdata/Service.asmx/g_Submit?sname=dlydcx00&spwd=rYfl76qL&scorpid=&sprdid=1012818&sdst={0}&smsg={1}";
        private string MessageAddressUrl = "http://ydsms.ic360.cn/Service.asmx/g_Submit?sname=dlydcx00&spwd=rYfl76qL&scorpid=&sprdid=1012818&sdst={0}&smsg={1}";

        private SMSService()
        {
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
                string MessageContent = "【芯达通】" + SendContent;
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

    /// <summary>
    /// 发送邮件
    /// </summary>
    public class SmtpService
    {
        static object locker = new object();
        static SmtpService current;

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

        private SmtpService()
        {
            SmtpService.MailServer = System.Configuration.ConfigurationManager.AppSettings["MailServer"].ToString();
            SmtpService.MailUserName = System.Configuration.ConfigurationManager.AppSettings["MailUserName"].ToString();
            SmtpService.MailPassword = System.Configuration.ConfigurationManager.AppSettings["MailPassword"].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public static SmtpService Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new SmtpService();
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
                SmtpClient client = new SmtpClient(SmtpService.MailServer);//邮件服务器
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(SmtpService.MailUserName, SmtpService.MailPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(SmtpService.MailUserName, "芯达通服务");//发件人
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

    /// <summary>
    /// 发送内容
    /// </summary>
    public class WarningContext : IUnique
    {
        public string ID { get; set; }
        public string MainID { get; set; }
        public string Title { get; set; }
        public string Context { get; set; }
        public DateTime CreateDate { get; set; }
        public WarningMethod WarningMethod { get; set; }
        public string AdminID { get; set; }
        public string Email { get; set; }
        public string Moblie { get; set; }
    }

    /// <summary>
    /// 提醒方式
    /// </summary>
    public enum WarningMethod
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        Mail = 1,

        /// <summary>
        /// 发短信
        /// </summary>
        Msg = 2,
    }
}