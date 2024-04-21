using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class nPayeesRoll : Origins.nPayeesOrigin
    {
        string nsupplierid;
        public nPayeesRoll()
        {

        }
        public nPayeesRoll(string nSupplierID)
        {
            this.nsupplierid = nSupplierID;
        }
        protected override IQueryable<nPayee> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.nsupplierid))
            {
                return base.GetIQueryable().Where(item => item.nSupplierID == this.nsupplierid);
            }
            return base.GetIQueryable();
        }
    }
}
