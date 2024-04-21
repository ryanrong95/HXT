using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments.Models.Rolls
{
    /// <summary>
    /// 现金
    /// </summary>
    public class Cash : AccountInfo
    {
        ///<summary>
        /// 可用的
        /// </summary>
        public decimal Available { get; internal set; }
    }
}
