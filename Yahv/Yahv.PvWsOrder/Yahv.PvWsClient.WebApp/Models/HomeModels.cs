using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebApp.Models
{

    /// <summary>
    /// 登录页面视图
    /// </summary>
    public class LoginViewModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RemberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    /// <summary>
    /// 投诉建议
    /// </summary>
    public class SuggestionViewModel
    {
        /// <summary>
        /// 申请人
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 建议
        /// </summary>
        public string summary { get; set; }
    }
}