using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class UnPackedHKEntryNoticeItem
    {
        public string ID { get; set; }
        public string OrderItemID { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        /// <summary>
        /// 应到数量--订单数量
        /// </summary>
        public decimal OrderItemQty { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public decimal? RelQty { get; set; }
        /// <summary>
        /// 已装箱数量
        /// </summary>
        public decimal PackedQty
        {
            get
            {
                if (this.RelQty == null)
                {
                    return this.OrderItemQty;
                }
                else
                {
                    return this.OrderItemQty - this.RelQty.Value;
                }
                
            }
        }
        public string Origin { get; set; }
        public string OriginText { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        public ItemCategoryType OrderItemType { get; set; }
    }
}
