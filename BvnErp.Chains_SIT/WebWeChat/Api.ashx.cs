using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using WeChat.Api;
using WeChat.Api.CustomMessageHandler;
using WeChat.Api.Model;

namespace WebWeChat
{
    /// <summary>
    /// Api 的摘要说明
    /// </summary>
    public class Api : IHttpHandler
    {

        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.HttpMethod.ToLower() == "post")
            {
                //回复消息的时候也需要验证消息，这个很多开发者没有注意这个，存在安全隐患  
                //微信中 谁都可以获取信息 所以 关系不大 对于普通用户 但是对于某些涉及到验证信息的开发非常有必要
                if (CheckSignature())
                {
                    //接收消息
                    // var openid=

                    WxPostModel wxPostModel = new WxPostModel()
                    {
                        Signature = context.Request.QueryString["Signature"],
                        Msg_Signature = context.Request.QueryString["Msg_Signature"],
                        Timestamp = context.Request.QueryString["Timestamp"],
                        Nonce = context.Request.QueryString["Nonce"],
                        Token = context.Request.QueryString["Token"],

                        EncodingAESKey = EncodingAESKey
                    };

                    string text = new WxCustomMessageHandler(context.Request.InputStream, wxPostModel).FinalResponseString;
                    //HttpContext.Current.Response.Write(resString);
                    context.Response.ClearContent();
                    context.Response.ContentType = "text/xml";
                    text = (text ?? "").Replace("\r\n", "\n");
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);

                    context.Response.End();
                }
                else
                {
                    HttpContext.Current.Response.Write("消息并非来自微信");
                    HttpContext.Current.Response.End();
                }
            }
            else
            {
                CheckWechat();
            }
        }

        #region 验证微信签名
        /// <summary>
        /// 返回随机数表示验证成功
        /// </summary>
        private void CheckWechat()
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["echoStr"]))
            {
                HttpContext.Current.Response.Write("消息并非来自微信");
                HttpContext.Current.Response.End();
            }
            string echoStr = HttpContext.Current.Request.QueryString["echoStr"];
            if (CheckSignature())
            {
                HttpContext.Current.Response.Write(echoStr);
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// <returns></returns>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        private bool CheckSignature()
        {
            string access_token = System.Configuration.ConfigurationManager.AppSettings["WeixinToken"].ToString();

            string signature = HttpContext.Current.Request.QueryString["signature"].ToString();
            string timestamp = HttpContext.Current.Request.QueryString["timestamp"].ToString();
            string nonce = HttpContext.Current.Request.QueryString["nonce"].ToString();
            string[] ArrTmp = { access_token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = CommonMethod.GetSwcSH1(tmpStr);

            if (tmpStr.ToLower() == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}