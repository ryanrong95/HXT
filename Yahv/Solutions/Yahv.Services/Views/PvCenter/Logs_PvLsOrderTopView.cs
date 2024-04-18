using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Views
{
    public class Logs_PvLsOrderTopView<TReponsitory> : UniqueView<Logs_PvLsOrder, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public Logs_PvLsOrderTopView()
        {

        }
        public Logs_PvLsOrderTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Logs_PvLsOrder> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_PvLsOrderTopView>()
                   select new Logs_PvLsOrder
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       Type = (LsOrderStatusType)entity.Type,
                       Status = entity.Status,
                       IsCurrent = entity.IsCurrent,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                   };
        }
    }

    public class Logs_PvLsOrderCurrentTopView<TReponsitory> : UniqueView<LsOrderCurrentStatus, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public Logs_PvLsOrderCurrentTopView()
        {

        }
        public Logs_PvLsOrderCurrentTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<LsOrderCurrentStatus> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_PvLsOrderCurrentTopView>()
                   select new LsOrderCurrentStatus
                   {
                       ID = entity.MainID,
                       MainStatus = entity.MainStatus,
                       InvoiceStatus = entity.InvoiceStatus,
                   };
        }
    }
}
