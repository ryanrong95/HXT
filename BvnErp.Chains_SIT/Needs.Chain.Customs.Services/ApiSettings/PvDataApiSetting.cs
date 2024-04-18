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
    public class PvDataApiSetting
    {
        /// <summary>
        /// 中心数据接口地址
        /// </summary>
        public string ApiName { get; private set; }

        /// <summary>
        /// 获取自动归类数据
        /// </summary>
        public string AutoClassify { get; private set; }

        /// <summary>
        /// 锁定
        /// </summary>
        public string Lock { get; private set; }

        /// <summary>
        /// 解锁
        /// </summary>
        public string UnLock { get; private set; }

        /// <summary>
        /// 预归类锁定
        /// </summary>
        public string PreLock { get; private set; }

        /// <summary>
        /// 预归类解锁
        /// </summary>
        public string PreUnLock { get; private set; }

        /// <summary>
        /// 预归类退回
        /// </summary>
        public string PreReturn { get; private set; }

        /// <summary>
        /// 删除系统管控信息
        /// </summary>
        public string DeleteSysControl { get; private set; }

        /// <summary>
        /// 更新Ccc管控历史记录
        /// </summary>
        public string UpdateCccControl { get; private set; }

        /// <summary>
        /// 更新禁运管控历史记录
        /// </summary>
        public string UpdateEmbargoControl { get; private set; }

        /// <summary>
        /// 产品报价
        /// </summary>
        public string ProductQuote { get; private set; }

        /// <summary>
        /// 判断产地是否需要消毒/检疫
        /// </summary>
        public string GetOriginDisinfection { get; private set; }

        /// <summary>
        /// 获取原产地附加税率
        /// </summary>
        public string GetOriginATRate { get; private set; }

        /// <summary>
        /// 获取海关税则
        /// </summary>
        public string GetTariff { get; private set; }

        /// <summary>
        /// 批量获取型号的管控信息
        /// </summary>
        public string GetMultiSysControls { get; private set; }

        /// <summary>
        /// 同步归类数据
        /// </summary>
        public string SyncClassified { get; private set; }

        /// <summary>
        /// 同步税费变更数据
        /// </summary>
        public string SyncTaxChanged { get; private set; }

        public PvDataApiSetting()
        {
            ApiName = "PvDataApiUrl";
            AutoClassify = "Classify/GetAutoClassified";
            Lock = "Classify/Lock";
            UnLock = "Classify/UnLock";
            PreLock = "PreClassify/Lock";
            PreUnLock = "PreClassify/UnLock";
            PreReturn = "PreClassify/Return";
            DeleteSysControl = "Classify/DeleteSysControl";
            UpdateCccControl = "Classify/UpdateCccControl";
            UpdateEmbargoControl = "Classify/UpdateEmbargoControl";
            ProductQuote = "Product/Quote";
            GetOriginDisinfection = "Classify/GetOriginDisinfection";
            GetOriginATRate = "Classify/GetOriginATRate";
            GetTariff = "Classify/GetTariff";
            GetMultiSysControls = "Classify/GetMultiSysControls";
            SyncClassified = "ClassifySync/SyncClassified";
            SyncTaxChanged = "ClassifySync/SyncTaxChanged";
        }
    }
}
