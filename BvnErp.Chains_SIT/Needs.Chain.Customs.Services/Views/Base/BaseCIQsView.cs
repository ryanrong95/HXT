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
    public class BaseCIQsView : UniqueView<Models.BaseCIQ, ScCustomsReponsitory>
    {
        public BaseCIQsView()
        {
        }

        internal BaseCIQsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseCIQ> GetIQueryable()
        {
            //return from unit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCIQs>()
            //       select new Models.BaseCIQ
            //       {
            //           ID = unit.ID,
            //           Code = unit.Code,
            //           Name = unit.Name,
            //       };
            return null;
        }
    }
}