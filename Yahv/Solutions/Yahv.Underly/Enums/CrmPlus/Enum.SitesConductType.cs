using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// Site下控业务类型
    /// </summary>
    public enum SitesConductType
    {
        /// <summary>
        /// 报关业务
        /// </summary>
        [Description("报关业务")]
        Declarings,

        /// <summary>
        /// 检测业务
        /// </summary>
        [Description("检测业务")]
        Testings,

        /// <summary>
        /// 库存代售业务
        /// </summary>
        [Description("库存代售业务")]
        Consignings,

        /// <summary>
        /// 代仓储
        /// </summary>
        [Description("代仓储")]
        Stoagings,

        /// <summary>
        /// 网站推广
        /// </summary>
        [Description("网站推广")]
        Spreading,

        /// <summary>
        /// 其他服务
        /// </summary>
        [Description("其他服务")]
        Others,

        /// <summary>
        /// 贸易
        /// </summary>
        [Description("贸易")]
        Tradings
    }
}
