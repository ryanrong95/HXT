using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.chonggous.Models
{
    /// <summary>
    /// Msg Model
    /// </summary>
    public class MsgDTO
    {
        /// <summary>
        /// 客户编号
        /// </summary>
        public string clientCode { get; set; }

        /// <summary>
        /// 代报关的值为 1， 代仓储的值是2
        /// </summary>
        public int systemID { get; set; }

        /// <summary>
        /// 节点
        /// </summary>
        public int spotType { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string orderID { get; set; }

        /// <summary>
        /// 发货运单号(深圳库房发快递时使用)
        /// </summary>
        public string waybillNo { get; set; }

        /// <summary>
        /// 送货司机(深圳库房 送货司机姓名)
        /// </summary>
        public string driverName { get; set; }

        /// <summary>
        /// 电话 (深圳库房 送货司机电话)
        /// </summary>
        public string driverPhone { get; set; }
    }
}
