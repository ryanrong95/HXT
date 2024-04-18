using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    public class InsideOrdersWaybill:Waybill
    {
        /// <summary>
        /// 自动处理下一个运单
        /// </summary>
        public bool IsAuto { get; set; }

        public Notice[] Notices { get; set; }
        
    }
}
