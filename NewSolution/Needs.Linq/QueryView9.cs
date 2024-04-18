
using System;
using System.Linq;

namespace Needs.Linq
{
    abstract public class QueryView9<TO, TL> : QueryBase<TO>, IDisposable where TL : Layer.Linq.IReponsitory, IDisposable, new()
    {
        protected TL Reponsitory { get; private set; }

        public QueryView9()
        {
            this.Reponsitory = new TL();
        }

        protected QueryView9(TL reponsitory)
        {
            this.Reponsitory = reponsitory == null ? new TL() : reponsitory;
        }

        protected QueryView9(TL reponsitory, IQueryable<TO> iQueryable) : base(iQueryable)
        {
            this.Reponsitory = reponsitory == null ? new TL() : reponsitory;
        }

        virtual public void Dispose()
        {
            if (this.Reponsitory == null)
            {
                return;
            }
            this.Reponsitory.Dispose();
        }

        public IQueryable<T> ReadTable<T>() where T : class
        {
            return this.Reponsitory.ReadTable<T>();
        }
    }
}
