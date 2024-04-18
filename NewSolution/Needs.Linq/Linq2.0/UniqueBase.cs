using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq.Linq2._0
{
    abstract public class UniqueBase<T> : Linq2._0.QueryBase<T> where T : IUnique
    {
        protected UniqueBase()
        {
        }

        protected UniqueBase(IQueryable<T> iQueryable) : base(iQueryable)
        {
        }

        public T this[string id]
        {
            get
            {
                return this.Single(id);
            }
        }

        virtual protected T Single(string id)
        {
            return this.IQueryable.SingleOrDefault(item => item.ID == id);
        }
    }
}
