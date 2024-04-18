using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.PdaApi.Services.Models;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    /// <summary>
    /// 运单基本信息视图
    /// </summary>
    public class WaybillInfosView : NoticeTransportsView
    {
        public WaybillInfosView()
        {
        }

        internal WaybillInfosView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        internal WaybillInfosView(PsWmsRepository reponsitory, IQueryable<NoticeTransport> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public override object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<NoticeTransport>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            #region 补充完整对象

            var ienum_iquery = iquery.ToArray();

            //运费信息
            var waybillCodes = ienum_iquery.Select(item => item.WaybillCode);
            var waybillsView = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Waybills>()
                               where waybillCodes.Contains(waybill.Code)
                               select new
                               {
                                   waybill.ID,
                                   waybill.Code,
                                   waybill.Freight,
                                   waybill.Weight
                               };
            var ienum_waybills = waybillsView.ToArray();

            var linq = from transport in ienum_iquery
                       join waybill in ienum_waybills on transport.WaybillCode equals waybill.Code into waybills
                       from waybill in waybills.DefaultIfEmpty()
                       select new
                       {
                           transport.ID,
                           transport.WaybillCode,
                           transport.Carrier,
                           waybill?.Freight,
                           waybill?.Weight
                       };

            #endregion

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                return new
                {
                    Total = total,
                    Size = pageSize,
                    Index = pageIndex,
                    Data = linq.ToArray(),
                };
            }
            else
            {
                return linq.ToArray();
            }
        }

        #region 搜索方法
        /// <summary>
        /// 根据运单号查询
        /// </summary>
        /// <param name="waybillCode"></param>
        /// <returns></returns>
        public WaybillInfosView SearchByWaybillCode(string waybillCode)
        {
            var noticeTransportsView = this.IQueryable.Cast<NoticeTransport>();
            var linq = from transaport in noticeTransportsView
                       where transaport.WaybillCode == waybillCode
                       select transaport;

            var view = new WaybillInfosView(this.Reponsitory, linq);
            return view;
        }
        #endregion
    }
}
