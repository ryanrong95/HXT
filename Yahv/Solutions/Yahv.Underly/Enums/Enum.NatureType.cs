using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 银行账户性质类型
    /// </summary>
    public enum NatureType
    {
        /// <summary>
        /// 公司账户
        /// </summary>
        [Description("公司账户")]
        Public = 1,

        /// <summary>
        /// 个人账户
        /// </summary>
        [Description("个人账户")]
        Private = 2,

        /// <summary>
        /// 未知
        /// </summary>
        [Description("")]
        UnKnown = 10000,
    }
}
