using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Logs.Services.View
{
    public class LogsView : View<Model.Log, ScCustomsReponsitory>
    {
        private string Name;

        public LogsView(string name)
        {
            this.Name = name;
        }

        protected override IQueryable<Model.Log> GetIQueryable()
        {
            var query = from c in this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Logs>()
                        orderby c.CreateDate descending
                        where c.Name == this.Name
                        select new Model.Log
                        {
                            ID = c.ID,
                            Name = c.Name,
                            AdminID = c.AdminID,
                            MainID = c.MainID,
                            Summary = c.Summary,
                            Json = c.Json,
                            CreateDate = c.CreateDate
                        };

            return query;
        }
    }
}