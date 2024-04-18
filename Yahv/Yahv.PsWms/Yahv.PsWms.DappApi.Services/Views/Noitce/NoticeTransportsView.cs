using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.PsWms.DappApi.Services.Enums;
using Layers.Linq;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class NoticeTransportsView : UniqueView<NoticeTransport, PsWmsRepository>
    {
        public NoticeTransportsView()
        {
        }

        public NoticeTransportsView(PsWmsRepository reponsitory):base(reponsitory)
        {
        }

        protected override IQueryable<NoticeTransport> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeTransports>()                     
                       select new NoticeTransport
                       {
                           ID = entity.ID,
                           TransportMode = (TransportMode)entity.TransportMode,
                           Carrier = entity.Carrier,
                           WaybillCode = entity.WaybillCode,
                           TrackingCode = entity.TrackingCode,
                           ExpressPayer = (FreightPayer)entity.ExpressPayer,
                           ExpressTransport = entity.ExpressTransport,
                           ExpressEscrow = entity.ExpressEscrow,
                           ExpressFreight = entity.ExpressFreight,
                           TakingTime = entity.TakingTime,
                           TakerName = entity.TakerName,
                           TakerLicense = entity.TakerLicense,
                           TakerPhone = entity.TakerPhone,
                           TakerIDType = (IDType?)entity.TakerIDType,
                           TakerIDCode = entity.TakerIDCode,
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
