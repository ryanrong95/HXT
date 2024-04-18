using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    public sealed class MySZExitNoticesView : SZExitNoticeView
    {
        IGenericAdmin Admin;

        public MySZExitNoticesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.SZExitNotice> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from exitOrder in base.GetIQueryable()
                   where exitOrder.Admin.ID == this.Admin.ID
                   select exitOrder;
        }
    }

    public sealed class MyCenterSZExitNoticesView : CenterSZExitNoticeView
    {
        IGenericAdmin Admin;

        public MyCenterSZExitNoticesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.SZExitNotice> GetIQueryable()
        {
            //return base.GetIQueryable();
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            //是否有交接记录
            var adminLeaveIDs = new HandOverView().Where(t => t.AdminWork == this.Admin.ID && t.Status == Ccs.Services.Enums.Status.Normal).Select(t => t.AdminLeave).Distinct().ToList();
            adminLeaveIDs.Add(this.Admin.ID);

            return from exitOrder in base.GetIQueryable()
                   where adminLeaveIDs.Contains(exitOrder.Admin.ID)
                   select exitOrder;
        }
    }

    public sealed class MyNewCenterSZExitNoticesView: DeliveryOrderOriginView
    {
        IGenericAdmin Admin;

        public MyNewCenterSZExitNoticesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.DeliveryOrder> GetIQueryable()
        {
            //return base.GetIQueryable();
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            //是否有交接记录
            var adminLeaveIDs = new HandOverView().Where(t => t.AdminWork == this.Admin.ID && t.Status == Ccs.Services.Enums.Status.Normal).Select(t => t.AdminLeave).Distinct().ToList();
            adminLeaveIDs.Add(this.Admin.ID);

            return from exitOrder in base.GetIQueryable()
                   where adminLeaveIDs.Contains(exitOrder.Admin.ID)
                   select exitOrder;
        }
    }
}
