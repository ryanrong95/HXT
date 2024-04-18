using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.DataImport.Service.Utils
{
    public static class GuidUtil
    {
        /// <summary>
        /// 生成Guid
        /// </summary>
        /// <returns></returns>
        public static string NewGuidUp()
        {
            return System.Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}
