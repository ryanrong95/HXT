using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Erm.Services.Interfaces
{
    /// <summary>
    /// Api接口设置
    /// </summary>
    public class WlAdminApiSetting
    {
        public string ApiName { get; internal set; }

        /// <summary>
        /// 工作承接
        /// </summary>
        public string HandOver { get; private set; }

        /// <summary>
        /// 工作承接返回
        /// </summary>
        public string HandOverReturn { get; private set; }

        public WlAdminApiSetting()
        {
            ApiName = "WlAdminApiUrl";

            HandOver = "api/HandOver/HandOver";
            HandOverReturn = "api/HandOver/HandOverReturn";
        }
    }
}
