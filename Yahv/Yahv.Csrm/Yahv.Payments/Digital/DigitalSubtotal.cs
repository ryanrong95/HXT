using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 数字账户
    /// </summary>
    public class DigitalSubtotal
    {
        ///<summary>
        /// 可用的
        /// </summary>
        public decimal Available { get; internal set; }

        public Currency Currency { get; set; }
    }
}
