using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall.Views
{
    class VersionsView : Linq.UniqueView<Models.Version, BvOverallsReponsitory>
    {
        public VersionsView()
        {

        }

        protected override IQueryable<Models.Version> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOveralls.Versions>()
                   select new Models.Version
                   {
                       Code = entity.Code,
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       LastGenerationDate = entity.LastGenerationDate,
                       Name = entity.Name,
                       UpdateDate = entity.UpdateDate
                   };
        }
    }
}
