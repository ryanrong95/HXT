using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseContainersView : UniqueView<Models.BaseContainer, ScCustomsReponsitory>
    {
        public BaseContainersView()
        {
        }

        internal BaseContainersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseContainer> GetIQueryable()
        {
            return from monitorWay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseContainer>()
                   select new Models.BaseContainer
                   {
                       ID = monitorWay.ID,
                       Code = monitorWay.Code,
                       Name = monitorWay.Name
                   };
        }
    }
}
