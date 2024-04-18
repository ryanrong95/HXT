using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings.Models
{
    class SsoSettings : ISettings, ISsoSettings
    {
        public string LoginCookieName { get; set; }

        public SsoSettings()
        {
            this.LoginCookieName = "ydxcyht_new_big_sso";
        }
    }
}
