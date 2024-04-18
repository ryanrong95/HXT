using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings.Models
{
    class AdminSettings : ISettings, IAdminSettings
    {
        public string LoginCookieName { get; set; }
        public string SuperID { get; set; }

        public string SuperName { get; set; }

        public string SuperRoleID { get; set; }
        
        public AdminSettings()
        {
            this.LoginCookieName = "ydxcyht_new_big_erp";
            this.SuperID = "SA0000000001";
            this.SuperName = "sa";
            this.SuperRoleID = "Role000001";
        }
    }
}
