//using Needs.Ccs.Services.Models;
using Needs.Wl.CustomsTool.WinForm.Models;
using Needs.Wl.CustomsTool.WinForm.Models.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Needs.Wl.CustomsTool.WinForm.App_Utils
{
    public class XmlHelper
    {
        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">XML字符</param>
        /// <returns></returns>
        public static T DeserializeToObject<T>(string xml)
        {
            T myObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xml);
            myObject = (T)serializer.Deserialize(reader);
            reader.Close();
            return myObject;
        }

        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">XML字符</param>
        /// <returns></returns>
        public static T DeserializeToObject<T>(string xml, string root)
        {
            T myObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));

            //去除命名空间
            xml = System.Text.RegularExpressions.Regex.Replace(xml, @"(xmlns:?[^=]*=[""][^""]*[""])|(ns2:)", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            StringReader reader = new StringReader(xml);
            //reader.Namespaces = false;
            myObject = (T)serializer.Deserialize(reader);
            reader.Close();
            return myObject;
        }

        /// <summary>
        /// 加载xml文档
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns></returns>
        public static string LoadXmlFile(string xmlFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            return doc.InnerXml;
        }

        public static DecImportResponse GetFailBox(string path)
        {
            return DeserializeToObject<DecImportResponse>(LoadXmlFile(path));
        }

        #region 报关单回执报文

        public static DEC_RESULT GetDecReceipt(string path)
        {
            return DeserializeToObject<DEC_RESULT>(LoadXmlFile(path));
        }

        public static DecImportResponse GetDecResponse(string path)
        {
            return DeserializeToObject<DecImportResponse>(LoadXmlFile(path));
        }

        #endregion

        #region 舱单回执报文

        public static Needs.Ccs.Services.Models.ManifestMessage.Messages.Manifest GetMftResponse(string path)
        {
            return DeserializeToObject<Needs.Ccs.Services.Models.ManifestMessage.Messages.Manifest>(LoadXmlFile(path), "Manifest");
        }

        #endregion
    }
}
