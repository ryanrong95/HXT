using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    /// <summary>
    /// 修改密码
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// 原密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; }
    }

}