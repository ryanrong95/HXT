using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Needs.Wl.CustomsTool
{
    public class XmlService
    {
        /// <summary>
        /// 加载xml文档
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns></returns>
        public static string LoadXmlFile(string xmlFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            var xml = doc.InnerXml;
           // xml = System.Text.RegularExpressions.Regex.Replace(xml, @"(xmlns:?[^=]*=[""][^""]*[""])|(ns2:)", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return xml;
        }

    }
}
