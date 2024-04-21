using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class SuppliersRoll : Origins.SuppliersOrigin
    {
        public SuppliersRoll()
        {
        }
        protected override IQueryable<Supplier> GetIQueryable()
        {
            return base.GetIQueryable() ;
        }
    }
    public class TradingSuppliersRoll : Origins.TradingSuppliersOrigin
    {
        public TradingSuppliersRoll()
        {

        }
        protected override IQueryable<Models.Origins.TradingSupplier> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
