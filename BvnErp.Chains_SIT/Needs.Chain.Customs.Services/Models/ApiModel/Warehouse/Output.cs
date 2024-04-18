using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class Output
    {
        /// <summary>
        /// 四位年+2位月+2日+6位流水
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InputID { get; set; }
        /// <summary>
        /// MainID
        /// </summary>
        public string OrderID { get; set; }


        /// <summary>
        /// 小订单编号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 项ID
        /// </summary>
        public string ItemID { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 销售员
        /// </summary>
        public string SalerID { get; set; }
        /// <summary>
        /// 跟单员
        /// </summary>
        public string CustomerServiceID { get; set; }
        /// <summary>
        /// 采购员编号
        /// </summary>
        public string PurchaserID { get; set; }
        /// <summary>
        /// 保值
        /// </summary>
        public Needs.Ccs.Services.Models.ApiModels.Warehouse.Currency? Currency { get; set; }
        /// <summary>
        /// 保值
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 出库的库存ID
        /// </summary>
        public string StorageID { get; set; }


        public DateTime CreateDate { get; set; }

        public string Checker { get; set; }

        /// <summary>
        /// 跟单员ID 20200616 陈翰要求修改字段名称
        /// </summary>
        public string TrackerID { get; set; }
        
    }
}
