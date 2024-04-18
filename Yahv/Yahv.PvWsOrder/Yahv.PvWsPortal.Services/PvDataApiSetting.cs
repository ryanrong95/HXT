using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsPortal.Services
{
    /// <summary>
    /// api设置
    /// </summary>
    public class PvDataApiSetting
    {
        public string ApiName { get; internal set; }

        //获取税则详细信息
        public string ClassifiedProductInfo { get; internal set; }

        public string ClassifiedProduct { get; internal set; }

        public PvDataApiSetting()
        {
            ClassifiedProductInfo = "ClassifyInfo";
            ApiName = "PvDataApiUrl";
            ClassifiedProduct = "Data/ClassifiedInfo";
        }
    }
}
