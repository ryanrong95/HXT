using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchViewModel:IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string CaseNo { get; set; }
        public string BatchNo { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Origin { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderItemID { get; set; }
        public string NewOrderItemID { get; set; }
        public string SortingID { get; set; }
        public SortingDecStatus SortingDecStatus { get; set; }
        public WarehouseType WarehouseType { get; set; }
        /// <summary>
        /// 订单项数量
        /// </summary>
        public decimal OrderItemQty { get; set; }
    }
}
