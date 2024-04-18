using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.ClientViews;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class Supplier : Yahv.Services.Models.wsnSuppliers
    {
        #region 扩展属性
        /// <summary>
        /// 供应商受益人
        /// </summary>
        public MySupplierPayees MySupplierPayees
        {
            get
            {
                return new MySupplierPayees(this.OwnID, this.ID);
            }
        }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public MySupplierConsignors MySupplierAddress
        {
            get
            {
                return new MySupplierConsignors(this.OwnID, this.ID);
            }
        }

        /// <summary>
        /// 联系人
        /// </summary>
        public MySupplierContacts MySupplierContact
        {
            get
            {
                return new MySupplierContacts(this.OwnID, this.ID);
            }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string AdminID { get; set; }
        #endregion
    }
}
