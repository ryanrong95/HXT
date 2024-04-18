using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Views
{
    public class WsOrderStatusTopView<TReponsitory> : UniqueView<Logs_PvWsOrder, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public WsOrderStatusTopView()
        {

        }
        public WsOrderStatusTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Logs_PvWsOrder> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WsOrderStatusTopView>()
                   select new Logs_PvWsOrder
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       Type = (OrderStatusType)entity.Type,
                       Status = entity.Status,
                       IsCurrent = entity.IsCurrent,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                   };
        }
    }

    public class CurrentWsOrderStatusTopView<TReponsitory> : UniqueView<WsOrderCurrentStatus, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CurrentWsOrderStatusTopView()
        {

        }
        public CurrentWsOrderStatusTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<WsOrderCurrentStatus> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.CurrentWsOrderStatusTopView>()
                   select new WsOrderCurrentStatus
                   {
                       ID = entity.MainID,
                       MainStatus = entity.MainStatus,
                       PaymentStatus = entity.PaymentStatus,
                       InvoiceStatus = entity.InvoiceStatus,
                       RemittanceStatus = entity.RemittanceStatus
                   };
        }
    }
}
