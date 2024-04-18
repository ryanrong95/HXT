using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class RankChangeHandler : ChangeHandler
    {
        public RankChangeHandler(Client client, List<RiskChanges> changes,Admin admin):base(client,changes,admin)
        {
            this.Client = client;
            this.Changes = changes;
            this.Admin = admin;
        }

        public override void HandleReqeust(string content, string summary)
        {
            RiskChanges change = Changes.Where(t => t.ChangeType == RiskControlChangeType.RankChange).FirstOrDefault();
            if (change != null)
            {
                //ClientLog log = new ClientLog();
                //log.ClientID = Client.ID;
                //log.Admin = Client.Admin;
                //log.ClientRank = (Enums.ClientRank)Convert.ToInt16(change.NewValue);
                //log.Summary = "风控人员[" + this.Admin.RealName + "]将客户等级从" + Client.ClientRank.GetDescription() + "改为:" + log.ClientRank.GetDescription();
                //log.Enter();
                Enums.ClientRank newClientRank = (Enums.ClientRank)Convert.ToInt16(change.NewValue);
                Enums.ClientRank oldClientRank = (Enums.ClientRank)Convert.ToInt16(change.OldValue);
                content += "将客户类型从" + oldClientRank.GetDescription() + "改为: " + newClientRank.GetDescription()+",";
            }
            this.NextHandler?.HandleReqeust(content,summary);
        }
    }
}
