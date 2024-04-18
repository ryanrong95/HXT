using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.ApiSettings
{
    public class PsOrderApiSetting
    {
        public string ApiName { get; internal set; }

        /// <summary>
        /// 同步实收数据
        /// </summary>
        public string SynPayeeRight { get; private set; }

        public PsOrderApiSetting()
        {
            ApiName = "PsOrderApiUrl";
            SynPayeeRight = "PayeeRight/SynPayeeRight";
        }
    }
}
