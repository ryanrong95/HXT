using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Needs.Underly.Collections
{
    /// <summary>
    /// 临时做一个
    /// </summary>
    public static class XmlSerializersUtils
    {
        /// <summary>
        /// 可视化CData序列化
        /// </summary>
        /// <param name="txt">字符内容</param>
        /// <example>不适合在代码中直接使用</example>
        /// <returns>XmlNode</returns>
        static public XmlNode CDate(string txt)
        {
            XmlNode node;
            if (txt.Contains('<') || txt.Contains('>'))
            {
                node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
            }
            else
            {
                node = new XmlDocument().CreateNode(XmlNodeType.Text, "", "");
            }

            node.InnerText = txt;

            return node;
        }

        /// <summary>
        /// 可视化XmlNode序列化
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <example>不适合在代码中直接使用</example>
        /// <returns>XmlNode.InnerText</returns>
        static public string CDate(XmlNode node)
        {
            return node.InnerText;
        }
    }
}
