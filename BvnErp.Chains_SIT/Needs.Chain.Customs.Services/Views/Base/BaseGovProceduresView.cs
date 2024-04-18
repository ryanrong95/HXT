using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseGovProceduresView : UniqueView<Models.BaseGovProcedure, ScCustomsReponsitory>
    {
        public BaseGovProceduresView()
        {
        }

        internal BaseGovProceduresView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseGovProcedure> GetIQueryable()
        {
            return from baseGovProcedure in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseGovProcedure>()
                   select new Models.BaseGovProcedure
                   {
                       ID = baseGovProcedure.ID,
                       Code = baseGovProcedure.Code,
                       Name = baseGovProcedure.Name
                   };
        }
    }
}