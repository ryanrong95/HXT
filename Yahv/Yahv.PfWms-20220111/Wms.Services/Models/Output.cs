using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Underly;
using Yahv.Usually;

namespace Wms.Services.Models
{
    /// <summary>
    /// 销项信息
    /// </summary>
    public class Output : IUnique
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
        public Currency? Currency { get; set; }
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
    }
}
