using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments.Models.Rolls
{
    /// <summary>
    /// 信用信息
    /// </summary>
    public class Credit : AccountInfo
    {
        /// <summary>
        /// 信用总额
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 信用花费
        /// </summary>
        public decimal Cost { get; set; }
    }
}
