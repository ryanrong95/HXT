using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Rolls;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class nPayersView : Origins.nPayersViewOrigin
    {
        string nsupplierid;
        public nPayersView(string nSupplierID)
        {
            this.nsupplierid = nSupplierID;
        }
        protected override IQueryable<nPayer> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.nSupplierID == this.nsupplierid);
        }
    }
}
