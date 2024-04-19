using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Utils.Converters;

namespace NtErp.Wss.Sales.Services.Underly.Products.Coding
{
    abstract public class Coders<T> : System.Xml.Serialization.IXmlSerializable, ICodersSerializers<T> where T : Coder, new()
    {
        List<T> source;
        District district;

        public string Current
        {
            get
            {
                return this[this.district];
            }
        }

        public int Count
        {
            get
            {
                return this.source.Count;
            }
        }

        public Coders()
        {
            this.source = new List<T>();
            this.district = District.Unknown;
        }

        public Coders(District district)
        {
            this.source = new List<T>();
            this.district = district;
        }

        public string this[int index]
        {
            get
            {

                return (this.source[index] ?? Coder<T>.Free).Value;
            }
        }

        public string this[District index]
        {
            get
            {
                return (this.source.SingleOrDefault(item => item.District == index) ?? Coder<T>.Free).Value;
            }
        }

        public void Add(IEnumerable<T> collection)
        {
            this.source.AddRange(collection);
        }

        public void Add(T t)
        {
            this.source.Add(t);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public XmlSchema GetSchema()
        {
            return new XmlSchema();
        }

        public void ReadXml(XmlReader reader)
        {
            string xml = reader.ReadOuterXml();
            XElement xele = XElement.Load(new StringReader(xml));
            var eleDistict = xele.Element(nameof(this.district).FirstUpper());
            if (eleDistict != null)
            {
                this.district = (District)Enum.Parse(typeof(District), eleDistict.Value);
            }
            var items = xele.Elements(nameof(this.source).FirstUpper());
            foreach (var item in items)
            {
                var free = item.Element(nameof(Coder<T>.Free));
                T t = new T();
                t.District = (District)Enum.Parse(typeof(District), free.Element(nameof(t.District)).Value);
                t.Value = free.Element(nameof(t.Value)).Value;
                this.source.Add(t);
            }
        }

        public void WriteXml(XmlWriter writer)
        {

            writer.WriteElementString(nameof(this.district).FirstUpper(), this.district.ToString());
            foreach (var item in this)
            {
                writer.WriteStartElement(nameof(this.source).FirstUpper());
                writer.WriteStartElement(nameof(Coder<T>.Free));
                writer.WriteElementString(nameof(item.District), item.District.ToString());
                writer.WriteElementString(nameof(item.Value), item.Value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }



    }
}
