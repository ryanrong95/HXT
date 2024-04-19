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
    [Serializable]
    public class XmlSortedList<TKey, TValue> : SortedList<TKey, TValue>, IXmlSerializable
    {
        XmlTopDictionary<TKey, TValue> utils;

        public XmlSortedList()
        {
            this.utils = new XmlTopDictionary<TKey, TValue>(nameof(XmlSortedList<TKey, TValue>), this);
        }

        public void WriteXml(XmlWriter writer)
        {
            this.utils.WriteXml(writer);
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
