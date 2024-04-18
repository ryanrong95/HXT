using Needs.Erp.Generic;
using System.Linq;
using Needs.Ccs.Services.Views;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 我的付款通知
    /// </summary>
    public sealed class MyPaymentNoticesView : PaymentNoticesView
    {
        IGenericAdmin Admin;

        public MyPaymentNoticesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.PaymentNotice> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from notice in base.GetIQueryable()
                   where notice.Payer.ID == this.Admin.ID
                   select notice;
        }
    }
}