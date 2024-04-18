using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq.Generic
{
    abstract public class UniqueClassics<Tentity, Treponsitory> : QueryClassics<Tentity, Treponsitory>
      where Tentity : Needs.Linq.IUnique
      where Treponsitory : Layer.Linq.IReponsitory, IDisposable, new()
    {
        public UniqueClassics()
        {
        }

        protected UniqueClassics(Treponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 单条的
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Tentity this[string index]
        {
            get
            {
                return GetTop(1, (Expression<Func<Tentity, bool>>)(item => item.ID == index)).SingleOrDefault();
            }
        }
    }
}
