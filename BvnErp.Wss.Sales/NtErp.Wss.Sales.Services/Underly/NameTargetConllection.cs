using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public class NameTargetConllection<T> : IEnumerable<KeyValuePair<string, T>>
    {
        List<string> namings;
        List<T> list;

        public int Count { get { return this.namings.Count; } }

        public NameTargetConllection()
        {
            this.namings = new List<string>();
            this.list = new List<T>();
        }

        public T this[int index]
        {
            get { return this.list[index]; }
            set { this.list[index] = value; }
        }

        public T this[string index]
        {
            get
            {
                for (int reffer = 0; reffer < this.namings.Count; reffer++)
                {
                    if (this.namings[reffer].Equals(index, StringComparison.OrdinalIgnoreCase))
                    {
                        return this.list[reffer];
                    }
                }

                return default(T);
            }
            set
            {
                for (int reffer = 0; reffer < this.namings.Count; reffer++)
                {
                    if (this.namings[reffer].Equals(index, StringComparison.OrdinalIgnoreCase))
                    {
                        this.list[reffer] = value;
                        return;
                    }
                }

                this.namings.Add(index);
                this.list.Add(value);

            }
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return this.namings.Select((item, index) => new KeyValuePair<string, T>(item, this.list[index])).GetEnumerator();
        }

        public string[] Namings { get { return this.namings.ToArray(); } }

        public T[] Targets { get { return this.list.ToArray(); } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
