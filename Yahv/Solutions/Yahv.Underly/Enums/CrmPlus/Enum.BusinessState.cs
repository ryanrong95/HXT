using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.CrmPlus
{
    /// <summary>
    /// 企业经营状态。
    /// </summary>
    public enum BusinessState
    {
        /// <summary>
        /// 存续
        /// </summary>
        [Description("存续")]
        Subsisting,

        /// <summary>
        /// 开业
        /// </summary>
        [Description("开业")]
        Opened,

        /// <summary>
        /// 吊销
        /// </summary>
        [Description("吊销")]
        Revoke,

        /// <summary>
        /// 
        /// </summary>
        [Description("注销")]
        Logout,

        /// <summary>
        /// 迁出
        /// </summary>
        [Description("迁出")]
        Moveout,

        /// <summary>
        /// 迁入
        /// </summary>
        [Description("迁入")]
        Movein,

        /// <summary>
        /// 停业
        /// </summary>
        [Description("停业")]
        Closed,

        /// <summary>
        /// 清算
        /// </summary>
        [Description("清算")]
        Liquidation,
    }
}
