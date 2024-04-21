using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class CopWsClientsView : Origins.CopWsClientsOrigin
    {
        public CopWsClientsView()
        {
        }
        protected override IQueryable<CopWsClient> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
