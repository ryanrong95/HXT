using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Services
{
    public enum UserStatus
    {
        /// <summary>
        /// 待激活
        /// </summary>
        [Description("待激活")]
        Activing = 100,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 400
    }
}
