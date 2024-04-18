using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class NoticesView : UniqueView<Notice, PsWmsRepository>
    {
        #region 构造函数
        public NoticesView()
        {
        }

        public NoticesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public NoticesView(PsWmsRepository reponsitory, IQueryable<Notice> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<Notice> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Notices>()          
                       select new Notice
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           CompanyID = entity.CompanyID,
                           ConsigneeID = entity.ConsigneeID,
                           ConsignorID = entity.ConsignorID,
                           FormID = entity.FormID,
                           NoticeType = (NoticeType)entity.NoticeType,
                           Status = (NoticeStatus)entity.Status,
                           WarehouseID = entity.WarehouseID,
                           WaybillID = entity.WaybillID,
                           TrackerID = entity.TrackerID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                           Exception = entity.Exception,
                       };
            return view;
        }
    }
}
