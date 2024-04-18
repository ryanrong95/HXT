using Needs.Utils.Http;
using Needs.Web.Models;
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Needs.Web.Mvc
{
    [Obsolete("前后端已经分离，随时准备废弃")]
    abstract public class ClientController : Controller
    {

        static public User Current
        {
            get
            {
                if (Cookies.Supported)
                {
                    //jzf 20180628 先注释
                    return SsoLogon.ByToken(Cookies.Cross[SsoLogon.CookieName], UserTokenType.UserLogin);
                }
                else
                {
                    //返回Npc
                    //机器人
                    return null;
                }
            }
        }

        protected List<string> RegisterBlock { get; private set; }

        protected ClientController()
        {
            this.RegisterBlock = new List<string>();
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            var list = this.RegisterBlock;

            if (list.Count == 0)
            {
                return;
            }

            if (list.Count == 1)
            {
                ViewBag.Alert = $"<script>{list.First()}</script>";
            }
            else
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.AppendLine("<script>");
                foreach (string block in this.RegisterBlock)
                {
                    builder.AppendLine(block);
                }
                builder.AppendLine("</script>");
            }
        }

        /// <summary>
        /// 务必在模版与视图中实现  @Html.Raw(ViewBag.Alert);
        /// </summary>
        /// <param name="message">内容</param>
        /// <returns></returns>
        protected void Alert(string message)
        {
            this.RegisterBlock.Add(new EasyuiAlert
            {
                Context = message,
                Title = "提示"
            }.Execute());
        }

        /// <summary>
        /// 提示跳转函数
        /// </summary>
        /// <param name="message">内容</param>
        /// <param name="uri">跳转地址默认为环境首页</param>
        /// <returns></returns>
        protected ContentResult Alert(string message, string url)
        {
            return this.Alert("提示", message, url);
        }

        /// <summary>
        /// 提示跳转函数
        /// </summary>
        /// <param name="message">内容</param>
        /// <param name="title">标题</param>
        /// <param name="url">跳转地址默认为环境首页</param>
        /// <returns></returns>
        protected ContentResult Alert(string title, string message, string url)
        {
            return Content(new EasyuiAlert
            {
                Context = message,
                Title = title,
                Url = url
            }.Execute());
        }




    }
}