using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Rolls;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// Crm后台使用
    /// </summary>
    public class nPayeesView : Origins.nPayeeViewOrigin
    {
        string nsupplierid;
        public nPayeesView(string nSupplierID)
        {
            this.nsupplierid = nSupplierID;
        }
        protected override IQueryable<nPayee> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.nSupplierID == this.nsupplierid);
        }
    }
}
