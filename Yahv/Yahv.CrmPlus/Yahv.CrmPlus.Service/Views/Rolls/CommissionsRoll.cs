using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class CommissionsRoll : Origins.CommissionsOrigin
    {
        string suplierid;
        public CommissionsRoll(string SupplierID)
        {
            this.suplierid = SupplierID;
        }
        public CommissionsRoll()
        {

        }
        protected override IQueryable<Models.Origins.Commission> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.suplierid))
            {
                return base.GetIQueryable().Where(item => item.SupplierID == this.suplierid && item.Status == Underly.DataStatus.Normal);
            }
            return base.GetIQueryable().Where(item => item.Status == Underly.DataStatus.Normal);
        }
    }
}
