using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecContainersView : UniqueView<Models.DecContainer, ScCustomsReponsitory>
    {
        public DecContainersView()
        {
        }
        internal DecContainersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecContainer> GetIQueryable()
        {
            var containerView = new BaseContainersView(this.Reponsitory);

            return from container in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecContainers>()
                   join con in containerView on container.ContainerMd equals con.Code
                   select new Models.DecContainer
                   {
                       ID = container.ID,
                       DeclarationID = container.DeclarationID,
                       ContainerID = container.ContainerID,
                       ContainerMd = container.ContainerMd,
                       GoodsNo = container.GoodsNo,
                       LclFlag = container.LclFlag,
                       GoodsContaWt = container.GoodsContaWt,
                       Container = con
                   };
        }
    }
}