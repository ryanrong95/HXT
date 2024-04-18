using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils;

namespace Needs.Underly
{
    public enum ApplySource
    {
        /// <summary>
        /// 信用
        /// </summary>
        Credit = 1,
        /// <summary>
        /// 信用提额
        /// </summary>
        AddCredit = 2,
        /// <summary>
        /// 换货
        /// </summary>
        Replace=3,
        /// <summary>
        /// 退货
        /// </summary>        
        Return=4
    }
}
