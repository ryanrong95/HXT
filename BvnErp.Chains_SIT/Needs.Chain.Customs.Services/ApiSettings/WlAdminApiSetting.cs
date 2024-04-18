using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.ApiSettings
{
    /// <summary>
    /// Api接口配置
    /// </summary>
    public class WlAdminApiSetting
    {
        /// <summary>
        /// 子系统接口地址
        /// </summary>
        public string ApiName { get; private set; }

        /// <summary>
        /// 产品归类，提交归类信息
        /// </summary>
        public string SubmitClassified { get; private set; }

        /// <summary>
        /// 继续归类，获取下一条归类信息
        /// </summary>
        public string GetNext { get; private set; }

        /// <summary>
        /// 产品预归类，提交归类信息
        /// </summary>
        public string PreSubmitClassified { get; private set; }

        /// <summary>
        /// 继续归类，获取下一条预归类信息
        /// </summary>
        public string PreGetNext { get; private set; }

        public WlAdminApiSetting()
        {
            ApiName = "WlAdminApiUrl";
            SubmitClassified = "api/Classify/SubmitClassified";
            GetNext = "api/Classify/GetNext";
            PreSubmitClassified = "api/PreClassify/SubmitClassified";
            PreGetNext = "api/PreClassify/GetNext";
        }
    }
}
