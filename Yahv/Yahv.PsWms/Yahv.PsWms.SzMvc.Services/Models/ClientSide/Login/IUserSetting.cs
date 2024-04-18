using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    interface IUserSetting
    {
        /// <summary>
        /// web网站
        /// </summary>
        string WebCookieName { get; }

        /// <summary>
        /// 登录cookie 名称
        /// </summary>
        string LoginCookieName { get; }

        /// <summary>
        /// 登录是否记住密码
        /// </summary>
        string LoginRemeberName { get; }

        /// <summary>
        /// 登录时用户名
        /// </summary>
        string LoginUserIDName { get; }
    }
}
