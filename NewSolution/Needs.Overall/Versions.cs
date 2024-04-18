using Needs.Overall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Needs.Linq;
using Needs.Overall.Views;

namespace Needs.Overall
{
    public class Versions : IEnumerable<IVersion>
    {
        IVersion[] source;

        Versions()
        {
            this.source = Adapter<IVersion, VersionsView>.Current.ToArray();
        }

        public IEnumerator<IVersion> GetEnumerator()
        {
            return this.source.Select(item => item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        static Versions current;
        static object locker = new object();

        static public Versions Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Versions();
                        }
                    }
                }
                return current;
            }
        }
    }
}
