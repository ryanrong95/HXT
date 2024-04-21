using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models.LsOrder;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 租赁订单通用视图
    /// </summary>
    public class LsOrderTopView : UniqueView<LsOrder, PvbCrmReponsitory>
    {
        protected override IQueryable<LsOrder> GetIQueryable()
        {
            return new Yahv.Services.Views.LsOrderTopView<PvbCrmReponsitory>();
        }
    }
}
