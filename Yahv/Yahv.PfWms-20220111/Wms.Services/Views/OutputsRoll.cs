using System.Linq;
using Layers.Data.Sqls;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Underly;

namespace Wms.Services.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputsRoll : UniqueView<Output, PvWmsRepository>
    {
        public OutputsRoll()
        {
        }

        public OutputsRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Output> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                   select new Output()
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       Currency = (Currency?)entity.Currency,
                       TinyOrderID = entity.TinyOrderID,
                       ItemID = entity.ItemID,
                       CreateDate = entity.CreateDate,
                       Price = entity.Price,
                       //Checker = entity.Checker
                       CustomerServiceID = entity.CustomerServiceID,
                       InputID = entity.InputID,
                       OwnerID = entity.OwnerID,
                       PurchaserID = entity.PurchaserID,
                       SalerID = entity.SalerID,
                       //StorageID = entity.st
                   };
        }
    }
}