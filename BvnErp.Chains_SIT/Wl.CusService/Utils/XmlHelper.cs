using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Wl.CusService.Models;

namespace Wl.CusService
{
    public class XmlHelper
    {
        /// <summary>
        /// 将自定义对象序列化为XML字符串
        /// </summary>
        /// <param name="myObject">自定义对象实体</param>
        /// <returns>序列化后的XML字符串</returns>
        //public static string SerializeToXml<T>(T myObject)
        //{
        //    if (myObject != null)
        //    {
        //        XmlSerializer xs = new XmlSerializer(typeof(T));

        //        MemoryStream stream = new MemoryStream();
        //        XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
        //        writer.Formatting = Formatting.None;//缩进
        //        xs.Serialize(writer, myObject);

        //        stream.Position = 0;
        //        StringBuilder sb = new StringBuilder();
        //        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //        {
        //            string line;
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                sb.Append(line);
        //            }
        //            reader.Close();
        //        }
        //        writer.Close();
        //        return sb.ToString();
        //    }
        //    return string.Empty;
        //}

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

        /// <summary>
        /// 创建xml文档
        /// </summary>
        /// <param name="xml">XML字符</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名</param>
        //public static void SaveXmlFile(string xml, string fileName)
        //{
        //    //创建空的XML文档
        //    XmlDocument xmldoc = new XmlDocument();
        //    xmldoc.LoadXml(xml);

        //    string path = ConfigurationManager.AppSettings["OutBox"];
        //    xmldoc.Save(path + @"\" + fileName);
        //}

        /// <summary>
        /// 获取导入回执
        /// </summary>
        /// <param name="fileName">文件名</param>
        //public static DecImportResponse GetDecImportResponse(string fileName) //订单ID?
        //{
        //    string path = ConfigurationManager.AppSettings["InBox"];
        //    DirectoryInfo root = new DirectoryInfo(path);
        //    FileInfo[] files = root.GetFiles("*.xml");
        //    foreach (var file in files)
        //    {
        //        if (file.Name.Contains(fileName))
        //        {
        //            string filePath = file.DirectoryName + @"\" + file.Name;
        //            DecImportResponse response = DeserializeToObject<DecImportResponse>(LoadXmlFile(filePath));
        //            File.Delete(filePath);//删除回执文件
        //            return response;
        //        }
        //    }
        //    return null;
        //}

        /// <summary>
        /// 获取业务回执
        /// </summary>
        /// <param name="fileName">文件名</param>
        //public static DecData GetDecData(string fileName) //导入回执的SeqNo?
        //{
        //    string path = ConfigurationManager.AppSettings["InBox"];
        //    DirectoryInfo root = new DirectoryInfo(path);
        //    FileInfo[] files = root.GetFiles("*.xml");
        //    foreach (var file in files)
        //    {
        //        if (file.Name.Contains(fileName))
        //        {
        //            string filePath = file.DirectoryName + @"\" + file.Name;
        //            DecData response = DeserializeToObject<DecData>(LoadXmlFile(filePath));
        //            return response;
        //        }
        //    }
        //    return null;
        //}


        public static List<DecImportResponse> GetFailBox()
        {
            string path = ConfigurationManager.AppSettings["FailBox"];
            string pathbk = ConfigurationManager.AppSettings["FailBox_BK"];
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles("*.xml");
            var result = new List<DecImportResponse>();
            foreach (var file in files)
            {

                string filePath = file.DirectoryName + @"\" + file.Name;
                DecImportResponse response = DeserializeToObject<DecImportResponse>(LoadXmlFile(filePath));
                response.FileName = file.Name;

                ////回执时间
                /////DateTime.ParseExact("20180709171807","yyyyMMddHHmmsss", System.Globalization.CultureInfo.CurrentCulture)
                var fileName = file.Name.Substring(0,file.Name.LastIndexOf("."));
                var responsTime = fileName.Substring(fileName.LastIndexOf("_") + 1, 14);
                response.ResponseTime = DateTime.ParseExact(responsTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                response.BackupUrl = pathbk + fileName + ".xml";
                result.Add(response);

                //备份
                file.MoveTo(response.BackupUrl);
            }
            return result;
        }
    }
}
