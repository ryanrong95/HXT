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
    public class PvWmsApiSetting
    {
        public string ApiName { get; internal set; }

        /// <summary>
        /// 生成入库通知
        /// </summary>
        public string CgNoticeEnter { get; private set; }

        /// <summary>
        /// 生成出库通知
        /// </summary>
        public string CgOutNotice { get; private set; }

        /// <summary>
        /// 报关装箱通知
        /// </summary>
        public string CgBoxingNotice { get; private set; }

        /// <summary>
        /// 取消出库通知
        /// </summary>
        public string CgCancelOutNotice { get; private set; }

        /// <summary>
        /// 租赁通知
        /// </summary>
        public string LsNotice { get; private set; }

        /// <summary>
        /// 库房锁库存
        /// </summary>
        public string LockStore { get; private set; }

        /// <summary>
        /// 库房解锁库存
        /// </summary>
        public string CancelLockStore { get; private set; }

        /// <summary>
        /// 库房更新在库数量和可用数量
        /// </summary>
        public string CgStorageUpdate { get; private set; }

        public PvWmsApiSetting()
        {
            ApiName = "ApiWmsUrl";
            LsNotice = "lsnotice/submit";
            LockStore = "Pickings/LockStore";
            CancelLockStore = "Pickings/CancelLockStore";
            CgNoticeEnter = "cgNotices/In";
            CgOutNotice = "cgNotices/Out";
            CgCancelOutNotice = "cgNotices/CancleOutNoitce";
            CgBoxingNotice = "cgNotices/Boxing";
            CgStorageUpdate = "cgStorages/UpdateDeliveredQty";
        }
    }
}
