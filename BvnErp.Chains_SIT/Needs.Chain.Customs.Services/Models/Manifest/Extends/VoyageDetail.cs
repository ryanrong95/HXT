using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 运输批次明细
    /// </summary>
    public class VoyageDetail : IUnique
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
        /// 装箱日期
        /// </summary>
        public DateTime PackingDate { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; }

        /// <summary>
        /// 条数
        /// </summary>
        public int ItemsCount { get; set; }
    }
}
