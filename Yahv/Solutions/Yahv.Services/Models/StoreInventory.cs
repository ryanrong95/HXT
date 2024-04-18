using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 库存清单类
    /// </summary>
    public class StoreInventory : IUnique
    {
        /// <summary>
        /// StorageID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 库房编号
        /// </summary>
        public string WareHouseID { get; set; }

        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 库存的总数量
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// 库存当前的数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 是否加锁，当前没有用到
        /// </summary>
        public bool IsLock { get; set; }

        /// <summary>
        /// 货架ID
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 产品批次
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 库存备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomsName { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 跟单员ID
        /// </summary>
        public string TrackerID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        public string PackageCase { get; set; }

        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }
        
        /// <summary>
        /// 进入库存时间
        /// </summary>
        public DateTime CreateDate { get; set; }


        /// <summary>
        /// 包装
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string ClientName { get; set; }
    }
}
