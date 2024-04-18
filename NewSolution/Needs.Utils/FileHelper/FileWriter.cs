using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils
{
    public class FileWriter
    {
        /// <summary>
        /// 将json写入js文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="jsName">用于json对象的名称,用法:window[jsName]=json</param>
        /// <param name="data">要写入文件的json对象</param>
        /// <param name="path">文件路径</param>
        public static void WriteJSON(string fileName, string jsName, Newtonsoft.Json.Linq.JObject data, string path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            }
            else
            {
                path = System.IO.Path.Combine(path, fileName);
            }
            if (!new FileInfo(path).Directory.Exists)
            {
                new FileInfo(path).Directory.Create();
            }
            if (System.IO.File.Exists(path))
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write($"window['{jsName}']={data}");
                }
            }
            else
            {
                using (var sw = System.IO.File.Create(path))
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes($"window['{jsName}']={data}");
                    sw.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
