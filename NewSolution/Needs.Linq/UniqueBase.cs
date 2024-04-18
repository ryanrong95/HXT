using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq
{

    abstract public class UniqueBase<T> : QueryBase<T>, ILinqUnique<T> where T : IUnique
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

