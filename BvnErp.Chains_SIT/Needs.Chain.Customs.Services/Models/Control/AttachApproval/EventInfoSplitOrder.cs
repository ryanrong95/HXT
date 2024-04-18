using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class EventInfoSplitOrder
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApplyAdminName { get; set; }

        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public List<string> Packs { get; set; }
    }
}
