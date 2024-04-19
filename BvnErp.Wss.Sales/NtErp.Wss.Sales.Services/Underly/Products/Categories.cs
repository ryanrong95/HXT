using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly.Products
{
    [Newtonsoft.Json.JsonConverter(typeof(EnumerableConverter))]
    public class Categories : IForSerializers<string>
    {
        List<string> source;

        string Current
        {
            get
            {
                return this.LastOrDefault();
            }
        }

        public int Count
        {
            get
            {
                return this.source.Count;
            }
        }

        public Categories()
        {
            this.source = new List<string>();
        }

        Categories(params string[] arry)
        {
            this.source = new List<string>(arry);
        }

        public string this[int index]
        {
            get
            {
                return this.source[index];
            }
        }

        public void Add(IEnumerable<string> collection)
        {
            this.source.AddRange(collection);
        }

        public void Add(string t)
        {
            this.source.Add(t);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static implicit operator string[] (Categories entity)
        {
            return entity.ToArray();
        }

        public static implicit operator Categories(string[] v)
        {
            return new Categories(v);
        }
    }
}
