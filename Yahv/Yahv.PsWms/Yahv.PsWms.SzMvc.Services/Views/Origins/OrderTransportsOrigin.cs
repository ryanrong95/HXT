using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class OrderTransportsOrigin : UniqueView<Models.Origin.OrderTransport, PsOrderRepository>
    {
        #region 构造函数
        public OrderTransportsOrigin()
        {
        }

        public OrderTransportsOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.OrderTransport> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.OrderTransports>()
                       select new Models.Origin.OrderTransport
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           TransportMode = (TransportMode)entity.TransportMode,
                           Carrier = entity.Carrier,
                           WaybillCode = entity.WaybillCode,
                           ExpressPayer = (FreightPayer)entity.ExpressPayer,
                           ExpressTransport = entity.ExpressTransport,
                           ExpressEscrow = entity.ExpressEscrow,
                           PickerID = entity.PickerID,
                           TakingTime = entity.TakingTime,
                           Address = entity.Address,
                           Contact = entity.Contact,
                           Phone = entity.Phone,
                           Email = entity.Email,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                       };
            return view;
        }
    }
}
