using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class UserExitNoticeView : UniqueView<Models.ExitNotice, ScCustomsReponsitory>
    {
        public UserExitNoticeView()
        {
        }

        public UserExitNoticeView(ScCustomsReponsitory reponsitory ) : base(reponsitory)
        {

        }

        /// <summary>
        /// 深圳出库通知View(自提)
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.ExitNotice> GetIQueryable()
        {
            return from exitNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                   join exitDeliver in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>() on exitNotice.ID equals exitDeliver.ExitNoticeID
                   where exitNotice.ExitType== (int)ExitType.PickUp && exitNotice.WarehouseType==(int)WarehouseType.ShenZhen && exitNotice.Status== (int)Status.Normal
                   select new Models.SZExitNotice
                   {
                       ID = exitNotice.ID,
                       OrderId = exitNotice.OrderID,
                       ExitDeliver = new ExitDeliver
                       {
                           ID= exitNotice.ID,
                           PackNo=exitDeliver.PackNo
                       },
                   };
        }
    }

}
