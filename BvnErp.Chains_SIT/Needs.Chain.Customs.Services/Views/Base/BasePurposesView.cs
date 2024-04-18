using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BasePurposesView : UniqueView<Models.BasePurpose, ScCustomsReponsitory>
    {
        public BasePurposesView()
        {
        }

        internal BasePurposesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BasePurpose> GetIQueryable()
        {
            return from basePurpose in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BasePurpose>()
                   select new Models.BasePurpose
                   {
                       ID = basePurpose.ID,
                       Code = basePurpose.Code,
                       Name = basePurpose.Name
                   };
        }
    }
}