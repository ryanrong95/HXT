using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Needs.Underly
{
    public partial class Document : IXmlSerializable
    {
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

        void ReadXml(XElement xeles, Document current)
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

                    if (value_ele.FirstNode != null)
                    {
                        var typeAttr = value_ele.Attribute("Type");
                        if (typeAttr != null)
                        {
                            current[name_ele.Value] = Convert.ChangeType(value_ele.Value, Type.GetType(typeAttr.Value));
                        }
                        else
                        {
                            var valueSerializer = new XmlSerializer(typeof(List<dynamic>));
                            current[name_ele.Value] = valueSerializer.Deserialize(value_ele.FirstNode.CreateReader());
                        }

                    }
                    //current[name_ele.Value] = value_ele.Value;
                }
                else
                {
                    ReadXml(items_ele, current[name_ele.Value]);
                }
            }
        }

        static Dictionary<string, Type> typeMappings = new[] {
            new KeyValuePair<string, Type>("int", typeof(int)),
            new KeyValuePair<string, Type>("string", typeof(string)),
        }.ToDictionary(item => item.Key, item => item.Value);


        public void WriteXml(XmlWriter writer)
        {
            this.WriteXml(writer, this);
        }

        void WriteXml(XmlWriter writer, Document infosame)
        {
            writer.WriteStartElement("Items");
            foreach (var item in infosame)
            {
                writer.WriteStartElement("Item");

                writer.WriteStartElement("Name");

                this.WriteValue(writer, item.Key);

                writer.WriteEndElement();
                if (item.Value is Document)
                {
                    this.WriteXml(writer, item.Value as Document);
                }
                else
                {

                    writer.WriteStartElement("Value");
                    //this.WriteValue(writer, item.Value.ToString());
                    if (item.Value != null)
                    {
                        var valueSerializer = new XmlSerializer(item.Value.GetType());
                        valueSerializer.Serialize(writer, item.Value);
                    }



                    //Type type = item.Value.GetType();
                    //if (type.IsValueType)
                    //{

                    //}

                    //if (item.Value is string)
                    //{
                    //    this.WriteValue(writer, item.Value as string);
                    //}
                    //else
                    //{
                    //    var valueSerializer = new XmlSerializer(item.Value.GetType());
                    //    valueSerializer.Serialize(writer, item.Value);
                    //}
                    //var valueSerializer = new XmlSerializer(item.Value.GetType());
                    //valueSerializer.Serialize(writer, item.Value);

                    writer.WriteEndElement();
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
    }
}
