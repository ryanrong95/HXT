using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.WL.ConsoleToolApp
{
    public class UnSortingOrder
    {
        public string OrderID { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 快递公司ID
        /// </summary>
        public string CarrierID { get; set; }
        /// <summary>
        /// 快递公司名称
        /// </summary>
        public string CarrierName { get; set; }
        /// <summary>
        /// 装箱状态
        /// 110-等待分捡
        /// 115-部分入库
        /// </summary>
        public int ExcuteStatus { get; set; }
        public string EnterCode { get; set; }
    }
}
