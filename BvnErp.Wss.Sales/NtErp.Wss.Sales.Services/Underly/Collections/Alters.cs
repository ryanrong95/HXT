
using NtErp.Wss.Sales.Services.Underly.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly.Collections
{
    public class Alters<T> : IEnumerable<T> where T : class, IAlter
    {
        List<T> currents;

        public Alters()
        {
            this.currents = new List<T>();
        }

        public int Count
        {
            get { return this.currents.Count; }
        }

        virtual public void Add(T t)
        {
            if (t == null)
            {
                return;
            }

            foreach (var item in this.currents.Where(item => item.Equals(t) && item.Status == AlterStatus.Normal).ToArray())
            {
                item.Status = AlterStatus.Altered;
            }

            this.currents.Add(t);
        }

        public void Log(T t)
        {
            var old = t.Xml().XmlTo<T>();
            old.Status = AlterStatus.Altered;
            this.currents.Add(t);
        }

        virtual public void Remove(Func<T, bool> predicate)
        {
            foreach (var item in this.currents.Where(predicate))
            {
                item.Status = AlterStatus.Cancel;
            }
        }

        virtual public IEnumerator<T> GetEnumerator()
        {
            return this.currents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
