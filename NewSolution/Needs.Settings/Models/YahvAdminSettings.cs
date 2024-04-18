using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings.Models
{
    class YahvAdminSettings : IYahvAdminSettings
    {
        public string LoginCookieName { get; set; }
        public string SuperID { get; set; }

        public string SuperName { get; set; }

        public string SuperRoleID { get; set; }

        public YahvAdminSettings()
        {
            this.LoginCookieName = "ydcx_Yahv.Erp";
            this.SuperID = "SA01";
            this.SuperName = "sa";
            this.SuperRoleID = "SR01";
        }
    }

    public interface IYahvAdminSettings : ISettings
    {
        /// <summary>
        /// 超级权限
        /// </summary>
        string SuperRoleID { get; }
        string SuperID { get; }
        string LoginCookieName { get; }

        string SuperName { get; }
    }
}
