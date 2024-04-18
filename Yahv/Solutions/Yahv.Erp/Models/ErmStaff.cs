using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Models
{
    /// <summary>
    /// 员工
    /// </summary>
    public class ErmStaff 
    {
        /// <summary>
        /// ID
        /// </summary>
        internal string ID { get; set; }

        /// <summary>
        /// 大赢家ID
        /// </summary>
        public string DYJID { get; internal set; }
    }
}
