using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class ExitNoticesView : UniqueView<Models.ExitNotice, ScCustomsReponsitory>
    {
        public ExitNoticesView()
        {

        }

        public ExitNoticesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ExitNotice> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                   select new Models.ExitNotice
                   {
                       ID = entity.ID,
                       WarehouseType = (Enums.WarehouseType)entity.WarehouseType,
                       OrderID = entity.OrderID,
                       DecHeadID = entity.DecHeadID,
                       ExitType = (Enums.ExitType)entity.ExitType,
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)entity.ExitNoticeStatus,
                       AdminID = entity.AdminID,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                       IsPrint = entity.IsPrint,
                   };
        }
    }
}
