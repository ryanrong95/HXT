using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    /// <summary>
    /// 账户类型 1 现金余额 2 信用
    /// </summary>
    public enum ClientAccountType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 现金余额
        /// </summary>
        Cash = 1,
        /// <summary>
        /// 信用
        /// </summary>
        Credit = 2
    }
}
