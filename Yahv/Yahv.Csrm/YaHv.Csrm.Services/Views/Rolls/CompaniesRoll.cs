using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class CompaniesRoll : Origins.CompaniesOrigin
    {
        public CompaniesRoll()
        {
        }
        protected override IQueryable<Company> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
