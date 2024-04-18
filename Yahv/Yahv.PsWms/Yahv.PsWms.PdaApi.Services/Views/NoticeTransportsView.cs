using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.PdaApi.Services.Enums;
using Yahv.PsWms.PdaApi.Services.Models;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    public class NoticeTransportsView : UniqueView<NoticeTransport, PsWmsRepository>
    {
        public NoticeTransportsView()
        {
        }

        protected NoticeTransportsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected NoticeTransportsView(PsWmsRepository reponsitory, IQueryable<NoticeTransport> iQueryable) : base(reponsitory, iQueryable)
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
                           ExpressPayer = (FreightPayer)entity.ExpressPayer,
                           ExpressTransport = entity.ExpressTransport,
                           ExpressEscrow = entity.ExpressEscrow,
                           ExpressFreight = entity.ExpressFreight,
                           TakingTime = entity.TakingTime,
                           TakerName = entity.TakerName,
                           TakerLicense = entity.TakerLicense,
                           TakerPhone = entity.TakerPhone,
                           TakerIDType = (IDType)entity.TakerIDType,
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

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        public object Single()
        {
            return (this.ToMyPage() as object[]).SingleOrDefault();
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<NoticeTransport>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);

                return new
                {
                    Total = total,
                    Size = pageSize,
                    Index = pageIndex,
                    Data = iquery.ToArray(),
                };
            }
            else
            {
                return iquery.ToArray();
            }
        }
    }
}
