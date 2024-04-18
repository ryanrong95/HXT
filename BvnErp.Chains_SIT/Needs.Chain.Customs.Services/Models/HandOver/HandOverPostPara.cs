using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class HandOverPostPara
    {
        /// <summary>
        /// 移交人ID
        /// </summary>
        public string AdminLeave { get; set; }
        /// <summary>
        /// 承接人ID
        /// </summary>
        public string AdminWork { get; set; }
        /// <summary>
        /// 请假ID
        /// </summary>
        public string ApplyID { get; set; }
    }
}
