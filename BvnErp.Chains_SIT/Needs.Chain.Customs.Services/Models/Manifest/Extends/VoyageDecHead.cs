using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 运输批次的报关单
    /// </summary>
    public class VoyageDecHead : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 运输批次号
        /// </summary>
        public string VoyageNo { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int PackNo { get; set; }

        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal GQty { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal GrossWt { get; set; }

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DeclTotal { get; set; }

        /// <summary>
        /// 总型号的条数
        /// </summary>
        public int ItemsCount { get; set; }
    }
}
