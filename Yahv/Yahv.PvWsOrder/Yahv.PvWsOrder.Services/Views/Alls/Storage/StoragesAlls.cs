using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Layers.Data.Sqls;

namespace Yahv.PvWsOrder.Services.Views
{
    public class StoragesAlls : UniqueView<Storage, PvWsOrderReponsitory>
    {
        public StoragesAlls()
        {

        }

        internal StoragesAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Storage> GetIQueryable()
        {
            var storagesTopView = new Yahv.Services.Views.StoragesTopView<PvWsOrderReponsitory>(this.Reponsitory).
                OrderByDescending(item => item.EnterDate);
            var wsClientTopView = new Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory);

            var linq = from entity in storagesTopView
                       join client in wsClientTopView on entity.ClientID equals client.ID
                       select new Storage
                       {
                           ID = entity.ID,
                           //Type = entity.Type, //流水库、库存库、运营库、在途库、报废库、检测库、暂存库	
                           WareHouseID = entity.WareHouseID, //库房编号
                           //SortingID = entity.SortingID, // 分拣编号:入库时是分拣编号，出库时是拣货编号
                           InputID = entity.InputID, //进项
                           ProductID = entity.ProductID, //产品编号
                           Supplier = entity.Supplier, //供应商
                           DateCode = entity.DateCode, //批次号
                           Quantity = (decimal)entity.Quantity, //入库时正值，出库时负值
                           Total = entity.Total,
                           //NoticeID = entity.NoticeID, //通知编号
                           IsLock = entity.IsLock, //是否锁定

                           EnterDate = entity.EnterDate, //入库时间

                           //Status = entity.Status, //状态：正常、废弃
                           ShelveID = entity.ShelveID, //库位编号
                           //Code = entity.Code, //全局唯一码
                           //OriginID = entity.OriginID,
                           ClientID = entity.ClientID, //所属企业
                           TrackerID = entity.TrackerID, //跟单员
                           //SalerID = entity.SalerID, //AdminID
                           //PurchaserID = entity.PurchaserID, //采购员
                           Origin = entity.Origin, //原产地
                           PartNumber = entity.PartNumber, //型号
                           Manufacturer = entity.Manufacturer, //制造商
                           PackageCase = entity.PackageCase, //封装
                           Packaging = entity.Packaging, //包装 
                           Currency = entity.Currency,//币种
                           UnitPrice = entity.UnitPrice,//单价

                           ClientName = client.Name,
                           EnterCode = client.EnterCode,
                       };
            return linq;
        }
    }
    /// <summary>
    /// //香港库存可供报关和发货
    /// </summary>
    public class HKStoragesAlls : StoragesAlls
    {
        public HKStoragesAlls()
        {

        }

        internal HKStoragesAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Storage> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       .Where(s=>s.WareHouseID.Contains("HK"))
                       select new Storage
                       {
                           ID = entity.ID,
                           WareHouseID = entity.WareHouseID, //库房编号
                           InputID = entity.InputID, //进项
                           ProductID = entity.ProductID, //产品编号
                           Supplier = entity.Supplier, //供应商
                           DateCode = entity.DateCode, //批次号
                           Quantity = (decimal)entity.Quantity, //入库时正值，出库时负值
                           IsLock = entity.IsLock, //是否锁定
                           EnterDate = entity.EnterDate, //入库时间
                           ShelveID = entity.ShelveID, //库位编号
                           ClientID = entity.ClientID, //所属企业
                           TrackerID = entity.TrackerID, //跟单员
                           Origin = entity.Origin, //原产地
                           PartNumber = entity.PartNumber, //型号
                           Manufacturer = entity.Manufacturer, //制造商
                           PackageCase = entity.PackageCase, //封装
                           Packaging = entity.Packaging, //包装 
                           Currency = entity.Currency,//币种
                           UnitPrice = entity.UnitPrice,//单价
                           ClientName = entity.ClientName,
                           EnterCode = entity.EnterCode,
                       };
            return linq;
        }
    }
}
