using System;
using System.Linq;

namespace Needs.Linq
{
    abstract public class UniqueView<TO, TL> : UniqueBase<TO>, IDisposable where TO : IUnique where TL : Layer.Linq.IReponsitory, IDisposable, new()
    {
        protected TL Reponsitory { get; private set; }

        public UniqueView()
        {
            this.Reponsitory = new TL();
        }

        protected UniqueView(TL reponsitory)
        {
            this.Reponsitory = reponsitory == null ? new TL() : reponsitory;
        }

        protected UniqueView(TL reponsitory, IQueryable<TO> iQueryable) : base(iQueryable)
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
    }
}
