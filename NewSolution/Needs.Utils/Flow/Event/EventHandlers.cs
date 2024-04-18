using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Flow.Event
{
    public delegate object EventHandler<T>(object sender, EventArgs<T> e);

    public class EventArgs<T> : EventArgs
    {
        public T Entity;

        public EventArgs(T entity)
        {
            this.Entity = entity;
        }
    }
}
