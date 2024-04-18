using System;
using System.Linq;

namespace Needs.Linq
{
    public class UniqueAdapter<Tinterface, Tview> : UniqueBase<Tinterface>, IDisposable where Tinterface : IUnique where Tview : class, IDisposable, IQueryable<Tinterface>
    {
        protected Tview Visitor { get; private set; }

        public UniqueAdapter()
        {
            this.Visitor = Activator.CreateInstance(typeof(Tview), true) as Tview;
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
