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
    public class PvDataApiSetting
    {
        public string ApiName { get; internal set; }

        /// <summary>
        /// 产品保存
        /// </summary>
        public string SaveProduct { get; private set; }

        /// <summary>
        /// 自动归类
        /// </summary>
        public string AutoClassify { get; private set; }

        /// <summary>
        /// 锁定
        /// </summary>
        public string Lock { get; private set; }

        /// <summary>
        /// 同步归类数据
        /// </summary>
        public string SyncClassified { get; private set; }

        public PvDataApiSetting()
        {
            SaveProduct = "Product/GetIdByInfo";
            AutoClassify = "Classify/GetAutoClassified";
            Lock = "Classify/Lock";
            ApiName = "PvDataApiUrl";
            SyncClassified = "ClassifySync/SyncClassified";
        }
    }
}
