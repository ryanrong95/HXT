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
    public class MyMerchandiserControlsView : MerchandiserControlsView
    {
        IGenericAdmin Admin;

        public MyMerchandiserControlsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.MerchandiserControl> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from control in base.GetIQueryable()
                   where control.Order.Client.Merchandiser.ID == this.Admin.ID &&
                        control.Status == Ccs.Services.Enums.Status.Normal

                   select control;
        }
    }

    /// <summary>
    /// 跟单员管控的视图（Admin过滤）
    /// </summary>
    public class MyMerchandiserControlsNotHangUpView : MerchandiserControlsNotHangUpView
    {
        IGenericAdmin Admin;

        public MyMerchandiserControlsNotHangUpView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.MerchandiserControl> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from control in base.GetIQueryable()
                   where control.Order.Client.Merchandiser.ID == this.Admin.ID &&
                        control.Status == Ccs.Services.Enums.Status.Normal

                   select control;
        }
    }
}
