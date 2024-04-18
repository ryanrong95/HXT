using Newtonsoft.Json;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Needs.Underly
{
    [JsonConverter(typeof(DocumentJsonConvert))]
    public partial class Document : IEnumerable<KeyValuePair<string, object>>, IXmlSerializable
    {
        /// <summary>
        /// 装箱
        /// </summary>
        Dictionary<string, object> dictionary;

        public Document()
        {
            this.dictionary = new Dictionary<string, object>();
        }

        protected Document(XElement xelement)
        {
            var doc = xelement.XmlEleTo(this.GetType()) as Document;
            this.dictionary = doc.dictionary;
        }

        /// <summary>
        /// [运行时]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public dynamic this[string index]
        {
            get
            {
                object outer;
                if (!this.dictionary.TryGetValue(index, out outer))
                {
                    outer = this.dictionary[index] = new Document();
                }
                return outer;
            }
            set
            {
                this.dictionary[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Remove(string name)
        {
            return this.dictionary.Remove(name);
        }

        public void Clear()
        {
            this.dictionary.Clear();
        }
    }
}
