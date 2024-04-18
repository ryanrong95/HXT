using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ManifestConsignmentContainersView : UniqueView<Models.ManifestConsignmentContainer, ScCustomsReponsitory>
    {
        public ManifestConsignmentContainersView()
        {
        }

        internal ManifestConsignmentContainersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ManifestConsignmentContainer> GetIQueryable()
        {
            return from container in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignmentContainers>()
                   select new Models.ManifestConsignmentContainer {
                       ID = container.ID,
                       ManifestConsignmentID = container.ManifestConsignmentID,
                       ContainerNo = container.ContainerNo
                   };
        }
    }
}
