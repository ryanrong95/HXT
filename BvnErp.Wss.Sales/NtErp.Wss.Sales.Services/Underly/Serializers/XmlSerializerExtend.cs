using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly.Serializers
{
    /// <summary>
    /// 序列化小打
    /// </summary>
    public static class XmlSerializerExtend
    {
        /// <summary>
        /// 编码写者
        /// </summary>
        private class EncodingWriter : StringWriter
        {
            Encoding encoding;
            public override Encoding Encoding
            {
                get { return this.encoding; }
            }
            public EncodingWriter(Encoding encoding)
            {
                this.encoding = encoding;
            }
        }

        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <param name="encoding">编码 默认值"utf-16"</param>
        /// <param name="indent">是否缩进</param>
        /// <param name="omitxmldeclaration">是否忽略xml描述</param>
        /// <param name="omitxsn">是否忽略root命名空间</param>
        /// <param name="newlineonattributes">属性是否在新行中显示</param>
        /// <param name="namespacehandling">命名空间声明模式</param>
        /// <returns>序列化xml消息</returns>
        static public string Xml(this object obj, Encoding encoding = null
#if DEBUG
            , bool indent = true

#else
            , bool indent = false
           
#endif
            , bool omitxmldeclaration = false
            , bool omitxsn = true
            , bool newlineonattributes = true

            , NamespaceHandling namespacehandling = NamespaceHandling.OmitDuplicates)
        {
            var coding = encoding == null ? Encoding.GetEncoding("utf-16") : encoding;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = coding;
            settings.Indent = indent;
            settings.OmitXmlDeclaration = omitxmldeclaration;
            settings.NewLineOnAttributes = newlineonattributes;
            settings.NamespaceHandling = namespacehandling;


            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (StringWriter swriter = new EncodingWriter(coding))
            using (XmlWriter xwriter = XmlWriter.Create(swriter, settings))
            {
                if (omitxsn)
                {
                    XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                    xsn.Add("", "");
                    serializer.Serialize(xwriter, obj, xsn);
                }
                else
                {
                    serializer.Serialize(xwriter, obj);
                }

                return swriter.ToString();
            }
        }

        /// <summary>
        /// xml反序列化
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="xml">xml内容</param>
        /// <returns>目标对象类型实例</returns>
        public static T XmlTo<T>(this string xml) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader sw = new StringReader(xml))
            {
                return serializer.Deserialize(sw) as T ?? default(T);
            }
        }

        public static XElement XmlEle(this object obj)
        {
            return XElement.Load(new StringReader(obj.Xml()));
        }
        public static T XmlEleTo<T>(this XElement ele) where T : class
        {
            return ele.ToString().XmlTo<T>();
        }

        public static object XmlEleTo(Type type, XElement ele)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            return serializer.Deserialize(ele.CreateReader());
        }


        public static object XmlTo(Type type, string xml)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            using (StringReader sw = new StringReader(xml))
            {
                return serializer.Deserialize(sw);
            }
        }
    }
}
