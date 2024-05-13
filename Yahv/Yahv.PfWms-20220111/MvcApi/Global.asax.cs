using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcApi
{
    public class MvcApilication : Yahv.Web.Faces.MyMvcApilication
    {
        public MvcApilication()
        {
            this.Error += MvcApilication_Error;
        }

        private void MvcApilication_Error(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 在接收到一个应用程序请求时触发对于一个请求来说，它是第一个被触发的事件，请求一般是用户输入的一个页面请求（URL）
        /// </summary>
        protected void Application_BeginRequest()
        {
            ////获得浏览器的userAgent
            //var userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
            ////获取请求的绝对路径
            //var url = Request.Url.AbsoluteUri;
            //if (userAgent.ToLower() != "yuanda-V23.29.28.47.98.70.63.54K93".ToLower())
            //{
            //    throw new Exception("浏览器版本不符合");
            //}
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

     
    }
}
