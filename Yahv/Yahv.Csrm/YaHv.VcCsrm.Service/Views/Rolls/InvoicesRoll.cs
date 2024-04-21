using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.VcCsrm.Service.Models;

namespace YaHv.VcCsrm.Service.Views.Rolls
{
    public class InvoicesRoll : Origins.InvoicesOrigin
    {
        string clientid;
        public InvoicesRoll(string clientid)
        {
            this.clientid = clientid;
        }
        protected override IQueryable<Invoice> GetIQueryable()
        {
            return from entity in base.GetIQueryable() where entity.EnterpriseID == this.clientid select entity;
        }
    }
}
