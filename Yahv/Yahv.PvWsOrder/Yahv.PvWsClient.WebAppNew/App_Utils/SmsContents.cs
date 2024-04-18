using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.App_Utils
{
    /// <summary>
    /// 手机短信内容模板
    /// </summary>
    public class SmsContents
    {
        /// <summary>
        /// 注册时发送的短信模板
        /// </summary>
        public const string Register = "您好，您注册芯达通账号的校验码为{0}，请不要把校验码泄露给其他人！该校验码3分钟内有效，非本人操作，请忽略本条消息。【芯达通】";

        /// <summary>
        /// 修改手机绑定的短信模板
        /// </summary>
        public const string ChangeMobile = "您好，您正在申请变更您的手机号码，请确保该操作是您本人进行，您的校验码是{0}，该校验码3分钟内有效，非本人操作，请忽略本条消息。【芯达通】";

        /// <summary>
        /// 变更初始化密码
        /// </summary>
        public const string ChangePassword = "您好，您正在申请变更您的登录密码，请确保该操作是您本人进行，您的校验码是{0}，该校验码3分钟内有效，非本人操作，请忽略本条消息。【芯达通】";

        /// <summary>
        /// 手机号登录
        /// </summary>
        public const string MobileLogin = "您好，您的登录验证码为{0}，请不要把验证码泄露给其他人！该校验码3分钟内有效，非本人操作，请忽略本条消息。【芯达通】";
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
            try
            {
                WebAppNew.SmsService.Service1 sms = new WebAppNew.SmsService.Service1();
                sms.g_Submit("dlydcx00", "rYfl76qL", "", "1012818", mobile, message);
            }
            catch (Exception ex)
            {
                //TODO:记录日志
                //ex.Message; 记录在系统日志中
            }
        }
    }
}