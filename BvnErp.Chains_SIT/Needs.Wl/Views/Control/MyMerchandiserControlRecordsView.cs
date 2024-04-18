using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 跟单员管控记录的视图（Admin过滤）
    /// </summary>
    public class MyMerchandiserControlRecordsView : MerchandiserControlRecordsView
    {
        IGenericAdmin Admin;

        public MyMerchandiserControlRecordsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.OrderControlRecord> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            //是否有交接记录
            var adminLeaveIDs = new HandOverView().Where(t => t.AdminWork == this.Admin.ID && t.Status == Ccs.Services.Enums.Status.Normal).Select(t => t.AdminLeave).Distinct().ToList();
            adminLeaveIDs.Add(this.Admin.ID);

            return from record in base.GetIQueryable()
                   where adminLeaveIDs.Contains(record.Admin.ID) &&
                        record.Status == Ccs.Services.Enums.Status.Normal

                   select record;
        }
    }
}
