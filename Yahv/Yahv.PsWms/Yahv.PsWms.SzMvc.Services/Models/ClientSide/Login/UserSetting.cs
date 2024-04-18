using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    class UserSetting : IUserSetting
    {
        public UserSetting()
        {
            this.WebCookieName = "Yahv.PsWms.Client.Web";
            this.LoginCookieName = "Yahv.PsWms.Client";
            this.LoginRemeberName = "Yahv.PsWms.Client.User.IsRemeber";
            this.LoginUserIDName = "Yahv.PsWms.Client.User.Name";
        }

        public string LoginCookieName { get; private set; }

        public string LoginRemeberName { get; private set; }

        public string LoginUserIDName { get; private set; }

        public string WebCookieName { get; set; }
    }
}
