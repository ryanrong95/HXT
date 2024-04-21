using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class nConsignorsRoll : Origins.nConsignorsOrigin
    {
        string nsupplierid;
        public nConsignorsRoll(string nSupplierID)
        {
            this.nsupplierid = nSupplierID;
        }
        protected override IQueryable<nConsignor> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.nSupplierID == this.nsupplierid);
        }
    }
}
