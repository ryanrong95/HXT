using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebWeChat.Models
{
    /// <summary>
    /// 会员登录
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url{ get; set; }
    }
}