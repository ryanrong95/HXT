using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 流水账视图
    /// </summary>
    public class FlowAccountsTopView : UniqueView<FlowAccount, PvbCrmReponsitory>
    {
        protected override IQueryable<FlowAccount> GetIQueryable()
        {
            return new Yahv.Services.Views.FlowAccountsTopView<PvbCrmReponsitory>();
        }
    }
}
