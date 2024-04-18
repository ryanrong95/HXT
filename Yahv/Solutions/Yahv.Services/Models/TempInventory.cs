using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class TempInventory : IUnique
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
        /// 分拣ID
        /// </summary>
        public string SortingID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 暂存的总数量
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 暂存当前的剩余数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public Origin Origin { get; set; }

        /// <summary>
        /// 是否加锁
        /// </summary>
        public bool IsLock { get; set; }

        /// <summary>
        /// 库存创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

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
        /// 库存备注信息
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomsName { get; set; }

        /// <summary>
        /// 通知ID, NoticeID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 分拣对应的WaybillID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 分拣装箱箱号
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 分拣操作人员ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal NetWeight { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal Volume { get; set; }

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
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }
    }
}
