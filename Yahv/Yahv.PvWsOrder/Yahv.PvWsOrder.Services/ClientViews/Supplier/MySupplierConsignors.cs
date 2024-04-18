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
    /// 供应商提货地址
    /// </summary>
    public class MySupplierConsignors : wsnSupplierConsignorsTopView<PvbCrmReponsitory>
    {
        string ClientID;
        string[] SupplierIDs;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enterpriseid">客户ID</param>
        /// <param name="supplierid">供应商ID</param>
        internal MySupplierConsignors(string clientid, string supplierid)
        {
            this.ClientID = clientid;
            this.SupplierIDs = new string[] { supplierid };
        }

        public MySupplierConsignors(string clientid, string[] supplierids)
        {
            this.ClientID = clientid;
            this.SupplierIDs = supplierids;
        }

        /// <summary>
        /// 获取结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<wsnSupplierConsignor> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.OwnID == this.ClientID && this.SupplierIDs.Contains(item.nSupplierID))
                .Where(item => item.Status != GeneralStatus.Deleted); ;
        }
    }
}
