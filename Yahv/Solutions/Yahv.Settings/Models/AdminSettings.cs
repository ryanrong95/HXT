using System;
using Yahv.Settings.Attributes;

namespace Yahv.Settings.Models
{
    class AdminSettings : ISettings, IAdminSettings
    {
        [Label("Cookie Name where Logining")]
        public string LoginCookieName { get; set; }

        [Label("Super administrator ID")]
        public string SuperID { get; set; }

        [Label("Super administrator name")]
        public string SuperName { get; set; }

        [Label("Super administrator  realname")]
        public string SuperRealName { get; set; }

        [Label("Super Role ID")]
        public string SuperRoleID { get; set; }

        [Label("Super Role name")]
        public string SuperRoleName { get; set; }

        [Label("组织机构")]
        public string League { get; set; }

        public AdminSettings()
        {
            this.LoginCookieName = "ydcx_Yahv.Erp";
            this.SuperID = "SA01";
            this.SuperName = "sa";
            this.SuperRealName = "超级管理员";

            this.SuperRoleID = "SR01";
            this.SuperRoleName = "超级管理员";
            this.League = "ydcx_Yahv.Erp.League";
        }
    }
}
