using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 银行账户管理类型
    /// </summary>
    public enum ManageType
    {
        /// <summary>
        /// 普通账户
        /// </summary>
        [Description("普通账户")]
        Normal = 1,

        /// <summary>
        /// 特殊账户
        /// </summary>
        [Description("特殊账户")]
        Special = 2,
    }
}
