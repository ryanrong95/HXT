using Layer.Data.Sqls;
using Needs.Interpreter.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Interpreter.Views
{
    public class En_TopObjectsView : UniqueView<ITopObject, BvOverallsReponsitory>
    {
        public En_TopObjectsView()
        {

        }

        protected override IQueryable<ITopObject> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOveralls.TopObjects_En>()
                   select new En_TopObject
                   {
                       ID = entity.ID,
                       Language = entity.Language,
                       Name = entity.Name,
                       Project = entity.Project,
                       Type = (TopObjectType)entity.Type,
                       Value = entity.Value
                   };
        }
    }
}
