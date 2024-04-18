using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseLeviesView : UniqueView<Models.BaseLevies, ScCustomsReponsitory>
    {
        public BaseLeviesView()
        { }

        internal BaseLeviesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
            { }

        protected override IQueryable<Models.BaseLevies> GetIQueryable()
        {
            //return from unit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseLevies>()
            //       select new Models.BaseLevies
            //       {
            //           ID = unit.ID,
            //           Code = unit.Code,
            //           BriefName = unit.BriefName,
            //           FullName = unit.FullName,
            //       };
            return null;
        }
    }
}
