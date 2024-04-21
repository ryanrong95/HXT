using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 仓储业务下的admins
    /// </summary>
    public class AdminsWmsRoll : AdminsBusiness<PvbErmReponsitory>
    {
        public AdminsWmsRoll(SysBusiness bussiness) : base(bussiness)
        {
        }

        public AdminsWmsRoll(PvbErmReponsitory reponsitory, SysBusiness bussiness) : base(reponsitory, bussiness)
        {
        }
    }
}
