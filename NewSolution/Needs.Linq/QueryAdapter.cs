using System;
using System.Linq;

namespace Needs.Linq
{
    public class QueryAdapter<Tinterface, Tview> : QueryBase<Tinterface>, IDisposable where Tview : class, IDisposable, IQueryable<Tinterface>
    {
        protected Tview Visitor { get; private set; }

        public QueryAdapter()
        {
            this.Visitor = Activator.CreateInstance(typeof(Tview), true) as Tview;
        }

        protected QueryAdapter(Tview visitor)
        {
            this.Visitor = visitor;
        }

        protected QueryAdapter(Tview visitor, IQueryable<Tinterface> iQueryable) : base(iQueryable)
        {
            this.Visitor = visitor;
        }

        virtual public void Dispose()
        {
            if (this.Visitor == null)
            {
                return;
            }
            this.Visitor.Dispose();
        }

        protected override IQueryable<Tinterface> GetIQueryable()
        {
            return this.Visitor;
        }
    }
}
