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
    public class PfWmsApiSetting
    {
        public string ApiName { get; internal set; }

        public string UpdateItem { get; set; }

        /// <summary>
        /// 香港出库通知
        /// </summary>
        public string HKExitNotice { get; set; }

        /// <summary>
        /// 香港出库通知--重构
        /// </summary>
        public string CgHKExitNotice { get; set; }

        /// <summary>
        /// 深圳出库通知
        /// </summary>
        public string SzExitNotice { get; set; }

        /// <summary>
        /// 运输批次截单
        /// </summary>
        public string VoyageSureCut { get; set; }

        /// <summary>
        /// 深圳出库通知删除
        /// </summary>
        public string SZExitNoticeDelete { get; set; }
        /// <summary>
        /// 出入库价格传给 深圳库房
        /// </summary>
        public string SZPriceCompute { get; set; }
        public string UpdateInput { get; set; }
        public string UpdateOutput { get; set; }
        public string cgInternalOrders { get; set; }

        public PfWmsApiSetting()
        {
            ApiName = "PfWmsApi";
            UpdateItem = "Sortings/UpdateItem";
            SzExitNotice = "cgNotices/Out";
            VoyageSureCut = "cgDelcare/DelcareCutting";
            SZExitNoticeDelete = "cgNotices/CancleOutNoitce";
            CgHKExitNotice = "cgDelcare/AutoHkExitNotice";
            SZPriceCompute = "cgDelcare/SzPriceUpdate";
            UpdateInput = "cgDelcare/UpdateInput";
            UpdateOutput = "cgDelcare/UpdateOutput";
            cgInternalOrders = "cgInternalOrders/Enter";
        }
    }
}
