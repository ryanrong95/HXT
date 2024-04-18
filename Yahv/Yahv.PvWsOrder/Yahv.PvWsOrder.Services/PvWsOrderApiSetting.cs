using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services
{
    /// <summary>
    /// Api接口设置
    /// </summary>
    public class PvWsOrderApiSetting
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


        public PvWsOrderApiSetting()
        {
            ApiName = "PvWsOrderApiUrl";
            SubmitClassified = "Classify/SubmitClassified";
            GetNext = "Classify/GetNext";
        }
    }

    
}
