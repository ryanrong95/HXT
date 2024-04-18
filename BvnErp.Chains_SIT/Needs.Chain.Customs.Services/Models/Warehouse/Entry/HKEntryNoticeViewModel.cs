using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class HKEntryNoticeViewModel
    {
        public string ID { get; set; }

        /// <summary>
        /// 客户订单
        /// </summary>
        public string OrderID { get; set; }
      

        public string DecHeadID { get; set; }
     
        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }
        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 分拣要求
        /// </summary>
        public virtual Enums.SortingRequire SortingRequire { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public Enums.WarehouseType WarehouseType { get; set; }

        public Enums.EntryNoticeStatus EntryNoticeStatus { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        public string ClientSupplierName { get; set; }
        public Enums.HKDeliveryType HKDeliveryType { get; set; }

        public IEnumerable<HKEntryNoticeItem> HKItems { get; set; }
        public string WayBillNo { get; set; }
    }
}
