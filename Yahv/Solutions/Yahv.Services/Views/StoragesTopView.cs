using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Layers.Data.Sqls.PvWms;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class StorageTop1 : IUnique
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

    public class StorageTop : IUnique
    {
        public string ID { get; set; }

        public StoragesType Type { get; set; }
        public string WareHouseID { get; set; }
        public string InputID { get; set; }
        public decimal? Total { get; set; }
        public decimal? Quantity { get; set; }
        public string Origin { get; set; }
        public bool IsLock { get; set; }
        public string ShelveID { get; set; }
        public string Supplier { get; set; }
        public string DateCode { get; set; }
        public string Summary { get; set; }
        public string CustomsName { get; set; }
        public string OrderID { get; set; }
        public string TinyOrderID { get; set; }
        public string ItemID { get; set; }
        public string TrackerID { get; set; }
        public Currency? Currency { get; set; }
        public decimal? UnitPrice { get; set; }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }
        public string PackageCase { get; set; }
        public string Packaging { get; set; }
        public string ProductID { get; set; }

        /// <summary>
        /// 客户ID    
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime EnterDate { get; set; }
    }

    public class StoragesTopView<TReponsitory> : UniqueView<StorageTop, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public StoragesTopView()
        {

        }
        public StoragesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<StorageTop> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<CgStoragesTopView>()
                   where entity.Type == (int)StoragesType.Inventory
                   select new StorageTop
                   {
                       ID = entity.ID,
                       Type = (StoragesType)entity.Type, //流水库、库存库、运营库、在途库、报废库、检测库、暂存库	
                       WareHouseID = entity.WareHouseID,
                       InputID = entity.InputID,
                       Total = entity.Total,
                       Quantity = entity.Quantity,
                       Origin = entity.Origin,
                       IsLock = entity.IsLock,
                       ShelveID = entity.ShelveID,
                       Supplier = entity.Supplier,
                       DateCode = entity.DateCode,
                       Summary = entity.Summary,
                       CustomsName = entity.CustomsName,
                       OrderID = entity.OrderID,
                       TinyOrderID = entity.TinyOrderID,
                       ItemID = entity.ItemID,
                       TrackerID = entity.TrackerID,
                       Currency = (Currency?)entity.Currency,
                       UnitPrice = entity.UnitPrice,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       PackageCase = entity.PackageCase,
                       Packaging = entity.Packaging,
                       ProductID = entity.ProductID,
                       ClientID = entity.ClientID,
                       ClientName = entity.ClientName,
                       EnterDate = entity.CreateDate
                   };


            

            //select new
            //{
            //    ID = entity.ID,
            //    Type = (StoragesType)entity.Type, //流水库、库存库、运营库、在途库、报废库、检测库、暂存库	
            //    WareHouseID = entity.WareHouseID, //库房编号
            //    SortingID = entity.SortingID, // 分拣编号:入库时是分拣编号，出库时是拣货编号
            //    InputID = entity.InputID, //进项
            //    ProductID = entity.ProductID, //产品编号
            //    IsLock = entity.IsLock, //是否锁定
            //    CreateDate = entity.CreateDate, //创建时间
            //    Status = (GeneralStatus)entity.Status, //状态：正常、废弃
            //    ShelveID = entity.ShelveID, //库位编号
            //    PartNumber = entity.PartNumber, //型号
            //    Manufacturer = entity.Manufacturer, //制造商
            //    PackageCase = entity.PackageCase, //封装
            //    Packaging = entity.Packaging, //包装 
            //    ClientID = entity.ClientID, //所属企业
            //    PurchaserID = entity.PurchaserID, //采购员
            //    SalerID = entity.SalerID, //AdminID
            //    TrackerID = entity.TrackerID, //跟单员
            //    Code = entity.Code, //全局唯一码
            //    OriginID = entity.OriginID,
            //    Origin = entity.Origin, //原产地
            //    Currency = (Currency?)(entity.Currency == null ? 0 : entity.Currency),
            //    UnitPrice = entity.UnitPrice,
            //    Supplier = entity.Supplier,
            //    DateCode = entity.DateCode,
            //    ItemID = entity.ItemID,
            //    OrderID = entity.OrderID,
            //    Quantity = entity.Quantity == null ? 0 : (decimal)entity.Quantity, //初始值

            //};
        }
    }
}
