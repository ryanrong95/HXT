using Layers.Data.Sqls;
using System;
using System.Linq;
using Yahv.Underly.Erps;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class WareHousesRoll : Origins.WareHousesOrigin
    {
        IErpAdmin admin;
        public WareHousesRoll()
        {

        }
        public WareHousesRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<WareHouse> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
