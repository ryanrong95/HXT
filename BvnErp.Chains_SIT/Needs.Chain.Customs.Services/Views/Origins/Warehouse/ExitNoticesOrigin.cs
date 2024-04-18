using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class ExitNoticesOrigin : UniqueView<Models.ExitNotice, ScCustomsReponsitory>
    {
        public ExitNoticesOrigin()
        {
        }

        public ExitNoticesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExitNotice> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                   select new Models.ExitNotice
                   {
                       ID = item.ID,
                       WarehouseType = (Enums.WarehouseType)item.WarehouseType,
                       OrderId = item.OrderID,
                       DecHeadID = item.DecHeadID,
                       ExitType = (Enums.ExitType)item.ExitType,
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)item.ExitNoticeStatus,
                       AdminID = item.AdminID,
                       Status = (Enums.Status)item.Status,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate,
                       Summary = item.Summary,
                       IsPrintInt = item.IsPrint,
                       OutStockTime = item.OutStockTime,
                   };
        }
    }
}
