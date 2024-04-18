using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services
{
    public class PvCrmApiSetting
    {
        public string ApiUrl { get; private set; }

        /// <summary>
        /// 客户信息对接接口
        /// </summary>
        public string ClientInfo { get; private set; }

        /// <summary>
        /// 修改客户名称
        /// </summary>
        public string ClientNameModify { get; private set; }

        /// <summary>
        /// 根据关键字模糊查询
        /// </summary>
        public string GetEnterpriseByKeyword { get; private set; }

        /// <summary>
        /// 根据名称精确查询
        /// </summary>
        public string GetEnterpriseByName { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public PvCrmApiSetting()
        {
            ApiUrl = ConfigurationManager.AppSettings["CrmApi"];
            ClientInfo = "Shenc/ForIC";
            ClientNameModify = "Shenc/EnterpriseReName";
            GetEnterpriseByKeyword = "Common/GetEnterpriseByKeyword";
            GetEnterpriseByName = "Common/GetEnterpriseByName";
        }
    }
}
