using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Services.Models
{
    public class Logon
    {
        /// <summary>
        /// 登陆名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 真实名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }
    }
}
