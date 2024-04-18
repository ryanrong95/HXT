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
   /// 跟单员管控的视图（Admin过滤）
   /// </summary>
    public class RiskControlApprovalView : MerchandiserControlsView
    {
        IGenericAdmin Admin;
        public RiskControlApprovalView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.MerchandiserControl> GetIQueryable()
        {
            return from control in base.GetIQueryable()
                   where control.Status == Ccs.Services.Enums.Status.Normal
                   && control.ControlType == Ccs.Services.Enums.OrderControlType.ExceedLimit

                   select control;
        }
    }

    public class RiskControlApprovalNotHangUpView : MerchandiserControlsNotHangUpView
    {
        IGenericAdmin Admin;
        public RiskControlApprovalNotHangUpView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.MerchandiserControl> GetIQueryable()
        {
            return from control in base.GetIQueryable()
                   where control.Status == Ccs.Services.Enums.Status.Normal
                   && control.ControlType == Ccs.Services.Enums.OrderControlType.ExceedLimit
                   select control;
        }
    }
    public class RiskControlApprovalNotHangUpView1 : MerchandiserControlsNotHangUpView
    {
        IGenericAdmin Admin;
        public RiskControlApprovalNotHangUpView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.MerchandiserControl> GetIQueryable()
        {
            return from control in base.GetIQueryable()
                   where control.Status == Ccs.Services.Enums.Status.Normal
                   && (control.ControlType == Ccs.Services.Enums.OrderControlType.ExceedLimit ||
                   control.ControlType == Ccs.Services.Enums.OrderControlType.OverdueAdvancePayment)

                   select control;
        }
    }
}
