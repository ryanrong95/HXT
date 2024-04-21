using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class ChainsClientsRoll : Origins.ChainsClientOrigin
    {
        public ChainsClientsRoll()
        {

        }
        protected override IQueryable<Models.Origins.ChainsClient> GetIQueryable()
        {
            return base.GetIQueryable();
        }
        public IQueryable<Models.Origins.ChainsClient> this[Underly.ApprovalStatus status]
        {
            get { return this.GetIQueryable().Where(item => item.Status == status); }
        }
    }
    /// <summary>
    /// 我的客户
    /// </summary>
    public class MyChainsClientsRoll : Origins.ChainsClientOrigin
    {
        IErpAdmin Admin;
        public MyChainsClientsRoll(IErpAdmin Admin)
        {
            this.Admin = Admin;
        }
        public MyChainsClientsRoll()
        {

        }
        protected override IQueryable<Models.Origins.ChainsClient> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.OwnerID == this.Admin.ID || item.TrackerID == this.Admin.ID || item.ReferrerID == this.Admin.ID);
        }
    }
}
