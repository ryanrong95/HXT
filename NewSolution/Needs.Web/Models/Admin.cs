using System;

namespace Needs.Web.Models
{
    /// <summary>
    /// 管理员
    /// </summary>
    /// <example>
    /// 这是管理员视图
    /// </example>
    partial class Admin
    {
        /// <summary>
        /// 默认超级管理员ID
        /// </summary>
        internal const string SuperID = "SA0000000001";
        internal const string CookieName = "ydxcyht_new_big_erp";
            
        public string ID { get; set; }
        public string RealName { get; set; }
        public string UserName { get; set; }

        public Admin()
        {

        }

        public bool IsSa
        {
            get
            {
                return this.UserName.Equals("sa", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
