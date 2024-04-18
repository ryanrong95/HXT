using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class NatureChangeHandler : ChangeHandler
    {
        public NatureChangeHandler(Client client, List<RiskChanges> changes,Admin admin):base(client,changes,admin)
        {
            this.Client = client;
            this.Changes = changes;
            this.Admin = admin;
        }

        public override void HandleReqeust(string content, string summary)
        {
            RiskChanges change = Changes.Where(t => t.ChangeType == RiskControlChangeType.NatureChange).FirstOrDefault();
            if (change != null)
            {
                //ClientLog log = new ClientLog();
                //log.ClientID = Client.ID;
                //log.Admin = Client.Admin;
                //log.ClientRank = (Enums.ClientRank)Convert.ToInt16(change.NewValue);
                //log.Summary = "风控人员[" + this.Admin.RealName + "]将客户等级从" + Client.ClientRank.GetDescription() + "改为:" + log.ClientRank.GetDescription();
                //log.Enter();
                Enums.ClientNature newClientRank = (Enums.ClientNature)Convert.ToInt16(change.NewValue);
                Enums.ClientNature oldClientRank = (Enums.ClientNature)Convert.ToInt16(change.OldValue);
                content += "将客户从" + oldClientRank.GetDescription() + "改为: " + newClientRank.GetDescription()+",";
            }
            this.NextHandler?.HandleReqeust(content,summary);
        }
    }
}
