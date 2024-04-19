using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NtErp.Wss.Sales.Services.Underly
{
    abstract public class ElementBase : IDocument<Elements>
    {
        SortedList<string, Elements> source;

        protected ElementBase()
        {
            this.source = new SortedList<string, Elements>();
        }

        protected ElementBase(Document document)
        {
            this.source = document.source;
        }

        protected ElementBase(XElement xml)
        {
            var docment = (Serializers.XmlSerializerExtend.XmlEleTo(this.GetType(), xml) as Document);
            this.source = docment.source;
        }

        public int Count
        {
            get
            {
                return this.source.Values.Count;
            }
        }

        virtual public Elements this[string index]
        {
            get
            {
                Elements value;
                if (this.source.TryGetValue(index, out value))
                {
                    return value as Elements;
                }
                else
                {
                    var elements = new Elements(index, null);
                    this.source[index] = elements;
                    return elements;
                }
            }
            set
            {
                //if (value == null || value.Value == null)
                //{
                //    return;
                //}

                //this.source[index] = new Elements(index, value.Value);

                if (value != null && value.Value != null)
                {
                    this.source[index] = new Elements(index, value.Value);
                }
                else if (value.Count > 0)
                {
                    this.source[index] = value;
                }
            }
        }

        public IEnumerator<Elements> GetEnumerator()
        {
            return this.source.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 内部重置
        /// </summary>
        /// <param name="ebase">来源数据</param>
        protected void ResetSource(ElementBase ebase)
        {
            this.source = ebase.source;
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            this.source.Clear();
        }

        public KeyValuePair<string, string>[] ToKeyValuePairs()
        {
            return this.source.Select(item => new KeyValuePair<string, string>(item.Key, item.Value)).ToArray();
        }
    }
}
