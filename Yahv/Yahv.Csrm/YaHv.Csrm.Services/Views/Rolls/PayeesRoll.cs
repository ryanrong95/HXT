using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class PayeesRoll : Origins.PayeesOrigin
    {
        string enterpriseid;
        public PayeesRoll(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }
        protected override IQueryable<Payee> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
        }
    }
}
