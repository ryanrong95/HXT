using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class GoodsChangeHandler : ChangeHandler
    {

        public GoodsChangeHandler(Client client, List<RiskChanges> changes, Admin admin) : base(client, changes, admin)
        {
            this.Client = client;
            this.Changes = changes;
            this.Admin = admin;
        }

        public override void HandleReqeust(string content,string summary)
        {
            RiskChanges change = Changes.Where(t => t.ChangeType == RiskControlChangeType.GoodsUpperLimitChange).FirstOrDefault();
            if (change != null)
            {
                //ClientLog log = new ClientLog();
                //log.ClientID = Client.ID;
                //log.Admin = Client.Admin;
                //log.ClientRank = Client.ClientRank;
                //log.Summary = "风控人员[" + this.Admin.RealName + "]将货款垫款上限从" + change.OldValue + "改为:" + change.NewValue;
                //log.Enter();
                content += "将货款垫款上限从" + change.OldValue + "改为: " + change.NewValue + ",";
            }
            this.NextHandler?.HandleReqeust(content,summary);
        }
    }
}
