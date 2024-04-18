using Layer.Data.Sqls;
using Needs.Overall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall.Views
{
    class DevlopersView : Linq.UniqueView<IDevlopNote, BvOverallsReponsitory>
    {
        protected override IQueryable<IDevlopNote> GetIQueryable()
        {
            return from entity in this.Reponsitory.GetTable<Layer.Data.Sqls.BvOveralls.DevlopNotes>()
                   orderby entity.CsProject, entity.TypeName, entity.UpdateDate descending
                   select new DevlopNote
                   {
                       Context = entity.Context,
                       Devloper = (Devloper)entity.Devloper,
                       ID = entity.ID,
                       MethodName = entity.MethodName,
                       TypeName = entity.TypeName,
                       Number = entity.Number,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       CsProject = (CsProject)entity.CsProject
                   };
        }
    }
}
