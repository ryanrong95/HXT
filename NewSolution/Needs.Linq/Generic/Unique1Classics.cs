using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq.Generic
{
    abstract public class Unique1Classics<Tentity, Treponsitory> : Query1Classics<Tentity, Treponsitory>
      where Tentity : Needs.Linq.IUnique
      where Treponsitory : Layer.Linq.IReponsitory, IDisposable, new()
    {
        public Unique1Classics()
        {
        }

        protected Unique1Classics(Treponsitory reponsitory) : base(reponsitory)
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
                return GetTop(1, item => item.ID == index).SingleOrDefault();
            }
        }
    }
}
