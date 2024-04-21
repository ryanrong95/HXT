using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace YaHv.Csrm.Services
{
    /// <summary>
    /// 合作类型
    /// </summary>
    [Flags]
    public enum CooperType
    {
        [Description("未知")]
        None = 0,

        /// <summary>
        /// 线上销售业务
        /// </summary>
        [Description("线上销售业务")]
        Online = 2,
        /// <summary>
        /// 传统贸易
        /// </summary>
        [Description("传统贸易")]
        OldTrade = 2 << 1,

        /// <summary>
        /// 采购业务
        /// </summary>
        [Description("采购业务", "近")]
        Agent = 2 << 2,
    }
}
