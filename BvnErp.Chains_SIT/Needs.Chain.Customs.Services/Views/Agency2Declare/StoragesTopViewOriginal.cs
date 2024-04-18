using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class StoragesTopOriginalView : UniqueView<Models.StorageTop, ScCustomsReponsitory>
    {
        public StoragesTopOriginalView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public StoragesTopOriginalView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.StorageTop> GetIQueryable()
        {
            var clientView = new ClientsView(this.Reponsitory);

            var result = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgStoragesTopView>()
                         join client in clientView on entity.ClientName equals client.Company.Name into clients
                         from client in clients.DefaultIfEmpty()
                         where entity.Type == (int)StoragesType.Inventory && entity.Quantity>=1
                         select new Models.StorageTop
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
                             Currency = (ClientCurrency?)entity.Currency,
                             UnitPrice = entity.UnitPrice,
                             PartNumber = entity.PartNumber,
                             Manufacturer = entity.Manufacturer,
                             PackageCase = entity.PackageCase,
                             Packaging = entity.Packaging,
                             ProductID = entity.ProductID,
                             ClientID = entity.ClientID,
                             ClientName = entity.ClientName,
                             EnterDate = entity.CreateDate,
                             MyClient = client==null?null:client,
                         };
            return result;
        }
    }
}
