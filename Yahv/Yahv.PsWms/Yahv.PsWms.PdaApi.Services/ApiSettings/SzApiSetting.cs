using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.PdaApi.Services.ApiSettings
{
    /// <summary>
    /// 网站接口配置
    /// </summary>
    public class SzApiSetting
    {
        /// <summary>
        /// 网站接口地址
        /// </summary>
        public string ApiName { get; private set; }

        /// <summary>
        /// 修改订单状态
        /// </summary>
        public string ChangeOrderStatus { get; private set; }

        public SzApiSetting()
        {
            ApiName = "SzApiUrl";
            ChangeOrderStatus = "Order/RealChangeOrderStatus";
        }
    }
}
