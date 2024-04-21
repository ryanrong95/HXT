using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class ProjectCompeleteOrigin : Yahv.Linq.UniqueView<ProjectCompelete, PvdCrmReponsitory>
    {

        public ProjectCompeleteOrigin()
        {
        }

        public ProjectCompeleteOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<ProjectCompelete> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectCompeletes>()
                   select new ProjectCompelete
                   {
                       ID = entity.ID,
                       ProjectID = entity.ProjectID,
                       ProjectProductID=entity.ProjectProductID,
                       ProductID = entity.ProductID,
                       SpnID=entity.SpnID,
                       CreatorID=entity.CreatorID,
                       UnitPrice=entity.UnitPrice,
                       DataStatus=(DataStatus)entity.Status,
                       CreateDate=entity.CreateDate,
                       ModifyDate=entity.ModifyDate

                   };

        }
    }
}
