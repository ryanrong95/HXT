using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class SiteUsersXdtRoll : Origins.SiteUsersXdtOrigin
    {
        Enterprise enterprise;
        public SiteUsersXdtRoll(Enterprise Enterprise)
        {
            this.enterprise = Enterprise;
        }
        protected override IQueryable<SiteUserXdt> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterprise.ID);
        }
    }
    public class SiteUsersRoll : Origins.SiteUsersOrigin
    {
        public SiteUsersRoll()
        {

        }
        protected override IQueryable<SiteUser> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
