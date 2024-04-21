using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls.ProjectReports
{
    public class AgentQuoteRoll : Origins.AgentQuoteOrigin
    {
        public AgentQuoteRoll()
        {

        }

        protected override IQueryable<AgentQuote> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }


    public class MyAgentQuotRoll : Origins.AgentQuoteOrigin
    {

        IErpAdmin Admin;
        public MyAgentQuotRoll() { }
        public MyAgentQuotRoll(IErpAdmin admin)
        {
           
            this.Admin = admin;
        }
        protected override IQueryable<AgentQuote> GetIQueryable()
        {
            if (Admin.IsSuper)
            {
                return base.GetIQueryable();
            }
            else {
            return base.GetIQueryable().Where(item => item.CreatorID==Admin.ID);
            }
        }

    }
}
