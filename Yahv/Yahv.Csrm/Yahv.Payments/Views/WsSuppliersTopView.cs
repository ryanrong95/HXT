using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 代仓储供应商
    /// </summary>
    public class WsSuppliersTopView : UniqueView<WsSupplier, PvbCrmReponsitory>
    {
        protected override IQueryable<WsSupplier> GetIQueryable()
        {
            return new Yahv.Services.Views.WsSuppliersTopView<PvbCrmReponsitory>();
        }
    }
}
