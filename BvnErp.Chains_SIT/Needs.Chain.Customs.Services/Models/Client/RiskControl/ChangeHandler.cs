using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public abstract class ChangeHandler
    {
        public Client Client { get; set; }

        public List<RiskChanges> Changes { get; set; }

        public Admin Admin { get; set; }

        public ChangeHandler NextHandler { get; set; }

        public ChangeHandler(Client client, List<RiskChanges> changes,Admin admin)
        {
            this.Client = client;
            this.Changes = changes;
            this.Admin = admin;
        }

        public void setNextHandler(ChangeHandler nextHandler)
        {
            this.NextHandler = nextHandler;
        }

        public abstract void HandleReqeust(string content,string summary);
    }
}
