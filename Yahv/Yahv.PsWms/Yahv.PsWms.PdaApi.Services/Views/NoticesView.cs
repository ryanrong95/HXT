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
    /// <summary>
    /// 通知视图
    /// </summary>
    public class NoticesView : UniqueView<Notice, PsWmsRepository>
    {
        public NoticesView()
        {
        }

        protected NoticesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected NoticesView(PsWmsRepository reponsitory, IQueryable<Notice> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Notice> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Notices>()
                       select new Notice
                       {
                           ID = entity.ID,
                           WarehouseID = entity.WarehouseID,
                           ClientID = entity.ClientID,
                           CompanyID = entity.CompanyID,
                           NoticeType = (NoticeType)entity.NoticeType,
                           FormID = entity.FormID,
                           ConsigneeID = entity.ConsigneeID,
                           ConsignorID = entity.ConsignorID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Status = (NoticeStatus)entity.Status,
                           WaybillID = entity.WaybillID,
                           TrackerID = entity.TrackerID,
                           Summary = entity.Summary,
                           Exception = entity.Exception,
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
            var iquery = this.IQueryable.Cast<Notice>();
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
