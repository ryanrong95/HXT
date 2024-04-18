/*
 泛解析的思路已经被同意，因此放弃本方法
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Yahv.Web.Erp
{
    /// <summary>
    /// 平台重写测试
    /// </summary>
    /// <remarks>
    /// 只在泛解析中只用
    /// </remarks>
    public class PlatsUrlWriter : IHttpModule
    {
        ///// <summary>
        ///// 用于判断泛解析
        ///// </summary>
        //static public bool IsResolution
        //{
        //    get
        //    {
        //        bool value1;
        //        if (bool.TryParse(ConfigurationManager.AppSettings["IsResolution"], out value1))
        //        {
        //            return value1;
        //        }

        //        int value2;
        //        if (int.TryParse(ConfigurationManager.AppSettings["IsResolution"], out value2))
        //        {
        //            return default(int) != value2;
        //        }

        //        return false;
        //    }
        //}

        /// <summary>
        /// 判断是否为泛解析
        /// </summary>
        static public bool IsResolution
        {
            get
            {
                return HttpContext.Current.Request.Url.Host.IndexOf("admin-", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }


        /// <summary>
        /// 获取当前非泛解析地址
        /// </summary>
        /// <returns></returns>
        static public string GetUrl()
        {
            //用替换的办法做就是因为拿不准未来的方法！！！！！！！！！

            string url = HttpContext.Current.Request.Url.OriginalString;
            string target;
            if (IsResolution)
            {
                //获取地址中的UserName：登录名称
                Regex regex = new Regex(@"admin-.*?\.", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                target = regex.Replace(url, "");
            }
            else
            {
                target = url;
            }

            return target;
        }




        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        protected void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            HttpContext context = application.Context; //上下文
            string url = context.Request.Url.LocalPath; //获得请求URL
            Regex panelRegex = new Regex("/(.*?)/Panels.aspx$"); //定义规则
            if (panelRegex.IsMatch(url))
            {
                string paramStr = url.Substring(url.LastIndexOf('/') + 1);
                context.RewritePath("/Panels.aspx");
            }
            else
            {
                //context.RewritePath("/");
            }
        }

        /// <summary>
        /// 接口必须的释放
        /// </summary>
        public void Dispose()
        {

        }
    }
}
