using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Models
{
    /// <summary>
    /// 账号信息
    /// </summary>
    public partial class MyAccountInfoViewModel
    {
        //用户ID
        public string AdminID { get; set; }

        //用户名
        public string UserName { get; set; }

        //公司名称
        public string Company { get; set; }

        //密码
        public string Password { get; set; }

        //手机号
        public string Phone { get; set; }

        //邮箱
        public string Mail { get; set; }

    }

    /// <summary>
    /// 登陆页面视图
    /// </summary>
    public class LoginViewModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RemberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    /// <summary>
    /// 忘记密码
    /// </summary>
    public class FindPasswordViewModel
    {
        public string Email { get; set; }
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    public class ChangePasswordViewModel
    {
        public string Email { get; set; }

        public string Token { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// 注册页面视图
    /// </summary>
    public class RegisterViewModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string CompanyName { get; set; }

        public string Phone { get; set; }

        public int RegisterType { get; set; }

        public string Email { get; set; }
    }

    /// <summary>
    /// 报名申请视图
    /// </summary>
    public class ServiceApplyViewModel
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        public string Phone { get; set; }

        public string Tel { get; set; }

        public string Email { get; set; }
    }
}