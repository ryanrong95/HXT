using Needs.Erp.Generic;
using System.Linq;
using Needs.Ccs.Services.Views;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 我的付汇申请
    /// </summary>
    public sealed class MyPayExchangeApplysView : AdminPayExchangeApplyView
    {
        IGenericAdmin Admin;

        public MyPayExchangeApplysView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.AdminPayExchangeApply> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            var clientIds=new ClientAdminsView().Where(t=>t.Admin.ID==this.Admin.ID).Select(t=>t.ClientID).ToList();
            return from notice in base.GetIQueryable()
                where clientIds.Contains(notice.ClientID)
                select notice;
        }
    }

    /// <summary>
    /// 我的付汇申请（待审核）
    /// </summary>
    public sealed class MyUnAuditedPayExchangeApplysView : UnAuditedPayExchangeApplyView
    {
        IGenericAdmin Admin;

        public MyUnAuditedPayExchangeApplysView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnAuditedPayExchangeApply> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }
            var clientIds = new ClientAdminsView().Where(t => t.Admin.ID == this.Admin.ID).Select(t => t.ClientID).ToList();
            return from notice in base.GetIQueryable()
                   where clientIds.Contains(notice.ClientID)
                   select notice;
        }
    }

    /// <summary>
    /// 我的付汇申请（待审批）
    /// </summary>
    public sealed class MyUnApprovalPayExchangeApplyView : UnApprovalPayExchangeApplyView
    {
        IGenericAdmin Admin;

        public MyUnApprovalPayExchangeApplyView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnApprovalPayExchangeApply> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from notice in base.GetIQueryable()
                   where notice.Admin.ID == this.Admin.ID
                   select notice;
        }
    }
}