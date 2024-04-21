using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.CrmPlus.Service.Models.Rolls
{

    public class SupplierRegisteRecord : BaseApplyRecord
    {
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { set; get; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { set; get; }

    }
}
