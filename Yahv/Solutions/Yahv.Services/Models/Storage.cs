using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{

    public class _Input
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string OriginID { get; set; }
        public string OrderID { get; set; }
        public string TinyOrderID { get; set; }
        public string ItemID { get; set; }
        public string ProductID { get; set; }
        public string ClientID { get; set; }
        public string PayeeID { get; set; }
        public string ThirdID { get; set; }
        public string TrackerID { get; set; }
        public string SalerID { get; set; }
        public string PurchaserID { get; set; }
        public Currency? Currency { get; set; }
        public decimal? UnitPrice { get; set; }
        public string DateCode { get; set; }
        public string Origin { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class _Storage : IUnique
    {
        public string ID { get; set; }
        public StoragesType Type { get; set; }
        public string WareHouseID { get; set; }
        public string SortingID { get; set; }
        public string InputID { get; set; }
        public string ProductID { get; set; }
        public decimal? Total { get; set; }
        public decimal Quantity { get; set; }
        //public string NoticeID { get; set; }
        public string Origin { get; set; }
        //public string IsLock { get; set; }
        public DateTime CreateDate { get; set; }
        //public string Status { get; set; }
        public string ShelveID { get; set; }
        public string Supplier { get; set; }
        public string DateCode { get; set; }
        public string Summary { get; set; }
        public CenterProduct Product { get; set; }
        public _Input Input { get; set; }

        public decimal? UnitPrice { get; set; }
    }

    public class Storage : IUnique
    {
        /// <summary>
        /// 唯一码,四位年+2位月+2日+6位流水
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 流水库、库存库、运营库、在途库、报废库、检测库、暂存库	
        /// </summary>
        public StoragesType Type { get; set; }
        /// <summary>
        /// 库房编号
        /// </summary>
        public string WareHouseID { get; set; }
        /// <summary>
        ///  分拣编号:入库时是分拣编号，出库时是拣货编号
        /// </summary>
        public string SortingID { get; set; }
        /// <summary>
        /// 进项
        /// </summary>
        public string InputID { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }
        /// <summary>
        /// 入库时正值，出库时负值
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 通知编号
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLock { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 状态：正常、废弃
        /// </summary>
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 库位编号
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 上架状态
        /// </summary>
        public string ShelvesStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.ShelveID))
                {
                    return "未上架";
                }
                else return "已上架";
            }
        }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }
        /// <summary>
        /// 制造商
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
        /// 全局唯一码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 原inputid，拆项是用，不拆项时为空
        /// </summary>
        public string OriginID { get; set; }

        /// <summary>
        /// 所属企业
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 跟单员
        /// </summary>
        public string TrackerID { get; set; }
        /// <summary>
        /// AdminID
        /// </summary>
        public string SalerID { get; set; }
        /// <summary>
        /// 采购员
        /// </summary>
        public string PurchaserID { get; set; }

        public string OrderID { get; set; }

        public string ItemID { get; set; }


        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        public Currency? Currency { get; set; }

        public decimal? UnitPrice { get; set; }

    }



    public class StoreChange
    {
        public List<ChangeItem> List = new List<ChangeItem>();
    }

    public class ChangeItem
    {
        public string orderid { get; set; }
    }


}
