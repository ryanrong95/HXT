using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    public class Folder
    {
        /// <summary>
        /// 报关单主目录
        /// </summary>
        public string DecMainFolder { get; set; }

        /// <summary>
        /// 舱单主目录
        /// </summary>
        public string RmftMainFolder { get; set; }

        /// <summary>
        /// 其它
        /// </summary>
        public string OtherMainFolder { get; set; }
    }
}
