using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AgencyFeeChangeHandler : ChangeHandler
    {
        public AgencyFeeChangeHandler(Client client, List<RiskChanges> changes, Admin admin) : base(client, changes, admin)
        {
            this.Client = client;
            this.Changes = changes;
            this.Admin = admin;
        }

        public override void HandleReqeust(string context, string summary)
        {
            RiskChanges change = Changes.Where(t => t.ChangeType == RiskControlChangeType.AgencyFeeUpperLimitChange).FirstOrDefault();
            if (change != null)
            {
                //ClientLog log = new ClientLog();
                //log.ClientID = Client.ID;
                //log.Admin = Client.Admin;
                //log.ClientRank = Client.ClientRank;
                //log.Summary = "风控人员[" + this.Admin.RealName + "]将代理费垫款上限从" + change.OldValue + "改为:" + change.NewValue;
                //log.Enter();
                context += "将代理费垫款上限从" + change.OldValue + "改为:" + change.NewValue + ",";
            }
            this.NextHandler?.HandleReqeust(context,summary);
        }
    }
}
