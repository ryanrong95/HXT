using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Client.Services
{
    public class SupplierUtils
    {
        /// <summary>
        /// 供应商是否使用
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public static bool SupplierIsUesedByOrder(string supplierID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                return reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>().Count(s => s.ClientSupplierID == supplierID) > 0;
            }
        }

        /// <summary>
        /// 供应商是否使用
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public static bool SupplierIsUesedByOrderPayExchange(string supplierID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                return reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers>().Count(s => s.ClientSupplierID == supplierID) > 0;
            }
        }
    }
}