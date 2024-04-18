using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 代仓储客户
    /// </summary>
    public class WsSupplier : Yahv.Services.Models.wsnSuppliers
    {
        #region 扩展属性
        public Yahv.Services.Models.wsnContact Contact
        {
            get
            {
                var contact =  new Yahv.Services.Views.wsnSupplierContactsTopView<PvWsOrderReponsitory>()
                    .Where(item => item.nSupplierID == this.ID).FirstOrDefault();
                return contact;
            }
        }
        #endregion
    }
}
