using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq
{
    public class Adapter<Tinterface, Tview> where Tinterface : IUnique where Tview : class, IDisposable, IQueryable<Tinterface>
    {
        /// <summary>
        /// 限制实例
        /// </summary>
        static public UniqueAdapter<Tinterface, Tview> Current
        {
            get
            {
                return new UniqueAdapter<Tinterface, Tview>();
            }
        }

        ///// <summary>
        ///// 限制实例
        ///// </summary>
        //static public UniqueAdapter<Tinterface, Tview> Create(object obj)
        //{
        //    return new UniqueAdapter<Tinterface, Tview>(obj);
        //}
    }
}
