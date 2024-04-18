using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// Api接口设置
    /// </summary>
    public class PvDataApiSetting
    {
        public string ApiName { get; internal set; }

        public PvDataApiSetting()
        {
            ApiName = "PvDataApiUrl";
        }
    }
}
