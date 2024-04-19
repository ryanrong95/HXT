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
    /// 可序列化字典
    /// </summary>
    /// <typeparam name="TKey">指定键类型</typeparam>
    /// <typeparam name="TValue">指定值类型</typeparam>
    [Serializable]
    public class XmlSortedDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>, IXmlSerializable
    {
        XmlTopDictionary<TKey, TValue> utils;

        public XmlSortedDictionary()
        {
            this.utils = new XmlTopDictionary<TKey, TValue>(nameof(XmlTopDictionary<TKey, TValue>), this);
        }

        public void WriteXml(XmlWriter write)
        {
            this.utils.WriteXml(write);
        }

        public void ReadXml(XmlReader reader)
        {
            this.utils.ReadXml(reader);
        }
        public XmlSchema GetSchema()
        {
            return this.utils.GetSchema();
        }
    }
}
