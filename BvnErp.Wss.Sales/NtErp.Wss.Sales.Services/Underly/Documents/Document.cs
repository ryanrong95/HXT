using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtErp.Wss.Sales.Services.Utils.Convertibles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly
{
    [Serializable]
    [JsonConverter(typeof(ElementsConverter))]
    public class Document : ElementBase, IXmlSerializable
    {
        [Obsolete("防止报错临时")]
        public bool Readonly;
        [Obsolete("防止报错临时")]
        public Document(bool isReadonly) { }

        public Document()
        {

        }

        protected Document(Document document) : base(document)
        {

        }

        protected Document(XElement xml) : base(xml)
        {

        }

        public XmlSchema GetSchema()
        {
            return new XmlSchema();
        }

        public void ReadXml(XmlReader reader)
        {
            string xml = reader.ReadOuterXml();
            XElement xele = XElement.Load(new StringReader(xml));
            var top = xele.Element("Items");
            this.ReadXml(top, this);
        }

        void ReadXml(XElement xeles, IDocument<Elements> current)
        {
            if (xeles == null)
            {
                return;
            }
            var items = xeles.Elements("Item");
            foreach (var item in items)
            {
                var name_ele = item.Element("Name");
                var items_ele = item.Element("Items");

                if (items_ele == null)
                {
                    var value_ele = item.Element("Value");
                    current[name_ele.Value] = ReadValue(value_ele);
                }
                else
                {
                    ReadXml(items_ele, current[name_ele.Value]);
                }
            }
        }

        DValue ReadValue(XElement xeles)
        {
            var valueType = xeles.Attribute("Type");
            if (valueType != null)
            {
                if (this.GetTypeFullName(typeof(Document)) == valueType.Value)
                {
                    var doc = new Document();
                    this.ReadXml(xeles.Element("Items"), doc);
                    return new DValue(doc);
                }
                else if (this.GetTypeFullName(typeof(Document[])) == valueType.Value)
                {
                    var docs = new Document[xeles.Elements().Count()];
                    int index = 0;
                    foreach (var ele in xeles.Elements("Items"))
                    {
                        docs[index] = new Document();
                        this.ReadXml(ele, docs[index]);
                        index++;
                    }
                    return new DValue(docs);
                }
                else {
                    return new DValue(xeles.Value.ChangeType(Type.GetType(valueType.Value, false, true)));
                }
            }

            if (xeles.HasElements)
            {
                var arry = xeles.Element("XmlArray");
                var type_ele = arry.Attribute("Type");

                var items = arry.Elements("XmlArrayItem");
                ArrayList list = new ArrayList(items.Count());
                Type type;

                if (type_ele == null || (type = Type.GetType(type_ele.Value, false, true)) == null)
                {
                    foreach (var item in items)
                    {
                        list.Add(item.Value);
                    }
                    return new DValue(list.ToArray());
                }
                else
                {
                    foreach (var item in items)
                    {
                        list.Add(item.Value.ChangeType(type));
                    }
                    return new DValue(list.ToArray(type));
                }
            }
            else
            {
                return new DValue(xeles.Value);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            this.WriteXml(writer, this);
        }

        void WriteXml(XmlWriter writer, IDocument<Elements> eles)
        {
            writer.WriteStartElement("Items");
            foreach (var item in eles)
            {
                writer.WriteStartElement("Item");

                writer.WriteStartElement("Name");

                this.WriteValue(writer, item.Name);

                writer.WriteEndElement();
                if (item.Count == 0)
                {
                    writer.WriteStartElement("Value");

                    if (item.Value != null)
                    {
                        if (item.Value is Document)
                        {
                            string typeFullName = this.GetTypeFullName(item.Value.GetType());
                            writer.WriteAttributeString(nameof(Type), typeFullName);
                            this.WriteXml(writer, item.Value as Document);
                        }
                        else if (item.Value is Document[])
                        {
                            string typeFullName = this.GetTypeFullName(item.Value.GetType());
                            writer.WriteAttributeString(nameof(Type), typeFullName);
                            foreach (var doc in item.Value as Document[])
                            {
                                this.WriteXml(writer, doc);
                            }

                        }
                        else if (item.Value is Array)
                        {
                            this.WriteValue(writer, (item.Value as Array));
                        }
                        else if (item.Value is IEnumerable && !(item.Value is string))
                        {
                            this.WriteValue(writer, (item.Value as IEnumerable));
                        }
                        else
                        {
                            string typeFullName = this.GetTypeFullName(item.Value.GetType());
                            writer.WriteAttributeString(nameof(Type), typeFullName);
                            this.WriteValue(writer, item.Value);
                        }

                    }
                    writer.WriteEndElement();
                }
                else
                {
                    this.WriteXml(writer, item);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        void WriteValue(XmlWriter writer, string value)
        {
            if (value.Contains('<') || value.Contains('>'))
            {
                writer.WriteCData(value);
            }
            else
            {
                writer.WriteValue(value);
            }
        }

        void WriteValue(XmlWriter writer, object value)
        {
            if (value is string)
            {
                this.WriteValue(writer, value as string);
            }
            else
            {
                Type type = value.GetType();
                if (type.IsEnum)
                {
                    writer.WriteValue(Enum.GetName(type, value));
                }
                else if (type.IsValueType && value is IConvertible)
                {
                    writer.WriteValue(value);
                }
                else
                {
                    this.WriteValue(writer, value.ToString());
                }
            }
        }

        void WriteValue(XmlWriter writer, Array arry)
        {
            writer.WriteStartElement("XmlArray");

            if (arry is Document[])
            {
                foreach (var item in arry as Document[])
                {
                    writer.WriteStartElement(nameof(Document));
                    this.WriteXml(writer, item);
                    writer.WriteEndElement();
                }
            }
            else
            {
                string typeFullName = this.GetTypeFullName(arry.GetType().GetElementType());

                writer.WriteAttributeString(nameof(Type), typeFullName);

                foreach (var item in arry)
                {
                    writer.WriteStartElement("XmlArrayItem");
                    this.WriteValue(writer, item);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        void WriteValue(XmlWriter writer, IEnumerable ienums)
        {
            writer.WriteStartElement("XmlArray");
            Type attrType = null;
            Type ienumType = ienums.GetType();

            if (ienumType.IsGenericType)
            {
                Type[] argTypes = ienumType.GetGenericArguments();

                if (argTypes.Length == 1)
                {
                    attrType = argTypes.First();

                }
            }
            if (attrType == null)
            {
                attrType = typeof(string);
            }

            string typeFullName = this.GetTypeFullName(attrType);

            writer.WriteAttributeString(nameof(Type), typeFullName);

            foreach (var item in ienums)
            {
                writer.WriteStartElement("XmlArrayItem");
                this.WriteValue(writer, item);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }


        string GetTypeFullName(Type type)
        {
            if (type.Namespace != nameof(System) && type.Namespace != typeof(Document).Namespace)
            {
                return $"{type.FullName},{type.Namespace}";
            }
            else
            {
                return type.FullName;
            }
        }

    }
}
