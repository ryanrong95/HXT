using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApp
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

#if !TEST && !DEBGU
//#if TEST && !DEBGU //20200709测试版本发布
            //获得浏览器的userAgent
            var userAgent = Request.ServerVariables["HTTP_USER_AGENT"];

            //获取请求的绝对路径
            var url = Request.Url.AbsoluteUri;

            if (!userAgent.Equals("yuanda-V23.29.28.47.98.70.63.54K93", StringComparison.OrdinalIgnoreCase))
            {
                Response.Clear();
                ////设置页面编码（GB2312可以支持汉字的输出）
                //Response.ContentEncoding = Encoding.GetEncoding("GB2312"); 
               
                Response.Write("The page visited does not exist!!");
                Response.End();

                //返回状态码
                //Response.StatusCode = 404;

                //throw new Exception("浏览器版本不符合");
            }
#endif

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}