using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Converters
{
    /// <summary>
    /// 文件扩展类
    /// 备注芯达通专用
    /// </summary>
    public static class FileExtend
    {
        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReName(this string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName);
            var random = new Random(Guid.NewGuid().GetHashCode());
            var newName = DateTime.Now.ToString("hhmmssfff") + random.Next(1000, 9999);
            return newName + ext;
        }
    }
}
