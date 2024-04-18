using System;
using System.Linq;

namespace Needs.Linq
{

    abstract public class UniqueFiter<Tinterface, Tview> : UniqueBase<Tinterface>, IDisposable where Tinterface : IUnique where Tview : class, IDisposable
    {
        protected Tview View { get; private set; }

        protected UniqueFiter(Tview view)
        {
            this.View = view;
        }

        virtual public void Dispose()
        {
            if (this.View == null)
            {
                return;
            }
            this.View.Dispose();
        }

        abstract protected override IQueryable<Tinterface> GetIQueryable();
    }
}
