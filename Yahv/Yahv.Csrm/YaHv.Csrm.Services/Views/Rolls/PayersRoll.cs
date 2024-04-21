using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class PayersRoll : Origins.PayersOrigin
    {
        string EnterpriseID;
        public PayersRoll()
        {
           
        }
        public PayersRoll(string enterpriseid)
        {
            this.EnterpriseID = enterpriseid;
        }
        protected override IQueryable<Payer> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.EnterpriseID))
            {
                return base.GetIQueryable().Where(item => item.EnterpriseID == this.EnterpriseID);
            }
            return base.GetIQueryable();
        }
    }

}
