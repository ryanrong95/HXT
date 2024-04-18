using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseTradeView : UniqueView<Models.BaseTrade, ScCustomsReponsitory>
    {
        public BaseTradeView()
        { }

        internal BaseTradeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
            { }

        protected override IQueryable<BaseTrade> GetIQueryable()
        {
            //return from unit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseTrades>()
            //       select new Models.BaseTrade
            //       {
            //           ID = unit.ID,
            //           Code = unit.Code,
            //           Name = unit.Name,
            //       };
            return null;
        }
    }
}
