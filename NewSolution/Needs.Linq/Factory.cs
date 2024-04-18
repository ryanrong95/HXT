using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq
{
    /// <summary>
    /// 工厂
    /// </summary>
    /// <typeparam name="Tinterface">什么都可以</typeparam>
    /// <typeparam name="Tview"></typeparam>
    public class Factory<Tinterface, Tview> where Tview : class, IDisposable, IQueryable<Tinterface>
    {
        /// <summary>
        /// 因此用工厂实例
        /// </summary>
        static public QueryAdapter<Tinterface, Tview> Current
        {
            get
            {
                return new QueryAdapter<Tinterface, Tview>();
            }
        }
    }
}
