using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 供应商联系人查询
    /// </summary>
    public class MySupplierContacts : wsnSupplierContactsTopView<PvbCrmReponsitory>
    {
        string ClientID, SupplierID;

        public MySupplierContacts(string clientid, string supplieid)
        {
            this.ClientID = clientid;
            this.SupplierID = supplieid;
        }

        /// <summary>
        /// 获取结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<wsnContact> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.OwnID == this.ClientID && item.nSupplierID == this.SupplierID)
                .Where(item => item.Status != GeneralStatus.Deleted); ;
        }

    }
}
