using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly.Serializers
{
    /// <summary>
    /// 可序列化排序字典表
    /// </summary>
    /// <typeparam name="TKey">指定键类型</typeparam>
    /// <typeparam name="TValue">指定值类型</typeparam>
    class XmlTopDictionary<TKey, TValue> : IXmlSerializable
    {
        IDictionary<TKey, TValue> source;
        XmlSerializer keySerializer;
        XmlSerializer valueSerializer;

        string startElementName;
        string namingKey;
        string namingValue;

        KeyValuePair<TKey, TValue> singleCase;

        XmlTopDictionary(IDictionary<TKey, TValue> source)
        {
            this.source = source;
            this.keySerializer = new XmlSerializer(typeof(TKey));
            this.valueSerializer = new XmlSerializer(typeof(TValue));

            this.startElementName = "Dictionary";
            this.namingKey = nameof(this.singleCase.Key);
            this.namingValue = nameof(this.singleCase.Value);
        }

        public XmlTopDictionary(string startName, IDictionary<TKey, TValue> source) : this(source)
        {
            this.startElementName = startName;
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (KeyValuePair<TKey, TValue> pair in this.source)
            {
                writer.WriteStartElement(this.startElementName);
                writer.WriteStartElement(this.namingKey);
                if (typeof(TKey) == typeof(string))
                {
                    this.WriteValue(writer, pair.Key as string);
                }
                else
                {
                    this.keySerializer.Serialize(writer, pair.Key);
                }

                writer.WriteEndElement();
                writer.WriteStartElement(this.namingValue);
                if (typeof(TValue) == typeof(string))
                {
                    this.WriteValue(writer, pair.Value as string);
                }
                else
                {
                    this.valueSerializer.Serialize(writer, pair.Value);
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
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

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            //reader.ReadStartElement();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement(this.startElementName);
                reader.ReadStartElement(this.namingKey);

                TKey tk;
                if (typeof(TKey) == typeof(string))
                {
                    object obj = reader.Value;
                    tk = (TKey)obj;
                    reader.Read();
                }
                else
                {
                    tk = (TKey)this.keySerializer.Deserialize(reader);
                }

                reader.ReadEndElement();
                reader.ReadStartElement(this.namingValue);

                TValue vl;

                if (typeof(TValue) == typeof(string))
                {
                    object obj = reader.Value;
                    vl = (TValue)obj;
                    reader.Read();
                }
                else
                {
                    vl = (TValue)this.valueSerializer.Deserialize(reader);
                }

                reader.ReadEndElement();
                reader.ReadEndElement();
                //reader.ReadEndElement();
                this.source.Add(tk, vl);
                reader.MoveToContent();
            }
            reader.ReadEndElement();

        }

        public XmlSchema GetSchema()
        {
            return null;
        }
    }
}
