using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Yahv.Csrm.WebApp
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

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
#if !DEBUG
            //对系统错误进行处理和记录
            Exception ex = this.Context.Server.GetLastError();

            Yahv.Erp.Current.RFQ.Logs_Error(new Yahv.Underly.Logs.Logs_Error
            {
                Page = this.Context.Request.Url.ToString(),
                Message = ex.Message,
                Source = ex.Source,
                Stack = ex.StackTrace,
                Codes = "",
            });

            //清除系统错误
            this.Context.Server.ClearError();
           
            ////输出页面跳转代码
            //this.Context.Response.Clear();
            //this.Context.Response.Write("<script>top.location.href='/Error/';</script>");
            //this.Context.Response.End();
#endif
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}