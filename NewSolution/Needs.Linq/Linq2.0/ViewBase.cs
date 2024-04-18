using System;
using System.Linq;
using System.Linq.Expressions;

namespace Needs.Linq
{
    public abstract class ViewBase<TSource, TReponsitory> : Linq2._0.UniqueBase<TSource> where TSource : IUnique
    where TReponsitory : Layer.Linq.IReponsitory, IDisposable, new()
    {
        protected TReponsitory Reponsitory { get; private set; }

        public ViewBase()
        {
            this.Reponsitory = new TReponsitory();
        }

        protected ViewBase(TReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory == null ? new TReponsitory() : reponsitory;
        }

        public void Dispose()
        {
            if (this.Reponsitory == null)
            {
                return;
            }

            this.Reponsitory.Dispose();
        }
    }
}