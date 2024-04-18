using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Interpreter.Models;
using System.Linq;

namespace Needs.Interpreter.Views
{
    public class TopObjectsView : UniqueView<ITopObject, BvOverallsReponsitory>
    {
        public TopObjectsView()
        {

        }

        protected override IQueryable<ITopObject> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOveralls.TopObjects>()
                   select new TopObject
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
