using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseOriginAreaView : UniqueView<Models.BaseOriginArea, ScCustomsReponsitory>
    {
        public BaseOriginAreaView()
        {
        }

        internal BaseOriginAreaView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseOriginArea> GetIQueryable()
        {
            return from port in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseOriginArea>()
                   select new Models.BaseOriginArea
                   {
                       ID = port.ID,
                       Code = port.Code,
                       Name = port.Name,                       
                   };
        }
    }
}
