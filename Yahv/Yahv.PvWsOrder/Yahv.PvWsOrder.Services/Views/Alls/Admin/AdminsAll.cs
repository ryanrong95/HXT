using Layers.Data.Sqls;
using Layers.Data.Sqls.PvWsOrder;
using Layers.Linq;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 系统管理员
    /// </summary>
    public class AdminsAll : Yahv.Services.Views.AdminsAll<PvWsOrderReponsitory>
    {
        public AdminsAll()
        {

        }

        public AdminsAll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Yahv.Services.Models.Admin> GetIQueryable()
        {
            return base.GetIQueryable().Where(a => a.Status != Underly.AdminStatus.Closed);
        }
    }
}
