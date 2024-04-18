using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsPortal.Services
{
    public class PvCrmApiSetting
    {
        public string ApiUrl { get; private set; }

        /// <summary>
        /// 客户信息对接接口
        /// </summary>
        public string ClientInfo { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public PvCrmApiSetting()
        {
            ApiUrl = ConfigurationManager.AppSettings["CrmApi"];
            ClientInfo = "Shenc/ForIC";
        }
    }
}
