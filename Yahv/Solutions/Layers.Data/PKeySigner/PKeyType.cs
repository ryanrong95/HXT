using Layers.Linq;
using Layers.Data.Sqls;
using System;

namespace Layers.Data
{
    /// <summary>
    /// 主键类型
    /// </summary>
    [Obsolete("这是示例，把类似这样的东西放在逻辑层")]
    enum PKeyType
    {

        /// <summary>
        /// 订单
        /// </summary>
        [Repository(typeof(PvbErmReponsitory))]
        [PKey("Admin", PKeySigner.Mode.Date, 10)]
        Order = 10000,

    }
}
