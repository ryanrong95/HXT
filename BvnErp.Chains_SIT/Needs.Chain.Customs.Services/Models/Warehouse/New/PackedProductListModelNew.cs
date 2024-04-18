using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{ 
    public class PackedProductListModelNew : IUnique
{
        /// <summary>
        /// SortingID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// PackingID
        /// </summary>
        public string PackingID { get; set; }

        //public Packing Packing { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal NetWeight { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal GrossWeight { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 状态：未封箱、已封箱
        /// </summary>
        public Needs.Ccs.Services.Enums.PackingStatus PackingStatus { get; set; }

        /// <summary>
        /// 特殊类型
        /// </summary>
        public Needs.Ccs.Services.Enums.ItemCategoryType SpecialType { get; set; }
        public decimal OrderItemQty { get; set; }
        public string OrderWaybillCode { get; set; }
        public string PvbCarrierID { get; set; }
        public string CarrierName { get; set; }
        public string OrderID { get; set; }
        public string Batch { get; set; }
    }
}
