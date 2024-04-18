using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IncidentalFeeChangeHandler : ChangeHandler
    {
        public IncidentalFeeChangeHandler(Client client, List<RiskChanges> changes, Admin admin) : base(client, changes, admin)
        {
            this.Client = client;
            this.Changes = changes;
            this.Admin = admin;
        }

        public override void HandleReqeust(string context,string summary)
        {
            RiskChanges change = Changes.Where(t => t.ChangeType == RiskControlChangeType.IncidentalUpperLimitChange).FirstOrDefault();
            if (change != null)
            {               
                context+= "将杂费垫款上限从" + change.OldValue + "改为:" + change.NewValue + ",";
            }
            this.NextHandler?.HandleReqeust(context,summary);
        }
    }
}
