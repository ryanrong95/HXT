using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 供应商的私有联系人
    /// </summary>
    public class nContactsRoll : Origins.nContactsOrigin
    {
        string nsupplierid;
        public nContactsRoll(string nSupplierID)
        {
            this.nsupplierid = nSupplierID;
        }
        protected override IQueryable<nContact> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.nSupplierID == this.nsupplierid);
        }
    }
}
