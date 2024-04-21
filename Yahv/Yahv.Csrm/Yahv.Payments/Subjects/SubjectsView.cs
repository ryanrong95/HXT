using Layers.Data.Sqls;
using System.Linq;
using Yahv.Underly;

namespace Yahv.Payments
{
    public class SubjectsView : Yahv.Linq.UniqueView<Subject, PvbCrmReponsitory>
    {
        public SubjectsView()
        {

        }

        internal SubjectsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Subject> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Subjects>()
                   select new Subject
                   {
                       ID = entity.ID,
                       Catalog = entity.Catalog,
                       Conduct = entity.Conduct,
                       Currency = (Currency)entity.Currency,
                       IsCount = entity.IsCount,
                       IsToCustomer = entity.IsToCustomer,
                       Name = entity.Name,
                       Price = entity.Price,
                       Steps = entity.Steps,
                       Type = (SubjectType)entity.Type,
                   };
        }

        public Subject this[SubjectType type, string business, string catalog, string name]
        {
            get
            {
                return this.SingleOrDefault(item =>
                    item.Type == type && item.Conduct == business && item.Catalog == catalog && item.Name == name);
            }
        }
    }
}
