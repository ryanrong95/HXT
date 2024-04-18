using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.App_Utils
{
    public static class FileExtends
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