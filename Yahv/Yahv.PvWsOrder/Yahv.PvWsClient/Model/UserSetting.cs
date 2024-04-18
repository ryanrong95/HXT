using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsClient.Model
{
    class UserSetting : IUserSetting
    {
        public UserSetting()
        {
            this.WebCookieName = "Yahv.WsOrder.Client.Web";
            this.LoginCookieName = "Yahv.WsOrder.Client";
            this.LoginRemeberName = "Yahv.WsOrder.Client.User.IsRemeber";
            this.LoginUserIDName = "Yahv.WsOrder.Client.User.Name";
        }

        public string LoginCookieName { get; private set; }

        public string LoginRemeberName { get; private set; }

        public string LoginUserIDName { get; private set; }

        public string WebCookieName { get; set; }
    }
}
