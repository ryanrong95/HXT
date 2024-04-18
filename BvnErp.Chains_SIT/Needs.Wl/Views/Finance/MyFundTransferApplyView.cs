using Needs.Erp.Generic;
using System.Linq;
using Needs.Ccs.Services.Views;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 我的收款通知
    /// </summary>
    public sealed class MyFundTransferApplyView : ReceiptNoticesViewForAcceptanceBill
    {
        IGenericAdmin Admin;

        public MyFundTransferApplyView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.FundTransferApplies> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from notice in base.GetIQueryable()
                   where notice.Client == null || notice.Client.Merchandiser.ID == this.Admin.ID
                   select notice;
        }
    }
}