using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CgDeliveriesTopViewModel:IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 箱号
        /// </summary>
        public string CaseNo { get; set; }      
        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal? TotalPrice { get; set; }
        public string OrderItemID { get; set; }
        /// <summary>
        /// 小订单号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 大订单号
        /// </summary>
        public string MainOrderID { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNo { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string Batch { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 仓库类型
        /// 200:流水库
        /// 300:库存库
        /// </summary>
        public int Type { get; set; }
    }
}
