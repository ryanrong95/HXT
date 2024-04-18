using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 供应商收款人
    /// </summary>
    public class MySupplierPayees : wsnSupplierPayeesTopView<PvbCrmReponsitory>
    {
        string ClientID, SupplierID;

        public MySupplierPayees(string clientid)
        {
            this.ClientID = clientid;
        }

        public MySupplierPayees(string clientid,string supplieid)
        {
            this.ClientID = clientid;
            this.SupplierID = supplieid;
        }

        /// <summary>
        /// 获取结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<wsnSupplierPayee> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.OwnID == this.ClientID && item.nSupplierID == this.SupplierID)
                .Where(item => item.Status != GeneralStatus.Deleted);
        }

        public wsnSupplierPayee[] GetSupplierPayeesByClientID()
        {
            return base.GetIQueryable().Where(item => item.OwnID == this.ClientID)
                .Where(item => item.Status == GeneralStatus.Normal).ToArray();
        }

    }
}
